# Open Sleep Music recordings

Selected project recordings, editable MIDI files and their source/license evidence
live here, one stable directory per work. These media files have the licenses
declared in their song directories; the application's MIT license does not
relicense the music or instrument samples.

| Work | Published versions | License |
| --- | --- | --- |
| [Friedrich Burgmüller — Berceuse op. 109 No. 7](burgmuller-berceuse-op109-no7/README.md) | YDP piano; YDP piano with corrected chamber strings; each at original and sleep tempo | CC BY-SA 3.0 adaptations; YDP samples CC BY 3.0; VSCO strings CC0 1.0 |
| [Gabriel Fauré — Berceuse op. 56 No. 1](faure-berceuse-op56-no1/README.md) | YDP piano; YDP piano with new restrained string doublings; each at original and sleep tempo; string listening feedback pending | CC BY 3.0 adaptations and YDP samples; VSCO strings CC0 1.0 |

Each directory contains `song.json`, `audio/`, `midi/`, `sources/` and `licenses/`.
Only the selected finished versions belong here. Source recordings, instrument
banks, transcription models and temporary audition files are not committed.
See [the contributor guidance](../AGENTS.md#project-music-publications).

Run `./tools/Test-MusicPublications.ps1` to verify the publication file set,
manifests, media signatures and exact checksums before committing or uploading.

## Downloads

Download MP3s directly from **Raw GitHub**, using the commit-pinned `downloadUri`
in each `song.json`. Music is added incrementally to these directories, without
music releases or release ZIPs. Pin the existing commit containing each song's
audio; adding another song does not change older URLs. Do not overwrite selected
audio/MIDI files later: use new versioned paths if a revision is ever needed.
Catalog admission and changing an existing app recording remain separate steps.

GitHub supports [downloading raw file contents](https://docs.github.com/en/repositories/working-with-files/using-files/viewing-and-understanding-files#viewing-or-copying-the-raw-file-content).
Its general
[excessive-bandwidth policy](https://docs.github.com/en/site-policy/acceptable-use-policies/github-acceptable-use-policies#9-excessive-bandwidth-use)
permits throttling and other restrictions. Raw delivery has no blanket unlimited
bandwidth guarantee. The [repository-size guidance](https://docs.github.com/en/repositories/working-with-files/managing-large-files/about-large-files-on-github)
recommends staying ideally below 1 GB and strongly below 5 GB; ordinary Git files
over 100 MiB are blocked. The two current songs total approximately 43 MB. Watch
repository growth as hundreds of works are added, and review storage/distribution
if it becomes material. Checked 2026-10-23.
