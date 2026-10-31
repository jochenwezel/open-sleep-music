# Roadmap

## Background playback

Reliable background playback is a required product capability, especially on phones. The first Android implementation is part of the 0.1.0 preview.

Implemented:

- continued Android playback with the screen locked or the app in the background;
- an Android foreground media service with a persistent media notification;
- lock-screen and system media controls for play, pause, previous, and next;
- correct audio-focus handling for calls, alarms, navigation, and competing media apps;
- resuming after transient interruptions without overriding an intentional user pause;
- pausing when wired or Bluetooth audio becomes unavailable;
- Windows system media transport integration through the MAUI Community Toolkit media element;
- restoration of the active track, queue mode, approximate position, volume, and timer after an Android service restart;
- a sleep timer owned by the Android playback service, so it also expires with the screen locked.

Release acceptance still requires real-device tests for Android standby, battery optimization, phone-call interruption, Bluetooth controls, wired headset controls, process recreation, and rotation during playback, downloads, and sleep-timer operation. Results should be recorded in the release notes or a follow-up issue; emulator/build success does not replace these checks.

The 0.1.41 Preview also needs interactive child-mode acceptance on Android and
Windows: PIN setup/unlock/change/disable, automatic relock, collection selection
(including empty and partially downloaded libraries), offline behavior on an
online device, and fresh-launch autoplay/back navigation for a single collection.
Core tests cover the policies and cancellation/cleanup behavior; manual PIN and
device-playback checks were blocked by the locked workstation. See
[child mode](child-mode.md). This is a Preview limitation, not completed Store
Families or production acceptance.

Background playback must retain the project's privacy and offline guarantees. It must not introduce accounts, analytics, remote control services, or a permanent network requirement.
