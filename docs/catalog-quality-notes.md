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
| `faure-berceuse-op-56-no-1` | Gabriel Fauré — Berceuse op. 56 no. 1; Brian M. Jones piano recording | Poor, according to the maintainer; improvement is needed. | Keep the work and current recording until an acceptable replacement is available. The work remains desirable. | **High** | Sources found so far have not met the required listening quality. No synthetic replacement has been accepted. A separately authorized independent Transkun V2 excerpt test on 2026-10-25 is technically complete and awaits maintainer listening; retain the source recording and rejected-attempt history. |
| Chopin mazurka cohort; all stable IDs in the [initial](../tools/mazurka-auditions-2026-10-24.json) and [remaining-work](../tools/mazurka-auditions-remaining-2026-10-24.json) manifests | 57 Quiet Classics mazurkas | Initial three-work YDP solo/string comparison receives positive maintainer feedback; individual outcomes for the remaining 54 are pending. | Keep current catalog recordings while auditioning. | Requested preparation completed; per-work review pending. | Full solo/string sleep-tempo auditions are available in Nextcloud. Resolve outstanding recording-source questions before promotion or public publication. |

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
| Chopin mazurka cohort; 57 stable IDs linked above | 2026-10 | ChatGPT 6.1 Sol Hoch | Kong note/pedal transcription with expressive unquantized MIDI; FluidSynth/YDP B solo and quiet VSCO string doublings at event speed 0.666667. Initial three works followed by 54 requested additional works. [Settings, evidence and delivered hashes](mazurka-auditions-remaining-2026-10-24.md). | Positive maintainer feedback on the initial comparison prompts the expansion. The 54 additional full-work A/B auditions are prepared and technically checked; their individual musical listening outcomes remain pending. No catalog replacement or blanket arrangement approval. |
| `faure-berceuse-op-56-no-1` | 2026-10 | ChatGPT 6.1 Sol Hoch | Independent Transkun V2 bundled-checkpoint test, authorized 2026-10-25: opening and the reported slow 2:51–3:12 passage extended through 3:29, with surrounding transcription/render context. Estimated velocities and pedal, no manual note changes or quantization; YDP B / FluidSynth at event speed 0.833333, no strings. A/B/C compares source, Kong A2 and Transkun with matched loudness. [Exact settings, model/source hashes and delivered files](../tools/transkun-faure-test-2026-10-25.json). | Technical test completed locally on RTX 4060: eight MP3s and eight context MIDI files, source/credits and reports copied with SHA-256 verification. Musical outcome pending; fewer detected notes or inter-model agreement is not proof of better accuracy. No full-piece conversion, accepted improvement or catalog replacement. |
| Chopin mazurka cohort; 57 stable IDs linked above | 2026-10 | ChatGPT 6.1 Sol Hoch | On 2026-10-25, add one source/solo/strings quick comparison in each work folder at the maintainer's request: identical 15-second passages, event speed 0.666667, matched loudness and one-second pauses. Source reference uses pitch-preserving FFmpeg atempo; existing synthetic audio is excerpted without retranscription. [Positions, input hashes, chapter times and new-file checksums](../tools/mazurka-quick-comparisons-2026-10-25.json). | Comparisons prepared for all 57 works; full MP3/MIDI bytes unchanged. This improves audition access, not the transcription or arrangement itself. Individual musical judgments and recording-rights reviews remain pending. |

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
