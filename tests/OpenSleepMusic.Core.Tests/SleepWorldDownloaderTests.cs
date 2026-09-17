using System.Net;
using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Downloads;

namespace OpenSleepMusic.Core.Tests;

public sealed class SleepWorldDownloaderTests
{
    [Fact]
    public async Task StalledBodyTimesOutCleansPartialFileAndContinuesWithNextTrack()
    {
        var world = new SleepWorld("test", "Test", "Test", "T",
            [Track("stalled", "stalled.ogg", "https://example.test/stalled"),
             Track("next", "next.ogg", "https://example.test/next")]);
        using var body = new StalledStream();
        using var client = new HttpClient(new StubHandler(request => request.RequestUri!.AbsolutePath == "/stalled"
            ? new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(body) }
            : Response(HttpStatusCode.OK, [.. "OggS"u8, .. new byte[256]], "audio/ogg")));
        var log = new RecordingLogSink();
        var destination = Path.Combine(Path.GetTempPath(), $"open-sleep-music-tests-{Guid.NewGuid():N}");
        using var guard = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        try
        {
            var result = await new SleepWorldDownloader(client, log, TimeSpan.FromMilliseconds(200))
                .DownloadAsync(world, destination, cancellationToken: guard.Token);

            Assert.Equal(1, result.SkippedCount);
            Assert.Equal(1, result.DownloadedCount);
            Assert.Contains(log.Entries, entry => entry.Contains("stalled") && entry.Contains("timed out"));
            Assert.False(File.Exists(Path.Combine(destination, "test", "stalled.ogg.part")));
            Assert.False(File.Exists(Path.Combine(destination, "test", "stalled.ogg")));
            Assert.True(File.Exists(Path.Combine(destination, "test", "next.ogg")));
        }
        finally { Directory.Delete(destination, true); }
    }

    [Fact]
    public async Task UserCancellationStopsBatchAndRemovesPartialFile()
    {
        var world = new SleepWorld("test", "Test", "Test", "T",
            [Track("stalled", "stalled.ogg", "https://example.test/stalled"),
             Track("next", "next.ogg", "https://example.test/next")]);
        using var body = new StalledStream();
        var requests = 0;
        using var client = new HttpClient(new StubHandler(_ =>
        {
            requests++;
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(body) };
        }));
        var log = new RecordingLogSink();
        var destination = Path.Combine(Path.GetTempPath(), $"open-sleep-music-tests-{Guid.NewGuid():N}");
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        try
        {
            var download = new SleepWorldDownloader(client, log).DownloadAsync(world, destination, cancellationToken: cancellation.Token);
            await body.Stalled.Task.WaitAsync(TimeSpan.FromSeconds(5));
            cancellation.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => download);
            Assert.Equal(1, requests);
            Assert.Empty(log.Entries);
            Assert.Empty(Directory.GetFiles(Path.Combine(destination, "test")));
        }
        finally { Directory.Delete(destination, true); }
    }

    // Send valid initial bytes, then simulate a server that never finishes its response body.
    private sealed class StalledStream : Stream
    {
        private bool _sentHeader;
        public TaskCompletionSource Stalled { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (!_sentHeader)
            {
                _sentHeader = true;
                "OggS"u8.CopyTo(buffer.Span);
                return 4;
            }
            Stalled.TrySetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            return 0;
        }
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void Flush() => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

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

    [Fact]
    public async Task RemovesInvalidExistingFileWhenReplacementIsUnavailable()
    {
        var track = Track("changed", "changed.mp3", "https://example.test/missing") with
        {
            Sha1 = new string('0', 40)
        };
        var world = new SleepWorld("test", "Test", "Test", "T", [track]);
        using var client = new HttpClient(new StubHandler(_ =>
            Response(HttpStatusCode.NotFound, [], "text/plain")));
        var destination = Path.Combine(Path.GetTempPath(), $"open-sleep-music-tests-{Guid.NewGuid():N}");
        var targetDirectory = Path.Combine(destination, world.Id);
        var targetPath = Path.Combine(targetDirectory, track.FileName);

        try
        {
            Directory.CreateDirectory(targetDirectory);
            await File.WriteAllBytesAsync(targetPath, [.. "ID3"u8, .. new byte[256]]);

            var result = await new SleepWorldDownloader(client).DownloadAsync(world, destination);

            Assert.Equal(1, result.SkippedCount);
            Assert.False(File.Exists(targetPath));
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
