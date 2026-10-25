# Music model options and local preparation

Reviewed on 2026-10-25. These are preparation tools, not features bundled in the
mobile app. The app continues to play the finished MP3s using its existing player.

| Tool | Intended use | Cost and current licensing evidence | Practical decision |
| --- | --- | --- | --- |
| [Transkun](https://github.com/Yujia-Yan/Transkun) | Expressive solo-piano audio to MIDI; note intervals, velocities, timing and pedal estimates. | Local software under [MIT](https://github.com/Yujia-Yan/Transkun/blob/main/LICENSE); the default checkpoint is shipped in the project/package. No per-song API fee for running it locally. Inspect separate terms before using another externally linked checkpoint. | Independent Fauré excerpt test completed on RTX 4060 on 2026-10-25 with the bundled V2 checkpoint; musical listening pending. [Exact test record](../tools/transkun-faure-test-2026-10-25.json). |
| [Aria-AMT](https://github.com/EleutherAI/aria-amt) | Sequence-to-sequence piano transcription, designed for real recordings. | Code is Apache 2.0. The recommended checkpoint is hosted in the [Aria-MIDI repository](https://huggingface.co/datasets/loubb/aria-midi), whose dataset card declares CC BY-NC-SA 4.0; an explicit separate checkpoint grant has not been established in this review. Do not equate the code license with a cleared checkpoint license or the rights in dataset performances. | Possible research comparison after checkpoint terms and runtime support are clarified. The authors describe Linux/CUDA inference; do not assume the same installation as the current Windows CPU pipeline. |
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
This is not a listening approval, a full-piece conversion or an acceptance of
the previously rejected repairs. Only extend the test if listening establishes
that it addresses the actual note/timing defects. Outcomes remain in the central
[catalog quality register](catalog-quality-notes.md), including earlier failures.

Keep recording, model/code and instrument-sample rights separate. Running a model
does not remove source-performance rights, and a free code license does not
automatically license its weights, training dataset or generated audio.
General chat-model rankings and reasoning effort are not evidence of reliable
audio-to-MIDI recovery; [this music-perception evaluation](https://proceedings.mlr.press/v303/carone26a.html)
finds substantially weaker audio performance than symbolic MIDI reasoning.
