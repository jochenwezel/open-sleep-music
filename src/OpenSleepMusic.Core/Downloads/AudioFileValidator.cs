using System.Security.Cryptography;

namespace OpenSleepMusic.Core.Downloads;

public static class AudioFileValidator
{
    public static async Task<string?> GetValidationErrorAsync(
        string path,
        string? expectedSha1 = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var file = new FileInfo(path);
        if (!file.Exists || file.Length < 128)
        {
            return "Response is empty or too small to be a valid audio file";
        }

        // Read far enough to identify Ogg Skeleton's initial `fishead` packet.
        // Android's platform MediaPlayer rejects that otherwise valid container
        // layout on some devices even when a Vorbis stream follows it.
        var header = new byte[128];
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
}
