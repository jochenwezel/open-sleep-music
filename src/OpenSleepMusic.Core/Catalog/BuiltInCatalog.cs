using System.Reflection;
using System.Text.Json;

namespace OpenSleepMusic.Core.Catalog;

public static class BuiltInCatalog
{
    private const string ResourceName = "OpenSleepMusic.Core.Catalog.media-catalog.json";

    public static IReadOnlyList<SleepWorld> SleepWorlds { get; } = Load();

    public static TimeSpan TotalDuration => TimeSpan.FromSeconds(
        SleepWorlds.SelectMany(world => world.Tracks).Sum(track => track.DurationSeconds));

    private static IReadOnlyList<SleepWorld> Load()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"Embedded catalog resource '{ResourceName}' was not found.");

        var manifest = JsonSerializer.Deserialize(stream, CatalogJsonContext.Default.CatalogManifest)
            ?? throw new InvalidDataException("The embedded media catalog is empty or invalid.");

        if (manifest.SchemaVersion != 1)
        {
            throw new InvalidDataException($"Unsupported media catalog schema {manifest.SchemaVersion}.");
        }

        return manifest.SleepWorlds;
    }
}
