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

    [Theory]
    [InlineData(60, "noch 60 Sek.")]
    [InlineData(59.1, "noch 60 Sek.")]
    [InlineData(1, "noch 1 Sek.")]
    [InlineData(60.1, "noch 2 Min.")]
    public void RemainingTimeSwitchesToSecondsForTheLastMinute(double seconds, string expected) =>
        Assert.Equal(expected, SleepTimerDisplay.FormatRemaining(TimeSpan.FromSeconds(seconds)));

    [Theory]
    [InlineData(.8, 0, .8)]
    [InlineData(.8, .5, .4)]
    [InlineData(.8, 1, 0)]
    public void FadeVolumePreservesConfiguredVolume(double volume, double progress, double expected) =>
        Assert.Equal(expected, SleepTimerDisplay.FadeVolume(volume, progress), precision: 6);

    private sealed class TestTimeProvider(DateTimeOffset now) : TimeProvider
    {
        private DateTimeOffset _now = now;
        public override DateTimeOffset GetUtcNow() => _now;
        public void Advance(TimeSpan duration) => _now += duration;
    }
}
