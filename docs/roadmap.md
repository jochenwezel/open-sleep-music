# Roadmap

## Prioritized follow-up: background playback

Reliable background playback is a required product capability, especially on phones. It follows the current local-library and player MVP work as a dedicated, platform-sensitive implementation stage.

The implementation must cover:

- continued Android playback with the screen locked or the app in the background;
- an Android foreground media service with a persistent media notification;
- lock-screen and system media controls for play, pause, previous, and next;
- correct audio-focus handling for calls, alarms, navigation, and competing media apps;
- resuming after transient interruptions without overriding an intentional user pause;
- equivalent Windows system media transport integration;
- restoration of the active sleep world, track, queue mode, and position after lifecycle interruption;
- real-device tests for Android standby, battery optimization, Bluetooth controls, wired headset controls, and process recreation.

Background playback must retain the project's privacy and offline guarantees. It must not introduce accounts, analytics, remote control services, or a permanent network requirement.
