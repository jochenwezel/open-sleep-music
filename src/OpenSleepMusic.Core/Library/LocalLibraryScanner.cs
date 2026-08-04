using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Downloads;

namespace OpenSleepMusic.Core.Library;

public sealed class LocalLibraryScanner
{
    public async Task<IReadOnlyList<LocalLibraryTrack>> ScanAsync(string libraryRoot, IEnumerable<SleepWorld> sleepWorlds, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(libraryRoot);
        ArgumentNullException.ThrowIfNull(sleepWorlds);
        var result = new List<LocalLibraryTrack>();
        foreach (var world in sleepWorlds)
        {
            foreach (var track in world.Tracks)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var path = Path.Combine(libraryRoot, world.Id, track.FileName);
                if (!File.Exists(path)) continue;
                var header = new byte[16];
                await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
                var count = await stream.ReadAsync(header, cancellationToken);
                if (AudioFileInspector.IsSupportedAudio(header.AsSpan(0, count)))
                    result.Add(new LocalLibraryTrack(track, world, path));
            }
        }
        return result;
    }
}
