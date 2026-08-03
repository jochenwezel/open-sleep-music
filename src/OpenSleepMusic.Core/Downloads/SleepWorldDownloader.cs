using System.Net;
using System.Security.Cryptography;
using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.Core.Downloads;

public sealed class SleepWorldDownloader(HttpClient httpClient, IDownloadLogSink? logSink = null)
{
    private readonly IDownloadLogSink _logSink = logSink ?? new NullDownloadLogSink();

    public async Task<DownloadBatchResult> DownloadAsync(
        SleepWorld sleepWorld,
        string destinationRoot,
        IProgress<DownloadProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var destination = Path.Combine(destinationRoot, sleepWorld.Id);
        Directory.CreateDirectory(destination);

        var downloaded = 0;
        var existing = 0;
        var skipped = 0;

        for (var index = 0; index < sleepWorld.Tracks.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var track = sleepWorld.Tracks[index];
            progress?.Report(new DownloadProgress(index, sleepWorld.Tracks.Count, track.Title));

            var targetPath = Path.Combine(destination, track.FileName);
            if (File.Exists(targetPath)
                && await GetValidationErrorAsync(targetPath, track.Sha1, cancellationToken) is null)
            {
                existing++;
                continue;
            }

            if (await TryDownloadAsync(track, targetPath, cancellationToken))
            {
                downloaded++;
            }
            else
            {
                skipped++;
            }
        }

        progress?.Report(new DownloadProgress(sleepWorld.Tracks.Count, sleepWorld.Tracks.Count, string.Empty));
        return new DownloadBatchResult(downloaded, existing, skipped);
    }

    private async Task<bool> TryDownloadAsync(
        AudioTrack track,
        string targetPath,
        CancellationToken cancellationToken)
    {
        var temporaryPath = targetPath + ".part";

        try
        {
            using var response = await httpClient.GetAsync(
                track.DownloadUri,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                await LogAsync(track, $"HTTP {(int)response.StatusCode} ({response.ReasonPhrase})", cancellationToken);
                return false;
            }

            await using (var source = await response.Content.ReadAsStreamAsync(cancellationToken))
            await using (var target = File.Create(temporaryPath))
            {
                await source.CopyToAsync(target, cancellationToken);
            }

            var validationError = await GetValidationErrorAsync(temporaryPath, track.Sha1, cancellationToken);
            if (validationError is not null)
            {
                await LogAsync(
                    track,
                    $"{validationError} (Content-Type: {response.Content.Headers.ContentType})",
                    cancellationToken);
                return false;
            }

            File.Move(temporaryPath, targetPath, true);
            return true;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            await LogAsync(track, $"{exception.GetType().Name}: {exception.Message}", cancellationToken);
            return false;
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private static async Task<string?> GetValidationErrorAsync(
        string path,
        string? expectedSha1,
        CancellationToken cancellationToken)
    {
        var file = new FileInfo(path);
        if (!file.Exists || file.Length < 128)
        {
            return "Response is empty or too small to be a valid audio file";
        }

        var header = new byte[12];
        await using var stream = File.OpenRead(path);
        var bytesRead = await stream.ReadAsync(header, cancellationToken);
        if (!AudioFileInspector.IsSupportedAudio(header.AsSpan(0, bytesRead)))
        {
            return "Response is not a supported audio file";
        }

        if (string.IsNullOrWhiteSpace(expectedSha1))
        {
            return null;
        }

        stream.Position = 0;
        var actualSha1 = Convert.ToHexStringLower(await SHA1.HashDataAsync(stream, cancellationToken));
        return string.Equals(actualSha1, expectedSha1, StringComparison.OrdinalIgnoreCase)
            ? null
            : $"Audio checksum mismatch (expected {expectedSha1}, received {actualSha1})";
    }

    private async Task LogAsync(AudioTrack track, string reason, CancellationToken cancellationToken)
    {
        try
        {
            await _logSink.WriteSkippedAsync(track.Id, track.DownloadUri, reason, cancellationToken);
        }
        catch
        {
            // Logging must never turn a recoverable media failure into a user-visible download error.
        }
    }
}
