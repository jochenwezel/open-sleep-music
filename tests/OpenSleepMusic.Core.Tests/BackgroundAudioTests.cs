using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.Core.Tests;

public sealed class BackgroundAudioTests
{
    private static AudioTrack Track(string id) => new(id, id, "Creator",
        new Uri("https://example.org/" + id + ".mp3"), new Uri("https://example.org/" + id),
        "CC0 1.0", new Uri("https://creativecommons.org/publicdomain/zero/1.0/"), id + ".mp3", 30);
    private static SleepWorld World(params AudioTrack[] tracks) => new("forest", "Forest", "", "", tracks);

    [Fact]
    public void BackgroundDownloadsDoNotChangeVisibleCollectionAndReuseCanonicalBytes()
    {
        var source = Track("crickets") with { Sha1 = new string('a', 40), PlaybackSpeed = .8 };
        var main = Track("piano") with { BackgroundAudio = new("crickets", .15, 1000, 2000) };
        var world = World(main);
        SleepWorld[] catalog = [world, new("ambient", "Ambient", "", "", [source])];
        BackgroundAudioCatalog.Validate(catalog);
        var downloads = BackgroundAudioCatalog.IncludeDownloads(world, catalog);
        Assert.Single(world.Tracks);
        Assert.Equal(2, downloads.Tracks.Count);
        var background = downloads.Tracks[1];
        Assert.Equal(source.DownloadUri, background.DownloadUri);
        Assert.Equal(source.Sha1, background.Sha1);
        Assert.Equal(1, background.PlaybackSpeed);
        Assert.Equal(.15, background.VolumeGain);
        Assert.Equal(1000, background.StartOffsetMilliseconds);
        Assert.Equal(2000, background.EndOffsetMilliseconds);
        Assert.Equal(2, BackgroundAudioCatalog.IncludeDownloads(World(main, source), catalog).Tracks.Count);
    }

    [Theory]
    [InlineData("missing", .2, 0, 0)]
    [InlineData("piano", .2, 0, 0)]
    [InlineData("crickets", -1, 0, 0)]
    [InlineData("crickets", 1.1, 0, 0)]
    [InlineData("crickets", .2, -1, 0)]
    [InlineData("crickets", .2, 10000, 20000)]
    public void InvalidBackgroundIsRejected(string id, double volume, int start, int end)
    {
        var main = Track("piano") with { BackgroundAudio = new(id, volume, start, end) };
        Assert.Throws<InvalidDataException>(() => BackgroundAudioCatalog.Validate([World(main, Track("crickets"))]));
    }

    [Fact]
    public void NestedAndUncheckedPreviewBackgroundsAreRejected()
    {
        var main = Track("piano") with { BackgroundAudio = new("crickets") };
        var nested = Track("crickets") with { BackgroundAudio = new("piano") };
        Assert.Throws<InvalidDataException>(() => BackgroundAudioCatalog.Validate([World(main, nested)]));
        Assert.Throws<InvalidDataException>(() => BackgroundAudioCatalog.Validate([
            World(main), new("preselection", "Preview", "", "", [Track("crickets") with { LicenseReviewStatus = "unchecked" }])]));
    }

    [Fact]
    public void ReviewedPreviewBackgroundIsAllowedOnlyInPreviewBuilds()
    {
        var main = Track("piano") with { BackgroundAudio = new("crickets") };
        SleepWorld[] catalog = [World(main), new("preselection", "Preview", "", "", [Track("crickets")])];
        if (BuildChannel.IsPreview) BackgroundAudioCatalog.Validate(catalog);
        else Assert.Throws<InvalidDataException>(() => BackgroundAudioCatalog.Validate(catalog));
    }

    [Fact]
    public void PreviewBackgroundMustRetainItsApprovedRecordingLicense()
    {
        if (!BuildChannel.IsPreview) return;
        var source = BuiltInCatalog.SleepWorlds.Single(world => world.Id == "preselection").Tracks
            .Single(track => track.Id == "candidate-meadow-night-crickets-sardin") with { License = "CC BY 4.0" };
        var main = Track("piano") with { BackgroundAudio = new(source.Id) };
        Assert.Throws<InvalidDataException>(() => BackgroundAudioCatalog.Validate([
            World(main), new("preselection", "Preview", "", "", [source])]));
    }

    [Theory]
    [InlineData("candidate-romanza-ten-string-leon-egea")]
    [InlineData("candidate-recuerdos-de-la-alhambra")]
    public void GuitarCricketAuditionIsPreviewOnlyAndUsesOneHalfGain(string id)
    {
        var worlds = BuiltInCatalog.SleepWorlds;
        var classics = worlds.Single(world => world.Id == "quiet-classics");
        var lullabies = worlds.Single(world => world.Id == "lullabies");
        var primary = classics.Tracks.Single(track => track.Id == id);
        Assert.Same(primary, lullabies.Tracks.Single(track => track.Id == id));
        if (!BuildChannel.IsPreview)
        {
            Assert.Null(primary.BackgroundAudio);
            Assert.Null(BackgroundAudioCatalog.Resolve(primary, worlds));
            return;
        }
        var source = worlds.Single(world => world.Id == "preselection").Tracks
            .Single(track => track.Id == "candidate-meadow-night-crickets-sardin");
        var background = Assert.IsType<AudioTrack>(BackgroundAudioCatalog.Resolve(primary, worlds));
        Assert.Equal(.5, source.VolumeGain);
        Assert.Equal(.5, background.VolumeGain);
        Assert.Equal(1, background.PlaybackSpeed);
        Assert.Equal(0, background.StartOffsetMilliseconds);
        Assert.Equal(0, background.EndOffsetMilliseconds);
        Assert.Equal(source.DownloadUri, background.DownloadUri);
        Assert.Equal(source.Sha1, background.Sha1);
        Assert.True(background.PlaybackDurationSeconds > primary.PlaybackDurationSeconds);
        foreach (var world in new[] { classics, lullabies })
        {
            Assert.DoesNotContain(world.Tracks, track => track.Id == source.Id);
            Assert.Single(BackgroundAudioCatalog.IncludeDownloads(world, worlds).Tracks, track => track.Id == source.Id);
        }
    }

    [Fact]
    public void SessionRoundTripAndLoopPositionUseTrimmedOriginalDuration()
    {
        var source = new BackgroundPlaybackSource("crickets.mp3", .2, 1000, 2000, 30);
        Assert.Equal(source, BackgroundPlaybackSource.FromJson(source.ToJson()));
        Assert.Equal(5, source.LoopPosition(59));
        Assert.Null(BackgroundPlaybackSource.FromJson("invalid"));
        Assert.Null(BackgroundPlaybackSource.FromJson(null));
        Assert.Null(BackgroundPlaybackSource.FromJson((source with { Volume = -1 }).ToJson()));
        Assert.Null(BackgroundPlaybackSource.FromJson((source with { EndOffsetMilliseconds = 30000 }).ToJson()));
    }
}
