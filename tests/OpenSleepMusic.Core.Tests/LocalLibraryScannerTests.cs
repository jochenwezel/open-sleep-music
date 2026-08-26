using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Library;
using System.Security.Cryptography;

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
        await File.WriteAllBytesAsync(Path.Combine(_root, world.Id, valid.FileName), CreateAudioBytes(1));
        await File.WriteAllTextAsync(Path.Combine(_root, world.Id, invalid.FileName), "<html>error</html>");

        var result = await new LocalLibraryScanner().ScanAsync(_root, [world], verifyChecksums: true);

        var local = Assert.Single(result);
        Assert.Equal(valid, local.Track);
        Assert.Equal(world, local.SleepWorld);
    }

    [Fact]
    public async Task ScanAsync_RejectsAFileWhoseCatalogChecksumNoLongerMatches()
    {
        var originalBytes = CreateAudioBytes(1);
        var expectedSha1 = Convert.ToHexStringLower(SHA1.HashData(originalBytes));
        var track = Track("changed", "changed.mp3") with { Sha1 = expectedSha1 };
        var world = new SleepWorld("rain", "Regen", "", "", [track]);
        Directory.CreateDirectory(Path.Combine(_root, world.Id));
        await File.WriteAllBytesAsync(Path.Combine(_root, world.Id, track.FileName), CreateAudioBytes(2));

        var result = await new LocalLibraryScanner().ScanAsync(_root, [world], verifyChecksums: true);

        Assert.Empty(result);
    }

    private static byte[] CreateAudioBytes(byte marker)
    {
        var bytes = new byte[256];
        "ID3"u8.CopyTo(bytes);
        bytes[^1] = marker;
        return bytes;
    }

    private static AudioTrack Track(string id, string fileName) => new(id, id, "creator", new("https://example.test/audio"), new("https://example.test/source"), "CC0", new("https://creativecommons.org/publicdomain/zero/1.0/"), fileName, 60);

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, true);
    }
}
