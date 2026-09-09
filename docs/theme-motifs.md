# Motifs for the built-in sleep worlds

The five built-in collections use small vector motifs drawn at runtime. No third-party images,
fonts, or animation files are bundled, so there are no additional licenses or attribution
requirements. The renderer adds only a few kilobytes to the package and does not allocate
bitmaps or require the network.

| Sleep world | Motif | Motion |
| --- | --- | --- |
| Quiet classics | A moonlit, open music box | The shared night sky changes softly |
| Gentle rain | A rounded cloud and five raindrops | Drops drift slowly |
| Forest | Three simple fir trees and a moon | The shared night sky changes softly |
| Water & waves | A moon over two broad waves | Waves move a few pixels |
| Fireplace | A rounded hearth, logs, and two flame layers | Flame shapes and colors blend over about three seconds |

The starry background moves between blue and dark blue over 15 seconds per color phase. Colors
stay within a narrow, dark range; there are no flashes, abrupt transitions, or high-frequency
movement. Windows, Android, iOS, and Mac Catalyst system settings for reduced motion disable the
timer and leave a calm static frame.

## Relationship to issue #7

Issue #7 defines the future general-purpose theme and song-motif system. This implementation is
deliberately isolated in `SleepWorldMotifView`: cards bind only the stable sleep-world ID, while
rendering, timing, and motion preferences remain behind the control. The future system can replace
the drawable or supply its profiles without changing catalog entries or the card layout. There is
no source or runtime dependency on #7, but #7 should preserve the world IDs and reduced-motion
contract established here.

The motifs scale from their available bounds and contain no orientation-specific coordinates.
They therefore retain their proportions when the existing responsive layout switches between
one-column portrait and two-column landscape arrangements.
