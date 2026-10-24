# Music model options and local preparation

Reviewed on 2026-10-24. These are preparation tools, not features bundled in the
mobile app. The app continues to play the finished MP3s using its existing player.

| Tool | Intended use | Cost and current licensing evidence | Practical decision |
| --- | --- | --- | --- |
| [Transkun](https://github.com/Yujia-Yan/Transkun) | Expressive solo-piano audio to MIDI; note intervals, velocities, timing and pedal estimates. | Local software under [MIT](https://github.com/Yujia-Yan/Transkun/blob/main/LICENSE); the default checkpoint is shipped in the project/package. No per-song API fee for running it locally. Inspect separate terms before using another externally linked checkpoint. | Preferred candidate for an independent short comparison against the current Kong transcription. CPU and CUDA inference are documented. Not yet tested on our recordings. |
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

For a new Fauré comparison, first transcribe a short original-recording passage
with an independently trained model, render it with the same YDP B preset and
matched loudness, and compare it with the source and prior result. Only extend
the test if it addresses the actual note/timing defects. This is a proposed
method, not a completed attempt, a listening approval or a resumption of the
previously abandoned correction path. Log actual attempts and their outcomes in
the central [catalog quality register](catalog-quality-notes.md).

Keep recording, model/code and instrument-sample rights separate. Running a model
does not remove source-performance rights, and a free code license does not
automatically license its weights, training dataset or generated audio.
General chat-model rankings and reasoning effort are not evidence of reliable
audio-to-MIDI recovery; [this music-perception evaluation](https://proceedings.mlr.press/v303/carone26a.html)
finds substantially weaker audio performance than symbolic MIDI reasoning.
