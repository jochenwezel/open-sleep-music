using Android.App;
using Android.Content;
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
            SystemVolumeSnapshot.Refresh(requestOverlay: true);
        }
        return true;
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        VolumeControlStream = Android.Media.Stream.Music;
        SystemVolumeSnapshot.Refresh();
        HandlePlaybackIntent(Intent);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        Intent = intent;
        HandlePlaybackIntent(intent);
    }

    protected override void OnResume()
    {
        base.OnResume();
        VolumeControlStream = Android.Media.Stream.Music;
        SystemVolumeSnapshot.Refresh();
    }

    private static void HandlePlaybackIntent(Intent? intent)
    {
        if (intent?.Action != AndroidPlaybackBridge.ActionOpenCurrent) return;
        AndroidPlaybackBridge.RequestOpenCurrent(intent.GetStringExtra("trackId"));
    }
}
