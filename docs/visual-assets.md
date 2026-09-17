# Visual assets

The immersive player separates collection themes from track motifs. `PlaybackVisualCatalog` selects two dark collection colors and a transparent motif asset. This keeps future catalog-specific artwork data-driven and avoids duplicating player layouts.

`sleepy_bear_moon.png` and `sleepy_lamb_star.png` were generated for this project with OpenAI image generation on 2026-09-09. The collection-specific assets and the later lullaby motifs were generated for this project with OpenAI image generation in September 2026. They contain no third-party source artwork or embedded text and are distributed under the repository's MIT license.

The versioned source PNGs are published in the [artwork-v1 GitHub release](https://github.com/jochenwezel/open-sleep-music/releases/tag/artwork-v1). Every catalog track carries an HTTPS download URL, portable cache filename, and SHA-256 checksum for its artwork. The app downloads and validates artwork independently from audio and falls back silently when an image is unavailable or invalid. Only `sleepy_lamb_star.png` remains packaged as the guaranteed offline fallback; concrete collection and song artwork is not bundled into the APK.

The starry quiet-classics scene transitions between muted blue and dark blue over 15 seconds per color phase. The fireplace motif uses a subtle three-second opacity crossfade; no image geometry moves. Animation callbacks run at 20 frames per second to limit battery and rendering load, and all ambient animation is disabled when **Reduce motion** is enabled. Motifs use a strong silhouette, no flashes, and no rapid movement. New artwork should remain readable at phone size in portrait and landscape, use transparent backgrounds, avoid text, and document its source and license here.
