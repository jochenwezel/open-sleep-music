namespace OpenSleepMusic.Core.Privacy;

public sealed record ChildCollectionAvailability(string Id, bool HasDownloads, bool HasPlayableTracks);

/// <summary>Consume the launch decision once per app process, never on back navigation or resume.</summary>
public sealed class ChildModeStartup
{
    private int _handled;

    public string? TakeAutoStartCollection(ParentalControl control, IEnumerable<ChildCollectionAvailability> collections)
    {
        if (Interlocked.Exchange(ref _handled, 1) != 0 || !control.IsLocked) return null;
        var visible = collections.Where(item => control.IsCollectionVisible(item.Id, item.HasDownloads)).Take(2).ToArray();
        return visible.Length == 1 && visible[0].HasPlayableTracks ? visible[0].Id : null;
    }
}
