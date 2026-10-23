# Open Sleep Music recordings

Selected project recordings, editable MIDI files and their source/license evidence
live here, one stable directory per work. These media files have the licenses
declared in their song directories; the application's MIT license does not
relicense the music or instrument samples.

| Work | Published versions | License |
| --- | --- | --- |
| [Friedrich Burgmüller — Berceuse op. 109 No. 7](burgmuller-berceuse-op109-no7/README.md) | YDP piano; YDP piano with corrected chamber strings; each at original and sleep tempo | CC BY-SA 3.0 adaptations; YDP samples CC BY 3.0; VSCO strings CC0 1.0 |

Each directory contains `song.json`, `audio/`, `midi/`, `sources/` and `licenses/`.
Only the selected finished versions belong here. Source recordings, instrument
banks, transcription models and temporary audition files are not committed.
See [the contributor guidance](../AGENTS.md#project-music-publications).

Run `./tools/Test-MusicPublications.ps1` to verify the publication file set,
manifests, media signatures and exact checksums before committing or uploading.

## Downloads

The same selected MP3s and a complete per-song ZIP are distributed as individual
assets of [the versioned music release](https://github.com/jochenwezel/open-sleep-music/releases/tag/music-2026-10-23-v1).
Use the fixed asset URLs in each `song.json` for future app delivery. Catalog
admission and changing an existing app recording remain separate steps.

GitHub [recommends releases for distributing project binaries](https://docs.github.com/en/repositories/working-with-files/managing-large-files/about-large-files-on-github#distributing-large-binaries).
Its [release documentation](https://docs.github.com/en/repositories/releasing-projects-on-github/about-releases#storage-and-bandwidth-quotas)
currently permits up to 1,000 assets per release, each under 2 GiB, and states no
total release-size or bandwidth quota. The general
[excessive-bandwidth policy](https://docs.github.com/en/site-policy/acceptable-use-policies/github-acceptable-use-policies#9-excessive-bandwidth-use)
still permits throttling and other restrictions. This is suitable project-asset
distribution, not a promise of unlimited hosting capacity. Checked 2026-10-23.

Music releases use a `music-` tag, are not marked as the latest application
release, and are excluded from app-package publishing jobs. Assets should remain
available unchanged: later sound changes receive a new version and checksums.
