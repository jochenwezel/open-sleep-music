# Visual assets

The immersive player separates collection themes from track motifs. `PlaybackVisualCatalog` selects two dark collection colors and a transparent motif asset. This keeps future catalog-specific artwork data-driven and avoids duplicating player layouts.

`sleepy_bear_moon.png` and `sleepy_lamb_star.png` were generated for this project with OpenAI image generation on 2026-09-09. They contain no third-party source artwork or embedded text. The repository distributes them under the repository's MIT license. The source PNGs retain transparency; MAUI may resize them per target during packaging. A stable checksum-like parity of the track ID chooses a motif, so artwork does not jump between runs.

Animations use a 15-second color phase and are disabled when **Reduce motion** is enabled. Motifs use a strong silhouette, no flashes, and no rapid movement. New artwork should remain readable at phone size, use transparent backgrounds, avoid text, and document its source and license here.
