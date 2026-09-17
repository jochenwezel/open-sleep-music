namespace OpenSleepMusic.Core.Playback;

public static class PlaybackTimeline
{
    public static TimeSpan ToPlaybackTime(TimeSpan mediaTime, double speed) => mediaTime / ValidSpeed(speed);
    public static TimeSpan ToMediaTime(TimeSpan playbackTime, double speed) => playbackTime * ValidSpeed(speed);

    private static double ValidSpeed(double speed) => double.IsFinite(speed) && speed > 0
        ? speed
        : throw new ArgumentOutOfRangeException(nameof(speed));
}
