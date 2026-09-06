using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Library;
using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.Core.Tests;

public sealed class TrackPreferenceFilterTests
{
    [Fact]
    public void Apply_UsesAllNonBlockedTracks_WhenThereAreNoFavorites()
    {
        var tracks = Tracks("one", "two", "three");
        var result = TrackPreferenceFilter.Apply(tracks, _ => false, id => id == "two");
        Assert.Equal(["one", "three"], result.Select(item => item.Track.Id));
    }

    [Fact]
    public void Apply_UsesOnlyNonBlockedFavorites_WhenFavoriteExists()
    {
        var tracks = Tracks("one", "two", "three");
        var result = TrackPreferenceFilter.Apply(tracks, id => id is "one" or "two", id => id == "two");
        Assert.Equal(["one"], result.Select(item => item.Track.Id));
    }

    private static LocalLibraryTrack[] Tracks(params string[] ids)
    {
        var world = new SleepWorld("world", "World", "", "", []);
        return ids.Select(id => new LocalLibraryTrack(
            new AudioTrack(id, id, "creator", new("https://example.test/audio"), new("https://example.test/source"), "CC0", new("https://creativecommons.org/publicdomain/zero/1.0/"), $"{id}.mp3", 60),
            world,
            $"C:\\{id}.mp3")).ToArray();
    }
}
