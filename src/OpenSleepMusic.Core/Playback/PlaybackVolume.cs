namespace OpenSleepMusic.Core.Playback;

public static class PlaybackVolume
{
    public static double ApplyGain(double configuredVolume, double gain)
    {
        var adjusted = Math.Clamp(configuredVolume, 0, 1) * Math.Max(0, gain);
        return Math.Clamp(adjusted, 0, 1);
    }
}
