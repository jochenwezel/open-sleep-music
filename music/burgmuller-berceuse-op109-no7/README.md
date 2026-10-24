# Friedrich Burgmüller — Berceuse op. 109 No. 7

These four project recordings adapt BastienM's December 2007 piano performance.
The selected piano is **YDP Grand Piano variant B**. The string versions add the
maintainer-approved, full-piece corrected C2 violin, viola and cello accompaniment.
This is a new accompaniment, not a historical Burgmüller orchestration.

On 2026-10-24 the maintainer selects **piano with strings - sleep tempo** as the
replacement for the existing little-ones catalog recording. The catalog retains
its stable track ID, artwork and order, downloads the exact published MP3, and
uses normal playback speed/gain because timing and loudness are already rendered.
[Catalog admission and delivery checks](sources/catalog-admission-2026-10-24.json).

- **Piano - original tempo** (1:23): [MP3](https://raw.githubusercontent.com/jochenwezel/open-sleep-music/5edf1ebbff08dd82ab476cc4b7d8c23fcc30b967/music/burgmuller-berceuse-op109-no7/audio/burgmuller-berceuse-op109-no7-piano-original.mp3) / [MIDI](midi/burgmuller-berceuse-op109-no7-piano-original.mid).

- **Piano - sleep tempo** (1:39): [MP3](https://raw.githubusercontent.com/jochenwezel/open-sleep-music/5edf1ebbff08dd82ab476cc4b7d8c23fcc30b967/music/burgmuller-berceuse-op109-no7/audio/burgmuller-berceuse-op109-no7-piano-sleep.mp3) / [MIDI](midi/burgmuller-berceuse-op109-no7-piano-sleep.mid).

- **Piano with strings - original tempo** (1:23): [MP3](https://raw.githubusercontent.com/jochenwezel/open-sleep-music/5edf1ebbff08dd82ab476cc4b7d8c23fcc30b967/music/burgmuller-berceuse-op109-no7/audio/burgmuller-berceuse-op109-no7-piano-strings-original.mp3) / [MIDI](midi/burgmuller-berceuse-op109-no7-piano-strings-original.mid).

- **Piano with strings - sleep tempo** (1:39): [MP3](https://raw.githubusercontent.com/jochenwezel/open-sleep-music/5edf1ebbff08dd82ab476cc4b7d8c23fcc30b967/music/burgmuller-berceuse-op109-no7/audio/burgmuller-berceuse-op109-no7-piano-strings-sleep.mp3) / [MIDI](midi/burgmuller-berceuse-op109-no7-piano-strings-sleep.mid).

The sleep versions use event speed **0.833333**. Original tempo retains the source
performance's timing. MIDI timing is changed before synthesis; the encoded audio
has not been time-stretched. The approved slow piano and corrected string MP3
audio samples are retained exactly; only publication ID3 metadata is updated.
The original-tempo versions use the same arrangement and voice settings, but have
not received separate listening feedback. All four exports were fully decoded,
matched near -23.21 LUFS, checked for peak headroom and quiet endings. No dynamics
compression or limiting is used.

## Attribution and license

- Composition: Friedrich Burgmüller, Berceuse op. 109 No. 7.
- Source performance/recording: **BastienM**, December 2007, own work according to
  the [specific Wikimedia Commons file page](https://commons.wikimedia.org/wiki/File:Berceuse_Burgmuller.ogg).
- MIDI/audio adaptation and new string accompaniment: **Open Sleep Music contributors**, 2026.
- Published MP3s, MIDI adaptations and new accompaniment: **CC BY-SA 3.0 Unported**.
  Retain these credits, the source and license links, and the modification notice;
  redistribute adaptations under the same or a compatible license. No source
  performer's endorsement is implied. [License text](licenses/CC-BY-SA-3.0.txt).
- Piano samples: **YDP Grand Piano / Yamaha Disklavier Pro**, Zenph Studios,
  Dr. Mikhail Krishtal and team; OLPC sample library / Dr. Richard Boulanger;
  soundfont and sample modifications by **Roberto (roberto@zenvoid.org), FreePats**.
  [FreePats source and CC BY 3.0 grant](https://freepats.zenvoid.org/Piano/acoustic-grand-piano.html),
  [original credits](licenses/YDP-Grand-Piano-original-credits.txt),
  [CC BY 3.0 text](licenses/CC-BY-3.0.txt).
- String samples: **VSCO 2 Community Edition**, Versilian Studios/Sam Gossner,
  Ivy Audio/Simon Dalzell, sample cutting by Elan Hickler/Soundemote; **CC0 1.0**.
  [Source](https://github.com/sgossner/VSCO-2-CE),
  [original credits](licenses/VSCO-original-credits.txt),
  [CC0 text](licenses/VSCO-CC0-1.0.txt).

These media licenses are independent of the application's MIT license. The
recording-specific evidence and original checksum are preserved in
[source-evidence.json](licenses/source-evidence.json). We document source-declared
grants and provenance checks, not an independent guarantee of copyright status.

Changes include automatic audio-to-MIDI transcription; estimated note timings,
velocities, overlapping notes and sustain; render-only softened velocities;
selected tempo; sampled piano synthesis; new string arrangement where indicated;
EQ and room processing; constant loudness gain; removal of a quiet tail; MP3
encoding and ID3 credits. The upstream audio is not modified or committed here.

## Preparation inputs and limitations

[song.json](song.json) lists exact file hashes, measurements and versioned release
download URLs. [render-settings.json](sources/render-settings.json) freezes the
instrument and effect settings. [piano-transcription.json](sources/piano-transcription.json)
preserves estimated original note velocities and pedal events before render-only
softening. [string-arrangement-original-timing.json](sources/string-arrangement-original-timing.json)
preserves the original-performance timeline; multiply times by `1 / 0.833333` for
sleep tempo. Its per-note complete-release boundaries prevent incompatible
string harmonies from overlapping. [Sample provenance](sources/string-sample-provenance.json)
includes mapping, tuning and exact hashes; obtain those instrument samples
separately, outside tracked files.

The four MIDIs already contain the 0.8 piano render-velocity scale, explicit
sustain release and piano reverb send. Do not soften their velocities a second
time. Their 120 BPM timing grid preserves performance timing, rather than
representing a reliable score beat grid. Sustain is binary, not reliable
half-pedal. Automatic transcription can contain mistakes. General MIDI players
use different instruments and release envelopes and will not reproduce these
sample-rendered MP3s exactly. FluidSynth 2.6.1 rendered the YDP piano; the bowed
VSCO samples were mapped/resampled and release-gated separately, then mixed.

## Downloads

The MP3 links above download the actual audio directly from Raw GitHub. They are
pinned to the existing audio commit `5edf1ebbff08dd82ab476cc4b7d8c23fcc30b967`,
so adding further songs does not change these download bytes or URLs. `song.json`
records each URL and SHA-256. MIDI files and license evidence remain independently
available in this directory. No music release or release ZIP is required.
`checksums.sha256` covers all song files other than itself.
Publication does not automatically replace the app's existing catalog recording.

## Printable working scores

[Score notes and editable source](scores/README.md). These are rhythmically
quantized MIDI working editions requiring musical proofreading, with approximate
tempo references for both audio speeds.

- [Piano solo](scores/burgmuller-berceuse-op109-no7-piano-v1.pdf) - 2 page(s).
- [Piano with strings - full score](scores/burgmuller-berceuse-op109-no7-piano-strings-v1.pdf) - 3 page(s).
- [Violin part](scores/burgmuller-berceuse-op109-no7-violin-v1.pdf) - 1 page(s).
- [Viola part](scores/burgmuller-berceuse-op109-no7-viola-v1.pdf) - 1 page(s).
- [Cello part](scores/burgmuller-berceuse-op109-no7-cello-v1.pdf) - 1 page(s).
