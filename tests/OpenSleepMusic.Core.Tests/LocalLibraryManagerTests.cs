using OpenSleepMusic.Core.Catalog;
using OpenSleepMusic.Core.Library;

namespace OpenSleepMusic.Core.Tests;

public sealed class LocalLibraryManagerTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"osm-manager-{Guid.NewGuid():N}");

    [Fact]
    public async Task GetStatusAsync_ReturnsValidTrackCountAndSize()
    {
        var first = Track("first", "first.mp3");
        var second = Track("second", "second.mp3");
        var world = new SleepWorld("forest", "Wald", "", "", [first, second]);
        var directory = Path.Combine(_root, world.Id);
        Directory.CreateDirectory(directory);
        await File.WriteAllBytesAsync(Path.Combine(directory, first.FileName), CreateAudioBytes());

        var status = await new LocalLibraryManager().GetStatusAsync(_root, world);

        Assert.Equal(1, status.AvailableTrackCount);
        Assert.Equal(256, status.SizeBytes);
        Assert.True(status.HasDownloads);
        Assert.False(status.IsComplete);
    }

    [Fact]
    public async Task DeleteWorldAsync_RemovesOnlyTheSelectedWorldDirectory()
    {
        var selected = new SleepWorld("forest", "Wald", "", "", [Track("first", "first.mp3")]);
        var retained = new SleepWorld("rain", "Regen", "", "", [Track("second", "second.mp3")]);
        Directory.CreateDirectory(Path.Combine(_root, selected.Id));
        Directory.CreateDirectory(Path.Combine(_root, retained.Id));
        await File.WriteAllBytesAsync(Path.Combine(_root, selected.Id, "first.mp3"), CreateAudioBytes());
        await File.WriteAllBytesAsync(Path.Combine(_root, retained.Id, "second.mp3"), CreateAudioBytes());

        await new LocalLibraryManager().DeleteWorldAsync(_root, selected);

        Assert.False(Directory.Exists(Path.Combine(_root, selected.Id)));
        Assert.True(Directory.Exists(Path.Combine(_root, retained.Id)));
    }

    private static AudioTrack Track(string id, string fileName) => new(
        id,
        id,
        "creator",
        new("https://example.test/audio"),
        new("https://example.test/source"),
        "CC0",
        new("https://creativecommons.org/publicdomain/zero/1.0/"),
        fileName,
        60);

    private static byte[] CreateAudioBytes()
    {
        var bytes = new byte[256];
        "ID3"u8.CopyTo(bytes);
        return bytes;
    }

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, true);
    }
}
