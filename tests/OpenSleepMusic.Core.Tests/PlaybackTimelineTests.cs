using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.Core.Tests;

public sealed class PlaybackTimelineTests
{
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
