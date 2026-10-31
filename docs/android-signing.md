# Android release signing

## Distribution channels

On 31 October 2026 the maintainer selected Google Play for Android Preview
testing (internal/closed tracks) as well as Stable distribution, replacing the
previous GitHub Preview channel. The GitHub release workflow no longer publishes
Android APKs. Historical assets are retained. Google Play AAB publishing is
handled separately from GitHub release automation.
Local developer builds remain possible, with the existing development key
preserved for compatible local updates. Windows packaging and GitHub delivery of
audio, artwork and catalogs continue independently. The first Google Play
internal test, version 0.1.43, was activated on 1 November 2026.

A Google-managed Play app-signing key is acceptable. There is no requirement
for a Play installation to update an existing GitHub development APK in place.
Preserve the existing development key for future developer APKs, and keep the
Play upload key distinct from its app-signing key. Incompatible signatures with
the same application ID require uninstalling the existing app before switching
channels unless an explicitly supported signing migration is arranged.

## Google Play upload signing

The first internal AAB uses a separate RSA 3072-bit upload key. Its public
certificate SHA-256 is:

`A0:EB:E7:BB:4F:A8:BC:B4:9B:5E:D1:33:C0:4A:2E:C7:20:C0:2F:DD:EE:5B:89:60:10:39:CF:EE:EE:14:A0:8A`

The Play Console app-signing page's Digital Asset Links snippet identifies this
installed-app certificate SHA-256, distinct from both upload and development:

`8E:0E:C2:97:6F:F9:93:04:92:AB:59:22:C5:C8:59:09:33:86:04:8C:86:D9:84:96:E0:BE:3B:CA:C7:A5:F4:97`

Consequently, the existing development-signed package cannot be updated in place
with the Play package. Switching an existing development device requires an
explicitly chosen uninstall/reinstall, which removes app-private downloads and
settings. Do not uninstall automatically as part of Play publication.

The upload keystore and Windows-user-encrypted credentials are stored in ignored
`.local-secrets`, separately from the preserved development key. No upload
credentials have been added to GitHub Actions. Keep a secure recoverable backup
of the keystore and credentials: a Windows DPAPI blob alone is not a portable
backup. Future automation must provision protected signing credentials explicitly.
Google signs installed APKs with its separate app-signing key; an AAB upload
certificate is not the certificate used to update an installed Play app.

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
