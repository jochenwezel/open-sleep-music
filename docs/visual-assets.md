# Visual assets

Runtime images, motifs and artwork manifests use only project GitHub delivery.
Keep versioned payload URLs and checksums, including collection-specific layers;
do not add upstream-image fallbacks. See [GitHub content delivery](github-content-delivery.md).

The immersive player renders transparent ambient overlays and transparent song motifs as separate layers above the animated collection color. Every collection also owns a distinct transparent fallback motif that is used only when a track-specific motif is absent or unavailable. Ambient overlays may contain only sparse abstract particles such as star points, rain dashes, bubbles, fireflies, or embers; they must not contain a scene, landscape, room, animal, instrument, or other principal motif. Background overlays, fallback motifs, and track motifs must be different files; neither collection-level image may be assigned as a track motif. `PlaybackVisualCatalog` owns the animated collection colors and packaged emergency fallback. The catalog and remotely updateable artwork manifest provide independently validated PNG references for all three remote layers.

`sleepy_bear_moon.png` and `sleepy_lamb_star.png` were generated for this project with OpenAI image generation on 2026-09-09. The collection-specific assets and the later lullaby motifs were generated for this project with OpenAI image generation in September 2026. They contain no third-party source artwork or embedded text and are distributed under the repository's MIT license.

