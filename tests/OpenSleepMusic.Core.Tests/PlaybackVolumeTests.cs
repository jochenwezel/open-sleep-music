using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.Core.Tests;

public sealed class PlaybackVolumeTests
{
    [Theory]
    [InlineData(.1, 6, .6)]
    [InlineData(.7, 1, .7)]
    [InlineData(.5, 6, 1)]
    [InlineData(-1, 6, 0)]
    public void ApplyGain_CompensatesQuietTracksWithoutExceedingFullVolume(
        double configuredVolume,
        double gain,
        double expected) =>
        Assert.Equal(expected, PlaybackVolume.ApplyGain(configuredVolume, gain), 6);
}
