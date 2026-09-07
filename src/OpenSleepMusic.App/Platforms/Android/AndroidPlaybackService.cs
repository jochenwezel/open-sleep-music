using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.Media.Session;
using Android.OS;
using OpenSleepMusic.App.Playback;
using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.App;

[Service(Exported = false, ForegroundServiceType = ForegroundService.TypeMediaPlayback)]
internal sealed class AndroidPlaybackService : Service, AudioManager.IOnAudioFocusChangeListener
{
    private const int NotificationId = 4107;
    private const string ChannelId = "open-sleep-music-playback";
    private readonly List<QueueItem> _queue = [];
    private MediaPlayer? _player;
    private MediaSession? _session;
    private AudioManager? _audioManager;
    private AudioFocusRequestClass? _focusRequest;
    private Timer? _timer;
    private Timer? _positionTimer;
    private int _index;
    private bool _shuffle;
    private bool _repeatTrack;
    private bool _resumeAfterFocusGain;
    private double _volume = .7;
    private DateTimeOffset? _timerEndUtc;
    private BecomingNoisyReceiver? _noisyReceiver;
    private DateTimeOffset _lastSessionSaveUtc = DateTimeOffset.MinValue;
    private int _consecutiveFailures;
    private CancellationTokenSource? _fadeCancellation;

