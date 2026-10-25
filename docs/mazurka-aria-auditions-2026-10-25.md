# Complete Aria mazurka audition additions

Prepared 2026-10-25 at the maintainer's request, alongside the existing Kong/YDP
piano and piano/string versions. All 57 work folders receive:

- One full **C: Aria/YDP piano solo** MP3 at the existing .666667 sleep-event speed.
- One short **previous Kong piano → Aria piano** comparison: identical 15-second
  passages starting at 0:00 and 0:16, with one second of silence and matched levels.
- Original-performance and sleep-timing editable MIDI in `Aria/`, plus source,
  inference, preparation and exact copied-byte evidence.

The full Aria audio totals approximately 3.76 hours. Existing
114 full MP3s, 171 MIDI files and 57 source/solo/string quick comparisons retain
their exact hashes. No Aria strings, production catalog replacement, new default
ordering or blanket musical approval follows. The maintainer will choose per work.

## Preparation and limits

Aria-AMT `piano-medium-double-1.0` independently estimates each source's notes,
velocities, key-offs and sustain. Preserve its native 10 ms time tokens and
five-step velocities; add no beat grid or foreign pedal. Original estimates stay
separate from .8 render-only velocity softening. YDP B, the previous warm EQ,
verified room, static loudness matching near -23.21 LUFS, ample headroom and
quiet-tail trimming keep the instrument/processing comparison consistent.
Pieces with stronger transients use a lower constant-gain target to retain
headroom; no compressor or limiter flattens their dynamics.
Timing changes in MIDI before synthesis, with no stretching of rendered MP3s.

The RTX 4060 run uses bfloat16 and **one piece per inference batch**. Larger
batches were probed and not selected because they changed more event tuples.
CUDA graph replay calls the same native decoder operators; it reduces launch
overhead and is a documented runtime adaptation. In the one-piece graph/eager
probe, 441 notes retain pitches, velocities and count, with two native 10 ms
timing differences. This is neither bit identity nor an author accuracy benchmark.
Full context may also differ from cropped inference. Per-work reports record
same-pitch interval clipping and any detections outside the source interval;
measured technical validity does not establish score accuracy or listening quality.

## Rights and delivery

The [component-grant review](aria-amt-license-review-2026-10-25.md) documents the
author's specific Apache-2.0 statement for Aria-AMT and the exact official
checkpoint, distinguished from the NC-SA Aria-MIDI dataset. Dataset files and
weights are not redistributed. Earlier unresolved text within inherited
checkpoint snapshots is historical and superseded by the separate scoped review.

Each source recording still has its separate attribution and rights review.
The Archive mirror's CC0 label is not independent performer permission. Retain
recording-specific corroborations and outstanding discrepancies, including the
initial Olga Gurevich CCBY4/mirror difference and unresolved performer identities.
These remain private auditions until each work passes listening and source-rights
checks. YDP's CC BY 3.0 sample credits accompany both folder groups.

Nextcloud location: `Open Sleep Music/Probehören/2026-10-24-Chopin-Mazurken-YDP`.
`03-ARIA-UEBERSICHT.md` links every full C version and short comparison. Each
work's `00-ARIA-VERGLEICH.md` explains playback order; `Aria/` retains reports
and MIDIs. Exact copies and complete root/per-work inventories are hash-checked;
independent authenticated server-download verification has not been performed.

[Machine-readable results, settings, probes and exact hashes](../tools/mazurka-aria-auditions-2026-10-25.json)
retain the actual tools and session label. No upstream recordings, model weights,
working WAVs or audition MP3s are added to tracked project publications.
