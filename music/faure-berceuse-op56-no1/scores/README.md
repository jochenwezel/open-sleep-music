# Working notation for Gabriel Fauré - Berceuse op. 56 No. 1

These PDFs are **automatic MIDI working editions** for musicians, not a
note-for-note proofread original score. They use the published original-tempo
MIDI notes plus the stored complete-release string metadata. Both audio tempos
use the same written notes; approximate original/sleep tempo references are
printed on each first page.

## Printable parts

- [Piano solo](faure-berceuse-op56-no1-piano-v1.pdf) - 4 page(s).
- [Piano with strings - full score](faure-berceuse-op56-no1-piano-strings-v1.pdf) - 7 page(s).
- [Violin part](faure-berceuse-op56-no1-violin-v1.pdf) - 2 page(s).
- [Viola part](faure-berceuse-op56-no1-viola-v1.pdf) - 2 page(s).
- [Cello part](faure-berceuse-op56-no1-cello-v1.pdf) - 2 page(s).

## Editorial choices and limits

83 expanded measures, with a saved performance-time measure grid. The
nominal 120 BPM grid of the MIDI is not treated as a reliable score beat grid.
Actual attacks/durations are quantized to sixteenths; simultaneous attacks are
chordified. The piano uses two staves with separate melody/inner/bass voices;
inner durations are clipped at subsequent attacks rather than printing pedal
resonance as many overlapping written voices. Strings use their complete sounding
ends rather than the earlier note-off used to begin a sample release. Compatible
repeated held tones may be tied together. Repeats are written out.

104 piano attacks shorter than 80 ms were omitted for this working notation.
Their unchanged original MIDI events remain available in `../midi/`. Dynamics
are broad estimates; pedal is indicated as a general harmonic instruction, not
a reliable half-pedal transcription. No fingering or bowing edition is claimed.
Check pitches, enharmonic spelling, rhythms, hand distribution and ensemble
phrasing before rehearsing or performing. A layout review is not a musical
proofreading certificate.

## Editable source and reproduction

[LilyPond source](faure-berceuse-op56-no1.ly) and [engraving metadata](engraving.json)
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

Composition: Gabriel Fauré. Source arrangement/performance: Brian M. Jones; solo arrangement and performance, September 2010.
MIDI-based notation and new string arrangement/doubling: Open Sleep Music
contributors. These working scores retain **CC BY 3.0**: https://creativecommons.org/licenses/by/3.0/
Source recording: https://commons.wikimedia.org/wiki/File:Berceuse_by_Gabriel_Faur%C3%A9_op56_no1.ogg

Retain attribution, source/license links and these modification notices.
No source performer endorsement is claimed. Full recording-license evidence
and texts are in `../licenses/`; score licenses are independent of the app MIT
license. Piano/string instrument samples are not embedded in these PDFs.

Metric/form reference: https://www.mutopiaproject.org/cgibin/piece-info.cgi?id=1536
Mike Blackstock, Mutopia-2008/09/01-1536; public-domain flute/guitar adaptation. Used only for alignment, form, key changes and melody voice identification; the published note material is derived from Brian M. Jones project MIDI.
