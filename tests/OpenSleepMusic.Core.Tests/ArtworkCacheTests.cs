using System.Net;
using System.Security.Cryptography;
using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Downloads;

namespace OpenSleepMusic.Core.Tests;

public sealed class ArtworkCacheTests
{
    private static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, .. new byte[32]];

    [Fact]
    public async Task DownloadsValidPngAndReusesCache()
    {
        var requests = 0;
        using var client = new HttpClient(new StubHandler(_ => { requests++; return Response(HttpStatusCode.OK, Png, "image/png"); }));
        var root = Path.Combine(Path.GetTempPath(), $"open-sleep-art-{Guid.NewGuid():N}");
        try
        {
            var track = Track(Convert.ToHexStringLower(SHA256.HashData(Png)));
            var cache = new ArtworkCache(client);
            var first = await cache.GetAsync(track, root);
            var second = await cache.GetAsync(track, root);
            Assert.Equal(first, second);
            Assert.True(File.Exists(first));
            Assert.Equal(1, requests);
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound, "text/plain")]
    [InlineData(HttpStatusCode.OK, "text/html")]
    public async Task InvalidOrUnavailableArtworkFallsBackSilently(HttpStatusCode status, string contentType)
    {
        using var client = new HttpClient(new StubHandler(_ => Response(status, "<html>no image</html>"u8.ToArray(), contentType)));
        var log = new RecordingLogSink();
        var root = Path.Combine(Path.GetTempPath(), $"open-sleep-art-{Guid.NewGuid():N}");
        try
        {
            var result = await new ArtworkCache(client, log).GetAsync(Track(new string('0', 64)), root);
            Assert.Null(result);
            Assert.Single(log.Entries);
            Assert.Empty(Directory.Exists(root) ? Directory.GetFiles(root) : []);
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static AudioTrack Track(string sha256) => new(
        "track", "Track", "Creator", new("https://example.test/audio.mp3"), new("https://example.test/source"),
        "CC0", new("https://creativecommons.org/publicdomain/zero/1.0/"), "track.mp3",
        ArtworkUri: new("https://example.test/art.png"), ArtworkFileName: "art.png", ArtworkSha256: sha256);

    private static HttpResponseMessage Response(HttpStatusCode status, byte[] bytes, string contentType) =>
        new(status) { Content = new ByteArrayContent(bytes) { Headers = { ContentType = new(contentType) } } };

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(response(request));
    }

    private sealed class RecordingLogSink : IDownloadLogSink
    {
        public List<string> Entries { get; } = [];
        public Task WriteSkippedAsync(string trackId, Uri uri, string reason, CancellationToken cancellationToken)
        { Entries.Add($"{trackId}: {reason}"); return Task.CompletedTask; }
    }
}
