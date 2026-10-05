using OpenSleepMusic.Core.Playback;

namespace OpenSleepMusic.Core.Tests;

public sealed class PlaybackQueueTests
{
    [Theory]
    [InlineData("b", "a,c,d", "c")]
    [InlineData("d", "a,b,c", "c")]
    [InlineData("b", "a,d", "d")]
    [InlineData("b", "a", "a")]
    [InlineData("b", "", null)]
    public void RemovedTrackSelectsNextThenPreviousOrNothing(string removed, string remaining, string? expected) =>
        Assert.Equal(expected, PlaybackQueue.SelectAfterRemoval(
            ["a", "b", "c", "d"], remaining.Length == 0 ? [] : remaining.Split(','), removed));

    [Fact]
    public void RemovedTrackUsesShuffleCandidate() =>
        Assert.Equal("d", PlaybackQueue.SelectAfterRemoval(["a", "b", "c", "d"], ["a", "c", "d"], "b", 2));
    [Theory]
    [InlineData(3, 2, 1, 0)]
    [InlineData(3, 0, -1, 2)]
    [InlineData(3, -1, 1, 0)]
    public void MoveSequentialWrapsAndHandlesMissingCurrent(int count, int current, int offset, int expected) =>
        Assert.Equal(expected, PlaybackQueue.MoveSequential(count, current, offset));

    [Theory]
    [InlineData(4, 0, 0, 1)]
    [InlineData(4, 2, 1, 1)]
    [InlineData(4, 2, 2, 3)]
    public void ChooseDifferentNeverReturnsCurrent(int count, int current, int candidate, int expected) =>
        Assert.Equal(expected, PlaybackQueue.ChooseDifferent(count, current, candidate));
}
