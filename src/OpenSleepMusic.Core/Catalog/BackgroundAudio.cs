namespace OpenSleepMusic.Core.Catalog;

/// <summary>A single looping catalog recording underneath a primary track.</summary>
public sealed record BackgroundAudio(string TrackId, double Volume = 0.2,
    int StartOffsetMilliseconds = 0, int EndOffsetMilliseconds = 0);

public static class BackgroundAudioCatalog
{
    public static void Validate(IReadOnlyList<SleepWorld> worlds)
    {
        var assets = worlds.SelectMany(world => world.Tracks.Select(track => (world.Id, Track: track)))
            .GroupBy(item => item.Track.Id).ToDictionary(group => group.Key, group => group.First());
        foreach (var world in worlds)
        foreach (var track in world.Tracks)
        {
            if (track.BackgroundAudio is not { } background) continue;
            if (!assets.TryGetValue(background.TrackId, out var source) || background.TrackId == track.Id
                || source.Track.BackgroundAudio is not null
                || !double.IsFinite(background.Volume) || background.Volume is < 0 or > 1
                || background.StartOffsetMilliseconds < 0 || background.EndOffsetMilliseconds < 0
                || (long)background.StartOffsetMilliseconds + background.EndOffsetMilliseconds >= source.Track.DurationSeconds * 1000
                || world.Id != "preselection" && source.Id == "preselection"
                    && (!BuildChannel.IsPreview || source.Track.LicenseReviewStatus != "verified"))
                throw new InvalidDataException($"Invalid background audio for '{track.Id}'.");
            if (world.Id != "preselection" && source.Id == "preselection")
                BuiltInPreselection.EnsureProductionTrackAllowed(source.Track);
        }
    }

    public static AudioTrack? Resolve(AudioTrack primary, IReadOnlyList<SleepWorld> worlds)
    {
        if (primary.BackgroundAudio is not { } background) return null;
        var source = worlds.SelectMany(world => world.Tracks).First(track => track.Id == background.TrackId);
        return source with { BackgroundAudio = null, AdditionalWorldIds = null, PlaybackSpeed = 1,
            VolumeGain = background.Volume, StartOffsetMilliseconds = background.StartOffsetMilliseconds,
            EndOffsetMilliseconds = background.EndOffsetMilliseconds };
    }

    public static SleepWorld IncludeDownloads(SleepWorld world, IReadOnlyList<SleepWorld> worlds) => world with
    {
        Tracks = world.Tracks.Concat(world.Tracks.Select(track => Resolve(track, worlds))
                .OfType<AudioTrack>().Select(track => track with { AdditionalWorldIds = [world.Id] }))
            .DistinctBy(track => track.Id).ToArray()
    };
}
