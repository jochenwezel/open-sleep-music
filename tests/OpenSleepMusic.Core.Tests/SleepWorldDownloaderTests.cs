using System.Net;
using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Downloads;

namespace OpenSleepMusic.Core.Tests;

public sealed class SleepWorldDownloaderTests
{
    [Fact]
    public async Task Skips404AndHtmlButKeepsValidAudio()
    {
        var tracks = new[]
        {
            Track("valid", "valid.ogg", "https://example.test/valid"),
            Track("missing", "missing.mp3", "https://example.test/missing"),
            Track("html", "html.mp3", "https://example.test/html")
        };
        var world = new SleepWorld("test", "Test", "Test", "T", tracks);
        var log = new RecordingLogSink();
        using var client = new HttpClient(new StubHandler(request => request.RequestUri!.AbsolutePath switch
        {
            "/valid" => Response(HttpStatusCode.OK, [.. "OggS"u8, .. new byte[256]], "audio/ogg"),
            "/missing" => Response(HttpStatusCode.NotFound, [], "text/plain"),
            _ => Response(HttpStatusCode.OK, [.. "<html>not audio</html>"u8, .. new byte[256]], "text/html")
        }));
        var destination = Path.Combine(Path.GetTempPath(), $"open-sleep-music-tests-{Guid.NewGuid():N}");

        try
        {
            var result = await new SleepWorldDownloader(client, log).DownloadAsync(world, destination);

            Assert.Equal(1, result.DownloadedCount);
            Assert.Equal(2, result.SkippedCount);
            Assert.True(File.Exists(Path.Combine(destination, "test", "valid.ogg")));
            Assert.False(File.Exists(Path.Combine(destination, "test", "missing.mp3")));
            Assert.False(File.Exists(Path.Combine(destination, "test", "html.mp3")));
            Assert.Equal(2, log.Entries.Count);
        }
        finally
        {
            if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }
        }
    }

    private static AudioTrack Track(string id, string fileName, string uri) => new(
        id, id, "Tester", new Uri(uri), new Uri("https://example.test/source"),
        "CC0", new Uri("https://creativecommons.org/publicdomain/zero/1.0/"), fileName);

    private static HttpResponseMessage Response(HttpStatusCode status, byte[] content, string contentType) =>
        new(status) { Content = new ByteArrayContent(content) { Headers = { ContentType = new(contentType) } } };

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(response(request));
    }

    private sealed class RecordingLogSink : IDownloadLogSink
    {
        public List<string> Entries { get; } = [];

        public Task WriteSkippedAsync(string trackId, Uri uri, string reason, CancellationToken cancellationToken)
        {
            Entries.Add($"{trackId}: {reason}");
            return Task.CompletedTask;
        }
    }
}
