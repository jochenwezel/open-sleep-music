namespace OpenSleepMusic.Core.Catalog;

public sealed record CatalogSnapshot(
    DateTimeOffset GeneratedAtUtc,
    IReadOnlyList<SleepWorld> SleepWorlds);
