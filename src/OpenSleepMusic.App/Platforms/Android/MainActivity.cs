using Android.App;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Views;
using OpenSleepMusic.App.Playback;

namespace OpenSleepMusic.App;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ScreenOrientation = ScreenOrientation.Unspecified,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public override bool DispatchKeyEvent(KeyEvent? e)
    {
        var isVolumeKey = e?.KeyCode is Keycode.VolumeDown or Keycode.VolumeUp;
        if (!isVolumeKey) return base.DispatchKeyEvent(e);
        if (e?.Action == KeyEventActions.Down)
        {
            var manager = (AudioManager?)GetSystemService(AudioService);
            manager?.AdjustStreamVolume(
                Android.Media.Stream.Music,
                e.KeyCode == Keycode.VolumeUp ? Adjust.Raise : Adjust.Lower,
                0);
            var maximum = manager?.GetStreamMaxVolume(Android.Media.Stream.Music) ?? 1;
            var current = manager?.GetStreamVolume(Android.Media.Stream.Music) ?? 0;
            SystemVolumeSnapshot.Publish(maximum <= 0 ? 0 : current / (double)maximum);
        }
        return true;
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        VolumeControlStream = Android.Media.Stream.Music;
    }

    protected override void OnResume()
    {
        base.OnResume();
        VolumeControlStream = Android.Media.Stream.Music;
    }
}
