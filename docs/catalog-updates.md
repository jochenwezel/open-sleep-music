# Runtime catalog updates

Open Sleep Music always packages an embedded media catalog so first launch and offline use do not depend on a server. On startup, the app also checks the mutable `artwork-catalog` GitHub release for a newer catalog:

- stable builds use `media-catalog.json`;
- preview builds use `media-catalog.preview.json`.

A fully validated remote catalog replaces the embedded collection and track lists without an application update. **Refresh library** performs another check. The last valid download is cached atomically in a build-channel-specific directory. HTTP errors, timeouts, oversized files, malformed JSON, unsupported schemas, insecure URLs, duplicate IDs or filenames, invalid metadata, and incomplete downloads silently fall back to the last valid cache or embedded catalog.

Stable builds reject any remote catalog containing the preview-only Preselection collection. Tracks with unchecked recording rights are accepted only inside that collection and only by Preview builds.

Preview catalogs also contain the `pre-qualify` collection with `externalReferences`: source-page audition links, explicit recording-rights review status, creator credits and instrumentation. These references open externally rather than entering the audio-download pipeline. The installed Preview app renders them directly from the updated catalog. Stable builds reject this collection and all external references. Existing reference preferences retain their `preselection` storage keys for continuity.

`.github/workflows/publish-media-catalog.yml` validates and publishes both channel files whenever their repository sources change. The mutable release contains only metadata; audio and artwork continue to use their separately validated download URLs and caches.

Tracks may specify an optional integer `startOffsetMilliseconds` (default `0`) to skip a prelude during playback. It is a position in the original file, measured in milliseconds before applying playback speed, and must be nonnegative and strictly less than the original duration in milliseconds. `durationSeconds` and any upstream checksum still describe the unchanged downloaded file. Listening duration and displayed positions exclude the skipped prelude; seeking to player position zero and repeating a track return to the configured offset. Resumed positions are stored on this listening timeline. Candidate metadata uses the same field and the generator propagates it into Preview audio tracks. Original media downloads, validation and caching remain unchanged.

Support for this new playback field requires an app update once. Older apps ignore it and play from the original start; subsequent offset adjustments can arrive through ordinary runtime catalog updates.
