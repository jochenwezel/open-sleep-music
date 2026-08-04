namespace OpenSleepMusic.Core.Playback;

public sealed class SleepTimer(TimeProvider? timeProvider = null)
{
    private readonly TimeProvider _timeProvider = timeProvider ?? TimeProvider.System;
    public DateTimeOffset? EndsAt { get; private set; }
    public bool IsActive => EndsAt is not null;
    public TimeSpan Remaining => EndsAt is { } end ? TimeSpan.FromTicks(Math.Max(0, (end - _timeProvider.GetUtcNow()).Ticks)) : TimeSpan.Zero;
    public void Start(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(duration));
        EndsAt = _timeProvider.GetUtcNow() + duration;
    }
    public void Cancel() => EndsAt = null;
    public bool ConsumeIfElapsed()
    {
        if (EndsAt is null || Remaining > TimeSpan.Zero) return false;
        EndsAt = null;
        return true;
    }
}
