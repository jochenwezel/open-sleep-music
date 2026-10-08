using Android.Media;
using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.App;

internal sealed class AndroidBackgroundPlayer : IDisposable
{
    private MediaPlayer? _player;
    private BackgroundPlaybackSource? _source;
    private bool _ready, _playing, _seeking;
    private double _position, _volume;

    public void Configure(BackgroundPlaybackSource? source, double mainPosition)
    {
        Dispose();
        _source = source;
        if (source is null || !File.Exists(source.Path)) return;
        _position = source.LoopPosition(mainPosition);
        try
        {
            var player = new MediaPlayer();
            _player = player;
            player.SetAudioAttributes(new AudioAttributes.Builder()!.SetUsage(AudioUsageKind.Media)!
                .SetContentType(AudioContentType.Music)!.Build());
            player.SetVolume((float)_volume, (float)_volume);
            player.SetDataSource(source.Path);
            player.Prepared += (_, _) => { if (_player != player) return; _ready = true; SeekForMainPosition(_position); };
            player.SeekComplete += (_, _) => { if (_player != player) return; _seeking = false; ApplyPlaying(); };
            player.Completion += (_, _) => { if (_player == player && _playing) SeekForMainPosition(0); };
            player.Error += (_, e) => { e.Handled = true; if (_player == player) Dispose(); };
            player.PrepareAsync();
        }
        catch (Exception exception) { Android.Util.Log.Warn("OpenSleepMusic", $"Background audio unavailable: {exception.Message}"); Dispose(); }
    }

    public void SetPlaying(bool playing) { _playing = playing; ApplyPlaying(); }
    private void ApplyPlaying()
    {
        if (!_ready || _seeking || _player is null) return;
        try { if (_playing) _player.Start(); else if (_player.IsPlaying) _player.Pause(); }
        catch (Exception exception) { Android.Util.Log.Warn("OpenSleepMusic", exception.Message); Dispose(); }
    }
    public void SetVolume(double volume)
    {
        _volume = Math.Clamp(volume, 0, 1);
        _player?.SetVolume((float)_volume, (float)_volume);
    }
    public void SeekForMainPosition(double seconds)
    {
        if (_source is null) return;
        _position = _source.LoopPosition(seconds);
        if (!_ready || _player is null) return;
        try
        {
            if (_player.IsPlaying) _player.Pause();
            _seeking = true;
            _player.SeekTo((int)PlaybackTimeline.ToMediaTime(TimeSpan.FromSeconds(_position), 1,
                _source.StartOffsetMilliseconds, Duration(), _source.EndOffsetMilliseconds).TotalMilliseconds);
        }
        catch (Exception exception) { Android.Util.Log.Warn("OpenSleepMusic", exception.Message); Dispose(); }
    }
    private TimeSpan Duration() => PlaybackTimeline.ResolveMediaDuration(
        _ready ? TimeSpan.FromMilliseconds(_player?.Duration ?? 0) : TimeSpan.Zero, _source?.DurationSeconds ?? 0);
    public void Tick()
    {
        if (_playing && _ready && !_seeking && _player is { } player && _source is { } source
            && PlaybackTimeline.HasReachedEnd(TimeSpan.FromMilliseconds(player.CurrentPosition), Duration(), source.EndOffsetMilliseconds))
            SeekForMainPosition(0);
    }
    public void Dispose()
    {
        var player = _player;
        _player = null;
        _ready = _playing = _seeking = false;
        _source = null;
        if (player is null) return;
        try { player.Reset(); player.Release(); player.Dispose(); }
        catch (Exception exception) { Android.Util.Log.Debug("OpenSleepMusic", exception.Message); }
    }
}
