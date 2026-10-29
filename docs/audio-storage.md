# Persistent offline audio storage

Audio downloads and background dependencies use `Audio/<sleep-world>/<filename>`
below `FileSystem.AppDataDirectory`. They remain available across app restarts and
updates. This is persistent storage, not a temporary or automatically purged cache.
Playback, scanning, repair and collection deletion all use the same root. No music
library permission or music export feature is needed for ordinary offline use.

On packaged Windows, the root is instead `ApplicationData.Current.LocalCacheFolder/Audio`.
Windows documents this as persistent app-controlled data excluded from app backup,
unlike `TemporaryFolder`. Unpackaged Windows uses MAUI's app-specific local AppData
directory. Changing the Windows packaging model can change the app data identity;
that transition needs its own migration review.

## Legacy migration through 2026

`OpenSleepMusicReleaseDate` defaults to the UTC build date. The release workflow
pins it to the GitHub release's `published_at`, including on reruns. Builds dated
before 2027 define `OPEN_SLEEP_MUSIC_LEGACY_AUDIO_MIGRATION`. Builds dated 2027 or
later compile out the special-folder lookup and migration call entirely. A 2026
binary retains its migration behavior when run later; this is a release cutoff,
not a device-clock cutoff. CI builds both sides of the boundary and checks the
compiled application with `tools/Test-AudioStorageBuild.ps1`.

Initialization in 2026 builds checks only `MyMusic/Open Sleep Music` when the
platform supplies an absolute Music path. It does not request broader access when
that location is unavailable. It examines the old app-owned world's immediate
audio files, including backgrounds and former collections; it never scans the
user's general music library. Symbolic links/reparse points, unknown files and
unfinished downloads are left alone.

Each eligible source must pass the normal audio-signature validator. Migration
copies into a unique `.part` file, checks the complete copy's SHA-256 against the
source, installs the destination without overwriting an existing file and only
then deletes the source. An identical destination allows interrupted source
cleanup to resume. Different existing bytes are retained at both locations.
Invalid, inaccessible or locked files stay in place; other files continue and
the next startup can retry. Diagnostics go to the debug output. The app does not
reimport a collection that the user subsequently deleted: 2026 builds persist a
per-world deletion marker in local preferences and skip that world's legacy
directory on subsequent migration attempts. These markers retain the existing
backup eligibility of preferences. The app does not
play from the legacy location as a fallback. An installation that skips all
migration-capable releases must download its music again after updating to 2027+.
Old directories and unrelated files are never recursively deleted.

## Backup policy

- Android retains `allowBackup="true"` for existing eligible settings. The manifest
  references `backup_rules.xml` for Android 11 and older and
  `data_extraction_rules.xml` for Android 12+. Both exclude `file:Audio/`, including
  all descendant files and partial downloads; modern rules cover both cloud
  backup and device-to-device transfer. No cross-platform transfer is configured.
- iOS and Mac Catalyst mark the created Audio directory with
  `NSFileManager.SetSkipBackupAttribute`, using Apple's backup-exclusion resource
  property. Failure to set the exclusion prevents storage initialization rather
  than silently claiming successful exclusion.
- Packaged Windows uses the persistent `LocalCacheFolder` exclusion described
  above. Unpackaged Windows uses local AppData, outside the shared Music folder.
  User-selected full-disk images and third-party backup software can still copy
  files; the app cannot prohibit such backups.

App-owned audio is removable through collection deletion. Android app-specific
files are also removed when uninstalling or clearing app data. Unmigrated legacy
files and reports previously shared through other apps may remain. Settings and
other app state retain their existing platform backup eligibility.

Validate the final merged Android manifest and XML resources, restore settings
with Audio absent, and exercise device-to-device transfer on supported Android
versions before Store publication. Verify the Apple directory resource property
on a device before publishing an Apple build. Unit tests and compilation do not
establish that a real backup or restore has been exercised.

References: [Android backup rules](https://developer.android.com/identity/data/autobackup),
[MAUI file-system locations](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/file-system-helpers),
[Windows app-data lifetimes](https://learn.microsoft.com/en-us/uwp/api/windows.storage.applicationdata),
[Apple backup exclusion](https://developer.apple.com/documentation/foundation/urlresourcevalues/isexcludedfrombackup).
