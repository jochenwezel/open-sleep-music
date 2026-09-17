namespace OpenSleepMusic.App.Playback;

internal static class SystemVolumeSnapshot
{
    public static event EventHandler<double>? Changed;
    public static event EventHandler? OverlayRequested;
    public static double Value { get; private set; }

    public static void Publish(double value)
    {
        Value = Math.Clamp(value, 0, 1);
        Changed?.Invoke(null, Value);
    }

    public static void Refresh(bool requestOverlay = false)
    {
#if ANDROID
        var manager = (Android.Media.AudioManager?)Platform.AppContext.GetSystemService(Android.Content.Context.AudioService);
        var maximum = manager?.GetStreamMaxVolume(Android.Media.Stream.Music) ?? 1;
        var current = manager?.GetStreamVolume(Android.Media.Stream.Music) ?? 0;
        Publish(maximum <= 0 ? 0 : current / (double)maximum);
#endif
        if (requestOverlay)
        {
            OverlayRequested?.Invoke(null, EventArgs.Empty);
        }
    }
}