The first versioned PNG set is published in the [artwork-v1 GitHub release](https://github.com/jochenwezel/open-sleep-music/releases/tag/artwork-v1). Collection fallbacks and the title-specific forest series are published in `artwork-v2`; the transparent ambient overlays are published in `artwork-v3`. Every catalog track carries HTTPS download URLs, portable cache filenames, and SHA-256 checksums. The app downloads and validates artwork independently from audio and falls back silently from a missing title motif to its collection fallback. `sleepy_lamb_star.png` is the general packaged emergency image. Starting with Preview 0.1.40, the fireplace collection instead packages `fallback_fireplace_hearth_v1.png`, so its last offline fallback also shows a fire. Other concrete collection and song artwork remains remote.

The mutable [artwork-catalog release](https://github.com/jochenwezel/open-sleep-music/releases/tag/artwork-catalog) is the update channel used by installed apps. Its `artwork-catalog.json` can point existing track IDs to newer versioned background and optional song-motif releases and checksums without requiring an application update once the app supports the relevant metadata. The app caches the last valid manifest and falls back to the metadata embedded in its own media catalog when the channel is unavailable or invalid. A manifest entry must supply all three song-motif fields or none of them. Regenerate the manifest with `tools/Export-ArtworkManifest.ps1` after changing artwork assignments.

Shared tracks may carry `collectionArtwork`, a dictionary keyed by collection ID. Each value supplies a complete background layer, an optional title motif and an optional fallback motif; every present layer requires an HTTPS URI, portable PNG filename and SHA-256 checksum. The collection must be one of the track's memberships. The player's active collection and the collection download select these layers after resolving the remote manifest. Other collections use the canonical track's default layers. Audio IDs, recording metadata and downloads remain shared. Cached manifests retain these assignments offline; older manifests without collection metadata cannot overwrite embedded collection assignments. Older app versions ignore the new dictionary and use the default layers; using the distinct child motifs requires an app update with collection-artwork support.

The original seven toddler lullabies and eight original forest tracks have separately published song motifs. Forest motifs follow the recording context: tropical-rainforest titles use tropical vegetation and animals, never the temperate fox, conifers, or fly agarics from the earlier concept. Earlier collection entries without a reviewed title motif display their own collection fallback instead. Packaged emergency images remain available when no downloaded artwork layer is available, including the dedicated fire fallback.

The animated collection color transitions over 15 seconds per phase and remains visible through the transparent ambient overlay. Landscape ambient overlays are rotated by 90 degrees in portrait mode and always preserve their aspect ratio; this keeps circular particles round while covering the complete player surface. The fireplace overlay keeps its subtle three-second opacity crossfade. A visible song motif is centered in the free stage between the title and playback controls and drifts vertically by only six device-independent pixels over 12 seconds. It uses the full available stage width or height while preserving its aspect ratio. Animation callbacks run at 20 frames per second to limit battery and rendering load, and all ambient animation is stopped with the page and disabled when **Reduce motion** is enabled. Motifs use a strong silhouette, no flashes, and no rapid movement. New artwork should remain readable at phone size in portrait and landscape, use transparent backgrounds, avoid text, and document its source and license here.

For Lullabies for little ones, the maintainer requires every song motif to center on a small child, an animal (preferably young), a star or the moon. This focal subject dominates the composition; architecture, landscapes, instruments and flowers support it and must visibly relate to the song title or documented work context. A tiny qualifying subject within an otherwise dominant landscape does not meet this requirement. Stars and the moon may be gently personified. Use gentle, restful poses and the existing soft watercolor style. Setting details belong to the transparent song-motif layer, not the abstract ambient overlay. This rule also applies when proposing artwork for candidates intended for the little-ones collection; no new artwork has been generated or assigned by this policy update.

Every future recording addition to any collection must include a newly prepared, title-related motif, its documented provenance and license, and a validated published PNG assignment. A generic collection fallback is only an offline/unavailable-image safeguard, not completion of the artwork requirement. Shared recordings keep their audio identity and use collection-specific artwork where the visual languages differ. This rule is prospective; earlier admissions are not automatically claimed to have complete title-specific artwork.

Every fireplace/fire collection scene, including its collection fallback, must show an actual recognizable fire: a lit hearth, stove or campfire, for example. Warm colors, a basket animal or abstract sparks alone do not establish the theme. Use soft, quiet flames and a compact, transparent watercolor vignette. Keep the abstract ember overlay separate from the scene and fallback.

## Guitar lullaby motif options (2026-10-08)

Four title-related watercolor options were generated for this project using the built-in OpenAI image-generation tool. Each is a 1024 × 1536 RGBA PNG; alpha-channel inspection confirms actual transparency around the artwork. The child or young animal is the principal subject, with title-related details supporting it. No third-party input artwork, text or logos are included. These project-generated illustrations are distributed under the repository's MIT license. The original outputs were copied into the workspace without image editing; their SHA-256 values and track IDs are recorded in [assets.json](../design/song-motifs/2026-10-08/assets.json), and the exact prompt set is retained in [prompts.json](../design/song-motifs/2026-10-08/prompts.json).

| Recording | Option | Saved image |
| --- | --- | --- |
| Romanza española | A: a small child hugging a guitar on a Spanish balcony with roses | [Romanza A](../design/song-motifs/2026-10-08/motif_lullaby_romanza_espanola_a.png) |
| Recuerdos de la Alhambra | A: a sleeping lion cub with the Lion Courtyard's fountain and arches | [Alhambra A](../design/song-motifs/2026-10-08/motif_lullaby_alhambra_a.png) |
| Julia Florida | A: a small girl with a guitar in a blossoming garden | [Julia A](../design/song-motifs/2026-10-08/motif_lullaby_julia_florida_a.png) |
| Julia Florida | B: a sleeping duckling in a blossom-decorated boat, reflecting the Barcarola context | [Julia B](../design/song-motifs/2026-10-08/motif_lullaby_julia_florida_b.png) |

After reviewing the generated options on 2026-10-08, the maintainer selected A for Romanza, Alhambra and Julia Florida. The selected PNGs are published in [artwork-v4](https://github.com/jochenwezel/open-sleep-music/releases/tag/artwork-v4), assigned by the catalog generator and included in the mutable artwork manifest. They are assigned only to Lullabies for little ones through `collectionArtwork.lullabies`, alongside that collection's background and fallback layers. Quiet Classics retains its existing collection fallback until separate title motifs are selected. Original PNG bytes and checksums are preserved. No audio files or packaged image resources are changed.

Julia B (the duckling in a flower-decorated boat) is retained as an unassigned design option for a possible future song. The maintainer found it unsuitable for Julia Florida; it is not published or included in either runtime catalog or artwork manifest.

## Favorite promotions, 2026-10-30

The promoted little-ones recordings use the existing transparent collection
fallback (sleeping young rabbit holding a star) until separate title motifs are
selected. No collection fallback is assigned as a song motif. Shared Träumerei
and Lágrima receive explicit child-collection background/fallback layers while
retaining the adult collection's layers. Existing selected guitar motifs remain.
Jungle_Choco 01 reuses the existing tropical `forest_rainforest_2.png` motif;
no temperate forest motif is assigned to this rainforest recording. The existing
PNG bytes, project provenance and MIT artwork terms remain; no new image was
generated. The artwork manifest is regenerated for the promoted memberships.

## Complete little-ones motifs and visible fire, 2026-10-30

Six new title motifs complete all 13 current little-ones memberships: Little
Lullaby (a sleeping bunny in a cradle), Brahms' evening lullaby (a sleeping child
with flowers and the moon), El Noi de la Mare (an infant, cradle and guitar),
Twinkle Twinkle Little Star (a sleepy personified star), Träumerei (a child
dreaming on a crescent moon), and Lágrima (a young fawn, guitar and dew drop).
Träumerei and Lágrima use `collectionArtwork.lullabies` only; their Quiet Classics
presentation and shared audio remain unchanged.

Fire Favorite receives a kitten beside a visibly burning stone hearth. The
fireplace collection fallback is a separate lit wood stove; it is also packaged
as the last emergency image in Preview 0.1.40. The abstract ember overlay remains
unchanged. The previous basket-only fallback is retained in its historical asset
release but is no longer assigned by the current generator.

These eight illustrations were generated with the built-in OpenAI image tool
for this project, without third-party input artwork, and use the repository's
MIT artwork terms. The original output PNG bytes are preserved in
[`design/song-motifs/2026-10-30`](../design/song-motifs/2026-10-30), along with
[`prompts.json`](../design/song-motifs/2026-10-30/prompts.json) and
[`assets.json`](../design/song-motifs/2026-10-30/assets.json). The versioned
`artwork-v5` assets and their SHA-256 assignments are maintained in
[`tools/song-motif-artwork.json`](../tools/song-motif-artwork.json). Regenerate
both catalogs and the artwork manifest after changing assignments. Audio,
licenses, memberships, trim settings and persisted track order are unchanged.
