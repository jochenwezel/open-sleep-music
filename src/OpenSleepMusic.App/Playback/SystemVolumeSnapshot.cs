namespace OpenSleepMusic.App.Playback;

internal static class SystemVolumeSnapshot
{
    public static event EventHandler<double>? Changed;
    public static void Publish(double value) => Changed?.Invoke(null, Math.Clamp(value, 0, 1));
}
