using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.Core.Tests;

public sealed class SleepTimerTests
{
    [Fact]
    public void ConsumeIfElapsed_FiresExactlyOnce()
    {
        var clock = new TestTimeProvider(new DateTimeOffset(2026, 8, 4, 20, 0, 0, TimeSpan.Zero));
        var timer = new SleepTimer(clock);
        timer.Start(TimeSpan.FromMinutes(30));

        clock.Advance(TimeSpan.FromMinutes(29));
        Assert.False(timer.ConsumeIfElapsed());
        clock.Advance(TimeSpan.FromMinutes(1));
        Assert.True(timer.ConsumeIfElapsed());
        Assert.False(timer.ConsumeIfElapsed());
        Assert.False(timer.IsActive);
    }

    private sealed class TestTimeProvider(DateTimeOffset now) : TimeProvider
    {
        private DateTimeOffset _now = now;
        public override DateTimeOffset GetUtcNow() => _now;
        public void Advance(TimeSpan duration) => _now += duration;
    }
}
