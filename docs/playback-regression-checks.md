# Playback regression checks

These device checks complement the automated Core tests. Run them on Windows and Android.

## App feedback email

1. Mark a title and open **Katalog-Feedback teilen**. Verify the summary names
   `appsupport@compumaster.de` and lists the report scope. Cancel: no composer or
   automatic network upload should occur.
2. Confirm with a supported mail client. Verify the To address is exactly
   `appsupport@compumaster.de`, subject and body are present, and the JSON
   attachment opens with the expected fields. The app must not claim delivery.
3. Discard the draft. The app must remain usable and must not retry or send it.
4. Repeat without a supported mail client and with a failing composer. Verify a
   clear message and no fallback to a general share destination.
5. On Android, confirm the attachment URI exposes only the `sharing-root` cache
   subtree. Unrelated app files must not become shareable through this provider.
6. Before sending an actual test email, use the maintainer's authorized test
   procedure. Mailbox reachability and retention are operational checks separate
   from opening the composer. iOS/Mac Catalyst need their own physical-device
   validation before their releases.

## Download cancellation and stalled sources

1. Start a collection download and use its **Download abbrechen** action while a file
   is being transferred. Completed files must remain playable; the current `.part`
   file must be removed and subsequent tracks must not start.
2. Resume the collection download. Existing valid files must be retained and the
   remaining tracks downloaded.
3. In a controlled HTTP test, return audio headers and initial bytes, then stall the
   response body. Verify a timeout log entry, partial-file cleanup, and continuation
   with the next track. The automated downloader tests cover this case without
   depending on a live media server.

## Android sleep deadline and service restart

### Screen timeout and track transitions

