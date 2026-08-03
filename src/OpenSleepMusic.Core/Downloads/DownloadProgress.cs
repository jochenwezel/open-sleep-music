namespace OpenSleepMusic.Core.Downloads;

public sealed record DownloadProgress(int Completed, int Total, string CurrentTitle);
