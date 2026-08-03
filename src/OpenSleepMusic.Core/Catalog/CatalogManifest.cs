namespace OpenSleepMusic.Core.Catalog;

internal sealed record CatalogManifest(
    int SchemaVersion,
    DateTimeOffset GeneratedAtUtc,
    IReadOnlyList<SleepWorld> SleepWorlds);
