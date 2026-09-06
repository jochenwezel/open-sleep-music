using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.Core.Tests;

public sealed class BuiltInCatalogTests
{
    [Fact]
    public void CatalogIsLargeConsistentAndFullyAttributed()
    {
        var worlds = BuiltInCatalog.SleepWorlds;
        var tracks = worlds.SelectMany(world => world.Tracks).ToArray();

        Assert.Equal(5, worlds.Count);
        Assert.True(tracks.Length >= 140, $"Expected at least 140 tracks, found {tracks.Length}.");
        Assert.True(BuiltInCatalog.TotalDuration >= TimeSpan.FromHours(15));
        Assert.All(worlds, world => Assert.NotEmpty(world.Tracks));
        Assert.DoesNotContain(tracks, track => track.Title.Contains("Preludes, Op. 28", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(worlds, world => world.Id == "brown-noise");

        Assert.Equal(tracks.Length, tracks.Select(track => track.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(tracks.Length, tracks.Select(track => track.FileName).Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.Equal(tracks.Length, tracks.Select(track => track.DownloadUri).Distinct().Count());

        Assert.All(tracks, track =>
        {
            Assert.Equal(Uri.UriSchemeHttps, track.DownloadUri.Scheme);
            Assert.Equal(Uri.UriSchemeHttps, track.SourcePageUri.Scheme);
            Assert.Equal(Uri.UriSchemeHttps, track.LicenseUri.Scheme);
            Assert.True(track.DurationSeconds > 0, $"{track.Id} has no duration.");
            Assert.False(string.IsNullOrWhiteSpace(track.Creator));
            Assert.False(string.IsNullOrWhiteSpace(track.License));
            if (!string.IsNullOrWhiteSpace(track.Sha1))
            {
                Assert.Matches("^[0-9a-f]{40}$", track.Sha1);
            }
        });
    }

    [Fact]
    public void CatalogContainsNoKnownHighImpactStormRecordings()
    {
        var titles = BuiltInCatalog.SleepWorlds
            .SelectMany(world => world.Tracks)
            .Select(track => track.Title)
            .ToArray();

        Assert.DoesNotContain(titles, title => title.Contains("thunder", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(titles, title => title.Contains("storm", StringComparison.OrdinalIgnoreCase));
    }
}
