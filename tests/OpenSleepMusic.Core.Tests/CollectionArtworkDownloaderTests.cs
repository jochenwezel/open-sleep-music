using System.Net;
using System.Security.Cryptography;
using System.Text;
using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Downloads;

namespace OpenSleepMusic.Core.Tests;

public sealed class CollectionArtworkDownloaderTests
{
    private static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, .. new byte[32]];

    [Fact]
    public async Task DownloadsSharedArtworkOnceAndCountsEveryTrack()
    {
        var artworkRequests = 0;
        var sha = Convert.ToHexStringLower(SHA256.HashData(Png));
        var manifest = $$"""{"schemaVersion":1,"tracks":[]}""";
        using var client = new HttpClient(new StubHandler(request =>
        {
            if (request.RequestUri!.AbsolutePath.EndsWith("catalog.json", StringComparison.Ordinal))
                return Json(manifest);
            artworkRequests++;
            return Bytes(HttpStatusCode.OK, Png, "image/png");
        }));
        var root = Path.Combine(Path.GetTempPath(), $"open-sleep-collection-art-{Guid.NewGuid():N}");
        try
        {
            var manifestClient = new ArtworkManifestClient(client, new("https://example.test/catalog.json"));
            var downloader = new CollectionArtworkDownloader(manifestClient, new ArtworkCache(client));
            var tracks = new[] { Track("one", sha), Track("two", sha) };

            var result = await downloader.DownloadAsync(tracks, root);

            Assert.Equal(2, result.AvailableCount);
            Assert.Equal(0, result.UnavailableCount);
            Assert.Equal(1, artworkRequests);
            Assert.True(File.Exists(Path.Combine(root, "shared.png")));
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    [Fact]
    public async Task UnavailableArtworkDoesNotStopRemainingDownloads()
    {
        var sha = Convert.ToHexStringLower(SHA256.HashData(Png));
        var manifest = $$"""{"schemaVersion":1,"tracks":[]}""";
        using var client = new HttpClient(new StubHandler(request =>
        {
            if (request.RequestUri!.AbsolutePath.EndsWith("catalog.json", StringComparison.Ordinal))
                return Json(manifest);
            return request.RequestUri.AbsolutePath.Contains("missing", StringComparison.Ordinal)
                ? Bytes(HttpStatusCode.NotFound, "missing"u8.ToArray(), "text/plain")
                : Bytes(HttpStatusCode.OK, Png, "image/png");
        }));
        var root = Path.Combine(Path.GetTempPath(), $"open-sleep-collection-art-{Guid.NewGuid():N}");
        try
        {
            var manifestClient = new ArtworkManifestClient(client, new("https://example.test/catalog.json"));
            var downloader = new CollectionArtworkDownloader(manifestClient, new ArtworkCache(client));
            var tracks = new[] { Track("missing", sha, "missing.png"), Track("available", sha, "available.png") };

            var result = await downloader.DownloadAsync(tracks, root);

            Assert.Equal(1, result.AvailableCount);
            Assert.Equal(1, result.UnavailableCount);
            Assert.True(File.Exists(Path.Combine(root, "available.png")));
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    [Fact]
    public async Task DownloadsOptionalSongMotifAlongsideSharedBackground()
    {
        var requests = new List<string>();
        var sha = Convert.ToHexStringLower(SHA256.HashData(Png));
        using var client = new HttpClient(new StubHandler(request =>
        {
            if (request.RequestUri!.AbsolutePath.EndsWith("catalog.json", StringComparison.Ordinal))
                return Json("""{"schemaVersion":1,"tracks":[]}""");
            requests.Add(request.RequestUri.AbsolutePath);
            return Bytes(HttpStatusCode.OK, Png, "image/png");
        }));
        var root = Path.Combine(Path.GetTempPath(), $"open-sleep-collection-art-{Guid.NewGuid():N}");
        try
        {
            var track = Track("one", sha) with
            {
                SongMotifUri = new("https://example.test/song.png"),
                SongMotifFileName = "song.png",
                SongMotifSha256 = sha
            };
            var downloader = new CollectionArtworkDownloader(
                new ArtworkManifestClient(client, new("https://example.test/catalog.json")),
                new ArtworkCache(client));

            var result = await downloader.DownloadAsync([track], root);

            Assert.Equal(1, result.AvailableCount);
            Assert.Equal(0, result.UnavailableCount);
            Assert.Contains("/shared.png", requests);
            Assert.Contains("/song.png", requests);
            Assert.True(File.Exists(Path.Combine(root, "shared.png")));
            Assert.True(File.Exists(Path.Combine(root, "song.png")));
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static AudioTrack Track(string id, string sha, string fileName = "shared.png") => new(
        id, id, "Creator", new($"https://example.test/{id}.mp3"), new($"https://example.test/{id}"),
        "CC0", new("https://creativecommons.org/publicdomain/zero/1.0/"), $"{id}.mp3",
        ArtworkUri: new($"https://example.test/{fileName}"), ArtworkFileName: fileName, ArtworkSha256: sha);

    private static HttpResponseMessage Json(string content) =>
        new(HttpStatusCode.OK) { Content = new StringContent(content, Encoding.UTF8, "application/json") };

    private static HttpResponseMessage Bytes(HttpStatusCode status, byte[] content, string contentType) =>
        new(status) { Content = new ByteArrayContent(content) { Headers = { ContentType = new(contentType) } } };

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(response(request));
    }
}
