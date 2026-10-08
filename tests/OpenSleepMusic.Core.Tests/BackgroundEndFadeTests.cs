using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.Core.Tests;

public sealed class BackgroundEndFadeTests
{
    [Theory]
    [InlineData(10000, 9000, 1)]
    [InlineData(10000, 9400, 1)]
    [InlineData(10000, 9700, .5)]
    [InlineData(10000, 10000, 0)]
    [InlineData(10000, 11000, 0)]
    [InlineData(300, 0, 1)]
    [InlineData(300, 150, .5)]
    [InlineData(300, 300, 0)]
    [InlineData(0, 0, 0)]
    [InlineData(-1, 0, 0)]
    [InlineData(10000, -600, 1)]
    public void FadeEndsWithPrimaryAndNeverUsesNegativeInterval(double duration, double position, double expected)
        => Assert.Equal(expected, BackgroundEndFade.VolumeFactor(
            TimeSpan.FromMilliseconds(position), TimeSpan.FromMilliseconds(duration)), 6);
}
