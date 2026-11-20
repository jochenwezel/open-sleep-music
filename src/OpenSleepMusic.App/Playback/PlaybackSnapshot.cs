namespace OpenSleepMusic.App.Playback;

internal sealed record PlaybackSnapshot(
    string? TrackId,
    bool IsPlaying,
    TimeSpan Position,
    TimeSpan Duration,
    string? Error = null,
    string? WorldId = null,
    bool IsPreparing = false)
{
    public double ResumePositionFor(string trackId, string worldId, double fallbackSeconds) =>
        !IsPreparing && Duration > TimeSpan.Zero && TrackId == trackId && (WorldId is null || WorldId == worldId)
            ? Math.Max(0, Position.TotalSeconds) : Math.Max(0, fallbackSeconds);
}
