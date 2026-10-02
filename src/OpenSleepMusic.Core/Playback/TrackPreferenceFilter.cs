using OpenSleepMusic.Core.Library;

namespace OpenSleepMusic.Core.Playback;

public static class TrackPreferenceFilter
{
    public static IReadOnlyList<LocalLibraryTrack> Apply(
        IEnumerable<LocalLibraryTrack> tracks,
        Func<string, bool> isFavorite,
        Func<string, bool> isBlocked,
        bool favoritesOnly)
    {
        var available = tracks.Where(track => !isBlocked(track.Track.Id)).ToArray();
        return favoritesOnly
            ? available.Where(track => isFavorite(track.Track.Id)).ToArray()
            : available;
    }
}
