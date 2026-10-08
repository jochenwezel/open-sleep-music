using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.Core.Tests;

public sealed class SharedCatalogTests
{
    [Fact]
    public void LittleOnesUsePersistedMixedOrderIncludingSharedRecordings()
    {
        var world = BuiltInCatalog.SleepWorlds.Single(world => world.Id == "lullabies");
        Assert.NotNull(world.TrackOrder);
        Assert.Equal(world.Tracks.Select(track => track.Id), world.TrackOrder);
        Assert.Equal(7, world.TrackOrder.Count);
        var titles = world.Tracks.Select(track => track.Title).ToArray();
        Assert.False(titles.SequenceEqual(titles.Order(StringComparer.OrdinalIgnoreCase)));

        using var stream = typeof(BuiltInCatalog).Assembly.GetManifestResourceStream("OpenSleepMusic.Core.Catalog.media-catalog.json")!;
        using var json = System.Text.Json.JsonDocument.Parse(stream);
        var rawWorld = json.RootElement.GetProperty("sleepWorlds").EnumerateArray()
            .Single(entry => entry.GetProperty("id").GetString() == "lullabies");
        var canonicalIds = rawWorld.GetProperty("tracks").EnumerateArray()
            .Select(track => track.GetProperty("id").GetString()).ToArray();
        Assert.Equal(world.TrackOrder.Where(id => canonicalIds.Contains(id)), canonicalIds);
    }

    [Theory]
    [InlineData("candidate-romanza-ten-string-leon-egea", "CC BY-SA 4.0", 6400)]
    [InlineData("candidate-recuerdos-de-la-alhambra", "CC BY-SA 3.0", 0)]
    [InlineData("candidate-barrios-julia-florida-edson-lopes", "CC BY 3.0", 0)]
    public void SelectedRecordingsShareBothProductionCollectionsAndRetainApprovedDelivery(string id, string license, int offset)
    {
        var classics = BuiltInCatalog.SleepWorlds.Single(world => world.Id == "quiet-classics");
        var lullabies = BuiltInCatalog.SleepWorlds.Single(world => world.Id == "lullabies");
        var track = Assert.Single(classics.Tracks, track => track.Id == id);
        Assert.Same(track, Assert.Single(lullabies.Tracks, track => track.Id == id));
        Assert.Equal(new[] { "lullabies" }, track.AdditionalWorldIds);
        Assert.Equal("verified", track.LicenseReviewStatus);
        Assert.Equal(license, track.License);
        Assert.Equal(offset, track.StartOffsetMilliseconds);
        Assert.EndsWith(".mp3", track.FileName);
        Assert.Equal(1d, track.PlaybackSpeed);
        Assert.DoesNotContain(BuiltInCatalog.SleepWorlds.Where(world => world.Id == "preselection").SelectMany(world => world.Tracks),
            track => track.Id == id);

        using var stream = typeof(BuiltInCatalog).Assembly.GetManifestResourceStream("OpenSleepMusic.Core.Catalog.media-catalog.json")!;
        using var json = System.Text.Json.JsonDocument.Parse(stream);
        var canonical = json.RootElement.GetProperty("sleepWorlds").EnumerateArray()
            .SelectMany(world => world.GetProperty("tracks").EnumerateArray()).ToArray();
        Assert.Single(canonical, entry => entry.GetProperty("id").GetString() == id);
        Assert.Equal(canonical.Length, canonical.Select(entry => entry.GetProperty("id").GetString()).Distinct().Count());
        Assert.Equal(canonical.Length, canonical.Select(entry => entry.GetProperty("fileName").GetString()).Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.Equal(canonical.Length, canonical.Select(entry => entry.GetProperty("downloadUri").GetString()).Distinct().Count());
    }

    [Fact]
    public void ClairDeLuneRemainsInClassicsAfterLittleOnesRemoval()
    {
        const string id = "candidate-clair-de-lune-claude-debussy-suite-bergamasque";
        var classics = BuiltInCatalog.SleepWorlds.Single(world => world.Id == "quiet-classics");
        var lullabies = BuiltInCatalog.SleepWorlds.Single(world => world.Id == "lullabies");
        var track = Assert.Single(classics.Tracks, track => track.Id == id);
        Assert.Equal("verified", track.LicenseReviewStatus);
        Assert.Equal("CC BY 3.0", track.License);
        Assert.DoesNotContain(lullabies.Tracks, track => track.Id == id);
        Assert.DoesNotContain(id, lullabies.TrackOrder!);
        Assert.DoesNotContain("lullabies", track.AdditionalWorldIds ?? []);
    }
}
