using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.Core.Tests;

public sealed class PlaybackSleepDeadlineTests
{
    [Theory]
    [InlineData(-1, false, false)]
    [InlineData(0, false, false)]
    [InlineData(60, true, false)]
    [InlineData(60, false, false)]
    [InlineData(60, false, true)]
    public void UserStartKeepsAFutureDeadlineOrStartsAFreshTimerAfterLongPause(int minutes, bool future, bool missing)
    {
        var now = new DateTimeOffset(2026, 11, 20, 20, 0, 0, TimeSpan.Zero);
        DateTimeOffset? previous = missing ? null : now.AddHours(future ? 1 : -24);
        var result = PlaybackSleepDeadline.ForUserStart(minutes, previous, now);
        Assert.Equal(minutes <= 0 ? null : future ? previous : now.AddMinutes(minutes), result);
    }

    [Fact]
    public void UserStartAtTheExactDeadlineRestartsWithoutChangingTheAutomaticLatch()
    {
        var clock = new TestClock();
        var deadline = new PlaybackSleepDeadline(clock);
        deadline.Restart(clock.GetUtcNow());
        Assert.True(deadline.HasElapsed);
        var next = PlaybackSleepDeadline.ForUserStart(30, deadline.EndsAt, clock.GetUtcNow());
        Assert.Equal(clock.GetUtcNow().AddMinutes(30), next);
        Assert.True(deadline.HasElapsed);
        deadline.Restart(next);
        Assert.False(deadline.HasElapsed);
    }

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

    [Fact]
    public void RestartedServiceDoesNotResumeWhenDeadlineElapsedWhileProcessWasGone()
    {
        var clock = new TestClock();
        var savedDeadline = clock.GetUtcNow().AddMinutes(15);
        clock.Advance(TimeSpan.FromMinutes(20));

        var restored = new PlaybackSleepDeadline(clock);
        restored.Restore(savedDeadline, elapsed: false);
        Assert.True(restored.HasElapsed);
        Assert.Equal(savedDeadline, restored.EndsAt);
    }

    [Fact]
    public void RestartedServiceRetainsConsumedDeadlineEvenWhenUiClearedItsTimestamp()
    {
        var restored = new PlaybackSleepDeadline(new TestClock());
        restored.Restore(null, elapsed: true);
        Assert.True(restored.HasElapsed);
        restored.Restart(null);
        Assert.False(restored.HasElapsed);
    }

    [Fact]
    public void RestartedServiceCanResumeBeforeDeadline()
    {
        var clock = new TestClock();
        var savedDeadline = clock.GetUtcNow().AddMinutes(15);
        var restored = new PlaybackSleepDeadline(clock);
        restored.Restore(savedDeadline, elapsed: false);
        Assert.False(restored.HasElapsed);
        clock.Advance(TimeSpan.FromMinutes(15));
        Assert.True(restored.HasElapsed);
    }

    private sealed class TestClock : TimeProvider
    {
        private DateTimeOffset _now = new(2026, 9, 17, 20, 0, 0, TimeSpan.Zero);
        public override DateTimeOffset GetUtcNow() => _now;
        public void Advance(TimeSpan duration) => _now += duration;
    }
}
