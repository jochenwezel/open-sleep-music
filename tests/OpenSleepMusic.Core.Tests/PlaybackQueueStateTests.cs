using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Library;
using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.Core.Tests;

public sealed class PlaybackQueueStateTests
{
    [Fact]
    public void BrowsingAnotherOrEmptyCollectionDoesNotReplacePlayingQueue()
    {
        var first = Track("forest", "first");
        var second = Track("forest", "second");
        var rain = Track("rain", "rain");
        var state = new PlaybackQueueState();
        state.Select("forest", [first, second]);
        state.Activate("forest", state.SelectedTracks);

        state.Select("rain", [rain]);
        Assert.Equal("rain", state.SelectedWorldId);
        Assert.Equal("forest", state.ActiveWorldId);
        Assert.Equal(second, state.ActiveTracks[PlaybackQueue.MoveSequential(state.ActiveTracks.Count, 0, 1)]);

        state.Select("empty", []);
        Assert.Empty(state.SelectedTracks);
        Assert.Equal(2, state.ActiveTracks.Count);
        Assert.Equal("forest", state.ActiveWorldId);
    }

    [Fact]
    public void ExplicitPlaybackSwitchesQueueButPreferenceRefreshDoesNotChangeBrowsing()
    {
        var first = Track("forest", "first");
        var second = Track("forest", "second");
        var rain = Track("rain", "rain");
        var state = new PlaybackQueueState();
        state.Activate("forest", [first, second]);
        state.Select("rain", [rain]);
        state.Activate("forest", [second]);
        Assert.Equal("rain", state.SelectedWorldId);
        Assert.Equal(rain, Assert.Single(state.SelectedTracks));
        Assert.Equal(second, Assert.Single(state.ActiveTracks));

        state.Activate("rain", state.SelectedTracks);
        Assert.Equal("rain", state.ActiveWorldId);
        Assert.Equal(rain, Assert.Single(state.ActiveTracks));
        state.ClearPlayback();
        Assert.Empty(state.ActiveTracks);
        Assert.Equal(rain, Assert.Single(state.SelectedTracks));
    }

    private static LocalLibraryTrack Track(string worldId, string id)
    {
        var track = new AudioTrack(id, id, "Creator", new Uri("https://example.test/audio"),
            new Uri("https://example.test/source"), "CC0",
            new Uri("https://creativecommons.org/publicdomain/zero/1.0/"), id + ".ogg");
        var world = new SleepWorld(worldId, worldId, worldId, "", [track]);
        return new LocalLibraryTrack(track, world, Path.Combine(worldId, track.FileName));
    }
}
