# open-sleep-music

Open Sleep Music is a privacy-friendly, advertising-free app for downloading and playing freely licensed sleep music and ambient sounds offline.

The project is at an early prototype stage. The desktop interface offers curated **sleep worlds**, resilient collection downloads, a validated local library, playback controls, seeking, automatic track changes, and a sleep timer. Individual unavailable or invalid downloads never stop the batch: desktop builds record technical details locally, while the interface continues with every usable track.

## Principles

- no advertising, tracking, accounts, or cloud requirement
- offline playback after downloading a collection
- only public-domain or explicitly compatible freely licensed recordings
- source, creator, and license metadata for every file
- resilient catalogs: HTTP errors, HTML responses, and invalid audio are skipped
- Android and desktop first; iOS and Mac Catalyst remain solution targets

## Included sleep worlds

- Quiet Classics (97 tracks, about 5.3 hours)
- Gentle Rain (34 tracks, about 5.4 hours)
- Forest (8 tracks, about 3 hours)
- Water & Waves (12 tracks, about 5.2 hours)
- Fireplace
- Brown Noise

The built-in catalog currently contains 153 entries and about 19 hours of audio. Its reviewed collections and license pages are documented in [docs/music-sources.md](docs/music-sources.md). The complete machine-readable catalog is embedded in the app and can be regenerated with `tools/Update-MediaCatalog.ps1`.

## Open in Visual Studio

Requirements:

- Visual Studio 2026 or a compatible Visual Studio release
- .NET 10 SDK
- .NET MAUI workload with the desired platform components
- Android 8.0 (API 26) or newer for Android playback

Open `OpenSleepMusic.sln`, select `OpenSleepMusic.App`, choose the Windows target, and run it. The downloaded files are stored below the current user's Music folder in `Open Sleep Music/<sleep-world>`.

Command-line validation:

```powershell
dotnet restore OpenSleepMusic.sln
dotnet test tests/OpenSleepMusic.Core.Tests/OpenSleepMusic.Core.Tests.csproj --configuration Release
dotnet build src/OpenSleepMusic.App/OpenSleepMusic.App.csproj --framework net10.0-windows10.0.19041.0 --configuration Release
```

## Repository structure

```text
src/OpenSleepMusic.App/         .NET MAUI user interface
src/OpenSleepMusic.Core/        catalog, validation, resilient downloads
tests/OpenSleepMusic.Core.Tests automated unit tests
docs/                           product and media-source documentation
.github/workflows/              CI, tests, and release publishing
```

## Licensing

The application source is available under the [MIT License](LICENSE). Downloaded recordings retain their individual licenses; the source code license does not relicense media files.
