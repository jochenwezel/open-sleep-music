# Optional child mode

Child mode is opt-in. With no parental setup, Open Sleep Music retains its full
existing functionality. It neither infers who owns the device nor collects an
age, birthdate, account or biometric identifier.

## Family workflow

1. Adults download the desired collections, including artwork, in normal mode.
2. In the hamburger menu, choose **Set up child mode**. The setup explains what
   stays available and what is blocked before activation. Select the collections
   the child may use and set/confirm a 6–12 digit parent PIN.
3. Child mode shows only allowed collections containing downloaded tracks.
   Empty and non-allowed collections disappear without deleting their files.
   Playback, volume, timer, favorites and local track information remain usable.
   Partially downloaded collections can play their available tracks.
4. A fresh launch automatically opens/plays a collection only when exactly one
   allowed collection has downloads and that collection has playable tracks.
   Existing position is resumed when available. Multiple visible collections,
   an empty selection or a collection whose tracks are all blocked stay in the
   overview. The player's visible Back button returns to that overview; back
   navigation, resume and later mode changes do not repeat the startup action.
5. **Open parent area** asks for the PIN. The unlocked app has full access again.
   Its menu offers returning to child mode, changing the allowed collections,
   changing the PIN, or disabling protection. Leaving/deactivating the app
   relocks it; an unlocked session never survives process restart.

Downloads, catalog/image updates, external source/license/project links,
feedback, folder launching and collection repair/deletion are blocked while
locked. A visible label identifies child mode or an unlocked parent area. An
empty allowed/downloaded selection has an explanation pointing parents back to
the parent area. Restriction also applies to restored/current playback, Android
background playback and notification controls; locking stops a collection that
is not allowed. Background recordings remain dependencies of their main track.

The operating system and Google Play operate independently. This mode does not
revoke device networking or block other apps, backups, or store operations.
The parent PIN is a local gate, not verified legal consent or proof of adulthood.
No integration with Family Link or biometric prompt is required for this version.

## Implementation and persistence

- `ParentalControl` in Core owns the lock state, collection allowlist and request
  cancellation lifetime. `ChildModeStartup` consumes the startup decision once
  per process. New collections outside an explicit saved allowlist require adult
  selection; an empty list must never fall back to allowing everything.
- The app initializes the persisted enabled flag before constructing its HTTP
  client. `ParentalControlHandler` wraps the normal GitHub transport, rejects
  locked requests before sending and keeps cancellation active after response
  headers. Existing `.part` cleanup and cached/embedded fallbacks remain intact.
- Each unlock creates a fresh network lifetime; cancelling the previous lifetime
  cannot be undone by a later unlock. Every redirect goes through the handler.
- The enabled flag, selected collection IDs and failed-attempt/cooldown state are
  local MAUI preferences. Only a randomly salted PBKDF2-SHA256 verifier (100,000
  iterations, 16-byte salt, 32-byte hash) is saved in MAUI SecureStorage. The PIN
  itself is never persisted, logged or sent. Comparison uses fixed-time equality.
- Five failed PIN attempts impose a 30-second wait, persisted across restarts.
  Missing/unreadable secure credentials do not disable an enabled mode. Device
  backup/transfer can restore preferences without a readable secure credential;
  this stays locked and reports the issue. There is no remote recovery. Resetting
  app data removes local settings/downloads; setup states this before activation.
- Every external launch and final feedback handoff checks the current lock state,
  including dialogs that were opened before backgrounding. Android playback also
  checks collection access before opening/playing a saved item.

## Verification

Core tests cover opt-in defaults, restart/relock behavior, credentials and PIN
throttling, allowlist/empty selection, zero transport calls while locked, pending
header/body cancellation, cleanup/continuation after an interrupted audio transfer,
offline artwork/catalog fallbacks and one-time startup. On 31 October 2026, all
303 Core tests passed in Stable and all 303 in Preview. Final Windows Release
builds for Stable and Preview and the Android Stable Release build completed with
zero warnings and zero errors. `Test-GitHubDelivery.ps1` and `git diff --check`
passed. Subsequent delivery status is recorded in the application release notes.
The Windows development
app started successfully and exposed its normal collection UI; interactive setup
and device playback acceptance remain unverified because the desktop was locked
during the attempted UI inspection. Do not describe this as a completed manual
PIN/Android acceptance test.
Build both Windows and Android and run Stable/Preview Core tests after changes.
These tests do not claim a completed Google Play Families assessment or a device
backup/restore test.
