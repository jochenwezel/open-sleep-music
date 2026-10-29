using System.Security.Cryptography;
using OpenSleepMusic.Core.Downloads;

namespace OpenSleepMusic.Core.Library;

/// <summary>Moves validated audio from an app-owned library, retaining sources on failure.</summary>
public sealed class AudioLibraryMigrator
{
    public async Task MigrateAsync(string sourceRoot, string destinationRoot,
        Action<string>? reportFailure = null, CancellationToken cancellationToken = default,
        Func<string, bool>? includeWorld = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceRoot);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationRoot);
        sourceRoot = Path.GetFullPath(sourceRoot);
        destinationRoot = Path.GetFullPath(destinationRoot);
        if (string.Equals(sourceRoot, destinationRoot, StringComparison.OrdinalIgnoreCase)) return;
        if (!Directory.Exists(sourceRoot) || IsLink(sourceRoot)) return;
        if (Directory.Exists(destinationRoot) && IsLink(destinationRoot))
            throw new IOException("Destination root is a symbolic link or reparse point.");

        // Only the existing <world>/<audio> layout is eligible. Do not follow
        // links, import unrelated files or recursively remove old directories.
        foreach (var world in Directory.GetDirectories(sourceRoot))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                if (includeWorld is not null && !includeWorld(Path.GetFileName(world))) continue;
                if (IsLink(world)) continue;
                foreach (var source in Directory.GetFiles(world))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!IsAudioExtension(Path.GetExtension(source))) continue;
                    var destination = Path.Combine(destinationRoot, Path.GetFileName(world), Path.GetFileName(source));
                    try
                    {
                        if (IsLink(source)) continue;
                        await MoveAudioAsync(source, destination, cancellationToken);
                    }
                    catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
                    {
                        reportFailure?.Invoke($"Audio migration failed for {source}: {exception.Message}");
                    }
                }
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                reportFailure?.Invoke($"Audio migration failed for {world}: {exception.Message}");
            }
        }
    }

    private static async Task MoveAudioAsync(string source, string destination, CancellationToken cancellationToken)
    {
        if (await AudioFileValidator.GetValidationErrorAsync(source, cancellationToken: cancellationToken) is { } error)
            throw new IOException(error);

        var directory = Path.GetDirectoryName(destination)!;
        Directory.CreateDirectory(directory);
        if (IsLink(directory) || (File.Exists(destination) && IsLink(destination)))
            throw new IOException("Destination is a symbolic link or reparse point.");

        var sourceHash = await HashAsync(source, cancellationToken);
        if (File.Exists(destination))
        {
            // Keep a differing existing download and its source for review.
            var destinationHash = await HashAsync(destination, cancellationToken);
            if (!sourceHash.AsSpan().SequenceEqual(destinationHash))
                throw new IOException("A different destination file already exists; both files were retained.");
            File.Delete(source);
            return;
        }

        var partial = destination + $".{Guid.NewGuid():N}.part";
        try
        {
            await using (var input = File.OpenRead(source))
            await using (var output = new FileStream(partial, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                await input.CopyToAsync(output, cancellationToken);

            var partialHash = await HashAsync(partial, cancellationToken);
            if (!sourceHash.AsSpan().SequenceEqual(partialHash))
                throw new IOException("Migration copy checksum mismatch; source was retained.");
            File.Move(partial, destination);
            // Only delete after the exact bytes are verified and installed.
            File.Delete(source);
        }
        finally
        {
            if (File.Exists(partial)) File.Delete(partial);
        }
    }

    private static async Task<byte[]> HashAsync(string path, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        return await SHA256.HashDataAsync(stream, cancellationToken);
    }

    private static bool IsLink(string path) => (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0;

    private static bool IsAudioExtension(string extension) => extension.ToLowerInvariant() is
        ".mp3" or ".ogg" or ".oga" or ".flac" or ".wav" or ".mp4" or ".m4a";
}
