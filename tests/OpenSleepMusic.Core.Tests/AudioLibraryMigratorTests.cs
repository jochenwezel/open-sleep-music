using OpenSleepMusic.Core.Library;

namespace OpenSleepMusic.Core.Tests;

public sealed class AudioLibraryMigratorTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"osm-migration-{Guid.NewGuid():N}");
    private string OldRoot => Path.Combine(_root, "old");
    private string NewRoot => Path.Combine(_root, "new");

    [Fact]
    public async Task MovesAudioIncludingBackgroundsAndFormerCollectionsWithoutChangingBytes()
    {
        var first = Write(OldRoot, "world", "piano.mp3", Audio(1));
        var second = Write(OldRoot, "former-preview-world", "background.ogg", Audio(2));

        await Migrate();
        await Migrate(); // Restarting after completion is harmless.

        Assert.False(File.Exists(first));
        Assert.False(File.Exists(second));
        Assert.Equal(Audio(1), File.ReadAllBytes(Path.Combine(NewRoot, "world", "piano.mp3")));
        Assert.Equal(Audio(2), File.ReadAllBytes(Path.Combine(NewRoot, "former-preview-world", "background.ogg")));
        Assert.Empty(Directory.GetFiles(NewRoot, "*.part", SearchOption.AllDirectories));
    }

    [Fact]
    public async Task PreservesInvalidUnknownAndPartialFilesAndContinuesWithValidAudio()
    {
        var invalid = Write(OldRoot, "world", "invalid.mp3", "<html>error</html>"u8.ToArray());
        var unknown = Write(OldRoot, "world", "notes.txt", Audio(1));
        var partial = Write(OldRoot, "world", "track.mp3.part", Audio(1));
        var valid = Write(OldRoot, "world", "valid.mp3", Audio(1));
        var errors = new List<string>();

        await Migrate(errors.Add);

        Assert.Single(errors);
        Assert.True(File.Exists(invalid));
        Assert.True(File.Exists(unknown));
        Assert.True(File.Exists(partial));
        Assert.False(File.Exists(valid));
        Assert.Single(Directory.GetFiles(NewRoot, "*", SearchOption.AllDirectories));
    }

    [Fact]
    public async Task RemovesIdenticalLegacyDuplicateAfterInterruptedSourceCleanup()
    {
        var source = Write(OldRoot, "world", "same.mp3", Audio(1));
        Write(NewRoot, "world", "same.mp3", Audio(1));

        await Migrate();

        Assert.False(File.Exists(source));
        Assert.Equal(Audio(1), File.ReadAllBytes(Path.Combine(NewRoot, "world", "same.mp3")));
    }

    [Fact]
    public async Task PreservesBothDifferingFilesAndContinuesOtherWorlds()
    {
        var source = Write(OldRoot, "world", "conflict.mp3", Audio(1));
        var destination = Write(NewRoot, "world", "conflict.mp3", Audio(2));
        Write(OldRoot, "other", "valid.mp3", Audio(3));
        var errors = new List<string>();

        await Migrate(errors.Add);

        Assert.Single(errors);
        Assert.Equal(Audio(1), File.ReadAllBytes(source));
        Assert.Equal(Audio(2), File.ReadAllBytes(destination));
        Assert.True(File.Exists(Path.Combine(NewRoot, "other", "valid.mp3")));
    }

    [Fact]
    public async Task LockedSourceDoesNotBlockOtherFilesAndCanBeRetried()
    {
        var source = Write(OldRoot, "world", "locked.mp3", Audio(1));
        Write(OldRoot, "world", "valid.mp3", Audio(2));
        var errors = new List<string>();
        using (File.Open(source, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            await Migrate(errors.Add);

        Assert.NotEmpty(errors);
        Assert.True(File.Exists(source));
        Assert.True(File.Exists(Path.Combine(NewRoot, "world", "valid.mp3")));
        await Migrate();
        Assert.False(File.Exists(source));
    }

    [Fact]
    public async Task DeletedWorldsAreNotReimportedFromRetainedLegacySources()
    {
        var retained = Write(OldRoot, "deleted", "conflict.mp3", Audio(1));
        Write(OldRoot, "remaining", "valid.mp3", Audio(2));

        await new AudioLibraryMigrator().MigrateAsync(OldRoot, NewRoot,
            includeWorld: worldId => worldId != "deleted");

        Assert.True(File.Exists(retained));
        Assert.False(Directory.Exists(Path.Combine(NewRoot, "deleted")));
        Assert.True(File.Exists(Path.Combine(NewRoot, "remaining", "valid.mp3")));
    }

    [Fact]
    public async Task MissingOrIdenticalRootsDoNotDeleteAnything()
    {
        await Migrate();
        Assert.False(Directory.Exists(NewRoot));
        var source = Write(OldRoot, "world", "valid.mp3", Audio(1));
        await new AudioLibraryMigrator().MigrateAsync(OldRoot, OldRoot);
        Assert.True(File.Exists(source));
    }

    [Fact]
    public async Task CancellationRetainsSources()
    {
        var source = Write(OldRoot, "world", "valid.mp3", Audio(1));
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            new AudioLibraryMigrator().MigrateAsync(OldRoot, NewRoot, cancellationToken: cancellation.Token));
        Assert.True(File.Exists(source));
    }

    private Task Migrate(Action<string>? reportFailure = null) =>
        new AudioLibraryMigrator().MigrateAsync(OldRoot, NewRoot, reportFailure);

    private static string Write(string root, string world, string fileName, byte[] bytes)
    {
        var directory = Path.Combine(root, world);
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, fileName);
        File.WriteAllBytes(path, bytes);
        return path;
    }

    private static byte[] Audio(byte marker)
    {
        var bytes = new byte[256];
        "ID3"u8.CopyTo(bytes);
        bytes[^1] = marker;
        return bytes;
    }

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, recursive: true);
    }
}
