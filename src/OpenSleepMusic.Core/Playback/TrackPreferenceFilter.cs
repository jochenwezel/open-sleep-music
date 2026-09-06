using OpenSleepMusic.Core.Library;

namespace OpenSleepMusic.Core.Playback;

public static class TrackPreferenceFilter
{
    public static IReadOnlyList<LocalLibraryTrack> Apply(
        IEnumerable<LocalLibraryTrack> tracks,
        Func<string, bool> isFavorite,
        Func<string, bool> isBlocked)
    {
        var available = tracks.Where(track => !isBlocked(track.Track.Id)).ToArray();
        var favorites = available.Where(track => isFavorite(track.Track.Id)).ToArray();
        return favorites.Length > 0 ? favorites : available;
    }
}
