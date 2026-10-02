# Runtime catalog updates

Open Sleep Music always packages an embedded media catalog so first launch and offline use do not depend on a server. On startup, the app also checks the mutable `artwork-catalog` GitHub release for a newer catalog:

- stable builds use `media-catalog.json`;
- preview builds use `media-catalog.preview.json`.

A fully validated remote catalog replaces the embedded collection and track lists without an application update. **Refresh library** performs another check. The last valid download is cached atomically in a build-channel-specific directory. HTTP errors, timeouts, oversized files, malformed JSON, unsupported schemas, insecure URLs, duplicate IDs or filenames, invalid metadata, and incomplete downloads silently fall back to the last valid cache or embedded catalog.

Stable builds reject any remote catalog containing the preview-only Preselection collection. Tracks with unchecked recording rights are accepted only inside that collection and only by Preview builds.

`.github/workflows/publish-media-catalog.yml` validates and publishes both channel files whenever their repository sources change. The mutable release contains only metadata; audio and artwork continue to use their separately validated download URLs and caches.
