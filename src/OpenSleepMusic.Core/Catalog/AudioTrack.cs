namespace OpenSleepMusic.Core.Catalog;

public sealed record AudioTrack(
    string Id,
    string Title,
    string Creator,
    Uri DownloadUri,
    Uri SourcePageUri,
    string License,
    Uri LicenseUri,
    string FileName,
    double DurationSeconds = 0,
    string? Sha1 = null);