With the app visible, Android's normal screen timeout is inhibited during
playback and automatic track preparation. Pause, Stop and the sleep timer's
completed fade release that inhibition. Leaving the app or pressing the power
button still permits screen-off/background listening. Native MediaPlayer holds
the CPU awake while playing; a separate, at-most-30-second partial wake lock
covers release/prepare/seek between tracks and is released on playback start,
pause, stop or service destruction. No battery-optimization exemption is requested.
See [Android screen-on guidance](https://developer.android.com/develop/background-work/background-tasks/awake/screen-on).

1. Use a short Android screen timeout. With sleep timer Off, play across several
   track endings without touching the device: the visible app must stay awake.
2. Repeat with a sleep timer. The screen must stay awake through the final fade,
   then follow the normal system timeout; no subsequent track may start.
3. Pause and Stop, including from the notification. Check the screen-on flag is
   cleared and no transition wake lock remains. Resume: the flag returns.
4. Turn the screen off manually and separately send the app to the background.
   Test several natural completions, repeat, a trimmed ending and a failed next
   track. Playback must advance and the deadline must still stop it.
5. Reopen the app, navigate to collection/track/detail/settings pages, and rotate:
   screen behavior must follow playback, without a stale Activity subscription.
6. Deny audio focus, exhaust an invalid local queue and destroy the service.
   The app must permit screen timeout again. A hung native prepare must release
   the transition CPU lock within 30 seconds; inspect `dumpsys power`.
7. Interrupt playback with transient audio-focus loss, let it resume, then let
   the current track finish. The queue must continue to the following track.

These are required device checks, not a claim that they have all been completed.

Verification on 19 November 2026: 309 Core tests and Windows/Android Release
builds passed. A separate emulator-only Debug installation with two generated
silent audio fixtures passed native checks for idle, playing, automatic advance,
pause, resume, background advance, returning to the app, timer running/elapsed,
an exhausted invalid queue and Stop. Actual Activity flags were checked;
`dumpsys power` showed no active transition lock after Pause or Stop. The test
installation was removed afterward. These checks do not establish behavior
under a particular phone vendor's long-term battery management; manual screen
lock, long Doze intervals, orientation and focus interruptions remain physical
device acceptance checks.

### Deadline restoration

1. Set a sleep timer. Near expiry, seek close enough to the end of a track that its
   completion occurs during the three-second fade-out. Repeat in collection and
   single-track repeat modes. Playback must remain stopped after the deadline.
2. During the fade, change volume or a track preference. Neither a settings update
   nor rebuilding the queue may count as a new user request to start playback.
3. Exercise sticky-service recreation after a process death with a saved deadline
   that has elapsed before recreation. The restored track must stay paused. Also
   test recreation before the deadline, which may resume the saved playing session.
   Android **force-stop** is not an equivalent test: it deliberately prevents normal
   sticky-service recreation.
4. Explicitly start playback after expiry. The expired-state guard must no longer
   prevent that user action.

The automated deadline tests exercise the policy and restoration inputs, not the
native `MediaPlayer`, service lifecycle, audio focus, or device timer scheduling.

## Collection browsing and navigation

### Collection shuffle defaults

Collections whose complete catalog listening duration exceeds two hours default
to shuffle. Exactly two hours or less defaults to list order. Include playback
speed and trim offsets in that duration; downloading only part of a collection,
favorites-only filtering or blocking a track does not change its default.
The menu toggle saves an explicit choice for the active collection, or the
selected collection if no queue is active. Preserve explicit legacy global
choices until overridden for an individual collection; merely reading an
automatic default must not save it as a user choice. Catalog ordering is unchanged.

`CollectionShufflePreferenceTests` covers the exact boundary, combined duration,
speed/trims, independent persisted overrides and legacy choices. On 20 November
2026, all 320 Core tests and both Windows/Android Release builds passed.

1. Without a saved choice, start a long collection and check random selection of
   the first and subsequent tracks. Turn shuffle off, restart and revisit it:
   list order must remain selected. Other long collections keep their own defaults.
2. Start a shorter collection: list order is the default and can be overridden.
3. Browse collection B while A plays: A's order must remain active. Starting B
   must use B's own choice/default, including its initial-track selection.
4. Repeat with partial downloads, favorite/block filters, child-mode single-world
   startup, restored playback and Android notification/service restoration.

### Browsing and queue ownership

1. Play collection A, return to the collection list, and inspect collection B without
   pressing Play. Let A's current track end: its next track must still belong to A.
2. Repeat with an empty collection B. A must remain controllable and playing.
3. Explicitly start B and verify that the active queue switches to B.
4. Open a collection's track list and select a title, including a rapid double tap.
   The list must close before the immersive player opens. Back must return to the
   collection list, without a duplicate player or an unexpected track-list page.
5. Change a favorite/block preference while paused. Updating the queue must preserve
   the paused state, including while Android prepares a replacement player.

## Immersive player lifetime

1. Open and close a collection's immersive player at least 20 times.
2. With a debugger or profiler, verify that closed pages no longer receive `OnRefreshTick`
   calls and can be collected. The number of running refresh timers must not grow.
3. Open track details from the player, then return. Position, favorite state, and repeat
   mode must update again, with exactly one refresh timer for the visible player.
4. Navigate back to the collection list. Ambient animations must stop together with
   the player refresh timer; playback itself must continue.

## Playback speed and seeking

1. Play a catalog track whose `playbackSpeed` is `0.833333`.
2. Compare position and duration on the main player and immersive player: both must
   show listening time (media duration divided by speed).
3. Seek to one minute in the immersive player. Its position must stay around one
   minute rather than jumping back to 50 seconds.
4. Switch views and restart the app: the persisted listening position must use the
   same units. Repeat with a normal-speed track.

These are manual checks, not a record of completed device validation.

## Missing native duration

1. On the reporting Android device, refresh the Preview catalog and download the
   MP3 version of Recuerdos de la Alhambra. Verify approximately four minutes of
   duration, an advancing position slider, and seeking to 30 seconds and halfway.
2. With a test player reporting zero or -1 duration while its position advances,
   verify that catalog duration supplies the main player, immersive player and
   Android session metadata. Seek forward and backward; requests must retain
   their media position rather than being clamped to zero.
3. Repeat with nonzero start/end offsets and a slowed track. Display and seek
   positions must use listening time; duration fallback uses original media time.
4. Recreate the Android playback service with a saved queue. Duration metadata must
   survive; older saved queues without it must still restore built-in tracks.
5. Confirm that a later positive native duration takes precedence and that an
   untrimmed recording is not cut short solely by an estimated catalog length.

## Shared collection recordings

1. Refresh the catalog in an updated Stable or Preview app. Confirm Romanza,
   Recuerdos, Julia Florida and Clair de lune appear in both Quiet Classics and
   Lullabies for little ones; Preview Preselection must no longer list them.
2. Download one collection, then the other. Shared MP3s must be reused locally
   without another HTTP audio request. Repeat starting with existing Preview MP3s.
3. Delete either collection. Shared recordings in the other must remain playable
   offline. Cancel a reuse/download operation and check for leftover `.part` files.
4. Play consecutive shared recordings in Lullabies on Android. The queue and
   notification-opened player must stay in Lullabies rather than switching to
   Quiet Classics. Repeat in Quiet Classics. Browse another collection, restart
   the app and recreate the Android service: the playing collection must restore
   independently of the last browsed collection.
5. Romanza must skip its leading silence at 6400 ms; original media bytes and
   the visible listening timeline must retain the documented trim behavior.

These shared-collection device checks remain manual release validation.
