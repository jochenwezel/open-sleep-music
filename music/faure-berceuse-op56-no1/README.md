# Gabriel Fauré — Berceuse op. 56 No. 1

**Piano transcription review, 2026-10-23:** the maintainer reports wrong notes
and attacks, including in the opening. Version 1 requires musical correction;
the YDP instrument preference does not approve its automatically detected notes.
Private solo correction auditions are being reviewed. The existing MIDI, MP3
and derived working-score bytes are preserved, and the app retains the source
recording. [Review findings](sources/piano-review-2026-10-23.json).

A5 offers two short, matched-tempo/source-A4-A5 comparisons with 19 proposed
octave/repeat artifact removals. Every surviving performance event, velocity,
key-off and pedal control is unchanged from A4. These private excerpts await
listening review before any whole-piece preparation or publication replacement.

The maintainer still finds A4 unconvincing and, on 2026-10-24, rejected both
score-based auditions as mechanical and unattractive.
[Reference research and feedback](sources/score-basis-research-2026-10-23.json)
retain the historical Hamelle score, CC0 community four-hands edition via PDMX,
and public-domain Mutopia flute/guitar adaptation as notation references only.
Use them to suggest specific melody/harmony checks against the Jones recording,
while retaining the performance's timing, dynamics, overlaps and sustain.
Arrangement or broad pitch-class differences alone do not prove transcription
errors. The YDP B preference remains; no new rendition or replacement is approved.

These project recordings adapt **Brian M. Jones's solo piano arrangement and
September 2010 performance** of the Berceuse from the Dolly Suite. They use the
preferred warm **YDP Grand Piano variant B**, with a separate restrained string
variation requested by the maintainer. The new string variation awaits listening
feedback; publication does not replace the existing app recording.

