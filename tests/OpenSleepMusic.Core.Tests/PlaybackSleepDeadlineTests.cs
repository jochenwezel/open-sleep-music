using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.Core.Tests;

public sealed class PlaybackSleepDeadlineTests
{
    [Fact]
    public void AutomaticTrackTransitionsRemainBlockedAfterDeadlineAndSettingsUpdate()
    {
        var clock = new TestClock();
        var deadline = new PlaybackSleepDeadline(clock);
        deadline.Restart(clock.GetUtcNow().AddMinutes(15));
        Assert.False(deadline.HasElapsed);

        clock.Advance(TimeSpan.FromMinutes(15));
        Assert.True(deadline.HasElapsed);
        // The UI consumes its timer while the native player is still fading out.
        deadline.Update(null);
        Assert.True(deadline.HasElapsed);
        deadline.Update(clock.GetUtcNow().AddMinutes(30));
        Assert.True(deadline.HasElapsed);

        deadline.Restart(clock.GetUtcNow().AddMinutes(30));
        Assert.False(deadline.HasElapsed);
    }

    [Fact]
    public void CancellingTimerBeforeDeadlineDoesNotBlockPlayback()
    {
        var clock = new TestClock();
        var deadline = new PlaybackSleepDeadline(clock);
        deadline.Restart(clock.GetUtcNow().AddMinutes(15));
        deadline.Update(null);
        clock.Advance(TimeSpan.FromMinutes(20));
        Assert.False(deadline.HasElapsed);
    }

    private sealed class TestClock : TimeProvider
    {
        private DateTimeOffset _now = new(2026, 9, 17, 20, 0, 0, TimeSpan.Zero);
        public override DateTimeOffset GetUtcNow() => _now;
        public void Advance(TimeSpan duration) => _now += duration;
    }
}
