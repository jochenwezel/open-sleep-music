# Aria-AMT component and output provenance review

Reviewed 2026-10-25 for the selected Fauré recording and private mazurka auditions.

The author distinguishes the published Aria-MIDI dataset from the transcription
pipeline components. In the [author's pinned README](https://github.com/loubbrad/aria-midi/blob/24cd6d37d0dff4d63d1818f4da7b86ac53497064/README.md#dataset),
the paragraph introducing the pipeline says “we also release under the Apache-2.0
license” and specifically identifies Aria-AMT as the transcription model.
Its later license section assigns CC BY-NC-SA 4.0 to **Aria-MIDI**, the released
MIDI collection. The same distinction appears in the author's Hugging Face
dataset card. A repository's overall dataset label is not a substitute for this
component-specific scope.

The [official Aria-AMT README](https://github.com/EleutherAI/aria-amt/blob/a1ab73fc901d1759ec3bc173c146b3c6a3040261/README.md)
links the exact `piano-medium-double-1.0.safetensors` checkpoint used here.
Its SHA-256 is
`089d3129dbe93246aeda55efe668c8a48af08afaf9dd15c64cef0a07c0fb30a4`.
The [official component license](https://github.com/EleutherAI/aria-amt/blob/a1ab73fc901d1759ec3bc173c146b3c6a3040261/LICENSE)
is Apache 2.0. The published download is preserved at dataset revision
`8cc4cf5c83b47f2689ac256a947b2a57c17a4c8b`; its upstream LFS hash and complete
download bytes match. The component-specific author grant, official download
link and matching file identity form this review's authorization record.

**Review decision:** apply the author's Apache-2.0 component grant to that
official Aria-AMT transcription checkpoint. This is a documented interpretation
of the publisher's scoped grant, not an independent copyright warranty or a new
per-file declaration invented for the Hugging Face file. No separately tagged
checkpoint license was found there. Conflicting third-party mirror labels are
not relied on. This review supersedes the earlier unresolved classification;
retain that historical review in the audition evidence rather than rewriting it
as if clearance had existed then.

The project does not use or redistribute the Aria-MIDI collection, model weights
or training recordings in the application or music publication. Its input is
the independently licensed Brian M. Jones performance. Its output is an estimate
of that input's note, velocity and pedal events, rendered with separately licensed
YDP samples. Source performance, adaptation and YDP sample credits remain CC BY
3.0, independently of the transcription software's Apache license. No code or
model license is used to waive recording or instrument-sample rights.

Retain the author grant, exact Apache license, model/code identifiers, checkpoint
hash, source-performance grant, soundbank credits and modification description
in the selected song's evidence. This decision clears the identified preparation
component; each mazurka still needs its own recording-rights and musical review
before promotion. No blanket catalog admission follows from using Aria.
