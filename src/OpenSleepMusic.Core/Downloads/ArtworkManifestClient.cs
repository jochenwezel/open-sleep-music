using System.Text.Json;
using System.Text.RegularExpressions;
using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.Core.Downloads;

public sealed partial class ArtworkManifestClient(HttpClient httpClient, Uri manifestUri, IDownloadLogSink? logSink = null)
{
    private readonly IDownloadLogSink _logSink = logSink ?? new NullDownloadLogSink();
    private Task<IReadOnlyDictionary<string, AudioTrack>>? _loadTask;

    public async Task<AudioTrack> ResolveAsync(AudioTrack fallback, string cacheRoot)
    {
        _loadTask ??= LoadAsync(cacheRoot);
        var overrides = await _loadTask;
        return overrides.TryGetValue(fallback.Id, out var entry) ? fallback with
        {
            ArtworkUri = entry.ArtworkUri,
            ArtworkFileName = entry.ArtworkFileName,
            ArtworkSha256 = entry.ArtworkSha256
        } : fallback;
    }

    private async Task<IReadOnlyDictionary<string, AudioTrack>> LoadAsync(string cacheRoot)
    {
        Directory.CreateDirectory(cacheRoot);
        var cachedPath = Path.Combine(cacheRoot, "artwork-catalog.json");
        var temporaryPath = cachedPath + ".part";
        try
        {
            using var response = await httpClient.GetAsync(manifestUri, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            await using (var source = await response.Content.ReadAsStreamAsync())
            await using (var target = File.Create(temporaryPath))
                await source.CopyToAsync(target);
            var downloaded = await ReadAsync(temporaryPath);
            File.Move(temporaryPath, cachedPath, true);
            return downloaded;
        }
        catch (Exception exception)
        {
            try { await _logSink.WriteSkippedAsync("artwork-catalog", manifestUri, $"{exception.GetType().Name}: {exception.Message}", CancellationToken.None); }
            catch { }
            try { return await ReadAsync(cachedPath); }
            catch { return new Dictionary<string, AudioTrack>(); }
        }
        finally
        {
            try { File.Delete(temporaryPath); }
            catch { }
        }
    }

    private static async Task<IReadOnlyDictionary<string, AudioTrack>> ReadAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        var manifest = await JsonSerializer.DeserializeAsync(stream, CatalogJsonContext.Default.ArtworkManifest)
            ?? throw new InvalidDataException("Artwork catalog is empty.");
        if (manifest.SchemaVersion != 1) throw new InvalidDataException($"Unsupported artwork schema {manifest.SchemaVersion}.");
        var result = new Dictionary<string, AudioTrack>(StringComparer.Ordinal);
        foreach (var entry in manifest.Tracks)
        {
            if (string.IsNullOrWhiteSpace(entry.TrackId) || entry.ArtworkUri.Scheme != Uri.UriSchemeHttps
                || !PortablePngName().IsMatch(entry.ArtworkFileName)
                || !Sha256().IsMatch(entry.ArtworkSha256))
                throw new InvalidDataException($"Invalid artwork entry '{entry.TrackId}'.");
            if (!result.TryAdd(entry.TrackId, new AudioTrack(
                entry.TrackId, entry.TrackId, "artwork-catalog", entry.ArtworkUri, entry.ArtworkUri,
                "project artwork", entry.ArtworkUri, entry.ArtworkFileName,
                ArtworkUri: entry.ArtworkUri, ArtworkFileName: entry.ArtworkFileName, ArtworkSha256: entry.ArtworkSha256)))
                throw new InvalidDataException($"Duplicate artwork entry '{entry.TrackId}'.");
        }
        return result;
    }

    [GeneratedRegex("^[a-z0-9_-]+\\.png$", RegexOptions.CultureInvariant)]
    private static partial Regex PortablePngName();
    [GeneratedRegex("^[0-9a-f]{64}$", RegexOptions.CultureInvariant)]
    private static partial Regex Sha256();
}
