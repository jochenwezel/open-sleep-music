using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.Core.Downloads;

public sealed record ArtworkDownloadProgress(int Completed, int Total, string? CurrentTitle);

public sealed record ArtworkDownloadResult(int AvailableCount, int UnavailableCount);

public sealed class CollectionArtworkDownloader(
    ArtworkManifestClient manifestClient,
    ArtworkCache artworkCache)
{
    public async Task<ArtworkDownloadResult> DownloadAsync(
        IReadOnlyList<AudioTrack> tracks,
        string cacheRoot,
        IProgress<ArtworkDownloadProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        var available = 0;

        for (var index = 0; index < tracks.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var track = await manifestClient.ResolveAsync(tracks[index], cacheRoot);
            var backgroundAvailable = await DownloadLayerAsync(track, results, cacheRoot, cancellationToken);
            var motifAvailable = track.SongMotifUri is null
                || await DownloadLayerAsync(track with
                {
                    Id = $"{track.Id}:song-motif",
                    ArtworkUri = track.SongMotifUri,
                    ArtworkFileName = track.SongMotifFileName,
                    ArtworkSha256 = track.SongMotifSha256
                }, results, cacheRoot, cancellationToken);
            var isAvailable = backgroundAvailable && motifAvailable;

            if (isAvailable) available++;
            progress?.Report(new(index + 1, tracks.Count, track.Title));
        }

        return new(available, tracks.Count - available);
    }

    private async Task<bool> DownloadLayerAsync(
        AudioTrack track,
        Dictionary<string, bool> results,
        string cacheRoot,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKey(track);
        if (results.TryGetValue(cacheKey, out var isAvailable)) return isAvailable;
        isAvailable = await artworkCache.GetAsync(track, cacheRoot, cancellationToken) is not null;
        results[cacheKey] = isAvailable;
        return isAvailable;
    }

    private static string CacheKey(AudioTrack track) => string.Join('|',
        track.ArtworkFileName ?? string.Empty,
        track.ArtworkSha256 ?? string.Empty,
        track.ArtworkUri?.AbsoluteUri ?? string.Empty);
}
