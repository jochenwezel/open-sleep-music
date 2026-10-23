# Working notation for Friedrich Burgmüller - Berceuse op. 109 No. 7

These PDFs are **automatic MIDI working editions** for musicians, not a
note-for-note proofread original score. They use the published original-tempo
MIDI notes plus the stored complete-release string metadata. Both audio tempos
use the same written notes; approximate original/sleep tempo references are
printed on each first page.

## Printable parts

- [Piano solo](burgmuller-berceuse-op109-no7-piano-v1.pdf) - 2 page(s).
- [Piano with strings - full score](burgmuller-berceuse-op109-no7-piano-strings-v1.pdf) - 3 page(s).
- [Violin part](burgmuller-berceuse-op109-no7-violin-v1.pdf) - 1 page(s).
- [Viola part](burgmuller-berceuse-op109-no7-viola-v1.pdf) - 1 page(s).
- [Cello part](burgmuller-berceuse-op109-no7-cello-v1.pdf) - 1 page(s).

## Editorial choices and limits

41 expanded measures, with a saved performance-time measure grid. The
nominal 120 BPM grid of the MIDI is not treated as a reliable score beat grid.
Actual attacks/durations are quantized to sixteenths; simultaneous attacks are
chordified. The piano uses two staves with separate melody/inner/bass voices;
inner durations are clipped at subsequent attacks rather than printing pedal
resonance as many overlapping written voices. Strings use their complete sounding
ends rather than the earlier note-off used to begin a sample release. Compatible
repeated held tones may be tied together. Repeats are written out.

4 piano attacks shorter than 80 ms were omitted for this working notation.
Their unchanged original MIDI events remain available in `../midi/`. Dynamics
are broad estimates; pedal is indicated as a general harmonic instruction, not
a reliable half-pedal transcription. No fingering or bowing edition is claimed.
Check pitches, enharmonic spelling, rhythms, hand distribution and ensemble
phrasing before rehearsing or performing. A layout review is not a musical
proofreading certificate.

## Editable source and reproduction

[LilyPond source](burgmuller-berceuse-op109-no7.ly) and [engraving metadata](engraving.json)
retain the note material, meter/key plan, original-performance anchors, source
MIDI hash, processing choices and review status. The source is directly editable
with LilyPond or a compatible editor.

The project tool is `tools/Export-MidiScores.py`; install its Python requirements
from `tools/score-requirements.txt` and obtain LilyPond 2.26.0 separately. From the
repository root, run:

```text
python tools/Export-MidiScores.py --lilypond <path-to-lilypond-executable>
```

The tool writes draft PDFs/logs to ignored `output/pdf/`. Re-render and visually
inspect every page, then publish the reviewed version with updated `song.json`
and checksums. Keep approved audio/MIDI bytes and audio commit links unchanged.

## Attribution and license

Composition: Friedrich Burgmüller. Source arrangement/performance: BastienM.
MIDI-based notation and new string arrangement/doubling: Open Sleep Music
contributors. These working scores retain **CC BY-SA 3.0**: https://creativecommons.org/licenses/by-sa/3.0/
Source recording: https://commons.wikimedia.org/wiki/File:Berceuse_Burgmuller.ogg

Retain attribution, source/license links and these modification notices.
Adaptations must retain the same or a compatible ShareAlike license.
No source performer endorsement is claimed. Full recording-license evidence
and texts are in `../licenses/`; score licenses are independent of the app MIT
license. Piano/string instrument samples are not embedded in these PDFs.

Metric/form reference: https://imslp.org/wiki/18_%C3%89tudes,_Op.109_(Burgm%C3%BCller,_Friedrich)
Historical edition used only for meter/form reference; no scanned pages or external engraving are republished.
