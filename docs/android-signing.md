# Android release signing

## Distribution channels

On 31 October 2026 the maintainer selected Google Play for Android Preview
testing (internal/closed tracks) as well as Stable distribution, replacing the
previous GitHub Preview channel. The GitHub release workflow no longer publishes
Android APKs. Historical assets are retained. Google Play AAB publishing remains
a separate preparation task; a Play track has not been activated by this change.
Local developer builds remain possible, with the existing development key
preserved for compatible local updates. Windows packaging and GitHub delivery of
audio, artwork and catalogs continue independently.

A Google-managed Play app-signing key is acceptable. There is no requirement
for a Play installation to update an existing GitHub development APK in place.
Preserve the existing development key for future developer APKs, and keep the
Play upload key distinct from its app-signing key. Incompatible signatures with
the same application ID require uninstalling the existing app before switching
channels unless an explicitly supported signing migration is arranged.

## Existing development APK signing

Android accepts an APK as an update only when its application ID and signing certificate match the installed app and its version code is newer. Preview releases through `v0.1.0-preview.12` were unintentionally signed with a fresh runner-local debug key, so they cannot update one another.

Starting with Preview 13, the release workflow restores one durable PKCS#12 signing key from protected GitHub Actions secrets. The key, alias, and passwords must never be committed or printed in logs. A local backup is kept under the ignored `.local-secrets` directory and must be copied to a secure backup location by the repository owner. Losing both that backup and the GitHub secret makes future in-place updates impossible.

Required repository secrets:

- `ANDROID_KEYSTORE_BASE64`
- `ANDROID_KEYSTORE_PASSWORD`
- `ANDROID_KEY_ALIAS`
- `ANDROID_KEY_PASSWORD`

Current release certificate SHA-256 fingerprint:

`65:19:8F:04:41:33:EE:C5:94:7A:D7:A7:49:FF:B3:05:E5:4D:7F:93:6D:65:71:79:46:AB:A1:27:F9:07:53:CC`

The first installation of Preview 13 requires uninstalling any older preview because its certificate necessarily differs. This removes the old app's private preferences and app-private downloads; copy any wanted feedback exports first. Once Preview 13 is installed, later development APKs signed with the durable key can be installed as normal updates without uninstalling. This does not promise update compatibility with a separately signed Google Play build.
