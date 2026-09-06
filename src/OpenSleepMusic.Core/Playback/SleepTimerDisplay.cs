namespace OpenSleepMusic.Core.Playback;

public static class SleepTimerDisplay
{
    public static string FormatRemaining(TimeSpan remaining)
    {
        if (remaining <= TimeSpan.Zero) return "noch 0 Sek.";
        if (remaining <= TimeSpan.FromMinutes(1))
        {
            return $"noch {Math.Max(1, Math.Ceiling(remaining.TotalSeconds)):0} Sek.";
        }
        return $"noch {Math.Ceiling(remaining.TotalMinutes):0} Min.";
    }

    public static double FadeVolume(double configuredVolume, double progress) =>
        Math.Clamp(configuredVolume, 0, 1) * (1 - Math.Clamp(progress, 0, 1));
}
