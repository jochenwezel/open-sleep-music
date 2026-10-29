# Google Play visual assets

Prepared on 29 October 2026 for the draft English and German Play listings.
These files are project artwork and actual Android application screenshots,
not synthetic UI mockups. They do not constitute a release or store submission.

- `app-icon.png`: 512 x 512 RGBA PNG, exported from the existing app launcher
  artwork without changing its motif.
- `feature-graphic.png`: 1024 x 500 RGB PNG. Original artwork generated with the
  built-in OpenAI image-generation tool, using the launcher icon only as a style
  and palette reference. The title is identical in both listing languages.
- `en-US/` and `de-DE/`: 1080 x 1920 RGB PNG screenshots from the local Android
  emulator running application version 0.1.38. The packaged Core resource was
  checked to exclude Preview audition collections. The maintainer prepared two
  fully downloaded collections for the screenshots. Audio is not packaged here.

Screenshots are direct device captures; exporting to RGB removes the unused
alpha channel without retouching, inventing or rearranging the interface. The
emulator's temporary 9:16 display override is not part of the application and
the black borders in its host window are absent from the captured PNGs.
Actual track names, counts and current visuals are snapshots of the captured
app state. Review screenshots against the final delivered Play build before
publication. No tablet, desktop or XR support is claimed by reusing phone images.

All project-created artwork remains under the repository's MIT licence.
Individual recording rights remain independent; see the catalog attribution.
`assets.json` records provenance, the exact generation prompt and final hashes.