    public override void OnCreate()
    {
        base.OnCreate();
        _audioManager = (AudioManager?)GetSystemService(AudioService);
        CreateNotificationChannel();
        _session = new MediaSession(this, "OpenSleepMusic");
        _session.SetCallback(new SessionCallback(this));
        _session.SetFlags(MediaSessionFlags.HandlesMediaButtons | MediaSessionFlags.HandlesTransportControls);
        _session.Active = true;
    }

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        if (intent is null)
        {
            RestorePlayingSession();
            return StartCommandResult.Sticky;
        }
        switch (intent?.Action)
        {
            case AndroidPlaybackBridge.ActionLoad:
                Load(intent);
                break;
            case AndroidPlaybackBridge.ActionToggle:
                if (_player?.IsPlaying == true) Pause(); else Play();
                break;
            case AndroidPlaybackBridge.ActionPlay:
                Play();
                break;
            case AndroidPlaybackBridge.ActionPause:
                Pause();
                break;
            case AndroidPlaybackBridge.ActionNext:
                Move(1, forceSequential: !_shuffle);
                break;
            case AndroidPlaybackBridge.ActionPrevious:
                Move(-1, forceSequential: true);
                break;
            case AndroidPlaybackBridge.ActionSeek:
                Seek(intent.GetDoubleExtra("position", 0));
                break;
            case AndroidPlaybackBridge.ActionSettings:
                ApplySettings(intent);
                break;
            case AndroidPlaybackBridge.ActionStop:
                StopPlayback();
                break;
        }
        return StartCommandResult.Sticky;
    }

    public override IBinder? OnBind(Intent? intent) => null;

    public override void OnTaskRemoved(Intent? rootIntent)
    {
        StopPlayback();
        base.OnTaskRemoved(rootIntent);
    }

    public override void OnDestroy()
    {
        CancelFade();
        _timer?.Dispose();
        _positionTimer?.Dispose();
        ReleasePlayer();
        AbandonAudioFocus();
        if (_session is not null)
        {
            _session.Active = false;
            _session.Release();
            _session.Dispose();
        }
        base.OnDestroy();
    }

    public void OnAudioFocusChange(AudioFocus focusChange)
    {
        switch (focusChange)
        {
            case AudioFocus.Loss:
                _resumeAfterFocusGain = false;
                CancelFade();
                Pause();
                break;
            case AudioFocus.LossTransient:
            case AudioFocus.LossTransientCanDuck:
                _resumeAfterFocusGain = _player?.IsPlaying == true;
                if (_resumeAfterFocusGain)
                {
                    _ = FadeOutForFocusLossAsync();
                }
                break;
            case AudioFocus.Gain when _resumeAfterFocusGain:
                _resumeAfterFocusGain = false;
                _ = ResumeWithFadeInAsync();
                break;
        }
    }

    private void Load(Intent intent)
    {
        var ids = intent.GetStringArrayListExtra("ids") ?? [];
        var titles = intent.GetStringArrayListExtra("titles") ?? [];
        var creators = intent.GetStringArrayListExtra("creators") ?? [];
        var paths = intent.GetStringArrayListExtra("paths") ?? [];
        var gains = intent.GetDoubleArrayExtra("gains") ?? [];
        _queue.Clear();
        for (var i = 0; i < ids.Count && i < titles.Count && i < creators.Count && i < paths.Count; i++)
        {
            var gain = i < gains.Length ? gains[i] : 1;
            _queue.Add(new QueueItem(ids[i]!, titles[i]!, creators[i]!, paths[i]!, gain));
        }
        if (_queue.Count == 0)
        {
            StopPlayback();
            return;
        }
        _index = Math.Clamp(intent.GetIntExtra("index", 0), 0, _queue.Count - 1);
        ApplySettings(intent);
        OpenCurrent(intent.GetDoubleExtra("position", 0));
    }

    private void ApplySettings(Intent intent)
    {
        _shuffle = intent.GetBooleanExtra("shuffle", _shuffle);
        _repeatTrack = intent.GetBooleanExtra("repeat", _repeatTrack);
        _volume = Math.Clamp(intent.GetDoubleExtra("volume", _volume), 0, 1);
        SetPlayerVolume();
        var timerEnd = intent.GetLongExtra("timerEnd", 0);
        _timerEndUtc = timerEnd > 0 ? DateTimeOffset.FromUnixTimeMilliseconds(timerEnd) : null;
        ArmTimer();
        UpdateStateAndNotification();
    }

    private void OpenCurrent(double positionSeconds = 0)
    {
        if (_queue.Count == 0) return;
        ReleasePlayer();
        var item = _queue[_index];
        try
        {
            _player = new MediaPlayer();
            _player.SetAudioAttributes(new AudioAttributes.Builder()!
                .SetUsage(AudioUsageKind.Media)!
                .SetContentType(AudioContentType.Music)!
                .Build());
            _player.SetWakeMode(this, WakeLockFlags.Partial);
            SetPlayerVolume();
            _player.SetDataSource(item.Path);
            _player.Prepared += (_, _) =>
            {
                _consecutiveFailures = 0;
                if (positionSeconds > 0)
                {
                    _player.SeekTo((int)Math.Min(int.MaxValue, positionSeconds * 1000));
                }
                Play();
            };
            _player.Completion += (_, _) => { if (_repeatTrack) OpenCurrent(); else Move(1); };
            _player.Error += (_, e) =>
            {
                e.Handled = true;
                SkipFailedTrack(item.Id, "Android MediaPlayer rejected the local audio file.");
            };
            StartForeground(NotificationId, BuildNotification(false));
            _player.PrepareAsync();
            UpdateSessionMetadata();
        }
        catch (Exception exception)
        {
            AndroidPlaybackBridge.Publish(new(item.Id, false, TimeSpan.Zero, TimeSpan.Zero, exception.Message));
            SkipFailedTrack(item.Id, exception.Message);
        }
    }

    private void SkipFailedTrack(string trackId, string reason)
    {
        _consecutiveFailures++;
        Android.Util.Log.Warn("OpenSleepMusic", $"Playback failed for {trackId}: {reason}");
        if (_consecutiveFailures >= _queue.Count)
        {
            Pause();
            return;
        }
        Move(1);
    }

    private void Play()
    {
        CancelFade();
        if (_player is null || !RequestAudioFocus()) return;
        try
        {
            _player.Start();
            RegisterNoisyReceiver();
            StartPositionUpdates();
            UpdateStateAndNotification();
        }
        catch (Exception exception) { Android.Util.Log.Warn("OpenSleepMusic", exception.Message); }
    }

    private void Pause()
    {
        try { if (_player?.IsPlaying == true) _player.Pause(); }
        catch (Exception exception) { Android.Util.Log.Warn("OpenSleepMusic", exception.Message); }
        UnregisterNoisyReceiver();
        _positionTimer?.Dispose();
        _positionTimer = null;
        UpdateStateAndNotification();
        if (_queue.Count > 0)
        {
            SaveSession(false, _player?.CurrentPosition ?? 0);
        }
    }

    private void Seek(double seconds)
    {
        if (_player is null) return;
        _player.SeekTo((int)Math.Clamp(seconds * 1000, 0, int.MaxValue));
        UpdateStateAndNotification();
    }

    private void Move(int offset, bool forceSequential = false)
    {
        if (_queue.Count == 0) return;
        if (_shuffle && !forceSequential && _queue.Count > 1)
        {
            _index = PlaybackQueue.ChooseDifferent(_queue.Count, _index, Random.Shared.Next(_queue.Count - 1));
        }
        else
        {
            _index = PlaybackQueue.MoveSequential(_queue.Count, _index, offset);
        }
        OpenCurrent();
    }

    private void StopPlayback()
    {
        CancelFade();
        ReleasePlayer();
        AbandonAudioFocus();
        _queue.Clear();
        _timer?.Dispose();
        _timer = null;
        _timerEndUtc = null;
        ClearSavedSession();
        UnregisterNoisyReceiver();
        if (_session is not null)
        {
            _session.SetPlaybackState(new PlaybackState.Builder()!
                .SetState(PlaybackStateCode.Stopped, 0, 0)!
                .Build());
            _session.Active = false;
        }
        AndroidPlaybackBridge.Publish(new(null, false, TimeSpan.Zero, TimeSpan.Zero));
        StopForeground(StopForegroundFlags.Remove);
        ((NotificationManager?)GetSystemService(NotificationService))?.Cancel(NotificationId);
        StopSelf();
    }

    private bool RequestAudioFocus()
    {
        if (_audioManager is null) return false;
        var attributes = new AudioAttributes.Builder()!
                .SetUsage(AudioUsageKind.Media)!
                .SetContentType(AudioContentType.Music)!
                .Build()!;
        _focusRequest ??= new AudioFocusRequestClass.Builder(AudioFocus.Gain)!
            .SetAudioAttributes(attributes)!
            .SetOnAudioFocusChangeListener(this)!
            .SetWillPauseWhenDucked(true)!
            .Build();
        return _audioManager.RequestAudioFocus(_focusRequest!) == AudioFocusRequest.Granted;
    }

    private void AbandonAudioFocus()
    {
        if (_audioManager is not null && _focusRequest is not null)
        {
            _audioManager.AbandonAudioFocusRequest(_focusRequest);
        }
    }

    private void ReleasePlayer()
    {
        if (_player is null) return;
        try { _player.Stop(); } catch (Exception exception) { Android.Util.Log.Debug("OpenSleepMusic", exception.Message); }
        _player.Reset();
        _player.Release();
        _player.Dispose();
        _player = null;
    }

    private void RegisterNoisyReceiver()
    {
        if (_noisyReceiver is not null) return;
        _noisyReceiver = new BecomingNoisyReceiver(this);
        var filter = new IntentFilter(AudioManager.ActionAudioBecomingNoisy);
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
        {
#pragma warning disable CA1416
            RegisterReceiver(_noisyReceiver, filter, ReceiverFlags.NotExported);
#pragma warning restore CA1416
        }
        else
        {
#pragma warning disable CA1422
            RegisterReceiver(_noisyReceiver, filter);
#pragma warning restore CA1422
        }
    }

    private void UnregisterNoisyReceiver()
    {
        if (_noisyReceiver is null) return;
        try { UnregisterReceiver(_noisyReceiver); } catch (Java.Lang.IllegalArgumentException) { }
        _noisyReceiver.Dispose();
        _noisyReceiver = null;
    }

    private void ArmTimer()
    {
        CancelFade();
        _timer?.Dispose();
        _timer = null;
        if (_timerEndUtc is not { } end) return;
        var delay = end - DateTimeOffset.UtcNow;
        if (delay <= TimeSpan.Zero)
        {
            _ = FadeOutForTimerAsync();
            return;
        }
        _timer = new Timer(_ => _ = FadeOutForTimerAsync(), null, delay, Timeout.InfiniteTimeSpan);
    }

    private async Task FadeOutForTimerAsync()
    {
        _timerEndUtc = null;
        _fadeCancellation?.Cancel();
        _fadeCancellation?.Dispose();
        _fadeCancellation = new CancellationTokenSource();
        var token = _fadeCancellation.Token;
        try
        {
            const int steps = 30;
            for (var step = 1; step <= steps; step++)
            {
                token.ThrowIfCancellationRequested();
                var fadeVolume = (float)SleepTimerDisplay.FadeVolume(CurrentVolume(), step / (double)steps);
                _player?.SetVolume(fadeVolume, fadeVolume);
                await Task.Delay(TimeSpan.FromMilliseconds(100), token);
            }
            Pause();
        }
        catch (System.OperationCanceledException)
        {
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                SetPlayerVolume();
            }
        }
    }

    private async Task FadeOutForFocusLossAsync()
    {
        var token = BeginFade();
        var startingVolume = CurrentVolume();
        try
        {
            const int steps = 5;
            for (var step = 1; step <= steps; step++)
            {
                token.ThrowIfCancellationRequested();
                var volume = (float)SleepTimerDisplay.FadeVolume(startingVolume, step / (double)steps);
                _player?.SetVolume(volume, volume);
                await Task.Delay(TimeSpan.FromMilliseconds(50), token);
            }
            Pause();
        }
        catch (System.OperationCanceledException)
        {
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                SetPlayerVolume();
            }
        }
    }

    private async Task ResumeWithFadeInAsync()
    {
        var token = BeginFade();
        if (_player is null || !RequestAudioFocus()) return;
        var targetVolume = CurrentVolume();
        try
        {
            _player.SetVolume(0, 0);
            _player.Start();
            RegisterNoisyReceiver();
            StartPositionUpdates();
            UpdateStateAndNotification();

            const int steps = 6;
            for (var step = 1; step <= steps; step++)
            {
                token.ThrowIfCancellationRequested();
                var volume = (float)(targetVolume * step / steps);
                _player.SetVolume(volume, volume);
                await Task.Delay(TimeSpan.FromMilliseconds(50), token);
            }
        }
        catch (System.OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            Android.Util.Log.Warn("OpenSleepMusic", exception.Message);
        }
        finally
        {
            SetPlayerVolume();
        }
    }

    private CancellationToken BeginFade()
    {
        _fadeCancellation?.Cancel();
        _fadeCancellation?.Dispose();
        _fadeCancellation = new CancellationTokenSource();
        return _fadeCancellation.Token;
    }

    private void CancelFade()
    {
        _fadeCancellation?.Cancel();
        _fadeCancellation?.Dispose();
        _fadeCancellation = null;
        SetPlayerVolume();
    }

    private bool IsPlaying()
    {
        try { return _player?.IsPlaying == true; }
        catch (Exception) { return false; }
    }

    private void UpdateStateAndNotification()
    {
        PublishState();
        if (_queue.Count > 0)
        {
            StartForeground(NotificationId, BuildNotification(IsPlaying()));
        }
    }

    private void PublishState(bool save = true)
    {
        var item = _queue.Count > 0 ? _queue[_index] : null;
        var duration = _player?.Duration ?? 0;
        var position = _player?.CurrentPosition ?? 0;
        var playing = IsPlaying();
        if (item is not null)
        {
            AndroidPlaybackBridge.Publish(new(item.Id, playing, TimeSpan.FromMilliseconds(position), TimeSpan.FromMilliseconds(duration)));
            _session?.SetPlaybackState(new PlaybackState.Builder()!
                .SetActions(PlaybackState.ActionPlay | PlaybackState.ActionPause | PlaybackState.ActionPlayPause |
                            PlaybackState.ActionSkipToNext | PlaybackState.ActionSkipToPrevious | PlaybackState.ActionSeekTo)!
                .SetState(playing ? PlaybackStateCode.Playing : PlaybackStateCode.Paused, position, playing ? 1 : 0)!
                .Build());
            if (save && (playing || DateTimeOffset.UtcNow - _lastSessionSaveUtc >= TimeSpan.FromSeconds(5)))
            {
                SaveSession(playing, position);
            }
        }
    }

    private void StartPositionUpdates()
    {
        _positionTimer?.Dispose();
        _positionTimer = new Timer(_ => PublishState(), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    private void SaveSession(bool playing, int positionMilliseconds)
    {
        if (_queue.Count == 0) return;
        var preferences = GetSharedPreferences("playback-session", FileCreationMode.Private);
        var serializedQueue = string.Join("\n", _queue.Select(item => string.Join("|",
            Encode(item.Id), Encode(item.Title), Encode(item.Creator), Encode(item.Path), item.Gain.ToString(System.Globalization.CultureInfo.InvariantCulture))));
        preferences?.Edit()?
            .PutString("queue", serializedQueue)?
            .PutInt("index", _index)?
            .PutInt("position", positionMilliseconds)?
            .PutBoolean("playing", playing)?
            .PutBoolean("shuffle", _shuffle)?
            .PutBoolean("repeat", _repeatTrack)?
            .PutLong("timerEnd", _timerEndUtc?.ToUnixTimeMilliseconds() ?? 0)?
            .PutLong("volume", BitConverter.DoubleToInt64Bits(_volume))?
            .Apply();
        _lastSessionSaveUtc = DateTimeOffset.UtcNow;
    }

    private void RestorePlayingSession()
    {
        var preferences = GetSharedPreferences("playback-session", FileCreationMode.Private);
        if (preferences?.GetBoolean("playing", false) != true) return;
        var serializedQueue = preferences.GetString("queue", null);
        if (string.IsNullOrWhiteSpace(serializedQueue)) return;
        _queue.Clear();
        foreach (var line in serializedQueue.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var fields = line.Split('|');
            if (fields.Length >= 4)
            {
                var gain = fields.Length >= 5 && double.TryParse(fields[4], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsedGain)
                    ? parsedGain
                    : 1;
                _queue.Add(new QueueItem(Decode(fields[0]), Decode(fields[1]), Decode(fields[2]), Decode(fields[3]), gain));
            }
        }
        if (_queue.Count == 0) return;
        _index = Math.Clamp(preferences.GetInt("index", 0), 0, _queue.Count - 1);
        _shuffle = preferences.GetBoolean("shuffle", false);
        _repeatTrack = preferences.GetBoolean("repeat", false);
        _volume = Math.Clamp(BitConverter.Int64BitsToDouble(preferences.GetLong("volume", BitConverter.DoubleToInt64Bits(.7))), 0, 1);
        var timerEnd = preferences.GetLong("timerEnd", 0);
        _timerEndUtc = timerEnd > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            ? DateTimeOffset.FromUnixTimeMilliseconds(timerEnd)
            : null;
        ArmTimer();
        OpenCurrent(preferences.GetInt("position", 0) / 1000d);
    }

    private void ClearSavedSession() =>
        GetSharedPreferences("playback-session", FileCreationMode.Private)?.Edit()?.Clear()?.Apply();

    private static string Encode(string value) => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
    private static string Decode(string value) => System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value));

    private void UpdateSessionMetadata()
    {
        if (_session is null || _queue.Count == 0) return;
        var item = _queue[_index];
        _session.SetMetadata(new MediaMetadata.Builder()!
            .PutString(MediaMetadata.MetadataKeyTitle, item.Title)!
            .PutString(MediaMetadata.MetadataKeyArtist, item.Creator)!
            .Build());
    }

    private Notification BuildNotification(bool playing)
    {
        var item = _queue.Count > 0 ? _queue[_index] : null;
        var openIntent = PackageManager?.GetLaunchIntentForPackage(PackageName!);
        var contentIntent = openIntent is null ? null : PendingIntent.GetActivity(this, 0, openIntent,
            PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent);
        var builder = new Notification.Builder(this, ChannelId)!
            .SetSmallIcon(Android.Resource.Drawable.IcMediaPlay)
            .SetContentTitle(item?.Title ?? "Open Sleep Music")
            .SetContentText(item?.Creator ?? "Schlafmusik")
            .SetContentIntent(contentIntent)
            .SetOnlyAlertOnce(true)
            .SetOngoing(playing)
            .SetCategory(Notification.CategoryTransport)
            .SetVisibility(NotificationVisibility.Public)!;
        builder.AddAction(new Notification.Action.Builder(
            Android.Graphics.Drawables.Icon.CreateWithResource(this, Android.Resource.Drawable.IcMediaPrevious),
            "Zurück", ActionIntent(AndroidPlaybackBridge.ActionPrevious, 1))!.Build());
        builder.AddAction(new Notification.Action.Builder(
            Android.Graphics.Drawables.Icon.CreateWithResource(this,
                playing ? Android.Resource.Drawable.IcMediaPause : Android.Resource.Drawable.IcMediaPlay),
            playing ? "Pause" : "Wiedergabe", ActionIntent(AndroidPlaybackBridge.ActionToggle, 2))!.Build());
        builder.AddAction(new Notification.Action.Builder(
            Android.Graphics.Drawables.Icon.CreateWithResource(this, Android.Resource.Drawable.IcMediaNext),
            "Weiter", ActionIntent(AndroidPlaybackBridge.ActionNext, 3))!.Build());
        if (_session is not null)
        {
            var style = new Notification.MediaStyle()!;
            style.SetMediaSession(_session.SessionToken);
            style.SetShowActionsInCompactView(0, 1, 2);
            builder.SetStyle(style);
        }
        return builder.Build()!;
    }

    private PendingIntent ActionIntent(string action, int requestCode) => PendingIntent.GetService(
        this,
        requestCode,
        new Intent(this, typeof(AndroidPlaybackService)).SetAction(action),
        PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent)!;

    private void CreateNotificationChannel()
    {
        var manager = (NotificationManager?)GetSystemService(NotificationService);
        manager?.CreateNotificationChannel(new NotificationChannel(ChannelId, "Wiedergabe", NotificationImportance.Low)
        {
            Description = "Steuerung der laufenden Schlafmusik"
        });
    }

    private double CurrentVolume() => _queue.Count == 0
        ? _volume
        : PlaybackVolume.ApplyGain(_volume, _queue[_index].Gain);

    private void SetPlayerVolume()
    {
        var volume = (float)CurrentVolume();
        _player?.SetVolume(volume, volume);
    }

    private sealed record QueueItem(string Id, string Title, string Creator, string Path, double Gain);

    private sealed class BecomingNoisyReceiver(AndroidPlaybackService owner) : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (intent?.Action == AudioManager.ActionAudioBecomingNoisy)
            {
                owner._resumeAfterFocusGain = false;
                owner.Pause();
            }
        }
    }

    private sealed class SessionCallback(AndroidPlaybackService owner) : MediaSession.Callback
    {
        public override void OnPlay() => owner.Play();
        public override void OnPause() => owner.Pause();
        public override void OnSkipToNext() => owner.Move(1, forceSequential: !owner._shuffle);
        public override void OnSkipToPrevious() => owner.Move(-1, forceSequential: true);
        public override void OnSeekTo(long pos) => owner.Seek(pos / 1000d);
        public override void OnStop() => owner.StopPlayback();
    }
}
