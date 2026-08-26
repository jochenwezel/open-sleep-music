using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.Core.Library;

public sealed record SleepWorldLibraryStatus(
    SleepWorld SleepWorld,
    int AvailableTrackCount,
    long SizeBytes)
{
    public bool HasDownloads => AvailableTrackCount > 0;

    public bool IsComplete => AvailableTrackCount == SleepWorld.Tracks.Count;
}
