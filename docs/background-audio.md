# Per-track background audio

An optional secondary recording can loop underneath a primary song. Assign it in
`tools/track-backgrounds.json`, keyed by the primary song's stable ID:

```json
{
  "primary-track-id": {
    "trackId": "existing-ambient-track-id",
    "volume": 0.2,
    "startOffsetMilliseconds": 1000,
    "endOffsetMilliseconds": 2000
  }
}
```

Run `tools/Update-MediaCatalog.ps1` afterwards. Romanza española and Recuerdos de
la Alhambra currently use the meadow crickets at 0.5 volume for Preview audition,
in both Quiet Classics and Lullabies for little ones.

The referenced recording must already satisfy the catalog's recording-rights and
delivery policy. Nested backgrounds and self-references are rejected. Set the
configuration-only `previewOnly: true` flag for an audition assignment; the generator
removes that assignment from Stable and does not emit the flag into runtime metadata.
Only Preview may reference a recording in Preselection from a regular collection,
and its recording-rights status, approved license and audit must already pass the
production rights checks. Unchecked candidates remain forbidden. Stable continues
to reject all such references. Volume is a factor of the app volume,
not the main track's gain. Both trims use milliseconds of the original recording
at 100% speed; they must leave a positive interval. Background playback always
uses normal speed, even when the primary song is stretched.

Collection download and repair include the background file in the same collection
directory, using the ordinary validated download pipeline. It is not added to the
visible song list. Failure of a background download or playback is unobtrusive and
does not prevent the primary recording from playing. Existing downloads remain
unmodified, with their original checksums and attribution.

Pause, resume, audio-focus ducking and sleep-timer fades affect both players. The
background loops its trimmed interval until the main song ends. It fades out during
the final 600 ms of the primary song's playable, speed-adjusted duration and reaches
silence at the same endpoint, without delaying the next song or single-song repeat.
For primary songs shorter than 600 ms, the fade spans their entire playable duration.
Zero or invalid duration yields zero background volume, without negative intervals.
Explicit stop releases both players.
Seeking/resuming positions the background modulo its trimmed interval. Android
persists its configuration with the playback queue and uses the existing foreground
service, audio focus and notification rather than a second media session.
