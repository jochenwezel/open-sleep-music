using Microsoft.Maui.Storage;
using OpenSleepMusic.App;
using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.Core.Tests;

public sealed class CollectionShufflePreferenceTests
{
    [Theory]
    [InlineData(7199.999, false)]
    [InlineData(7200, false)]
    [InlineData(7200.001, true)]
    public void DefaultUsesStrictTwoHourBoundary(double seconds, bool expected) =>
        Assert.Equal(expected, PlaybackOrderDefaults.ShuffleByDefault(World("world", Track("one", seconds))));

    [Fact]
    public void DefaultUsesEntireCollectionAndDoesNotPersistAnAutomaticChoice()
    {
        var preferences = new MemoryPreferences();
        var state = new AppStateStore(preferences);
        var world = World("long", Track("one", 3600), Track("two", 3601));
        Assert.True(state.LoadShuffle(world));
        Assert.False(state.LoadShuffle(World("empty")));
        Assert.Empty(preferences.Values);

        // A catalog change can change an unset default, without overriding a user.
        Assert.False(state.LoadShuffle(World("long", Track("one", 3600))));
    }

    [Theory]
    [InlineData(3600, .5, 0, 0, false)]
    [InlineData(3601, .5, 0, 0, true)]
    [InlineData(7201, 1, 500, 500, false)]
    [InlineData(7201, 1, 500, 0, true)]
    public void DefaultUsesListeningDurationAfterSpeedAndTrims(
        double seconds, double speed, int start, int end, bool expected)
    {
        var track = Track("one", seconds) with
        {
            PlaybackSpeed = speed, StartOffsetMilliseconds = start, EndOffsetMilliseconds = end
        };
        Assert.Equal(expected, PlaybackOrderDefaults.ShuffleByDefault(World("world", track)));
    }

    [Fact]
    public void ExplicitChoicesSurviveRestartAndStayIndependentBetweenCollections()
    {
        var preferences = new MemoryPreferences();
        var state = new AppStateStore(preferences);
        var longWorld = World("long", Track("one", 8000));
        var otherLongWorld = World("other-long", Track("one", 8000));
        var shortWorld = World("short", Track("one", 600));
        state.SaveShuffle(longWorld.Id, false);
        state.SaveShuffle(shortWorld.Id, true);

        var restarted = new AppStateStore(preferences);
        Assert.False(restarted.LoadShuffle(longWorld));
        Assert.True(restarted.LoadShuffle(shortWorld));
        Assert.True(restarted.LoadShuffle(otherLongWorld));
        Assert.False(restarted.LoadShuffle(World("other-short", Track("one", 600))));
        Assert.False(restarted.LoadShuffle(longWorld with { Tracks = [Track("one", 16000)] }));
        Assert.True(restarted.LoadShuffle(shortWorld with { Tracks = [] }));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void LegacyExplicitGlobalChoiceIsPreservedUntilACollectionOverridesIt(bool legacyChoice)
    {
        var preferences = new MemoryPreferences();
        preferences.Set("player.shuffle", legacyChoice);
        var state = new AppStateStore(preferences);
        var longWorld = World("long", Track("one", 8000));
        var shortWorld = World("short", Track("one", 600));
        Assert.Equal(legacyChoice, state.LoadShuffle(longWorld));
        Assert.Equal(legacyChoice, state.LoadShuffle(shortWorld));
        state.SaveShuffle(longWorld.Id, !legacyChoice);
        Assert.Equal(!legacyChoice, new AppStateStore(preferences).LoadShuffle(longWorld));
        Assert.Equal(legacyChoice, new AppStateStore(preferences).LoadShuffle(shortWorld));
        Assert.Equal(legacyChoice, preferences.Get("player.shuffle", false));
    }

    private static SleepWorld World(string id, params AudioTrack[] tracks) => new(id, id, id, "", tracks);

    private static AudioTrack Track(string id, double seconds) => new(
        id, id, "Test", new Uri("https://example.test/audio"), new Uri("https://example.test/source"),
        "CC0", new Uri("https://creativecommons.org/publicdomain/zero/1.0/"), id + ".wav", seconds);

    private sealed class MemoryPreferences : IPreferences
    {
        public Dictionary<string, object?> Values { get; } = [];
        public bool ContainsKey(string key) => Values.ContainsKey(key);
        public void Remove(string key) => Values.Remove(key);
        public T Get<T>(string key, T defaultValue) => Values.TryGetValue(key, out var value) ? (T)value! : defaultValue;
        public void Set<T>(string key, T value) => Values[key] = value;
    }
}
