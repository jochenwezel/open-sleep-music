namespace OpenSleepMusic.Core.Catalog;

public sealed record SleepWorld(
    string Id,
    string Name,
    string Description,
    string Icon,
    IReadOnlyList<AudioTrack> Tracks);
