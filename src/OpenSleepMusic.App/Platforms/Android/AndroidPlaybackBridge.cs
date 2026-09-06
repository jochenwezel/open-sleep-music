using Android.Content;
using Android.OS;
using AndroidX.Core.Content;
using OpenSleepMusic.App.Playback;
using OpenSleepMusic.Core.Library;

namespace OpenSleepMusic.App;

internal static class AndroidPlaybackBridge
{
    internal const string ActionLoad = "org.opensleepmusic.action.LOAD";
    internal const string ActionToggle = "org.opensleepmusic.action.TOGGLE";
    internal const string ActionPlay = "org.opensleepmusic.action.PLAY";
    internal const string ActionPause = "org.opensleepmusic.action.PAUSE";
    internal const string ActionNext = "org.opensleepmusic.action.NEXT";
    internal const string ActionPrevious = "org.opensleepmusic.action.PREVIOUS";
    internal const string ActionSeek = "org.opensleepmusic.action.SEEK";
    internal const string ActionSettings = "org.opensleepmusic.action.SETTINGS";
    internal const string ActionStop = "org.opensleepmusic.action.STOP";

    public static event EventHandler<PlaybackSnapshot>? StateChanged;

    public static PlaybackSnapshot Snapshot { get; private set; } =
        new(null, false, TimeSpan.Zero, TimeSpan.Zero);

    public static void LoadAndPlay(
        IReadOnlyList<LocalLibraryTrack> queue,
        LocalLibraryTrack selected,
        double startSeconds,
        bool shuffle,
        bool repeatTrack,
        double volume,
        DateTimeOffset? timerEndUtc)
    {
        var intent = CreateIntent(ActionLoad);
        intent.PutStringArrayListExtra("ids", queue.Select(item => item.Track.Id).ToArray());
        intent.PutStringArrayListExtra("titles", queue.Select(item => item.Track.Title).ToArray());
        intent.PutStringArrayListExtra("creators", queue.Select(item => item.Track.Creator).ToArray());
        intent.PutStringArrayListExtra("paths", queue.Select(item => item.FilePath).ToArray());
        intent.PutExtra("index", Math.Max(0, queue.IndexOf(selected)));
        intent.PutExtra("position", Math.Max(0, startSeconds));
        AddSettings(intent, shuffle, repeatTrack, volume, timerEndUtc);
        ContextCompat.StartForegroundService(Platform.AppContext, intent);
    }

    public static void Toggle() => Send(ActionToggle);
    public static void Play() => Send(ActionPlay);
    public static void Pause() => Send(ActionPause);
    public static void Next() => Send(ActionNext);
    public static void Previous() => Send(ActionPrevious);
    public static void Stop() => Send(ActionStop);

    public static void Seek(double seconds)
    {
        var intent = CreateIntent(ActionSeek);
        intent.PutExtra("position", Math.Max(0, seconds));
        Start(intent);
    }

    public static void UpdateSettings(bool shuffle, bool repeatTrack, double volume, DateTimeOffset? timerEndUtc)
    {
        if (Snapshot.TrackId is null) return;
        var intent = CreateIntent(ActionSettings);
        AddSettings(intent, shuffle, repeatTrack, volume, timerEndUtc);
        Start(intent);
    }

    internal static void Publish(PlaybackSnapshot snapshot)
    {
        Snapshot = snapshot;
        MainThread.BeginInvokeOnMainThread(() => StateChanged?.Invoke(null, snapshot));
    }

    private static void Send(string action) => Start(CreateIntent(action));

    private static Intent CreateIntent(string action) =>
        new Intent(Platform.AppContext, typeof(AndroidPlaybackService)).SetAction(action);

    private static void Start(Intent intent)
    {
        try
        {
            Platform.AppContext.StartService(intent);
        }
        catch (Java.Lang.IllegalStateException)
        {
            ContextCompat.StartForegroundService(Platform.AppContext, intent);
        }
    }

    private static void AddSettings(Intent intent, bool shuffle, bool repeatTrack, double volume, DateTimeOffset? timerEndUtc)
    {
        intent.PutExtra("shuffle", shuffle);
        intent.PutExtra("repeat", repeatTrack);
        intent.PutExtra("volume", Math.Clamp(volume, 0, 1));
        intent.PutExtra("timerEnd", timerEndUtc?.ToUnixTimeMilliseconds() ?? 0);
    }
}
