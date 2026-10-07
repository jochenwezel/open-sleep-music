using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.Core.Tests;

public sealed class PlaybackTimelineTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void MissingPlayerDurationUsesCatalogForPositionAndSeek(double playerDurationSeconds)
    {
        var mediaDuration = PlaybackTimeline.ResolveMediaDuration(TimeSpan.FromSeconds(playerDurationSeconds), 239.885);
        var duration = PlaybackTimeline.Duration(mediaDuration, 1);
        var position = PlaybackTimeline.ToPlaybackTime(TimeSpan.FromSeconds(30), 1);
        Assert.Equal(239.885, duration.TotalSeconds, 3);
        Assert.Equal(30, Math.Min(position.TotalSeconds, duration.TotalSeconds));
        Assert.Equal(30, PlaybackTimeline.ToMediaTime(position, 1, mediaDuration: mediaDuration).TotalSeconds);
        Assert.Equal(239.885, PlaybackTimeline.ToMediaTime(TimeSpan.FromMinutes(10), 1, mediaDuration: mediaDuration).TotalSeconds, 3);
    }

    [Fact]
    public void AvailablePlayerDurationTakesPrecedenceOverCatalogEstimate()
    {
        var actual = TimeSpan.FromSeconds(245);
        Assert.Equal(actual, PlaybackTimeline.ResolveMediaDuration(actual, 239.885));
    }

    [Fact]
    public void CatalogDurationFallbackIsTrimmedBeforeApplyingPlaybackSpeed()
    {
        var mediaDuration = PlaybackTimeline.ResolveMediaDuration(TimeSpan.Zero, 171.312);
        var duration = PlaybackTimeline.Duration(mediaDuration, .8, 6400, 3600);
        Assert.Equal((171.312 - 10) / .8, duration.TotalSeconds, 6);
        Assert.Equal(167.712, PlaybackTimeline.ToMediaTime(duration, .8, 6400, mediaDuration, 3600).TotalSeconds, 6);
        Assert.True(PlaybackTimeline.HasReachedEnd(TimeSpan.FromSeconds(168), mediaDuration, 3600));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.MaxValue)]
    public void InvalidCatalogDurationDoesNotInventAPlaybackLength(double catalogDurationSeconds)
    {
        Assert.Equal(TimeSpan.Zero, PlaybackTimeline.ResolveMediaDuration(TimeSpan.Zero, catalogDurationSeconds));
    }

    [Theory]
    [InlineData(.5)]
    [InlineData(1)]
    [InlineData(1.25)]
    public void BothOffsetsUseOriginalMediaTimeBeforeSpeedAdjustment(double speed)
    {
        var original = TimeSpan.FromSeconds(100);
        var duration = PlaybackTimeline.Duration(original, speed, 6400, 3600);
        Assert.Equal(90 / speed, duration.TotalSeconds, 6);
        Assert.Equal(96.4, PlaybackTimeline.ToMediaTime(duration, speed, 6400, original, 3600).TotalSeconds, 6);
        Assert.Equal(96.4, PlaybackTimeline.ToMediaTime(TimeSpan.FromHours(1), speed, 6400, original, 3600).TotalSeconds, 6);
        Assert.False(PlaybackTimeline.HasReachedEnd(TimeSpan.FromSeconds(96.399), original, 3600));
        Assert.True(PlaybackTimeline.HasReachedEnd(TimeSpan.FromSeconds(96.4), original, 3600));
        Assert.Equal(6.4, PlaybackTimeline.ToMediaTime(TimeSpan.Zero, speed, 6400, original, 3600).TotalSeconds, 6);
    }

    [Fact]
    public void NegativeEndOffsetIsRejected() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => PlaybackTimeline.Duration(TimeSpan.FromSeconds(100), 1, 0, -1));

    [Theory]
    [InlineData(.5)]
    [InlineData(1)]
    [InlineData(1.25)]
    public void OffsetAppliesOnceToStartSeekResumeAndListeningDuration(double speed)
    {
        const int offset = 6400;
        Assert.Equal(6.4, PlaybackTimeline.ToMediaTime(TimeSpan.Zero, speed, offset).TotalSeconds, 6);
        Assert.Equal(TimeSpan.Zero, PlaybackTimeline.ToPlaybackTime(TimeSpan.FromSeconds(6), speed, offset));
        Assert.Equal(TimeSpan.Zero, PlaybackTimeline.ToMediaTime(TimeSpan.FromSeconds(-10), speed, offset) - TimeSpan.FromMilliseconds(offset));

        var displayedPosition = TimeSpan.FromSeconds(20);
        var mediaPosition = PlaybackTimeline.ToMediaTime(displayedPosition, speed, offset);
        Assert.Equal(6.4 + 20 * speed, mediaPosition.TotalSeconds, 6);
        Assert.InRange(Math.Abs((PlaybackTimeline.ToPlaybackTime(mediaPosition, speed, offset) - displayedPosition).Ticks), 0, 2);

        var track = new OpenSleepMusic.Core.Catalog.AudioTrack("offset-test", "Test", "Creator",
            new("https://example.test/audio.mp3"), new("https://example.test/source"), "CC BY-SA 4.0",
            new("https://creativecommons.org/licenses/by-sa/4.0/"), "test.mp3", 171.312,
            PlaybackSpeed: speed, StartOffsetMilliseconds: offset);
        Assert.Equal((171.312 - 6.4) / speed, track.PlaybackDurationSeconds, 6);
    }

    [Fact]
    public void NegativeOffsetsAreRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PlaybackTimeline.ToMediaTime(TimeSpan.Zero, 1, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => PlaybackTimeline.ToPlaybackTime(TimeSpan.Zero, 1, -1));
    }

    [Theory]
    [InlineData(.5)]
    [InlineData(.833333)]
    [InlineData(1)]
    [InlineData(1.25)]
    [InlineData(2)]
    public void SeekingToDisplayedPositionPreservesMediaPosition(double speed)
    {
        var mediaPosition = TimeSpan.FromSeconds(50);
        var displayedPosition = PlaybackTimeline.ToPlaybackTime(mediaPosition, speed);
        var seekPosition = PlaybackTimeline.ToMediaTime(displayedPosition, speed);
        Assert.InRange(Math.Abs((seekPosition - mediaPosition).Ticks), 0, 2);
    }

    [Fact]
    public void SlowedCatalogTrackUsesListeningTimeForBothDurationAndSeek()
    {
        const double speed = .833333;
        var displayedDuration = PlaybackTimeline.ToPlaybackTime(TimeSpan.FromMinutes(5), speed);
        Assert.Equal(360, displayedDuration.TotalSeconds, precision: 3);

        var mediaSeek = PlaybackTimeline.ToMediaTime(TimeSpan.FromSeconds(60), speed);
        Assert.Equal(50, mediaSeek.TotalSeconds, precision: 3);
        var displayedSeek = PlaybackTimeline.ToPlaybackTime(mediaSeek, speed);
        Assert.Equal(60, displayedSeek.TotalSeconds, precision: 3);
    }
}
