# Remaining Chopin mazurka auditions — 24 October 2026

Following positive feedback on the [initial three-work comparison](mazurka-auditions-2026-10-24.md),
the maintainer requests the same treatment for the other catalog mazurkas.
The additional batch covers 54 works: 108 complete MP3 auditions, 162 editable
MIDI files and six comparison overviews. Together, the two batches cover all 57
mazurkas currently in Quiet Classics. These are private listening candidates;
catalog recordings, playback settings and published project music are unchanged.

## Listening files

The synchronized Nextcloud directory is
`Open Sleep Music/Probehören/2026-10-24-Chopin-Mazurken-YDP/Weitere-Mazurken`.
Start with `01-UEBERSICHT.md`, which links both full versions of every work:

- **A:** YDP variant B piano solo, at the existing sleep event speed `0.666667`.
- **B:** the same piano with quiet violin/viola/cello accompaniment, at that speed.

The six `00-Ueberblick` MP3s cover numbers 04–12, 13–21, 22–30, 31–39,
40–48 and 49–57. Each presents 12 seconds solo, a 0.7-second pause and the same
12 seconds with strings, followed by a 1.3-second pause. Excerpts use deliberate
short fades. Section times and full-work links are retained in `batch-report.json`.
Original-performance solo MIDI, sleep solo MIDI and sleep piano/string MIDI
accompany each work; original-tempo MP3s are not part of this audition request.

## Preparation and its limits

The Kong high-resolution note/pedal model estimates expressive timing, velocities,
overlaps and sustain from the existing source performance. Events remain
unquantized. Tempo changes happen before synthesis. The MIDI retains estimated
velocities; only render velocities are softened by a factor of 0.8.
The [preferred sound settings](../tools/berceuse-rendering-preferences.json)
provide YDP B piano, warm EQ, verified room reverb with CC91=90, no chorus,
44.1 kHz stereo and 256 kbps MP3. Static gain targets −23.21 LUFS, with a quieter
shared A/B target where peak headroom requires it; no compressor, limiter or
encoded-audio time stretching is used.
Only the inaudible ending tail is trimmed, preserving the musical release.

The new optional strings double selected actual transcribed piano notes with
octave shifts only, rather than guessing new triads. Complete dry/wet releases
stay within the source key intervals; holds are capped at 1.6 original seconds.
The reference string RMS is 11 dB below the piano. These are new audition
arrangements, not historical Chopin orchestrations. Structural checks cannot
establish correct note recognition or musical suitability. Per-work listening
and arrangement review remain pending.

A separate CUDA runtime uses the host's RTX 4060, retaining the existing CPU
environment. A 36.5-second CPU/GPU comparison with the same checkpoint and
float32 processing, TF32 disabled, retains all 486 notes and integer velocities;
maximum onset/offset differences are below 4 microseconds. This validates that
case, not every transcription. The manifest records the device for each work,
the checkpoint hash, validation report and preparation-script hashes.

## Recording rights and credits

Every complete source MP3 matches its Archive file metadata size and SHA-1;
SHA-256 and embedded performer credits are retained separately. The Archive
collection's uploader-declared CC0 metadata is not treated as sufficient proof
of recording rights. Additional recording-specific Commons or IMSLP declarations
corroborate 41 of the 54 works, with the exact performer, declaration and evidence
URL retained in the manifest. Remaining questions must be resolved before
production admission or public project-music publication.

One explicit identity discrepancy concerns op. 17 no. 4: the Archive MP3 credits
Luke Faulkner while a corresponding Commons work listing credits Edward Neeman.
Another performer's grant must not be transferred to these bytes. The manifest
preserves this discrepancy rather than claiming an approved recording license.
Public-domain declarations remain distinct from CC0 and CC BY grants.

Project transcription/rendering/arrangement contributions are offered under
CC BY 4.0, separately from any source-performance rights. The YDP sample grant
is CC BY 3.0, crediting Zenph Studios / Dr. Mikhail Krishtal and team,
OLPC / Dr. Richard Boulanger, and Roberto / FreePats. VSCO 2 CE strings retain
CC0 1.0 with Sam Gossner, Simon Dalzell and Elan Hickler credits.
License texts, sample credits, source reports and per-work modifications
accompany the private listening files.

## Verification and follow-up

Full MP3 decoding, loudness/headroom, quiet endings, MIDI note pairing and final
sustain release, tempo ratios, actual reverb output and string release intervals
are checked during preparation. Full-work loudness measures −24.69 to −23.16
LUFS; all true peaks are below −2.14 dBTP. A/B loudness differences stay below
0.04 LU. Per-file SHA-256 verification covers all 554 files copied into
the synchronized Nextcloud folder; an independent authenticated download from
the Nextcloud server has not been verified. Initial comparison audio/MIDI bytes
remain unchanged. Working recordings, samples, model files and temporary renders
stay in ignored local artifacts; no new media is committed or bundled.

The [batch manifest](../tools/mazurka-auditions-remaining-2026-10-24.json)
identifies every work, performer, setting, measurement, delivered hash and
recording-review outcome. Continue listening outcomes in the central
[catalog quality register](catalog-quality-notes.md). Positive feedback on the
initial sound style does not approve every new transcription or accompaniment.
