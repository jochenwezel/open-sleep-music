#if !ANDROID
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.App.Playback;

internal sealed class DesktopBackgroundPlayer
{
    public MediaElement Element { get; } = new() { IsVisible = false, ShouldAutoPlay = false };
    private BackgroundPlaybackSource? _source;
    private bool _playing;
    private bool _ready;
    private bool _seeking;
    private double _position;
    private int _version;
    private double _endFadeFactor = 1;
    private double _eventFadeFactor = 1;

    public DesktopBackgroundPlayer()
    {
        Element.MediaOpened += async (_, _) => { _ready = true; await SeekAsync(_position); };
        Element.MediaEnded += async (_, _) => { if (_playing) await SeekAsync(0); };
        Element.MediaFailed += (_, _) => Stop();
    }

    public void Configure(BackgroundPlaybackSource? source, double mainPosition)
    {
        Stop();
        _source = source;
        if (source is null) return;
        _position = source.LoopPosition(mainPosition);
        Element.Speed = 1;
        Element.Source = MediaSource.FromFile(source.Path);
    }

    public void SetPlaying(bool playing)
    {
        _playing = playing;
        if (!_ready || _seeking) return;
        if (playing) Element.Play(); else Element.Pause();
    }

    public void SetVolume(double appVolume, double? fadeFactor = null)
    {
        if (fadeFactor is { } factor) _eventFadeFactor = factor;
        Element.Volume = Math.Clamp(appVolume * (_source?.Volume ?? 0) * _eventFadeFactor * _endFadeFactor, 0, 1);
    }

    public void SeekForMainPosition(double seconds)
    {
        if (_source is null) return;
        _position = _source.LoopPosition(seconds);
        if (_ready) _ = SeekAsync(_position);
    }

    public void Tick()
    {
        if (!_playing || !_ready || _seeking || _source is null) return;
        var duration = PlaybackTimeline.ResolveMediaDuration(Element.Duration, _source.DurationSeconds);
        if (PlaybackTimeline.HasReachedEnd(Element.Position, duration, _source.EndOffsetMilliseconds))
            _ = SeekAsync(0);
    }

    private async Task SeekAsync(double seconds)
    {
        if (_source is not { } source || _seeking) return;
        var version = _version;
        _seeking = true;
        try
        {
            Element.Pause();
            await Element.SeekTo(PlaybackTimeline.ToMediaTime(TimeSpan.FromSeconds(seconds), 1,
                source.StartOffsetMilliseconds, PlaybackTimeline.ResolveMediaDuration(Element.Duration, source.DurationSeconds), source.EndOffsetMilliseconds));
            if (version == _version && _playing) Element.Play();
        }
        catch (Exception exception) { System.Diagnostics.Debug.WriteLine(exception); if (version == _version) Stop(); }
        finally { if (version == _version) _seeking = false; }
    }

    public void Stop()
    {
        _version++;
        _endFadeFactor = 1;
        _eventFadeFactor = 1;
        _playing = _ready = _seeking = false;
        _source = null;
        Element.Stop();
        Element.Source = null;
    }

    public void UpdateEndFade(TimeSpan position, TimeSpan duration, double appVolume)
    {
        _endFadeFactor = BackgroundEndFade.VolumeFactor(position, duration);
        SetVolume(appVolume);
    }
}
#endif
