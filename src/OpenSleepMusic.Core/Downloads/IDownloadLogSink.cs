namespace OpenSleepMusic.Core.Downloads;

public interface IDownloadLogSink
{
    Task WriteSkippedAsync(string trackId, Uri uri, string reason, CancellationToken cancellationToken);
}
