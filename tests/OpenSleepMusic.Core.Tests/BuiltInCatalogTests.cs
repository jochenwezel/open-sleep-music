using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.Core.Tests;

public sealed class BuiltInCatalogTests
{
    [Fact]
    public void RemainingMazurkasUseOneAndAHalfTimesListeningDuration()
    {
        var world = Assert.Single(BuiltInCatalog.SleepWorlds, world => world.Id == "quiet-classics");
        var mazurkas = world.Tracks.Where(track => track.Title.Contains("Mazurka", StringComparison.OrdinalIgnoreCase)).ToArray();
        Assert.NotEmpty(mazurkas);
        Assert.All(mazurkas, track =>
        {
            Assert.Equal(1 / 1.5, track.PlaybackSpeed, 6);
            Assert.InRange(track.PlaybackDurationSeconds / track.DurationSeconds, 1.49999, 1.50001);
        });
        Assert.All(world.Tracks.Except(mazurkas), track => Assert.Equal(1d, track.PlaybackSpeed));
    }

    [Fact]
    public void CatalogExcludesPersistentlyBlacklistedTracks()
    {
        using var stream = typeof(BuiltInCatalogTests).Assembly.GetManifestResourceStream("OpenSleepMusic.Core.Tests.media-catalog-blacklist.json");
        Assert.NotNull(stream);
        var ids = System.Text.Json.JsonSerializer.Deserialize<string[]>(stream!)!;
        Assert.NotEmpty(ids);
        Assert.Equal(ids.Length, ids.Distinct(StringComparer.Ordinal).Count());
        var tracks = BuiltInCatalog.SleepWorlds.SelectMany(world => world.Tracks);
        Assert.All(ids, id => Assert.DoesNotContain(tracks, track => track.Id == id));
    }

    [Fact]
    public void CatalogIsLargeConsistentAndFullyAttributed()
    {
        var worlds = BuiltInCatalog.SleepWorlds;
        var tracks = worlds.SelectMany(world => world.Tracks).ToArray();

        Assert.Equal(BuildChannel.IsPreview ? 8 : 6, worlds.Count);
        Assert.True(tracks.Length >= 134, $"Expected at least 134 tracks, found {tracks.Length}.");
        Assert.True(BuiltInCatalog.TotalDuration >= TimeSpan.FromHours(15));
        Assert.All(worlds.Where(world => world.Id is not ("preselection" or "pre-qualify")), world => Assert.NotEmpty(world.Tracks));
        if (BuildChannel.IsPreview)
        {
            var preselection = Assert.Single(worlds, world => world.Id == "preselection");
            Assert.Equal("Vorauswahl", preselection.Name);
            Assert.Equal(50, preselection.Tracks.Count);
            var references = Assert.Single(worlds, world => world.Id == "pre-qualify");
            Assert.Empty(references.Tracks);
            Assert.Equal(20, references.ExternalReferences!.Count);
        }
        else
        {
            Assert.DoesNotContain(worlds, world => world.Id == "preselection");
            Assert.DoesNotContain(worlds, world => world.Id == "pre-qualify" || world.ExternalReferences is { Count: > 0 });
        }
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
            Assert.True(track.VolumeGain > 0, $"{track.Id} has an invalid volume gain.");
            Assert.InRange(track.PlaybackSpeed, .5, 2);
            Assert.False(string.IsNullOrWhiteSpace(track.Creator));
            Assert.False(string.IsNullOrWhiteSpace(track.License));
            Assert.DoesNotContain("BY-NC", track.License, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("BY-ND", track.License, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(track.ArtworkUri);
            Assert.Equal(Uri.UriSchemeHttps, track.ArtworkUri!.Scheme);
            Assert.Contains("/releases/download/artwork-v3/", track.ArtworkUri.AbsoluteUri, StringComparison.Ordinal);
            Assert.Matches("^[a-z0-9_-]+\\.png$", track.ArtworkFileName!);
            Assert.Matches("^[0-9a-f]{64}$", track.ArtworkSha256!);
            Assert.NotNull(track.FallbackMotifUri);
            Assert.Contains("/releases/download/artwork-v2/", track.FallbackMotifUri!.AbsoluteUri, StringComparison.Ordinal);
            Assert.Matches("^fallback_[a-z0-9_-]+\\.png$", track.FallbackMotifFileName!);
            Assert.Matches("^[0-9a-f]{64}$", track.FallbackMotifSha256!);
            Assert.NotEqual(track.ArtworkFileName, track.FallbackMotifFileName);
            if (!string.IsNullOrWhiteSpace(track.Sha1))
            {
                Assert.Matches("^[0-9a-f]{40}$", track.Sha1);
            }
        });

        var instrumentalWorlds = worlds.Where(world => world.Id is "quiet-classics" or "lullabies");
        Assert.All(instrumentalWorlds.SelectMany(world => world.Tracks), track =>
        {
            Assert.NotNull(track.Instrumentation);
            Assert.NotEmpty(track.Instrumentation!);
            Assert.False(string.IsNullOrWhiteSpace(track.EnsembleType));
        });
    }