- **Piano - original tempo** (3:34): [MP3](https://raw.githubusercontent.com/jochenwezel/open-sleep-music/92af08a8a3851ec2894e94566bd38d34a6070f1e/music/faure-berceuse-op56-no1/audio/faure-berceuse-op56-no1-piano-original.mp3) / [MIDI](midi/faure-berceuse-op56-no1-piano-original.mid).

- **Piano - sleep tempo** (4:16): [MP3](https://raw.githubusercontent.com/jochenwezel/open-sleep-music/92af08a8a3851ec2894e94566bd38d34a6070f1e/music/faure-berceuse-op56-no1/audio/faure-berceuse-op56-no1-piano-sleep.mp3) / [MIDI](midi/faure-berceuse-op56-no1-piano-sleep.mid).

- **Piano with strings - original tempo** (3:34): [MP3](https://raw.githubusercontent.com/jochenwezel/open-sleep-music/92af08a8a3851ec2894e94566bd38d34a6070f1e/music/faure-berceuse-op56-no1/audio/faure-berceuse-op56-no1-piano-strings-original.mp3) / [MIDI](midi/faure-berceuse-op56-no1-piano-strings-original.mid).

- **Piano with strings - sleep tempo** (4:16): [MP3](https://raw.githubusercontent.com/jochenwezel/open-sleep-music/92af08a8a3851ec2894e94566bd38d34a6070f1e/music/faure-berceuse-op56-no1/audio/faure-berceuse-op56-no1-piano-strings-sleep.mp3) / [MIDI](midi/faure-berceuse-op56-no1-piano-strings-sleep.mid).

Sleep timing uses event speed **0.833333**. Original timing follows the source
performance. MIDI event timing is changed before synthesis; the MP3s are not
time-stretched. The prepared audio samples are retained exactly, with publication
ID3 credits updated. All four exports were fully decoded, matched near -23.21 LUFS,
checked for peak headroom and a quiet natural ending, and exported without
dynamics compression or limiting. Original tempo has no separate listening feedback.

## Attribution and license

- Composition: **Gabriel Fauré**, Berceuse, Dolly Suite op. 56 No. 1.
- Source solo arrangement and recording: **Brian M. Jones**, September 2010.
  The [specific Commons file page](https://commons.wikimedia.org/wiki/File:Berceuse_by_Gabriel_Faur%C3%A9_op56_no1.ogg)
  declares **CC BY 3.0** for the performance and identifies its YouTube origin.
  This is a different source from the NC-licensed four-hand Dolly performance.
- MIDI/audio adaptation and new string doubling: **Open Sleep Music contributors**,
  2026, **CC BY 3.0 Unported**. Retain the credits, source and license links and
  modification notice; no endorsement by the source performer is implied.
  [Full license text](licenses/CC-BY-3.0.txt).
- Piano: **YDP Grand Piano / Yamaha Disklavier Pro**, Zenph Studios,
  Dr. Mikhail Krishtal and team; OLPC sample library / Dr. Richard Boulanger;
  soundfont and sample modifications **Roberto (roberto@zenvoid.org), FreePats**,
  **CC BY 3.0**. [Source and grant](https://freepats.zenvoid.org/Piano/acoustic-grand-piano.html),
  [original credits](licenses/YDP-Grand-Piano-original-credits.txt).
- Strings: **VSCO 2 Community Edition**, Versilian Studios/Sam Gossner,
  Ivy Audio/Simon Dalzell, sample cutting Elan Hickler/Soundemote, **CC0 1.0**.
  [Source](https://github.com/sgossner/VSCO-2-CE),
  [original credits](licenses/VSCO-original-credits.txt),
  [CC0 text](licenses/VSCO-CC0-1.0.txt).

These media licenses are independent of the application's MIT license.
[source-evidence.json](licenses/source-evidence.json) records the recording grant,
original upstream checksum and instrument provenance. Source-declared grants and
provenance checks are documented; no independent copyright guarantee is claimed.

Changes: automatic audio-to-MIDI transcription; estimated note timings, velocities,
overlaps and binary sustain; render-only velocity softening; selected MIDI tempo;
YDP synthesis; optional new string doubling; EQ, generated room acoustics, constant
loudness gain, removal of an inaudible tail, MP3 encoding and ID3 credits.

## Arrangement and preparation inputs

The new quiet cello, viola and violin voices double selected notes actually held
in the piano transcription, with only octave shifts. No triads or extra chord
tones are guessed. Weighted non-overlapping intervals select notes by register;
long source holds are capped at 2.2 original seconds. All 392 complete wet/dry
string-note releases stay inside the corresponding source piano key intervals.
The reference string RMS is 11 dB below the piano. This is a newly prepared
accompaniment, not a historical Fauré orchestration or a score-verified edition.

[piano-transcription.json](sources/piano-transcription.json) preserves estimated
original note velocities and pedal events. [The string arrangement](sources/string-arrangement-original-timing.json)
preserves original-performance timing and source-note references; divide times and
envelope durations by 0.833333 for sleep tempo. [Render settings](sources/render-settings.json)
freeze the YDP effects and string preparation. [Sample provenance](sources/string-sample-provenance.json)
records maps, tuning and hashes; obtain instrument banks separately outside Git.

The four MIDIs already contain render velocity factor 0.8 and piano CC91 reverb
send; do not soften them twice. Their 120 BPM grid preserves performance timing,
not reliable score beats. Sustain is binary; half-pedal is not reliably recovered.
Automatic note/velocity/pedal estimation can contain mistakes. General MIDI
players use different string sounds and envelopes and will differ from these
MP3s. FluidSynth 2.6.1 rendered the YDP piano; mapped VSCO samples were synthesized
and release-gated separately before mixing.

## Direct downloads

The MP3 links above use commit-pinned Raw GitHub downloads. `song.json` retains the
audio commit, direct HTTPS URLs, exact hashes, durations and measured loudness.
No music release or release ZIP is needed. `checksums.sha256` covers every song
file except itself. This publication is independent of app catalog promotion.

## Printable working scores

[Score notes and editable source](scores/README.md). These are rhythmically
quantized MIDI working editions requiring musical proofreading, with approximate
tempo references for both audio speeds.

- [Piano solo](scores/faure-berceuse-op56-no1-piano-v1.pdf) - 4 page(s).
- [Piano with strings - full score](scores/faure-berceuse-op56-no1-piano-strings-v1.pdf) - 7 page(s).
- [Violin part](scores/faure-berceuse-op56-no1-violin-v1.pdf) - 2 page(s).
- [Viola part](scores/faure-berceuse-op56-no1-viola-v1.pdf) - 2 page(s).
- [Cello part](scores/faure-berceuse-op56-no1-cello-v1.pdf) - 2 page(s).
