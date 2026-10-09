# OpenSleep main development handoff

Main development continues in **OpenSleep MainDev** from 8 October 2026.
The previous **Open-sleep-music Main dev** conversation completed Preview
0.1.36. Catalog and artwork work continues separately in **OpenSleep Katalog**.

## Completed release

[Preview 0.1.36](https://github.com/jochenwezel/open-sleep-music/releases/tag/v0.1.36)
was published on 8 October 2026 from commit
`39719c8bf31449ad341a2c897c4027a877ff087f`.
The signed Android APK, its SHA-256 file and the Windows ZIP are uploaded.
The [release workflow](https://github.com/jochenwezel/open-sleep-music/actions/runs/37837197537),
[Core tests](https://github.com/jochenwezel/open-sleep-music/actions/runs/37837194317)
and [Windows build and tests](https://github.com/jochenwezel/open-sleep-music/actions/runs/37837194417)
completed successfully. The release records 184 passing Core tests per channel
and successful Windows and Android builds; device listening validation remains
pending.

The release includes persistent song artwork and title when controls are hidden,
unchanged images across consecutive songs with identical motifs, and optional
looping background audio. The background fades during the final 600 ms of the
primary song and reaches silence with it, without delaying advancement.
Short and invalid durations must remain safe.

Preview 0.1.37 is now complete:
[release](https://github.com/jochenwezel/open-sleep-music/releases/tag/v0.1.37),
[successful package workflow](https://github.com/jochenwezel/open-sleep-music/actions/runs/37841272774).
Commit `e008fd59e98a285d4d65d47c009ab28872b15940` only increments the app
version and uses `[skip ci]`, as requested. Its implementation passed 197 tests
per channel, Windows and Android builds, and GitHub Tests, Build and test, and
Publish media catalog workflows.

The downloaded APK, checksum file and Windows ZIP match their published SHA-256
digests. The APK reports `org.opensleepmusic.app`, version code 37 and version
name 0.1.37; its signature verifies against the documented release certificate.
Both archives were checked for bundled audio and contain none. Install this
update to enable the separate artwork per collection, then refresh the library.
Device validation remains pending.

## Open todos

- [ ] Fix and validate [Android task-close cleanup, issue 19](https://github.com/jochenwezel/open-sleep-music/issues/19).
  Removing the app from Recents must stop playback, release the media session,
  remove the media card and prevent restoration of that explicitly stopped
  session. Home and screen-lock background playback must continue working.
- [x] Assign Romanza española and Recuerdos de la Alhambra to the reviewed
  meadow-cricket recording at 50% background volume for Preview audition in
  `tools/track-backgrounds.json` and regenerate both catalogs. Preserve the
  recording-rights, dependency-download and trim rules in
  [background-audio.md](background-audio.md).
- [ ] Perform Windows and Android listening checks with an assigned background:
  seamless trimmed loops, relative volume, pause/resume, seeking, speed changes,
  audio-focus interruptions, sleep-timer fades, track advance and single-song
  repeat. Confirm the final fade ends with the primary song and missing background
  audio still allows primary playback.
- [ ] Validate Preview 0.1.37 on the reporting Android device: separate artwork
  for shared recordings in Lullabies and Quiet Classics, duration display,
  advancing seek slider, seeking in Recuerdos, original-recording start/end trims,
  persisted mixed lullaby order, song image/title visibility and transitions
  between identical motifs. Retain downloads and settings by installing as an
  update.
- [ ] Complete the device acceptance checks in [roadmap.md](roadmap.md) and
  [playback-regression-checks.md](playback-regression-checks.md), recording results
  in release notes or a follow-up issue. These cover standby, battery optimization,
  calls, Bluetooth/headset controls, process recreation, rotation, navigation,
  download cancellation and sleep-timer expiry. Review shared-track examples
  against current catalog membership before running them.
- [ ] Continue recording-rights and listening reviews for Preview candidates in
  the catalog workflow. Favorites are listening feedback, not license approval;
  promotion requires verified recording rights and all production admission
  checks.

## Concurrent catalog and artwork work

The first meadow-cricket assignments were published after Preview 0.1.37.
That binary rejects backgrounds sourced from Preselection even when their
recording rights are verified, causing the catalog update to fall back silently.
Repairing a collection cannot change that binary validation rule. The tested
Preview-only rights guard and pairings from `de152b9` are included in Preview
0.1.38. After installing the update, repair the desired collection to fetch the
separate cricket dependency, then start Romanza or Recuerdos again. The app plays
both original recordings using its background feature; no mixed song file is
downloaded or packaged. Real-device listening remains pending.

Romanza A, Alhambra A and Julia Florida A were selected and assigned after the
0.1.36 app release. Julia B, the duckling, remains an unassigned design for
possible later use.

Collection-specific artwork for shared recordings was completed and published
in **OpenSleep Katalog** as commit `1643fb503ae72a722a7bc457b7b779155e10b4b0`.
Children's motifs appear in Lullabies for little ones, while Quiet Classics
initially uses its existing classical motif. More specific classical imagery
can be considered later. The change covers Core metadata, catalog generation,
artwork caching and both player views. Preview 0.1.37 includes the implementation;
0.1.36 packages predate it.

Preserve unrelated changes in the shared working tree and exclude
`.codex-remote-attachments/`, build output and local downloads from commits.

## Decisions to retain

- The default sleep timer remains 60 minutes; saved user settings take precedence.
- All trims use integer milliseconds of the original recording at 100% speed;
  stretching applies afterwards. Keep original media and checksums intact.
- Background audio uses normal speed, follows the primary player's fades and
  must never block primary playback when unavailable.
- Lullabies retain their persisted mixed catalog order independently of shuffle.
- Stable remains the default channel and excludes both Preview audition
  collections. Validate both channels whenever their gates change.
- Follow `AGENTS.md` for curation, resilient downloads, artwork, platform builds,
  repository language and branch cleanup.
