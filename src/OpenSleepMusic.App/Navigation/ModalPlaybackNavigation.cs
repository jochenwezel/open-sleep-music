namespace OpenSleepMusic.App.Navigation;

internal sealed class ModalPlaybackNavigation
{
    public bool IsOpening { get; private set; }

    public async Task OpenAsync(Func<Task> closeTrackList, Func<Task> openPlayer)
    {
        if (IsOpening) return;
        IsOpening = true;
        try
        {
            await closeTrackList();
            await openPlayer();
        }
        finally
        {
            IsOpening = false;
        }
    }
}
