namespace OpenSleepMusic.Core.Playback;

public static class PlaybackQueue
{
    public static int MoveSequential(int itemCount, int currentIndex, int offset)
    {
        if (itemCount <= 0) throw new ArgumentOutOfRangeException(nameof(itemCount));
        var normalizedCurrent = currentIndex >= 0 && currentIndex < itemCount
            ? currentIndex
            : offset < 0 ? 0 : itemCount - 1;
        return ((normalizedCurrent + offset) % itemCount + itemCount) % itemCount;
    }

    public static int ChooseDifferent(int itemCount, int currentIndex, int randomCandidate)
    {
        if (itemCount < 2) throw new ArgumentOutOfRangeException(nameof(itemCount));
        if (currentIndex < 0 || currentIndex >= itemCount) throw new ArgumentOutOfRangeException(nameof(currentIndex));
        if (randomCandidate < 0 || randomCandidate >= itemCount - 1) throw new ArgumentOutOfRangeException(nameof(randomCandidate));
        return randomCandidate >= currentIndex ? randomCandidate + 1 : randomCandidate;
    }
}
