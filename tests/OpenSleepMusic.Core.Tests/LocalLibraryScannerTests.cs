using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Library;

namespace OpenSleepMusic.Core.Tests;

public sealed class LocalLibraryScannerTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"osm-library-{Guid.NewGuid():N}");

    [Fact]
    public async Task ScanAsync_ReturnsOnlyValidDownloadedCatalogFiles()
    {
        var valid = Track("valid", "valid.mp3");
        var invalid = Track("invalid", "invalid.mp3");
        var missing = Track("missing", "missing.mp3");
        var world = new SleepWorld("rain", "Regen", "", "", [valid, invalid, missing]);
        Directory.CreateDirectory(Path.Combine(_root, world.Id));
        await File.WriteAllBytesAsync(Path.Combine(_root, world.Id, valid.FileName), "ID3valid audio"u8.ToArray());
        await File.WriteAllTextAsync(Path.Combine(_root, world.Id, invalid.FileName), "<html>error</html>");

        var result = await new LocalLibraryScanner().ScanAsync(_root, [world]);

        var local = Assert.Single(result);
        Assert.Equal(valid, local.Track);
        Assert.Equal(world, local.SleepWorld);
    }

    private static AudioTrack Track(string id, string fileName) => new(id, id, "creator", new("https://example.test/audio"), new("https://example.test/source"), "CC0", new("https://creativecommons.org/publicdomain/zero/1.0/"), fileName, 60);

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, true);
    }
}
