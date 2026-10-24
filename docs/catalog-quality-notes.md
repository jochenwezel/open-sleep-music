# Catalog quality notes and improvement history

This is the central, cross-song internal curation register. Identify every work
by its stable catalog track ID. Keep the current assessment separate from the
append-only history of attempted improvements; do not remove failed attempts
when a later method succeeds. Link evidence instead of duplicating large reports.

For every new attempt, record `YYYY-MM`, the AI engine/session label and effort,
the actual audio/transcription tools, the method and the observed outcome.
Distinguish a maintainer listening judgment from an automatic measurement. An
entry here changes neither runtime playback nor catalog admission or licensing.

## Current assessments

| Catalog track ID | Work | Current quality | Retention | Priority | Alternatives and current action |
| --- | --- | --- | --- | --- | --- |
| `faure-berceuse-op-56-no-1` | Gabriel Fauré — Berceuse op. 56 no. 1; Brian M. Jones piano recording | Poor, according to the maintainer; improvement is needed. | Keep the work and current recording until an acceptable replacement is available. The work remains desirable. | **High** | Sources found so far have not met the required listening quality. Synthetic improvement has not succeeded. Retain the original recording; do not promote the rejected conversions or repeat them without a materially different, justified approach. |

Assessment updated **2026-10** from the maintainer's explicit instructions.
The alternative-source result describes the search and listening feedback so far;
it does not assert that no better recording exists. Rights review remains separate.

## Attempt history

The AI engine/session label below is supplied by the maintainer:
**ChatGPT 6.1 Sol Hoch**. It records the assistant session used for the work;
it is not the name of the piano-transcription model or audio synthesizer.

| Catalog track ID | Month (`YYYY-MM`) | AI engine / effort | Attempt and actual tools | Outcome |
| --- | --- | --- | --- | --- |
| `faure-berceuse-op-56-no-1` | 2026-10 | ChatGPT 6.1 Sol Hoch | Initial performance-derived audio-to-MIDI transcription, retaining estimated velocities, overlaps and pedal; Kong et al. note/pedal model, YDP B / FluidSynth piano and optional strings. | Unsuccessful: the maintainer hears wrong pitches/timing, dissonances and an opening unlike the source. A warmer instrument does not repair the recognized notes. Strings were set aside to examine piano solo first. |
| `faure-berceuse-op-56-no-1` | 2026-10 | ChatGPT 6.1 Sol Hoch | A1: remove unsupported very short note detections using the transcription model's frame activations; rerender piano. | Unsuccessful as a replacement; later source checks and A2 were needed. No listening approval. |
| `faure-berceuse-op-56-no-1` | 2026-10 | ChatGPT 6.1 Sol Hoch | A2: opening-specific false re-attack checks, removal of unsupported events and selected key-off repairs; YDP B render. | Still unsatisfactory. The maintainer identifies stumbling at 2:51–3:12 in the slow version and, more mildly, afterwards through the ending. |
| `faure-berceuse-op-56-no-1` | 2026-10 | ChatGPT 6.1 Sol Hoch | A3: source-partial envelope analysis and correction of probable false repeated attacks in the reported passage. | Insufficient: the problem extends beyond that passage; a full-piece review was required. No accepted replacement. |
| `faure-berceuse-op-56-no-1` | 2026-10 | ChatGPT 6.1 Sol Hoch | A4: whole-piece repeat/partial-envelope review, including quiet-attack protection; retain performance timing, dynamics and pedal. | Rejected as still unconvincing. Additional score/MIDI references were requested. |
| `faure-berceuse-op-56-no-1` | 2026-10 | ChatGPT 6.1 Sol Hoch | Mutopia score MIDI rendered with the intended flute/guitar instrumentation. | Rejected as mechanical and unattractive. Keep notation only as a tentative melody/harmony reference. |
| `faure-berceuse-op-56-no-1` | 2026-10 | ChatGPT 6.1 Sol Hoch | tonic64 score-based MIDI rendered with the YDP piano. | Rejected as mechanical and unattractive. Keep notation only as a tentative melody/harmony reference; do not replace expressive performance events wholesale. |
| `faure-berceuse-op-56-no-1` | 2026-10 | ChatGPT 6.1 Sol Hoch | A5: targeted octave/shared-harmonic checks using independent Spotify Basic Pitch analysis, source spectra and notation clues; preserve all surviving timing, velocity and pedal events. | Rejected. The maintainer ends this conversion path on 2026-10-24. Keep the original app recording and the evidence of unsuccessful attempts. |

Evidence: [recording and alternative-source research](faure-berceuse-research-2026-10-14.md),
[detailed A1–A5 and score-audition review](../music/faure-berceuse-op56-no1/sources/piano-review-2026-10-23.json),
[score-basis research](../music/faure-berceuse-op56-no1/sources/score-basis-research-2026-10-23.json).

## Maintenance

The [music model comparison](music-models.md) records candidate tools, local-use
costs and outstanding weight-license questions. Proposed methods are not failed
attempts and must not be inserted into the history as if they had been tested.

- Add other songs to these same tables; do not create a separate quality register
  per song or a parallel replacement-priority list.
- Update current quality, retention, priority, known alternatives and next action
  when new maintainer feedback or evidence arrives.
- Append each actual research, correction or synthesis attempt with its month,
  session label, tools and result. Record pending outcomes explicitly; do not
  invent rejection or approval. State when the engine label is unknown.
- On success, mark the assessment resolved and link the accepted recording or
  catalog change; retain the previous failed-attempt history.
- Keep large technical reports and recording-rights evidence in their existing
  source locations and link them here. Do not put internal assessments into
  user-visible titles, creator credits or license text.
