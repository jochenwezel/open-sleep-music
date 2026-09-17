# Playback regression checks

These device checks complement the automated Core tests. Run them on Windows and Android.

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
