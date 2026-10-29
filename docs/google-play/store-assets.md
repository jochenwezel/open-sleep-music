# Google Play assets and reviewer instructions

The [29 October 2026 asset set](../../design/google-play/2026-10-29/README.md)
contains the exported launcher icon, a generated feature graphic and real
Android 0.1.38 screenshots. They are saved in the Play Console draft: six
English phone screenshots and four German ones. The language-neutral icon and
feature graphic are shared. App artwork and generated illustrations are not
substitutes for actual interface screenshots. Review this snapshot against the
final delivered Play build before publication.

## Graphics checklist

- [x] Store icon: 512 by 512 pixels, 32-bit PNG, at most 1024 KB. Export the
  approved launcher artwork as a store asset; do not substitute a stock icon.
- [x] Feature graphic: 1024 by 500 pixels, JPEG or 24-bit PNG without alpha.
- [x] At least two screenshots meeting Google's current requirements: JPEG or
  24-bit PNG without alpha, dimensions 320 to 3840 pixels, with the longer side
  no more than twice the shorter side. Recheck the current Console specifications
  when capturing; an unusually tall native phone screenshot may need cropping.
- [x] Phone portrait overview showing actual Stable collections.
- [x] Lullaby player with its selected motif and visible controls.
- [ ] Quiet Classics player showing the collection-specific artwork difference.
- [x] Track list with favorite controls and the sleep-timer dialog.
- [ ] Optional landscape/tablet screenshots after actual layout validation.

Use the German interface for the German listing. Do not show unchecked audition
collections, private file paths, personal feedback, development overlays or
unavailable features. Avoid medical promises, rankings and unsupported claims.
Verify both final image dimensions and readability at phone size.
Source: [Google Play preview asset requirements](https://support.google.com/googleplay/android-developer/answer/9866151?hl=en-GB).

## App access draft

No app login, subscription or paid entitlement is required. An internet
connection is required to download initial audio and artwork. Open a collection,
download its available tracks, and start playback. Downloaded tracks remain
playable offline. Source failures are skipped individually rather than blocking
the entire collection. Reviewers should test the submitted Stable bundle.

## Foreground media playback declaration draft

Type: `mediaPlayback` / media playback.

The user selects a downloaded recording and starts playback. The foreground
service continues the selected audio when the screen is locked or another app
is foregrounded, exposes system media controls and applies the sleep timer.
Deferring or interrupting the service would interrupt the audio the user has
chosen to hear. Users can pause or stop playback through the app and its system
controls. Validate these actions on the final bundle, including issue 19.

Record a demonstration video showing selection and explicit Play, Home/screen
lock, continued audio and notification controls, then Stop and notification
cleanup. Supply its reviewer-accessible URL in Play Console after verifying the
recording. Do not claim this video already exists.
Source: [Google's foreground-service declaration requirements](https://support.google.com/googleplay/android-developer/answer/13392821?hl=en-en).
