# Validation on 2026-09-17

Source revision: `72b93d56370e47ef6a6e0dded6e78d7588fe7212`.
This includes the seven defect fixes and the separate immersive-controls changes
from the parallel development task.

## Completed checks

- Release Core tests: 62 passed, none failed or skipped.
- Windows Release build: succeeded with zero warnings and zero errors.
- Android Release build: succeeded with zero warnings and zero errors.
- The existing Windows Release executable started, exposed the collection screen
  through accessibility, recognized local downloaded audio, and remained responsive.
  This confirms startup and library discovery; it is not a completed visual or audio
  playback test.
- The app process started for this check was stopped after testing.

## Device-test blockers

- Windows: activation failed with `failed to activate captured window`, including
  one retry after reacquiring the returned window. The screenshot did not show a
  usable application surface. No playback, navigation, seeking, animation, or memory
  checks are claimed from that session.
- Android: `adb devices -l` returned no devices. The configured Android SDK contains
  no emulator executable. Old AVD definitions alone do not provide a runnable emulator.
  No APK installation or on-device service test was performed.

## Remaining acceptance checks

Follow [playback-regression-checks.md](playback-regression-checks.md) on an accessible
Windows desktop and an ADB-connected Android test device or working emulator.
In particular, native sleep-timer/track-completion races, service recreation, repeated
modal navigation, closed-page collection, and slowed-track seeking still require
runtime verification. Passing unit tests and builds does not replace these checks.
