namespace OpenSleepMusic.Core.Catalog;

/// <summary>Expands collection membership without duplicating canonical manifest assets.</summary>
internal static class CatalogMembership
{
    public static IReadOnlyList<SleepWorld> Resolve(IReadOnlyList<SleepWorld> worlds)
    {
        var tracksByWorld = worlds.ToDictionary(world => world.Id, world => world.Tracks.ToList(), StringComparer.Ordinal);
        foreach (var world in worlds)
        foreach (var track in world.Tracks)
        {
            if (track.AdditionalWorldIds is not { Count: > 0 } targets) continue;
            if (targets.Count >= worlds.Count || world.Id is "preselection" or "pre-qualify"
                || track.LicenseReviewStatus != "verified")
                throw new InvalidDataException($"Invalid shared collection membership for '{track.Id}'.");

            var seen = new HashSet<string>(StringComparer.Ordinal) { world.Id };
            foreach (var target in targets)
            {
                if (string.IsNullOrWhiteSpace(target) || !seen.Add(target)
                    || target is "preselection" or "pre-qualify"
                    || !tracksByWorld.TryGetValue(target, out var targetTracks)
                    || targetTracks.Any(existing => existing.Id == track.Id))
                    throw new InvalidDataException($"Invalid shared collection '{target}' for '{track.Id}'.");
                targetTracks.Add(track);
            }
        }
        return worlds.Select(world => world with { Tracks = ResolveOrder(world, tracksByWorld[world.Id]) }).ToArray();
    }

    private static IReadOnlyList<AudioTrack> ResolveOrder(SleepWorld world, IReadOnlyList<AudioTrack> tracks)
    {
        if (world.TrackOrder is null) return tracks.ToArray();
        var byId = tracks.ToDictionary(track => track.Id, StringComparer.Ordinal);
        var seen = new HashSet<string>(StringComparer.Ordinal);
        if (world.TrackOrder.Count != tracks.Count
            || world.TrackOrder.Any(id => string.IsNullOrWhiteSpace(id) || !seen.Add(id) || !byId.ContainsKey(id)))
            throw new InvalidDataException($"Invalid complete track order for '{world.Id}'.");
        return world.TrackOrder.Select(id => byId[id]).ToArray();
    }
}
