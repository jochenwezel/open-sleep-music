using System.Text.Json;
using System.Text.RegularExpressions;
using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.Core.Downloads;

public sealed partial class CatalogUpdateClient(
    HttpClient httpClient,
    Uri catalogUri,
    IDownloadLogSink? logSink = null,
    TimeSpan? timeout = null)
{
    private const long MaximumCatalogBytes = 5 * 1024 * 1024;
    private readonly IDownloadLogSink _logSink = logSink ?? new NullDownloadLogSink();
    private readonly TimeSpan _timeout = timeout ?? TimeSpan.FromSeconds(20);

    public async Task<CatalogSnapshot> ResolveAsync(
        CatalogSnapshot embedded,
        string cacheRoot,
        CancellationToken cancellationToken = default)
    {
        var cachedPath = Path.Combine(cacheRoot, "media-catalog.json");
        var temporaryPath = cachedPath + ".part";
        var fallback = embedded;

        try
        {
            var cached = await ReadAndValidateAsync(cachedPath, cancellationToken);
            if (cached.GeneratedAtUtc >= fallback.GeneratedAtUtc)
                fallback = cached;
        }
        catch
        {
            // A missing or invalid cache is expected on first launch and after interrupted writes.
        }

        try
        {
            Directory.CreateDirectory(cacheRoot);
            using var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCancellation.CancelAfter(_timeout);
            using var response = await httpClient.GetAsync(
                catalogUri,
                HttpCompletionOption.ResponseHeadersRead,
                timeoutCancellation.Token);
            response.EnsureSuccessStatusCode();
            if (response.Content.Headers.ContentLength is > MaximumCatalogBytes)
                throw new InvalidDataException("Media catalog exceeds the size limit.");

            await using (var source = await response.Content.ReadAsStreamAsync(timeoutCancellation.Token))
            await using (var target = new FileStream(temporaryPath, FileMode.Create, FileAccess.Write, FileShare.None))
                await CopyWithLimitAsync(source, target, timeoutCancellation.Token);

            var downloaded = await ReadAndValidateAsync(temporaryPath, timeoutCancellation.Token);
            if (downloaded.GeneratedAtUtc < fallback.GeneratedAtUtc)
                return fallback;

            File.Move(temporaryPath, cachedPath, true);
            return downloaded;
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            await TryLogAsync(exception);
            return fallback;
        }
        finally
        {
            try { File.Delete(temporaryPath); }
            catch { }
        }
    }

    private async Task TryLogAsync(Exception exception)
    {
        try
        {
            await _logSink.WriteSkippedAsync(
                "media-catalog",
                catalogUri,
                $"{exception.GetType().Name}: {exception.Message}",
                CancellationToken.None);
        }
        catch { }
    }

    private static async Task CopyWithLimitAsync(Stream source, Stream target, CancellationToken cancellationToken)
    {
        var buffer = new byte[81920];
        long total = 0;
        int read;
        while ((read = await source.ReadAsync(buffer, cancellationToken)) > 0)
        {
            total += read;
            if (total > MaximumCatalogBytes)
                throw new InvalidDataException("Media catalog exceeds the size limit.");
            await target.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
        }
    }

    private static async Task<CatalogSnapshot> ReadAndValidateAsync(string path, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        var manifest = await JsonSerializer.DeserializeAsync(
            stream,
            CatalogJsonContext.Default.CatalogManifest,
            cancellationToken) ?? throw new InvalidDataException("Media catalog is empty.");
        Validate(manifest);
        return new CatalogSnapshot(manifest.GeneratedAtUtc, manifest.SleepWorlds);
    }

    private static void Validate(CatalogManifest manifest)
    {
        if (manifest.SchemaVersion != 1)
            throw new InvalidDataException($"Unsupported media catalog schema {manifest.SchemaVersion}.");
        if (manifest.GeneratedAtUtc < new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
            || manifest.GeneratedAtUtc > DateTimeOffset.UtcNow.AddDays(1))
            throw new InvalidDataException("Media catalog generation timestamp is invalid.");
        if (manifest.SleepWorlds is null || manifest.SleepWorlds.Count is < 1 or > 100)
            throw new InvalidDataException("Media catalog collection count is invalid.");
        if (!BuildChannel.IsPreview && manifest.SleepWorlds.Any(world => world.Id is "preselection" or "pre-qualify"
                || world.ExternalReferences is { Count: > 0 }))
            throw new InvalidDataException("A stable app cannot load the preview preselection collection.");

        var worldIds = new HashSet<string>(StringComparer.Ordinal);
        var trackIds = new HashSet<string>(StringComparer.Ordinal);
        var fileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var trackCount = 0;
        foreach (var world in manifest.SleepWorlds)
        {
            if (world is null || !PortableId().IsMatch(world.Id) || string.IsNullOrWhiteSpace(world.Name)
                || string.IsNullOrWhiteSpace(world.Description) || world.Tracks is null
                || !worldIds.Add(world.Id))
                throw new InvalidDataException($"Invalid or duplicate collection '{world?.Id}'.");

            foreach (var track in world.Tracks)
            {
                trackCount++;
                if (trackCount > 10_000 || track is null || !PortableId().IsMatch(track.Id)
                    || string.IsNullOrWhiteSpace(track.Title) || string.IsNullOrWhiteSpace(track.Creator)
                    || string.IsNullOrWhiteSpace(track.License) || !IsHttps(track.DownloadUri)
                    || !IsHttps(track.SourcePageUri) || !IsHttps(track.LicenseUri)
                    || !PortableAudioName().IsMatch(track.FileName) || track.DurationSeconds <= 0
                    || track.DurationSeconds > TimeSpan.FromDays(1).TotalSeconds
                    || track.StartOffsetMilliseconds < 0
                    || track.StartOffsetMilliseconds >= track.DurationSeconds * 1000
                    || track.VolumeGain is <= 0 or > 8 || track.PlaybackSpeed is < 0.25 or > 4
                    || track.LicenseReviewStatus is not ("verified" or "unchecked")
                    || track.LicenseReviewStatus == "unchecked"
                        && (!BuildChannel.IsPreview || world.Id != "preselection")
                    || track.Sha1 is not null && !Sha1().IsMatch(track.Sha1)
                    || !trackIds.Add(track.Id) || !fileNames.Add(track.FileName))
                    throw new InvalidDataException($"Invalid or duplicate track '{track?.Id}'.");

                ValidateArtworkLayer(track.ArtworkUri, track.ArtworkFileName, track.ArtworkSha256, track.Id);
                ValidateArtworkLayer(track.SongMotifUri, track.SongMotifFileName, track.SongMotifSha256, track.Id);
                ValidateArtworkLayer(track.FallbackMotifUri, track.FallbackMotifFileName, track.FallbackMotifSha256, track.Id);
                if (world.Id != "preselection")
                    BuiltInPreselection.EnsureProductionTrackAllowed(track);
            }
            if (world.ExternalReferences is { Count: > 0 } references)
            {
                if (!BuildChannel.IsPreview || world.Id != "pre-qualify" || references.Count > 1000)
                    throw new InvalidDataException("External audition references are preview-only.");
                foreach (var reference in references)
                {
                    if (reference is null || !PortableId().IsMatch(reference.Id)
                        || !trackIds.Add(reference.Id) || string.IsNullOrWhiteSpace(reference.Title)
                        || string.IsNullOrWhiteSpace(reference.Creator) || !IsHttps(reference.SourcePageUri)
                        || reference.Instrumentation is not { Count: > 0 }
                        || reference.LicenseReviewStatus is not ("unchecked" or "verified" or "rejected")
                        || reference.DownloadUri is not null)
                        throw new InvalidDataException("Invalid external audition reference.");
                }
            }
        }
    }

    private static void ValidateArtworkLayer(Uri? uri, string? fileName, string? sha256, string trackId)
    {
        var present = new object?[] { uri, fileName, sha256 }.Count(value => value is not null);
        if (present == 0) return;
        if (present != 3 || !IsHttps(uri!) || !PortablePngName().IsMatch(fileName!) || !Sha256().IsMatch(sha256!))
            throw new InvalidDataException($"Invalid artwork metadata for track '{trackId}'.");
    }

    private static bool IsHttps(Uri uri) => uri.IsAbsoluteUri && uri.Scheme == Uri.UriSchemeHttps;

    [GeneratedRegex("^[a-z0-9][a-z0-9_-]*$", RegexOptions.CultureInvariant)]
    private static partial Regex PortableId();
    [GeneratedRegex("^[a-zA-Z0-9][a-zA-Z0-9._-]*\\.(mp3|ogg|oga|flac|wav|m4a|mp4)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex PortableAudioName();
    [GeneratedRegex("^[a-z0-9][a-z0-9._-]*\\.png$", RegexOptions.CultureInvariant)]
    private static partial Regex PortablePngName();
    [GeneratedRegex("^[0-9a-fA-F]{40}$", RegexOptions.CultureInvariant)]
    private static partial Regex Sha1();
    [GeneratedRegex("^[0-9a-fA-F]{64}$", RegexOptions.CultureInvariant)]
    private static partial Regex Sha256();
}
