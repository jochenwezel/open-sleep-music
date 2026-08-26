using OpenSleepMusic.Core.Catalog;

namespace OpenSleepMusic.Core.Library;

public sealed class LocalLibraryManager(LocalLibraryScanner? scanner = null)
{
    private readonly LocalLibraryScanner _scanner = scanner ?? new LocalLibraryScanner();

    public async Task<SleepWorldLibraryStatus> GetStatusAsync(
        string libraryRoot,
        SleepWorld sleepWorld,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(libraryRoot);
        ArgumentNullException.ThrowIfNull(sleepWorld);

        var tracks = await _scanner.ScanAsync(libraryRoot, [sleepWorld], cancellationToken);
        var size = tracks.Sum(track => new FileInfo(track.FilePath).Length);
        return new SleepWorldLibraryStatus(sleepWorld, tracks.Count, size);
    }

    public Task DeleteWorldAsync(
        string libraryRoot,
        SleepWorld sleepWorld,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(libraryRoot);
        ArgumentNullException.ThrowIfNull(sleepWorld);
        cancellationToken.ThrowIfCancellationRequested();

        var root = Path.GetFullPath(libraryRoot);
        var worldDirectory = Path.GetFullPath(Path.Combine(root, sleepWorld.Id));
        var rootWithSeparator = root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        if (!worldDirectory.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The sleep-world directory is outside the library root.");
        }

        if (Directory.Exists(worldDirectory))
        {
            Directory.Delete(worldDirectory, recursive: true);
        }

        return Task.CompletedTask;
    }
}
