# GitHub content delivery

All app-managed audio, background audio, images/motifs and runtime catalogs use
the project's GitHub delivery. Original source, licence and artist links remain
provenance for deliberate external browser visits, never automatic downloads or
fallback routes. No mirrored audio enters Git history or app packages; existing
approved project music publications remain unchanged.

## Publication and provenance

`tools/Publish-GitHubMedia.py` stages reviewed audio into ignored
`artifacts/github-media/<tag>/`. It checks complete upstream SHA-1 when present,
supported audio signatures and full-file SHA-1/SHA-256. Nothing is transcoded or
trimmed. Previously acquired copies require checksum/provenance receipts.
Acquisition tooling may contact original hosts; the app may not.

```powershell
python tools/Publish-GitHubMedia.py --tag media-v1
python tools/Publish-GitHubMedia.py --tag media-v1 --publish
./tools/Update-MediaCatalog.ps1
./tools/Test-GitHubDelivery.ps1
./tools/Test-CatalogOrdering.ps1
```

Prepare and inspect failures before publishing. The publisher creates a dedicated
media release, not an app release. It stages a draft, refuses mismatched existing
assets instead of overwriting, verifies every GitHub asset SHA-256, publishes the
complete set and probes every public audio URL. Only then does it write
`tools/github-media-delivery.json`. Reruns resume verified files; failures never
silently remove catalog tracks.

The registry and downloadable manifest retain IDs, filenames, original file and
source URLs, exact licences/links, creator credits, duration and checksums.
`originalDownloadUri` is provenance only. Existing recording-rights decisions
are reused; old Preview downloadability alone is not redistribution permission.
Uncleared or held recordings remain external-only references. NC/copyleft terms
remain independent of the code's MIT licence. Changed bytes need a new versioned
path; old pinned assets remain reachable.

The generator verifies upstream selections against the published mapping and
refuses missing/mismatched mirrors. Runtime candidate resources also use mirrors;
the maintainer candidate file retains upstream acquisition metadata. Stable still
excludes all three Preview-only collections. The catalog channel workflow checks
both channel suites and `Test-GitHubDelivery.ps1` before replacing the small
mutable manifests on `artwork-catalog`; payloads must exist first.

## Runtime enforcement

`GitHubContentDelivery` permits initial URLs only below this repository's Release
download path or raw URLs pinned to a 40-character commit. Embedded, cached and
remote media/artwork catalogs, collection-specific visual layers and downloadable
Preview candidates use the same policy. Source/licence metadata keep their actual
HTTPS origins. The transport never consumes them as a fallback.

The app's Core client disables cookies and automatic redirects. The helper follows
at most five redirects, checking HTTPS, normal port, absent credentials and exact
GitHub asset hosts before contacting the next destination. Foreign and HTTP
destinations are rejected. Standard TLS/hostname validation remains enabled.
Failures follow normal resilient download handling: log, clean partial files,
continue the collection and retain valid offline/embedded/cache fallbacks.

This applies to updated builds, not already installed historical executables.
Old clients can receive the new catalog through the usual channel update.

## Actual child/offline workflow

Ordinary operation refreshes the catalog on loading and fetches images as needed.
The optional [child mode](child-mode.md) adds a persisted parental gate: after
parents enable it, app-managed requests are blocked before transport, including
catalogs, artwork, audio and redirect hops. Locking cancels active requests and
closes response streams. Cached/embedded resources and local playback remain
available even when the device itself is connected. External links, feedback,
folder launching and destructive collection management are guarded separately.

Parents select the collections available to the child; only selected collections
with local tracks appear. The parent area requires the local PIN and relocks
when the app loses activation. By explicit product choice, child mode is opt-in;
the default remains full functionality. This is a local usage restriction, not
identity verification, a GitHub age exception, or control over OS/store traffic.
These controls require the updated app binary; they are not delivered by a
catalog update to historical installations.

## Verification on 31 October 2026

The published `media-v1` contains 167 audio files (166 Stable, one cleared Preview
audition), totaling 1,296,986,854 bytes, plus provenance manifest and README. All
GitHub asset digests and public audio signatures were verified, including an
idempotent publisher rerun. Real Core-client requests for audio, PNG and JSON
returned HTTP 200 through `release-assets.githubusercontent.com`.

Stable and Preview each passed 277 Core tests, including blocked initial hosts,
foreign/downgrade redirects, redirect timeout budgets and rejection of foreign
content in catalog updates. Windows and Android Release builds completed with
zero warnings/errors. Catalog order/delivery scripts passed; the media migration
retains IDs, files, source credits, licences and order. The coordinated 13-song
loudness trial changes only approved playback gains and two Preview background
levels; independent checks confirmed byte hashes and old-client acceptance.
This does not claim an Android-device redirect/certificate or child-mode test.