    [Fact]
    public void QuietClassicsRetainsReviewedGentleInstrumentDiversity()
    {
        var world = Assert.Single(BuiltInCatalog.SleepWorlds, world => world.Id == "quiet-classics");
        // The harp/recorder pilot was blacklisted after field feedback.
        Assert.True(world.Tracks.Count(track => track.Instrumentation?.Contains("cello") == true) >= 2);
        Assert.Contains(world.Tracks, track =>
            track.Id == "haydn-cello-concerto-no-1-adagio" &&
            track.Instrumentation?.Contains("strings") == true);
        Assert.DoesNotContain(world.Tracks, track => track.Id == "vivaldi-cello-concerto-rv-413-largo");
    }

    [Fact]
    public void CatalogContainsReviewedToddlerLullabyCollection()
    {
        var world = Assert.Single(BuiltInCatalog.SleepWorlds, world => world.Id == "lullabies");

        Assert.Equal("Schlaflieder für Kleine", world.Name);
        Assert.Equal(4, world.Tracks.Count);
        Assert.Equal(
            [
                "burgmuller-berceuse-op-109-no-7",
                "faure-berceuse-op-56-no-1",
                "music-box-guten-abend-gute-nacht",
                "music-box-schlafe-mein-prinzchen"
            ],
            world.Tracks.Select(track => track.Id).Order(StringComparer.Ordinal));
        Assert.All(world.Tracks, track =>
        {
            Assert.Equal(1 / 1.2, track.PlaybackSpeed, 5);
            Assert.DoesNotContain("vocal", track.Title, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("song", track.Title, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(track.SongMotifUri);
            Assert.Equal(Uri.UriSchemeHttps, track.SongMotifUri!.Scheme);
            Assert.Contains("/releases/download/artwork-v1/", track.SongMotifUri.AbsoluteUri, StringComparison.Ordinal);
            Assert.Matches("^[a-z0-9_-]+\\.png$", track.SongMotifFileName!);
            Assert.Matches("^[0-9a-f]{64}$", track.SongMotifSha256!);
        });
        Assert.All(
            world.Tracks.Where(track => track.Id.StartsWith("music-box-", StringComparison.Ordinal)),
            track =>
            {
                Assert.EndsWith(".mp3", track.FileName, StringComparison.OrdinalIgnoreCase);
                Assert.Contains("/transcoded/", track.DownloadUri.AbsoluteUri, StringComparison.Ordinal);
            });
    }

    [Fact]
    public void ForestTracksHaveUniqueTitleSpecificTropicalMotifs()
    {
        var tracks = Assert.Single(BuiltInCatalog.SleepWorlds, world => world.Id == "forest").Tracks;

        Assert.All(tracks, track =>
        {
            Assert.NotNull(track.SongMotifUri);
            Assert.Contains("/releases/download/artwork-v2/", track.SongMotifUri!.AbsoluteUri, StringComparison.Ordinal);
            Assert.Matches("^forest_[a-z0-9_]+\\.png$", track.SongMotifFileName!);
            Assert.Matches("^[0-9a-f]{64}$", track.SongMotifSha256!);
            Assert.NotEqual(track.ArtworkFileName, track.SongMotifFileName);
            Assert.NotEqual(track.FallbackMotifFileName, track.SongMotifFileName);
        });
        Assert.Equal(tracks.Count, tracks.Select(track => track.SongMotifFileName).Distinct(StringComparer.Ordinal).Count());
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
