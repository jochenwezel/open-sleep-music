using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.Core.Playback;

public static class PlaybackOrderDefaults
{
    // Use the complete collection, before download/favorite/block filtering.
    public static bool ShuffleByDefault(SleepWorld world) =>
        world.Tracks.Sum(track => track.PlaybackDurationSeconds) > TimeSpan.FromHours(2).TotalSeconds;
}
