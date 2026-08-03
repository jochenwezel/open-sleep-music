namespace OpenSleepMusic.Core.Downloads;

public sealed record DownloadBatchResult(int DownloadedCount, int ExistingCount, int SkippedCount)
{
    public int AvailableCount => DownloadedCount + ExistingCount;
}
