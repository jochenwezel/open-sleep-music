namespace OpenSleepMusic.Core.Playback;

public static class PlaybackTimeline
{
    public static TimeSpan ToPlaybackTime(TimeSpan mediaTime, double speed, int startOffsetMilliseconds = 0) =>
        TimeSpan.FromMilliseconds(Math.Max(0, mediaTime.TotalMilliseconds - ValidOffset(startOffsetMilliseconds))) / ValidSpeed(speed);

    public static TimeSpan ToMediaTime(TimeSpan playbackTime, double speed, int startOffsetMilliseconds = 0,
        TimeSpan? mediaDuration = null, int endOffsetMilliseconds = 0)
    {
        var start = ValidOffset(startOffsetMilliseconds);
        var end = ValidOffset(endOffsetMilliseconds);
        var position = Math.Max(0, playbackTime.TotalMilliseconds) * ValidSpeed(speed) + start;
        if (mediaDuration is { } duration)
            position = Math.Min(position, Math.Max(start, duration.TotalMilliseconds - end));
        return TimeSpan.FromMilliseconds(position);
    }

    public static TimeSpan Duration(TimeSpan mediaDuration, double speed,
        int startOffsetMilliseconds = 0, int endOffsetMilliseconds = 0) =>
        ToPlaybackTime(mediaDuration - TimeSpan.FromMilliseconds(ValidOffset(endOffsetMilliseconds)), speed, startOffsetMilliseconds);

    public static bool HasReachedEnd(TimeSpan mediaPosition, TimeSpan mediaDuration, int endOffsetMilliseconds = 0) =>
        mediaDuration > TimeSpan.Zero && mediaPosition >= mediaDuration - TimeSpan.FromMilliseconds(ValidOffset(endOffsetMilliseconds));

    private static int ValidOffset(int milliseconds) => milliseconds >= 0
        ? milliseconds
        : throw new ArgumentOutOfRangeException(nameof(milliseconds));

    private static double ValidSpeed(double speed) => double.IsFinite(speed) && speed > 0
        ? speed
        : throw new ArgumentOutOfRangeException(nameof(speed));
}
