using Microsoft.Maui.Storage;
using OpenSleepMusic.App;
using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.Core.Tests;

public sealed class PromotedRatingMigrationTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CopiesExistingAuditionChoiceToBothSharedDestinationsOnlyOnce(bool favorite)
    {
        var preferences = new MemoryPreferences();
        var state = new AppStateStore(preferences);
        if (favorite) state.SetFavorite("preselection", "promoted", true);
        else state.SetBlocked("preselection", "promoted", true);
        var worlds = new[] { World("quiet-classics"), World("lullabies") };

        state.MigratePreselectionRatings(worlds);
        foreach (var world in worlds)
        {
            Assert.Equal(favorite, state.IsFavorite(world.Id, "promoted"));
            Assert.Equal(!favorite, state.IsBlocked(world.Id, "promoted"));
        }
        Assert.Equal(favorite, state.IsFavorite("preselection", "promoted"));
        state.SetFavorite("lullabies", "promoted", false);
        state.SetBlocked("lullabies", "promoted", false);
        state.MigratePreselectionRatings(worlds);
        Assert.False(state.IsFavorite("lullabies", "promoted"));
        Assert.False(state.IsBlocked("lullabies", "promoted"));
    }

    [Fact]
    public void KeepsExplicitDestinationChoiceAndDoesNotInventRatings()
    {
        var state = new AppStateStore(new MemoryPreferences());
        state.SetFavorite("preselection", "promoted", true);
        state.SetBlocked("quiet-classics", "promoted", true);
        state.MigratePreselectionRatings([World("quiet-classics"), World("lullabies")]);
        Assert.True(state.IsBlocked("quiet-classics", "promoted"));
        Assert.False(state.IsFavorite("quiet-classics", "promoted"));
        Assert.True(state.IsFavorite("lullabies", "promoted"));

        var empty = new MemoryPreferences();
        new AppStateStore(empty).MigratePreselectionRatings([World("lullabies")]);
        Assert.Empty(empty.Values);
    }

    [Theory]
    [InlineData("preselection")]
    [InlineData("pre-qualify")]
    [InlineData("pre-qualify-licenses")]
    public void DoesNotMigrateIntoPreviewLists(string world)
    {
        var preferences = new MemoryPreferences();
        var state = new AppStateStore(preferences);
        state.SetFavorite("preselection", "promoted", true);
        var before = preferences.Values.Count;
        state.MigratePreselectionRatings([World(world)]);
        Assert.Equal(before, preferences.Values.Count);
    }

    private static SleepWorld World(string id) => new(id, id, "Collection", "🌙",
        [new AudioTrack("promoted", "Music", "Artist", new Uri("https://example.org/music.mp3"),
            new Uri("https://example.org/source"), "CC BY 4.0",
            new Uri("https://creativecommons.org/licenses/by/4.0/"), "music.mp3", 120)]);

    private sealed class MemoryPreferences : IPreferences
    {
        public Dictionary<string, object?> Values { get; } = [];
        public bool ContainsKey(string key) => Values.ContainsKey(key);
        public void Remove(string key) => Values.Remove(key);
        public T Get<T>(string key, T defaultValue) => Values.TryGetValue(key, out var value) ? (T)value! : defaultValue;
        public void Set<T>(string key, T value) => Values[key] = value;
    }
}
