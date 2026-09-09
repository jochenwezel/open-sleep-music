namespace OpenSleepMusic.App.Playback;

internal static class AppVolumeBridge
{
    private static double _volume = .3;
    public static event EventHandler<double>? Changed;
    public static double Volume => _volume;
    public static void Set(double volume)
    {
        _volume = Math.Clamp(volume, 0, 1);
        Changed?.Invoke(null, _volume);
    }
}
