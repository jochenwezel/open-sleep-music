using System.Reflection;
using System.Text.Json;

namespace OpenSleepMusic.Core.Catalog;

public static class BuiltInCatalog
{
    private const string ResourceName = "OpenSleepMusic.Core.Catalog.media-catalog.json";

    private static readonly CatalogManifest Manifest = Load();

    public static IReadOnlyList<SleepWorld> SleepWorlds { get; } = Manifest.SleepWorlds;

    public static DateTimeOffset GeneratedAtUtc => Manifest.GeneratedAtUtc;

    public static TimeSpan TotalDuration => TimeSpan.FromSeconds(
        SleepWorlds.SelectMany(world => world.Tracks).Sum(track => track.DurationSeconds));

    private static CatalogManifest Load()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"Embedded catalog resource '{ResourceName}' was not found.");

        var manifest = JsonSerializer.Deserialize(stream, CatalogJsonContext.Default.CatalogManifest)
            ?? throw new InvalidDataException("The embedded media catalog is empty or invalid.");

        if (manifest.SchemaVersion != 1)
        {
            throw new InvalidDataException($"Unsupported media catalog schema {manifest.SchemaVersion}.");
        }

        if (!BuildChannel.IsPreview && manifest.SleepWorlds.Any(world => world.Id is "preselection" or "pre-qualify"
                || world.ExternalReferences is { Count: > 0 }))
            throw new InvalidDataException("Preselection must not be included in a stable release catalog.");

        foreach (var track in manifest.SleepWorlds
                     .Where(world => !BuildChannel.IsPreview || world.Id != "preselection")
                     .SelectMany(world => world.Tracks))
        {
            BuiltInPreselection.EnsureProductionTrackAllowed(track);
        }

        return manifest;
    }
}
