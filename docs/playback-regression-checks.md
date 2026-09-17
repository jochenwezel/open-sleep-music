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

These are manual checks, not a record of completed device validation.
