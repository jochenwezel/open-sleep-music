using System.Diagnostics;
using OpenSleepMusic.Core.Library;

namespace OpenSleepMusic.App;

internal sealed class AudioLibraryStorage
{
    public string Root { get; } = GetRoot();
    private Task? _initialization;

    // Share initialization between startup, refresh, download and deletion.
    public Task InitializeAsync() => _initialization ??= InitializeCoreAsync();

    public void RememberDeletedWorld(string worldId)
    {
#if OPEN_SLEEP_MUSIC_LEGACY_AUDIO_MIGRATION
        // A retained legacy conflict must not resurrect a deleted collection.
        Preferences.Default.Set($"LegacyAudioMigration.Deleted.{worldId}", true);
#endif
    }

    private static string GetRoot()
    {
#if WINDOWS
        // LocalCache is persistent app data, not TemporaryFolder. Windows has
        // no per-subfolder backup exclusion for packaged LocalFolder data.
        if (AppInfo.Current.PackagingModel == AppPackagingModel.Packaged)
            return Path.Combine(Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path, "Audio");
#endif
        return Path.Combine(FileSystem.AppDataDirectory, "Audio");
    }

    private async Task InitializeCoreAsync()
    {
        Directory.CreateDirectory(Root);
#if IOS || MACCATALYST
        var error = Foundation.NSFileManager.SetSkipBackupAttribute(Root, true);
        if (error is not null)
            throw new IOException($"Cannot exclude downloaded audio from backup: {error.LocalizedDescription}");
#endif
#if OPEN_SLEEP_MUSIC_LEGACY_AUDIO_MIGRATION
        await MigrateLegacyAudioAsync();
#else
        await Task.CompletedTask;
#endif
    }

#if OPEN_SLEEP_MUSIC_LEGACY_AUDIO_MIGRATION
    private async Task MigrateLegacyAudioAsync()
    {
        try
        {
            var music = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            // Never turn an unsupported special folder into a relative path.
            if (string.IsNullOrWhiteSpace(music) || !Path.IsPathFullyQualified(music)) return;
            await new AudioLibraryMigrator().MigrateAsync(
                Path.Combine(music, "Open Sleep Music"), Root,
                message => Debug.WriteLine(message),
                includeWorld: worldId => !Preferences.Default.Get($"LegacyAudioMigration.Deleted.{worldId}", false));
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            // An inaccessible legacy library must not prevent new downloads.
            Debug.WriteLine($"Legacy audio migration unavailable: {exception}");
        }
    }
#endif
}
