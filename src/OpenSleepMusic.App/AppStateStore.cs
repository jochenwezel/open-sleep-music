using Microsoft.Maui.Storage;

namespace OpenSleepMusic.App;

internal enum PlaybackRepeatMode
{
    SleepWorld,
    Track
}

internal sealed record PersistedAppState(
    double Volume,
    bool ShuffleEnabled,
    PlaybackRepeatMode RepeatMode,
    string? SelectedWorldId,
    int SleepTimerMinutes,
    DateTimeOffset? SleepTimerEndUtc);

internal sealed record PersistedPlaybackState(string TrackId, double PositionSeconds);

internal sealed class AppStateStore(IPreferences? preferences = null)
{
    private const string VolumeKey = "player.volume";
    private const string ShuffleKey = "player.shuffle";
    private const string RepeatModeKey = "player.repeat-mode";
    private const string SelectedWorldKey = "library.selected-world";
    private const string TimerMinutesKey = "sleep-timer.minutes";
    private const string TimerEndKey = "sleep-timer.end-utc";
    private const string PlaybackTrackKey = "playback.track-id";
    private const string PlaybackPositionKey = "playback.position-seconds";
    private const string FavoritePrefix = "track.favorite.";
    private const string BlockedPrefix = "track.blocked.";
    private readonly IPreferences _preferences = preferences ?? Preferences.Default;

    public PersistedAppState Load()
    {
        var volume = Math.Clamp(_preferences.Get(VolumeKey, 0.7), 0, 1);
        var shuffle = _preferences.Get(ShuffleKey, false);
        var repeatText = _preferences.Get(RepeatModeKey, PlaybackRepeatMode.SleepWorld.ToString());
        var repeatMode = Enum.TryParse<PlaybackRepeatMode>(repeatText, out var parsedRepeatMode)
            ? parsedRepeatMode
            : PlaybackRepeatMode.SleepWorld;
        var selectedWorld = EmptyToNull(_preferences.Get(SelectedWorldKey, string.Empty));
        var timerMinutes = _preferences.Get(TimerMinutesKey, 0);
        var timerEndText = EmptyToNull(_preferences.Get(TimerEndKey, string.Empty));
        DateTimeOffset? timerEnd = DateTimeOffset.TryParse(timerEndText, out var parsedTimerEnd)
            ? parsedTimerEnd
            : null;

        return new PersistedAppState(
            volume,
            shuffle,
            repeatMode,
            selectedWorld,
            timerMinutes,
            timerEnd);
    }

    public PersistedPlaybackState? LoadPlayback()
    {
        var trackId = EmptyToNull(_preferences.Get(PlaybackTrackKey, string.Empty));
        if (trackId is null)
        {
            return null;
        }

        return new PersistedPlaybackState(
            trackId,
            Math.Max(0, _preferences.Get(PlaybackPositionKey, 0d)));
    }

    public void SaveVolume(double volume) =>
        _preferences.Set(VolumeKey, Math.Clamp(volume, 0, 1));

    public void SaveShuffle(bool enabled) => _preferences.Set(ShuffleKey, enabled);

    public void SaveRepeatMode(PlaybackRepeatMode mode) =>
        _preferences.Set(RepeatModeKey, mode.ToString());

    public void SaveSelectedWorld(string worldId) => _preferences.Set(SelectedWorldKey, worldId);

    public void SaveSleepTimer(int minutes, DateTimeOffset? endUtc)
    {
        _preferences.Set(TimerMinutesKey, minutes);
        _preferences.Set(TimerEndKey, endUtc?.ToString("O") ?? string.Empty);
    }

    public void SavePlayback(string trackId, double positionSeconds)
    {
        _preferences.Set(PlaybackTrackKey, trackId);
        _preferences.Set(PlaybackPositionKey, Math.Max(0, positionSeconds));
    }

    public void ClearPlayback()
    {
        _preferences.Remove(PlaybackTrackKey);
        _preferences.Remove(PlaybackPositionKey);
    }

    public bool IsFavorite(string worldId, string trackId) =>
        _preferences.Get(PreferenceKey(FavoritePrefix, worldId, trackId), false);

    public bool IsBlocked(string worldId, string trackId) =>
        _preferences.Get(PreferenceKey(BlockedPrefix, worldId, trackId), false);

    public void SetFavorite(string worldId, string trackId, bool value)
    {
        _preferences.Set(PreferenceKey(FavoritePrefix, worldId, trackId), value);
        if (value)
        {
            _preferences.Set(PreferenceKey(BlockedPrefix, worldId, trackId), false);
        }
    }

    public void SetBlocked(string worldId, string trackId, bool value)
    {
        _preferences.Set(PreferenceKey(BlockedPrefix, worldId, trackId), value);
        if (value)
        {
            _preferences.Set(PreferenceKey(FavoritePrefix, worldId, trackId), false);
        }
    }

    private static string PreferenceKey(string prefix, string worldId, string trackId) =>
        $"{prefix}{worldId}.{trackId}";

    private static string? EmptyToNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
