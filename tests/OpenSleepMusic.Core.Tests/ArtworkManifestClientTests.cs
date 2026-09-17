using System.Net;
using System.Security.Cryptography;
using System.Text;
using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Downloads;

namespace OpenSleepMusic.Core.Tests;

public sealed class ArtworkManifestClientTests
{
    [Fact]
    public async Task RemoteManifestOverridesEmbeddedArtworkAndIsCachedForOfflineUse()
    {
        var root = Path.Combine(Path.GetTempPath(), $"open-sleep-manifest-{Guid.NewGuid():N}");
        var sha = Convert.ToHexStringLower(SHA256.HashData("image"u8.ToArray()));
        var json = $$"""{"schemaVersion":1,"tracks":[{"trackId":"track","artworkUri":"https://example.test/v2.png","artworkFileName":"v2.png","artworkSha256":"{{sha}}","songMotifUri":"https://example.test/song.png","songMotifFileName":"song.png","songMotifSha256":"{{sha}}"}]}""";
        try
        {
            using var online = new HttpClient(new StubHandler(_ => Response(HttpStatusCode.OK, json)));
            var first = await new ArtworkManifestClient(online, new("https://example.test/catalog.json")).ResolveAsync(Track(), root);
            Assert.Equal("https://example.test/v2.png", first.ArtworkUri!.AbsoluteUri);
            Assert.Equal("https://example.test/song.png", first.SongMotifUri!.AbsoluteUri);

            using var offline = new HttpClient(new StubHandler(_ => Response(HttpStatusCode.NotFound, "missing")));
            var second = await new ArtworkManifestClient(offline, new("https://example.test/catalog.json")).ResolveAsync(Track(), root);
            Assert.Equal(first.ArtworkUri, second.ArtworkUri);
            Assert.Equal(first.ArtworkSha256, second.ArtworkSha256);
            Assert.Equal(first.SongMotifSha256, second.SongMotifSha256);
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    [Fact]
    public async Task PartialSongMotifMetadataRejectsManifestAndKeepsEmbeddedArtwork()
    {
        var sha = new string('0', 64);
        var json = $$"""{"schemaVersion":1,"tracks":[{"trackId":"track","artworkUri":"https://example.test/v2.png","artworkFileName":"v2.png","artworkSha256":"{{sha}}","songMotifFileName":"song.png"}]}""";
        using var client = new HttpClient(new StubHandler(_ => Response(HttpStatusCode.OK, json)));
        var root = Path.Combine(Path.GetTempPath(), $"open-sleep-manifest-{Guid.NewGuid():N}");
        try
        {
            var fallback = Track();
            var result = await new ArtworkManifestClient(client, new("https://example.test/catalog.json")).ResolveAsync(fallback, root);
            Assert.Equal(fallback, result);
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    [Fact]
    public async Task LegacyManifestDoesNotCollapseEmbeddedTwoLayerArtwork()
    {
        var sha = new string('0', 64);
        var json = $$"""{"schemaVersion":1,"tracks":[{"trackId":"track","artworkUri":"https://example.test/v2.png","artworkFileName":"v2.png","artworkSha256":"{{sha}}"}]}""";
        using var client = new HttpClient(new StubHandler(_ => Response(HttpStatusCode.OK, json)));
        var root = Path.Combine(Path.GetTempPath(), $"open-sleep-manifest-{Guid.NewGuid():N}");
        try
        {
            var fallback = Track() with
            {
                SongMotifUri = new("https://example.test/embedded-song.png"),
                SongMotifFileName = "embedded-song.png",
                SongMotifSha256 = sha
            };
            var result = await new ArtworkManifestClient(client, new("https://example.test/catalog.json")).ResolveAsync(fallback, root);
            Assert.Equal(fallback.ArtworkUri, result.ArtworkUri);
            Assert.Equal(fallback.SongMotifUri, result.SongMotifUri);
            Assert.Equal(fallback.SongMotifSha256, result.SongMotifSha256);
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    [Fact]
    public async Task InvalidManifestFallsBackToEmbeddedTrackMetadata()
    {
        using var client = new HttpClient(new StubHandler(_ => Response(HttpStatusCode.OK, "<html>not json</html>")));
        var root = Path.Combine(Path.GetTempPath(), $"open-sleep-manifest-{Guid.NewGuid():N}");
        try
        {
            var fallback = Track();
            var result = await new ArtworkManifestClient(client, new("https://example.test/catalog.json")).ResolveAsync(fallback, root);
            Assert.Equal(fallback, result);
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static AudioTrack Track() => new(
        "track", "Track", "Creator", new("https://example.test/audio.mp3"), new("https://example.test/source"),
        "CC0", new("https://creativecommons.org/publicdomain/zero/1.0/"), "track.mp3",
        ArtworkUri: new("https://example.test/v1.png"), ArtworkFileName: "v1.png", ArtworkSha256: new string('0', 64));

    private static HttpResponseMessage Response(HttpStatusCode status, string content) =>
        new(status) { Content = new StringContent(content, Encoding.UTF8, "application/json") };

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(response(request));
    }
}
