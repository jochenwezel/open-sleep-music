# Playback regression checks

These device checks complement the automated Core tests. Run them on Windows and Android.

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
