namespace OpenSleepMusic.Core.Playback;

public static class PlaybackTimeline
{
    public static TimeSpan ToPlaybackTime(TimeSpan mediaTime, double speed, int startOffsetMilliseconds = 0) =>
        TimeSpan.FromMilliseconds(Math.Max(0, mediaTime.TotalMilliseconds - ValidOffset(startOffsetMilliseconds))) / ValidSpeed(speed);

    public static TimeSpan ToMediaTime(TimeSpan playbackTime, double speed, int startOffsetMilliseconds = 0) =>
        TimeSpan.FromMilliseconds(Math.Max(0, playbackTime.TotalMilliseconds) * ValidSpeed(speed) + ValidOffset(startOffsetMilliseconds));

    private static int ValidOffset(int milliseconds) => milliseconds >= 0
        ? milliseconds
        : throw new ArgumentOutOfRangeException(nameof(milliseconds));

    private static double ValidSpeed(double speed) => double.IsFinite(speed) && speed > 0
        ? speed
        : throw new ArgumentOutOfRangeException(nameof(speed));
}
