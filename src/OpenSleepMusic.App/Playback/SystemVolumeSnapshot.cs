namespace OpenSleepMusic.App.Playback;

internal static class SystemVolumeSnapshot
{
    public static event EventHandler<double>? Changed;
    public static double Value { get; private set; }

    public static void Publish(double value)
    {
        Value = Math.Clamp(value, 0, 1);
        Changed?.Invoke(null, Value);
    }
}
