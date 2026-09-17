using OpenSleepMusic.Core.Library;

namespace OpenSleepMusic.Core.Playback;

/// <summary>Separates browsing a collection from the queue currently used by the player.</summary>
public sealed class PlaybackQueueState
{
    public string? SelectedWorldId { get; private set; }
    public IReadOnlyList<LocalLibraryTrack> SelectedTracks { get; private set; } = [];
    public string? ActiveWorldId { get; private set; }
    public IReadOnlyList<LocalLibraryTrack> ActiveTracks { get; private set; } = [];

    public void Select(string? worldId, IEnumerable<LocalLibraryTrack> tracks)
    {
        SelectedWorldId = worldId;
        SelectedTracks = tracks.ToArray();
    }

    public void Activate(string worldId, IEnumerable<LocalLibraryTrack> tracks)
    {
        ActiveWorldId = worldId;
        ActiveTracks = tracks.ToArray();
    }

    public void ClearPlayback()
    {
        ActiveWorldId = null;
        ActiveTracks = [];
    }
}
