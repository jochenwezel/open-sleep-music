namespace OpenSleepMusic.App.Playback;

internal sealed record PlaybackSnapshot(
    string? TrackId,
    bool IsPlaying,
    TimeSpan Position,
    TimeSpan Duration,
    string? Error = null);
