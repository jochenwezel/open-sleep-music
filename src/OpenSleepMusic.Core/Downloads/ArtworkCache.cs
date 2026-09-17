using System.Net;
using System.Security.Cryptography;
using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.Core.Downloads;

public sealed class ArtworkCache(HttpClient httpClient, IDownloadLogSink? logSink = null, TimeSpan? timeout = null)
{
    private readonly IDownloadLogSink _logSink = logSink ?? new NullDownloadLogSink();
    private readonly TimeSpan _timeout = timeout ?? TimeSpan.FromMinutes(2);

    public async Task<string?> GetAsync(AudioTrack track, string cacheRoot, CancellationToken cancellationToken = default)
    {
        if (track.ArtworkUri is null || string.IsNullOrWhiteSpace(track.ArtworkFileName)
            || string.IsNullOrWhiteSpace(track.ArtworkSha256)) return null;

        Directory.CreateDirectory(cacheRoot);
        var targetPath = Path.Combine(cacheRoot, track.ArtworkFileName);
        if (await IsValidAsync(targetPath, track.ArtworkSha256, cancellationToken)) return targetPath;
        TryDelete(targetPath);

        var temporaryPath = targetPath + ".part";
        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(_timeout);
        try
        {
            using var response = await httpClient.GetAsync(track.ArtworkUri, HttpCompletionOption.ResponseHeadersRead, timeoutSource.Token);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                await LogAsync(track, $"Artwork HTTP {(int)response.StatusCode} ({response.ReasonPhrase})", cancellationToken);
                return null;
            }

            await using (var source = await response.Content.ReadAsStreamAsync(timeoutSource.Token))
            await using (var target = File.Create(temporaryPath))
                await source.CopyToAsync(target, timeoutSource.Token);

            if (!await IsValidAsync(temporaryPath, track.ArtworkSha256, timeoutSource.Token))
            {
                await LogAsync(track, $"Invalid artwork or SHA-256 mismatch (Content-Type: {response.Content.Headers.ContentType})", cancellationToken);
                return null;
            }

            File.Move(temporaryPath, targetPath, true);
            return targetPath;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception exception)
        {
            await LogAsync(track, $"Artwork {exception.GetType().Name}: {exception.Message}", CancellationToken.None);
            return null;
        }
        finally { TryDelete(temporaryPath); }
    }

    private static async Task<bool> IsValidAsync(string path, string expectedSha256, CancellationToken cancellationToken)
    {
        if (!File.Exists(path)) return false;
        await using var stream = File.OpenRead(path);
        if (stream.Length < 8) return false;
        var signature = new byte[8];
        if (await stream.ReadAsync(signature, cancellationToken) != signature.Length
            || !signature.AsSpan().SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })) return false;
        stream.Position = 0;
        var actual = Convert.ToHexStringLower(await SHA256.HashDataAsync(stream, cancellationToken));
        return string.Equals(actual, expectedSha256, StringComparison.OrdinalIgnoreCase);
    }

    private async Task LogAsync(AudioTrack track, string reason, CancellationToken cancellationToken)
    {
        try { await _logSink.WriteSkippedAsync($"{track.Id}:artwork", track.ArtworkUri!, reason, cancellationToken); }
        catch { }
    }

    private static void TryDelete(string path)
    {
        try { File.Delete(path); }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException) { }
    }
}
