using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.Core.Tests;

public sealed class PlaybackTimelineTests
{
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
