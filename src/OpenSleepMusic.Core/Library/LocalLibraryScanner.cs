using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Downloads;

namespace OpenSleepMusic.Core.Library;

public sealed class LocalLibraryScanner
{
    public async Task<IReadOnlyList<LocalLibraryTrack>> ScanAsync(
        string libraryRoot,
        IEnumerable<SleepWorld> sleepWorlds,
        CancellationToken cancellationToken = default,
        bool verifyChecksums = false)
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
                var expectedSha1 = verifyChecksums ? track.Sha1 : null;
                if (await AudioFileValidator.GetValidationErrorAsync(path, expectedSha1, cancellationToken) is null)
                    result.Add(new LocalLibraryTrack(track, world, path));
            }
        }
        return result;
    }
}
