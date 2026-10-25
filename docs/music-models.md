# Music model options and local preparation

Reviewed on 2026-10-25. These are preparation tools, not features bundled in the
mobile app. The app continues to play the finished MP3s using its existing player.

| Tool | Intended use | Cost and current licensing evidence | Practical decision |
| --- | --- | --- | --- |
| [Transkun](https://github.com/Yujia-Yan/Transkun) | Expressive solo-piano audio to MIDI; note intervals, velocities, timing and pedal estimates. | Local software under [MIT](https://github.com/Yujia-Yan/Transkun/blob/main/LICENSE); the default checkpoint is shipped in the project/package. No per-song API fee for running it locally. Inspect separate terms before using another externally linked checkpoint. | Independent Fauré excerpt test completed on RTX 4060 on 2026-10-25 with the bundled V2 checkpoint; positive feedback on the problem passage, uneven articulation still under review and opening unreviewed. [Exact test record](../tools/transkun-faure-test-2026-10-25.json). |
| [Aria-AMT](https://github.com/EleutherAI/aria-amt) | Sequence-to-sequence piano transcription, designed for real recordings. | Apache 2.0 component grant from the author, naming the transcription model; the official checkpoint identity and scope interpretation are documented in the [2026-10-25 review](aria-amt-license-review-2026-10-25.md). The separately published Aria-MIDI dataset remains CC BY-NC-SA 4.0 and is not used or redistributed. | Complete Fauré Aria/YDP solo explicitly approved and selected for Lullabies for little ones on 2026-10-25. Individual mazurka listening and source-rights reviews remain pending. Local Windows compatibility run, not an author benchmark. |
| [Spotify Basic Pitch](https://github.com/spotify/basic-pitch) | Instrument-agnostic polyphonic note transcription; another independent estimate of pitches, timing and amplitude-derived velocities. | Code and bundled model distribution are Apache 2.0; retain Spotify credits separately from performance/sample rights. Local ONNX inference has no hosted API fee. | Rejected by the maintainer after the Fauré problem-passage audition; do not pursue it in this workflow. Retain earlier A5 corroboration and the rejected audition as evidence. [Exact test record and feedback](../tools/faure-other-models-test-2026-10-25.json). |
| [Music Flamingo](https://huggingface.co/nvidia/music-flamingo-2601-hf) | Audio-language analysis of harmony, timbre, structure and musical descriptions. | Downloadable model under the NVIDIA OneWay Noncommercial License; the model card explicitly limits it to noncommercial research. Hosting or rented hardware may cost extra. | A research lead, not a default production preparation tool or guaranteed note-accurate MIDI transcriber. No commercial permission or model deployment is inferred from the app's free price. |

Local inference does not incur a hosted model's per-request fee. It still consumes
electricity, storage and processing time. Optional rented GPUs or hosted APIs have
their own charges; none is required for the proposed first local Transkun test.
The host has an NVIDIA RTX 4060 with 8 GB VRAM. The existing transcription runtime
contains CPU-only PyTorch; a separate PyTorch 2.11.0 / CUDA 12.8 runtime recognizes
the RTX 4060 without modifying the working CPU environment. A 36.5-second
comparison against the existing CPU transcription retained all 486 notes and
their integer velocities; maximum onset/offset differences were under 4
microseconds with the same Kong checkpoint and float32 processing, TF32 disabled.
The validated CUDA configuration processes eight segments per inference batch
and is used for the remaining mazurka preparation. This validates that test
case, not every recording, another transcription model or its memory fit.

On 2026-10-25 the maintainer authorizes the independent Transkun test. A separate
runtime loads the bundled V2 checkpoint with `weights_only=True` and a strict
state-dict match, leaving the upstream model code and working runtimes unchanged.
Two contexts (original 0–36 and 130–187 seconds) yield estimates of notes,
velocities and sustain. Peak allocated GPU memory is about 883 MiB. The audible
excerpts cover original 1.65–26.65 and 142.499943–174.499943 seconds; the latter
corresponds to slow 2:51–3:29. Both synthetic versions use the same YDP B preset
and MIDI event speed 0.833333, with source-reference/Kong A2/Transkun comparisons
at matched loudness. The source reference alone uses FFmpeg pitch-preserving
`atempo`, as a diagnostic comparison rather than a new production recording.

Technical delivery checks pass for eight MP3s and eight contextual MIDI files.
The maintainer subsequently finds the problematic passage substantially better,
but reports uneven melody note lengths/apparent rhythm; the opening is unreviewed.
This is partial feedback, not full-piece approval or acceptance of the earlier
rejected repairs. An [articulation analysis and CC64-only diagnostic](../tools/transkun-faure-articulation-review-2026-10-25.json)
finds close timing among matched attacks but substantially different key-offs
and sustain-release patterns. The source-envelope probes remain inconclusive;
the diagnostic is not an established pedal correction. One problem-passage
comparison is prepared for later because the maintainer has limited listening
time. Preserve expressive timing and establish the cause before quantizing
beats or extending notes. Outcomes remain in the central
[catalog quality register](catalog-quality-notes.md), including earlier failures.

The maintainer subsequently requests the other software candidates for the same
problem passage. Aria-AMT and Basic Pitch each transcribe original seconds
130–187; audible renders retain the same 142.499943–174.499943 interval, YDP B
voice and 0.833333 event speed. A four-way overview presents identical 12-second
excerpts in the order source, unmodified Transkun, Aria-AMT and Basic Pitch, with
matched section loudness and one-second pauses. Only the two new full excerpts
and their contextual MIDI files are added; no whole-piece conversion or catalog
replacement follows from this test.

Aria's recommended `piano-medium-double-1.0` safetensors file is downloaded at a
pinned repository revision and verified against its upstream LFS SHA-256.
The model/state dictionary matches strictly. The runtime retains the authors'
mel transform, decoder, 30-second/10-second-stride windowing and token-sequence
reconciliation, substituting miniaudio input and a synchronous caller for the
legacy streaming/multiprocessing I/O. The local Python 3.12 / torch/torchaudio
2.11 environment differs from the package's torchaudio <=2.5 declaration;
it is a documented compatibility experiment. Native time tokens use 10 ms and
velocity tokens steps of five; no additional beat quantization is imposed.

Basic Pitch 0.4.0 runs its ONNX model with default thresholds, 127.7 ms minimum
note duration and Melodia post-processing. It has no inferred sustain events;
none are borrowed from another model. Its native note intervals and estimated
velocities remain in the MIDI, with raw pitch bends retained only in the raw
output. These model differences can change articulation even with an identical
soundbank. Music Flamingo is not executed here: its provided audio/text-to-text
interface does not supply direct AMT MIDI for an equivalent piano render.
The maintainer subsequently rejects Basic Pitch and the D1 pedal-substitution
diagnostic, and selects Aria as the best problem-passage variant. These listening
judgments do not establish a general benchmark ranking or approve a whole piece.

At the maintainer's request, complete native-pedal
[Transkun](../tools/faure-transkun-complete-2026-10-25.json) and
[Aria](../tools/faure-aria-complete-2026-10-25.json) solo auditions are now
prepared, each about 4:16 at the same sleep tempo and YDP B settings. Each
contains full-source inference, its own estimated pedal and original/sleep MIDI
with unsoftened estimated velocities. D1 is not applied; no further Basic Pitch
render is made. Full inference can differ from cropped inference because of
segment context; the previous excerpt is not spliced into the new output.
Complete listening remains pending. Aria's separate checkpoint-rights review
is also unresolved, so this is private audition/research rather than production
admission or public music publication.

Keep recording, model/code and instrument-sample rights separate. Running a model
does not remove source-performance rights, and a free code license does not
automatically license its weights, training dataset or generated audio.
General chat-model rankings and reasoning effort are not evidence of reliable
audio-to-MIDI recovery; [this music-perception evaluation](https://proceedings.mlr.press/v303/carone26a.html)
finds substantially weaker audio performance than symbolic MIDI reasoning.

## Complete Aria selection, 2026-10-25

The maintainer subsequently approves the complete Fauré Aria rendition and its
adoption in Lullabies for little ones. The earlier pending decisions above are
historical. The [author component-grant review](aria-amt-license-review-2026-10-25.md)
resolves this project's checkpoint classification with its exact source/scope
interpretation; no dataset performances or model weights are distributed.
The [selected publication](../music/faure-berceuse-op56-no1/README.md) retains the
approved audio/MIDI bytes. Other Fauré audition folders and rejected current v1
media/scores are retired; original sources and useful preparation inputs remain
local and failed attempts remain in the central quality register.
