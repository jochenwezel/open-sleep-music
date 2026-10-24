# Chopin mazurka auditions — 24 October 2026

The maintainer requests new versions of the slowed mazurkas in Quiet Classics,
using the successful Burgmuller preparation as the sound reference. The catalog
contains 57 mazurkas. This initial comparison covers three shorter works while
the scope of the remaining recordings is being clarified.

| Work | Complete audition duration | Source pianist |
| --- | --- | --- |
| Mazurka in A minor, op. 68 no. 2 | 3:30 | Olga Gurevich / Musopen |
| Mazurka in G minor, op. 67 no. 2 | 2:42 | Olga Gurevich / Musopen |
| Mazurka in F minor, op. 63 no. 2 | 2:25 | Olga Gurevich / Musopen |

Each work has an **A: YDP piano solo** and **B: YDP piano with quiet chamber
strings** MP3 at the current catalog event-speed factor of `0.666667`. Preserve
each performance's estimated expressive timing, velocities, overlapping notes and
pedal; do not substitute a quantized score MIDI. Only render velocities are
softened to 80%. Retain the original estimated velocities separately in the MIDI.
The YDP B piano, warm EQ, verified room reverb, stereo 44.1 kHz / 256 kbps MP3,
constant-gain matching near −23.21 LUFS, and quiet-ending trim follow the
[existing preparation preference](../tools/berceuse-rendering-preferences.json).
There is no audio time stretching, chorus, compressor or limiter in these renders.

The new optional violin/viola/cello accompaniment doubles selected actual
transcribed piano notes, with octave shifts only. It does not infer new triads.
Each complete wet/dry string release ends within its source piano key interval;
holds are capped at 1.6 original seconds. Strings measure 11 dB below the piano
RMS. These are new audition arrangements, not historical Chopin orchestrations or
approved adaptations of the Burgmuller C2 arrangement. Automatic transcription
can still contain wrong pitches, missing notes or timing errors. Neither full
listening approval nor musical proofreading is claimed.

The Nextcloud audition directory is
`Open Sleep Music/Probehören/2026-10-24-Chopin-Mazurken-YDP`. It contains the six
complete MP3s, original/sleep solo MIDI and sleep piano/string MIDI for each work,
credits, license text, source evidence and validation reports. The `00-Ueberblick`
MP3 gives 20 seconds solo followed by the same 20 seconds with strings for each
work, in the table's order. Its excerpt fades and pauses are intentional; it is
not a full-work recording. Preparation scripts, source audio, model outputs,
sound banks and working WAVs remain in ignored local artifacts. No new audio is
committed or bundled, and no catalog membership, download or playback setting is
replaced by this audition task.

## Recording rights and provenance

The existing [Archive mirror](https://archive.org/details/musopen-chopin-complete-works-flac)
declares CC0 for the collection. Exact source MP3 sizes and SHA-1 hashes match its
upstream file metadata. However, their embedded performer credit is `OIga
Gurevich`, rather than merely “Various artists”. The official
[Musopen performer page](https://musopen.org/music/performer/olga-gurevich/instrument/piano/)
corroborates Olga Gurevich. IMSLP identifies matching Musopen recordings by her,
with matching work and duration, under **CC BY 4.0**:

- [Op. 68, recording 479617](https://imslp.org/wiki/Mazurkas,_Op.68_(Chopin,_Fr%C3%A9d%C3%A9ric)):
  no. 2, 2:22.
- [Op. 67, recording 478375](https://imslp.org/wiki/Mazurkas,_Op.67_(Chopin,_Fr%C3%A9d%C3%A9ric)):
  no. 2, 1:51.
- [Op. 63, recording 478372](https://imslp.org/wiki/Mazurkas,_Op.63_(Chopin,_Fr%C3%A9d%C3%A9ric)):
  no. 2, 1:38.

Conservatively retain those recording-specific attribution terms for these private
auditions. Credit Olga Gurevich and Musopen, identify the transcription and new
arrangement, and release the project contributions under CC BY 4.0. Preserve the
mirror's different declaration as evidence rather than claiming the uploader can
waive the performer's rights. This is identity corroboration, not a claim of
byte identity between different portal transcodes. **Reconcile this discrepancy
before any production promotion or public music publication.** The existing
catalog's CC0 metadata has not been silently changed or treated as independently
guaranteed by this research.

The [YDP samples](https://freepats.zenvoid.org/Piano/acoustic-grand-piano.html)
retain CC BY 3.0 and Zenph Studios, Dr. Mikhail Krishtal and team, OLPC / Dr. Richard
Boulanger, and Roberto / FreePats credits. The [VSCO 2 CE strings](https://github.com/sgossner/VSCO-2-CE)
retain CC0 and Sam Gossner, Simon Dalzell and Elan Hickler credits. These sample
terms remain separate from the project adaptation and application MIT license.

## Verification

All six complete MP3s and the overview decode fully. MIDI checks cover note
pairing, final sustain release and tempo ratios; accompaniment checks cover all
source-note intervals. Actual wet/dry differences verify the piano reverb. The
complete MP3s measure approximately −23.21 LUFS with true peaks below −5 dBTP;
their quiet endings retain the musical decay. SHA-256 hashes match the copied
Nextcloud files. This verifies local delivery into the synchronized folder, not
an independent authenticated server download or a musical listening approval.
The compact [audition manifest](../tools/mazurka-auditions-2026-10-24.json) retains
the exact delivered-file hashes, settings and rights-review status.
