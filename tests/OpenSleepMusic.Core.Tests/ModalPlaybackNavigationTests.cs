using OpenSleepMusic.App.Navigation;

namespace OpenSleepMusic.Core.Tests;

public sealed class ModalPlaybackNavigationTests
{
    [Fact]
    public async Task WaitsForListToCloseAndIgnoresDoubleTapDuringTransition()
    {
        var navigation = new ModalPlaybackNavigation();
        var closing = new TaskCompletionSource();
        var opening = new TaskCompletionSource();
        var opened = false;
        var first = navigation.OpenAsync(() => closing.Task, () =>
        {
            opened = true;
            return opening.Task;
        });
        Assert.True(navigation.IsOpening);
        Assert.False(opened);
        await navigation.OpenAsync(() => throw new Exception("Duplicate pop"), () => throw new Exception("Duplicate push"));
        closing.SetResult();
        Assert.True(opened);
        Assert.True(navigation.IsOpening);
        await navigation.OpenAsync(() => throw new Exception("Duplicate pop"), () => throw new Exception("Duplicate push"));
        opening.SetResult();
        await first;
        Assert.False(navigation.IsOpening);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task FailedNavigationDoesNotLeaveSubsequentSelectionsBlocked(bool failOnClose)
    {
        var navigation = new ModalPlaybackNavigation();
        var playerOpened = false;
        await Assert.ThrowsAsync<InvalidOperationException>(() => navigation.OpenAsync(
            () => failOnClose ? Task.FromException(new InvalidOperationException()) : Task.CompletedTask,
            () =>
            {
                playerOpened = true;
                return Task.FromException(new InvalidOperationException());
            }));
        Assert.Equal(!failOnClose, playerOpened);
        Assert.False(navigation.IsOpening);
        await navigation.OpenAsync(() => Task.CompletedTask, () => Task.CompletedTask);
        Assert.False(navigation.IsOpening);
    }
}
