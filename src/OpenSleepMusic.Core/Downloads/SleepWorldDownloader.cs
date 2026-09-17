using System.Net;
using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.Core.Downloads;

public sealed class SleepWorldDownloader(HttpClient httpClient, IDownloadLogSink? logSink = null, TimeSpan? trackTimeout = null)
{
    private readonly IDownloadLogSink _logSink = logSink ?? new NullDownloadLogSink();
    private readonly TimeSpan _trackTimeout = trackTimeout is { } timeout && timeout <= TimeSpan.Zero
        ? throw new ArgumentOutOfRangeException(nameof(trackTimeout))
        : trackTimeout ?? TimeSpan.FromMinutes(15);

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
            if (File.Exists(targetPath))
            {
                var validationError = await AudioFileValidator.GetValidationErrorAsync(
                    targetPath,
                    track.Sha1,
                    cancellationToken);
                if (validationError is null)
                {
                    existing++;
                    continue;
                }

                try
                {
                    File.Delete(targetPath);
                }
                catch (Exception exception)
                {
                    await LogAsync(
                        track,
                        $"Invalid local file could not be removed ({exception.GetType().Name}: {exception.Message})",
                        cancellationToken);
                    skipped++;
                    continue;
                }
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
        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(_trackTimeout);
        var trackToken = timeoutSource.Token;

        try
        {
            using var response = await httpClient.GetAsync(
                track.DownloadUri,
                HttpCompletionOption.ResponseHeadersRead,
                trackToken);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                await LogAsync(track, $"HTTP {(int)response.StatusCode} ({response.ReasonPhrase})", cancellationToken);
                return false;
            }

            await using (var source = await response.Content.ReadAsStreamAsync(trackToken))
            await using (var target = File.Create(temporaryPath))
            {
                await source.CopyToAsync(target, trackToken);
            }

            var validationError = await AudioFileValidator.GetValidationErrorAsync(temporaryPath, track.Sha1, trackToken);
            if (validationError is not null)
            {
                await LogAsync(
                    track,
                    $"{validationError} (Content-Type: {response.Content.Headers.ContentType})",
                    cancellationToken);
                return false;
            }

            trackToken.ThrowIfCancellationRequested();
            File.Move(temporaryPath, targetPath, true);
            return true;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException) when (timeoutSource.IsCancellationRequested)
        {
            await LogAsync(track, $"Track download timed out after {_trackTimeout}", cancellationToken);
            return false;
        }
        catch (Exception exception)
        {
            await LogAsync(track, $"{exception.GetType().Name}: {exception.Message}", cancellationToken);
            return false;
        }
        finally
        {
            try
            {
                File.Delete(temporaryPath);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                await LogAsync(track, $"Partial file could not be removed: {exception.Message}", CancellationToken.None);
            }
        }
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
