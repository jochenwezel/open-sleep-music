using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Downloads;

namespace OpenSleepMusic.Core.Tests;

public sealed class CollectionArtworkTests
{
    private static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, .. new byte[32]];
    private static readonly string Sha = Convert.ToHexStringLower(SHA256.HashData(Png));
    private static readonly Uri ManifestUri = new("https://example.test/catalog.json");
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    [Theory]
    [InlineData("candidate-romanza-ten-string-leon-egea", "motif_lullaby_romanza_espanola_a.png")]
    [InlineData("candidate-recuerdos-de-la-alhambra", "motif_lullaby_alhambra_a.png")]
    [InlineData("candidate-barrios-julia-florida-edson-lopes", "motif_lullaby_julia_florida_a.png")]
    public void GuitarChildrenMotifsAreConfinedToLittleOnesWithoutDuplicatingAudio(string id, string motif)
    {
        var classics = BuiltInCatalog.SleepWorlds.Single(world => world.Id == "quiet-classics");
        var littleOnes = BuiltInCatalog.SleepWorlds.Single(world => world.Id == "lullabies");
        var canonical = classics.Tracks.Single(track => track.Id == id);
        Assert.Same(canonical, littleOnes.Tracks.Single(track => track.Id == id));
        var child = CollectionArtwork.ForCollection(canonical, littleOnes.Id);
        Assert.Equal(motif, child.SongMotifFileName);
        Assert.Equal("background_lullabies_v3.png", child.ArtworkFileName);
        Assert.Equal("fallback_lullabies.png", child.FallbackMotifFileName);
        Assert.Equal(canonical.DownloadUri, child.DownloadUri);
        Assert.Equal(canonical.FileName, child.FileName);
        Assert.Equal(canonical.BackgroundAudio, child.BackgroundAudio);
        var adult = CollectionArtwork.ForCollection(canonical, classics.Id);
        Assert.Null(adult.SongMotifUri);
        Assert.Equal("background_quiet_classics_v3.png", adult.ArtworkFileName);
        Assert.Equal("fallback_quiet_classics.png", adult.FallbackMotifFileName);
    }

    [Fact]
    public async Task RemoteCollectionLayersReplaceOldGlobalChildMotifAndSurviveOfflineCache()
    {
        var root = TemporaryRoot();
        try
        {
            var old = Track() with { SongMotifUri = new("https://example.test/old-child.png"),
                SongMotifFileName = "old-child.png", SongMotifSha256 = Sha };
            using var online = Client(_ => Json(Manifest(Assignments())));
            var resolver = new ArtworkManifestClient(online, ManifestUri);
            Assert.Equal("child-song.png", (await resolver.ResolveAsync(old, root, "lullabies")).SongMotifFileName);
            Assert.Equal("adult-song.png", (await resolver.ResolveAsync(old, root, "quiet-classics")).SongMotifFileName);
            Assert.Null((await resolver.ResolveAsync(old, root, "other")).SongMotifUri);
            using var offline = Client(_ => new(HttpStatusCode.NotFound));
            var cached = new ArtworkManifestClient(offline, ManifestUri);
            var child = await cached.ResolveAsync(old, root, "lullabies");
            Assert.Equal("child-background.png", child.ArtworkFileName);
            Assert.Equal("child-fallback.png", child.FallbackMotifFileName);
            Assert.Equal("adult-song.png", (await cached.ResolveAsync(old, root, "quiet-classics")).SongMotifFileName);
            Assert.Equal(old.DownloadUri, child.DownloadUri);
            Assert.Equal(old.Sha1, child.Sha1);
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public async Task LegacyGlobalChildManifestCannotOverwriteEmbeddedCollectionAssignments()
    {
        var root = TemporaryRoot();
        try
        {
            using var client = Client(_ => Json(Manifest(null, globalChild: true)));
            var resolver = new ArtworkManifestClient(client, ManifestUri);
            var track = Track() with { CollectionArtwork = new Dictionary<string, CollectionArtwork>
                { ["lullabies"] = Layers("child") } };
            Assert.Null((await resolver.ResolveAsync(track, root, "quiet-classics")).SongMotifUri);
            Assert.Equal("child-song.png", (await resolver.ResolveAsync(track, root, "lullabies")).SongMotifFileName);
        }
        finally { Directory.Delete(root, true); }
    }

    [Theory]
    [InlineData("artworkUri", "http://example.test/insecure.png")]
    [InlineData("songMotifFileName", "../unsafe.png")]
    [InlineData("songMotifSha256", "invalid")]
    [InlineData("songMotifUri", null)]
    public async Task InvalidCollectionLayerKeepsLastValidOfflineManifest(string field, string? value)
    {
        var root = TemporaryRoot();
        try
        {
            var valid = Manifest(Assignments());
            using (var first = Client(_ => Json(valid)))
                await new ArtworkManifestClient(first, ManifestUri).ResolveAsync(Track(), root, "lullabies");
            var invalid = JsonNode.Parse(valid)!;
            invalid["tracks"]![0]!["collectionArtwork"]!["lullabies"]![field] = value;
            using var client = Client(_ => Json(invalid.ToJsonString()));
            var result = await new ArtworkManifestClient(client, ManifestUri).ResolveAsync(Track(), root, "lullabies");
            Assert.Equal("child-song.png", result.SongMotifFileName);
            Assert.Equal(valid, await File.ReadAllTextAsync(Path.Combine(root, "artwork-catalog.json")));
            Assert.False(File.Exists(Path.Combine(root, "artwork-catalog.json.part")));
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public async Task CollectionDownloadCachesOnlyItsSelectedLayersAndReusesThemOffline()
    {
        var root = TemporaryRoot();
        var requests = new List<string>();
        try
        {
            using var client = Client(request =>
            {
                if (request.RequestUri == ManifestUri) return Json(Manifest(Assignments()));
                requests.Add(request.RequestUri!.AbsolutePath);
                return new(HttpStatusCode.OK) { Content = new ByteArrayContent(Png) };
            });
            var downloader = new CollectionArtworkDownloader(new(client, ManifestUri), new(client));
            Assert.Equal(1, (await downloader.DownloadAsync([Track()], root, worldId: "quiet-classics")).AvailableCount);
            Assert.Equal(new[] { "/adult-background.png", "/adult-song.png", "/adult-fallback.png" }, requests);
            requests.Clear();
            Assert.Equal(1, (await downloader.DownloadAsync([Track()], root, worldId: "lullabies")).AvailableCount);
            Assert.Equal(new[] { "/child-background.png", "/child-song.png", "/child-fallback.png" }, requests);
            using var offline = Client(_ => new(HttpStatusCode.NotFound));
            var cached = new CollectionArtworkDownloader(new(offline, ManifestUri), new(offline));
            Assert.Equal(1, (await cached.DownloadAsync([Track()], root, worldId: "quiet-classics")).AvailableCount);
            Assert.Equal(1, (await cached.DownloadAsync([Track()], root, worldId: "lullabies")).AvailableCount);
        }
        finally { Directory.Delete(root, true); }
    }

    private static Dictionary<string, CollectionArtwork> Assignments() => new()
    {
        ["lullabies"] = Layers("child"), ["quiet-classics"] = Layers("adult")
    };

    private static CollectionArtwork Layers(string prefix) => new(
        new($"https://example.test/{prefix}-background.png"), $"{prefix}-background.png", Sha,
        new($"https://example.test/{prefix}-song.png"), $"{prefix}-song.png", Sha,
        new($"https://example.test/{prefix}-fallback.png"), $"{prefix}-fallback.png", Sha);

    private static AudioTrack Track() => new("track", "Track", "Creator", new("https://example.test/audio.mp3"),
        new("https://example.test/source"), "CC0", new("https://creativecommons.org/publicdomain/zero/1.0/"),
        "track.mp3", 60, ArtworkUri: new("https://example.test/default.png"), ArtworkFileName: "default.png",
        ArtworkSha256: Sha, FallbackMotifUri: new("https://example.test/default-fallback.png"),
        FallbackMotifFileName: "default-fallback.png", FallbackMotifSha256: Sha);

    private static string Manifest(Dictionary<string, CollectionArtwork>? assignments, bool globalChild = false) =>
        JsonSerializer.Serialize(new { schemaVersion = 1, tracks = new[] { new
        {
            trackId = "track", artworkUri = "https://example.test/default.png", artworkFileName = "default.png",
            artworkSha256 = Sha, fallbackMotifUri = "https://example.test/default-fallback.png",
            fallbackMotifFileName = "default-fallback.png", fallbackMotifSha256 = Sha,
            songMotifUri = globalChild ? "https://example.test/legacy-child.png" : null,
            songMotifFileName = globalChild ? "legacy-child.png" : null, songMotifSha256 = globalChild ? Sha : null,
            collectionArtwork = assignments
        } } }, JsonOptions);

    private static string TemporaryRoot()
    {
        var path = Path.Combine(Path.GetTempPath(), $"open-sleep-collection-layers-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }

    private static HttpResponseMessage Json(string content) => new(HttpStatusCode.OK) { Content = new StringContent(content) };
    private static HttpClient Client(Func<HttpRequestMessage, HttpResponseMessage> response) => new(new StubHandler(response));
    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(response(request));
    }
}
