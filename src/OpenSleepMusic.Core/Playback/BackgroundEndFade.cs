namespace OpenSleepMusic.Core.Playback;

public static class BackgroundEndFade
{
    public const double DurationMilliseconds = 600;

    public static double VolumeFactor(TimeSpan position, TimeSpan duration)
    {
        var length = duration.TotalMilliseconds;
        if (length <= 0) return 0;
        var remaining = Math.Max(0, length - Math.Max(0, position.TotalMilliseconds));
        return Math.Clamp(remaining / Math.Min(DurationMilliseconds, length), 0, 1);
    }
}
