# Open Sleep Music – contributor and agent guidance

This file applies to the entire repository. Preserve these rules when researching media, changing the catalog, implementing downloads, or preparing a release.

## Product intent

Open Sleep Music is a privacy-friendly, advertising-free application that makes freely licensed sleep music and ambient sound available offline. It must work without accounts, tracking, advertising, or a permanent network connection after media has been downloaded.

The app targets .NET MAUI with Windows and Android as the primary platforms. Keep the solution usable from Visual Studio and retain the iOS and Mac Catalyst targets unless a task explicitly changes the platform strategy.

## Media catalog admission policy

Every catalog entry must pass all of the following checks. A large catalog is less important than a trustworthy one.

### 1. Verify rights in the recording

- Verify the license of the **specific performance and recording**. A public-domain composition does not make a modern recording public domain.
- Prefer recordings released under CC0 or an unambiguous public-domain dedication.
- CC BY and CC BY-SA recordings are acceptable only when the source identifies the rights holder or performer and all required attribution is retained in the catalog.
- Do not admit material licensed as CC BY-NC, CC BY-ND, with custom non-commercial restrictions, or with unclear/incompatible terms.
- Do not infer a license from a search result, filename, collection name, or the general policy of a hosting portal. Open and review the authoritative item/file page.
- Treat implausible license claims cautiously. For example, an uploader applying CC0 to a recent commercial-label recording is not sufficient evidence that they own the rights. Exclude it unless independent provenance establishes the grant.
- Prefer Wikimedia Commons, Musopen-produced releases, Internet Archive items with credible provenance, and other sources that preserve stable rights metadata.
- Record the canonical source page, direct media URL, creator/performer, license name, and canonical license URL for every entry.

### 2. Check suitability for sleep

- Select calm, reasonably even recordings without abrupt loud passages, speech, alarms, advertising, applause, or intrusive transitions.
- Exclude thunder, storms, explosions, and other high-impact sounds from the built-in sleep catalog. Catalog tests enforce the obvious `thunder` and `storm` title cases, but human review is still required.
- For classical music, favor nocturnes, consolations, slow movements, gentle miniatures, and similarly quiet performances. Do not select a work solely because its title sounds calm.
- Avoid needless duplicates, excerpts that begin or end abruptly, and very low-quality or heavily distorted recordings.
- Assign each entry to the sleep world that best describes what the listener will hear.

### 3. Verify technical metadata and delivery

- Use HTTPS for the download URL, source page, and license URL.
- Direct download URLs must currently return actual audio. Probe at least the initial bytes and verify an MP3, Ogg/Vorbis, FLAC, WAV, MP4, or M4A signature; never trust only the extension or `Content-Type`.
- Store a positive duration and a unique, stable ID and local filename.
- Store the upstream SHA-1 checksum when the source provides one. Do not invent a checksum from a partial response.
- Prefer stable item/file URLs over temporary CDN, signed, session, or query-token URLs.
- Ensure filenames are portable across Windows, Android, iOS, and macOS and do not collide case-insensitively.

### 4. Preserve attribution and traceability

The machine-readable source of truth is `src/OpenSleepMusic.Core/Catalog/media-catalog.json`. Each track must retain:

- stable ID and display title;
- composer where applicable and performer/recordist or other credited creator;
- direct media URL and authoritative source-page URL;
- exact recording license and license URL;
- portable destination filename and duration;
- upstream checksum when available.

Also update `docs/music-sources.md` when adding or removing a source collection, changing licenses, or materially changing catalog totals. Do not claim that the project has independently guaranteed copyright status; document the source's declared license and the provenance checks performed.

## Catalog maintenance workflow

`tools/Update-MediaCatalog.ps1` is the reproducible catalog generator. Make selection changes in that script and regenerate the JSON instead of hand-editing generated entries.

After changing the catalog:

```powershell
./tools/Update-MediaCatalog.ps1
dotnet test tests/OpenSleepMusic.Core.Tests/OpenSleepMusic.Core.Tests.csproj --configuration Release
dotnet build src/OpenSleepMusic.App/OpenSleepMusic.App.csproj --framework net10.0-windows10.0.19041.0 --configuration Release
```

For platform-sensitive application changes, also build Android:

```powershell
dotnet build src/OpenSleepMusic.App/OpenSleepMusic.App.csproj --framework net10.0-android --configuration Release
```

Before committing, inspect the generated diff, run `git diff --check`, and confirm that catalog IDs, filenames, and download URLs remain unique. Do not lower the catalog size or duration test thresholds merely to make an accidental removal pass.

## Download resilience is a product requirement

A broken media source must never make an entire sleep-world download fail.

- Download into a `.part` file and move it into the library only after signature and optional checksum validation.
- Treat 404 responses, other HTTP failures, timeouts, HTML/error pages, invalid audio, and checksum mismatches as an unavailable individual track.
- Continue processing the remaining tracks and keep these failures unobtrusive in the user interface.
- Desktop builds must log the affected track, URL, status or validation reason, and timestamp to the existing JSON Lines download log.
- Clean up partial files after failures.
- A temporarily reduced playable duration is preferable to retaining corrupt or non-audio content. Repair stale URLs through a catalog/application update.

Do not weaken these behaviors when refactoring networking, validation, logging, or UI progress reporting.

## Code and repository quality

- Keep catalog parsing and download policy in `OpenSleepMusic.Core`; keep platform/UI concerns in `OpenSleepMusic.App`.
- Maintain nullable-reference-type correctness and the existing source-generated JSON model.
- Add or update tests for catalog invariants and downloader behavior when rules change.
- Never commit downloaded media, build output, secrets, temporary files, or local logs.
- Keep workflows green for both the dedicated tests and Windows build-and-test jobs. Release workflow changes must preserve reproducible artifacts and must not bundle third-party audio into the application package unless the repository policy explicitly changes.
- Preserve unrelated user changes in a dirty working tree and keep commits focused and reviewable.

## Documentation language

Code identifiers and repository-level technical documentation may remain in English. User-facing application text should be clear German unless localization support or the task calls for another language. Keep names, titles, creator credits, and official license names faithful to their sources.

## Branch cleanup

- After a pull request has been merged and all required pipelines have completed successfully, delete its feature branch both locally and on the remote. If either branch has already been deleted, clean up the remaining branch.
- Do not delete branches for open pull requests or branches whose required pipelines are still running or have failed.
