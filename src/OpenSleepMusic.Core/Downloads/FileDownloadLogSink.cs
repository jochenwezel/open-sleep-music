using System.Text.Json;

namespace OpenSleepMusic.Core.Downloads;

public sealed class FileDownloadLogSink(string logFilePath) : IDownloadLogSink
{
    public async Task WriteSkippedAsync(
        string trackId,
        Uri uri,
        string reason,
        CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(logFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var entry = JsonSerializer.Serialize(new
        {
            timestampUtc = DateTimeOffset.UtcNow,
            trackId,
            url = uri.AbsoluteUri,
            reason
        });

        await File.AppendAllTextAsync(logFilePath, entry + Environment.NewLine, cancellationToken);
    }
}
