# Gabriel FaurÃ© â€” Berceuse op. 56 No. 1

These project recordings adapt **Brian M. Jones's solo piano arrangement and
September 2010 performance** of the Berceuse from the Dolly Suite. They use the
preferred warm **YDP Grand Piano variant B**, with a separate restrained string
variation requested by the maintainer. The new string variation awaits listening
feedback; publication does not replace the existing app recording.

| Version | MP3 | Editable MIDI | Duration |
| --- | --- | --- | --- |
| Piano, original timing | [MP3](https://raw.githubusercontent.com/jochenwezel/open-sleep-music/92af08a8a3851ec2894e94566bd38d34a6070f1e/music/faure-berceuse-op56-no1/audio/faure-berceuse-op56-no1-piano-original.mp3) | [MIDI](midi/faure-berceuse-op56-no1-piano-original.mid) | 3:34 |
| Piano, sleep tempo | [MP3](https://raw.githubusercontent.com/jochenwezel/open-sleep-music/92af08a8a3851ec2894e94566bd38d34a6070f1e/music/faure-berceuse-op56-no1/audio/faure-berceuse-op56-no1-piano-sleep.mp3) | [MIDI](midi/faure-berceuse-op56-no1-piano-sleep.mid) | 4:16 |
| Piano and strings, original timing | [MP3](https://raw.githubusercontent.com/jochenwezel/open-sleep-music/92af08a8a3851ec2894e94566bd38d34a6070f1e/music/faure-berceuse-op56-no1/audio/faure-berceuse-op56-no1-piano-strings-original.mp3) | [MIDI](midi/faure-berceuse-op56-no1-piano-strings-original.mid) | 3:34 |
| Piano and strings, sleep tempo | [MP3](https://raw.githubusercontent.com/jochenwezel/open-sleep-music/92af08a8a3851ec2894e94566bd38d34a6070f1e/music/faure-berceuse-op56-no1/audio/faure-berceuse-op56-no1-piano-strings-sleep.mp3) | [MIDI](midi/faure-berceuse-op56-no1-piano-strings-sleep.mid) | 4:16 |

Sleep timing uses event speed **0.833333**. Original timing follows the source
performance. MIDI event timing is changed before synthesis; the MP3s are not
time-stretched. The prepared audio samples are retained exactly, with publication
ID3 credits updated. All four exports were fully decoded, matched near -23.21 LUFS,
checked for peak headroom and a quiet natural ending, and exported without
dynamics compression or limiting. Original tempo has no separate listening feedback.

## Attribution and license

- Composition: **Gabriel FaurÃ©**, Berceuse, Dolly Suite op. 56 No. 1.
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
accompaniment, not a historical FaurÃ© orchestration or a score-verified edition.

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
