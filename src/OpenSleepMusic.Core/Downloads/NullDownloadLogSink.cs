namespace OpenSleepMusic.Core.Downloads;

public sealed class NullDownloadLogSink : IDownloadLogSink
{
    public Task WriteSkippedAsync(string trackId, Uri uri, string reason, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
