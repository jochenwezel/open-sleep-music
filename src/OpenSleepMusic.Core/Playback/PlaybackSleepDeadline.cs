namespace OpenSleepMusic.Core.Playback;

/// <summary>Keeps an elapsed sleep deadline latched until an explicit playback restart.</summary>
public sealed class PlaybackSleepDeadline(TimeProvider? timeProvider = null)
{
    private readonly TimeProvider _clock = timeProvider ?? TimeProvider.System;
    private bool _elapsed;

    public DateTimeOffset? EndsAt { get; private set; }
    public bool HasElapsed => _elapsed || EndsAt is { } end && end <= _clock.GetUtcNow();

    public void Update(DateTimeOffset? endsAt)
    {
        // UI settings may clear a consumed timer. That is not permission to resume playback.
        _elapsed = HasElapsed;
        EndsAt = endsAt;
    }

    public void Restart(DateTimeOffset? endsAt)
    {
        _elapsed = false;
        EndsAt = endsAt;
    }

}
