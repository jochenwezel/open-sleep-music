# Reviewed media catalog

The stable catalog contains 134 downloadable entries in six sleep worlds, totaling about 18.1 hours. Preview adds 50 downloadable audition candidates (about 3.65 hours) and 20 external listening references in Preselection. The exact per-file download URL, source page, creator, recording license, duration, instrumentation, ensemble type, file name, and SHA-1 (when supplied upstream) are stored in `src/OpenSleepMusic.Core/Catalog/media-catalog.json`.

A composition being in the public domain does **not** automatically make a modern recording public domain. The catalog therefore uses recordings whose collection pages explicitly declare CC0 or public-domain status. It deliberately excludes tracks whose names indicate thunder or storms.

## Collection sources

| Sleep world | Entries | Approx. duration | Source collection | Declared license |
| --- | ---: | ---: | --- | --- |
| Quiet Classics | 83 | 4.62 h | [Musopen – Complete Works of Frédéric Chopin](https://archive.org/details/musopen-chopin-complete-works-flac) | CC0 1.0 |
| Quiet Classics | 3 | 10.6 min | [Liszt audio files on Wikimedia Commons](https://commons.wikimedia.org/wiki/Category:Audio_files_of_music_by_Franz_Liszt): Consolations 3 and 5, Romance S.169 | CC BY-SA 4.0 |
| Quiet Classics | 1 | 8.3 min | [Haydn Cello Concerto No. 1 – II. Adagio](https://commons.wikimedia.org/wiki/File:The_Metropolitan_Chamber_Orchestra_-_Haydn%27s_Cello_Concerto_No._1_in_C_major,_Hob.VIIb-1_-_II._Adagio.ogg): Metropolitan Chamber Orchestra | Public Domain Dedication |
| Quiet Classics | 1 | 2.1 min | [Bach Cello Suite No. 1 – Sarabande](https://commons.wikimedia.org/wiki/File:JOHN_MICHEL_CELLO-J_S_BACH_CELLO_SUITE_1_in_G_Sarabande.ogg): John Michel, cello | CC BY-SA 3.0 |
| Quiet Classics | 1 | 11.0 min | [Schubert Octet D 803 – II. Adagio](https://commons.wikimedia.org/wiki/File:Franz_Schubert_-_Octet_-_2._Adagio.ogg): Monica Huggett ensemble; Wikipedia Featured Sound | CC BY-SA 2.0 |
| Gentle Rain / Forest | 12 | 4.99 h | [Relaxing Rain Sounds](https://archive.org/details/relaxingrainsounds) | CC0 1.0 |
| Rain / Forest / Waves | 4 | 1.66 h | [Nature Sounds (Birds, Rain, Water)](https://archive.org/details/naturesounds-soundtheraphy) | CC0 1.0 |
| Gentle Rain | 14 | 1.78 h | [Rain Sounds, Gentle Rain, Thunderstorms](https://archive.org/details/rain-sounds-gentle-rain-thunderstorms) (only non-storm selections) | CC0 1.0 |
| Water & Waves | 9 | 4.25 h | [Ocean and Sea Sounds](https://archive.org/details/ocean-sea-sounds) (only non-storm selections) | CC0 1.0 |
| Fireplace | 1 | 4 min | [FireFavorite](https://archive.org/details/FireFavorite) / inchadney (Freesound); playback gain 6× compensates for the unusually quiet source without exceeding the player's full-volume ceiling | CC0 1.0 |
| Water & Waves | 1 | 4.8 min | [Waves by Dsw4](https://commons.wikimedia.org/wiki/File:Waves.ogg) | Public Domain |
| Lullabies for little ones | 4 | 7.2 min | [Fauré Berceuse](https://commons.wikimedia.org/wiki/File:Berceuse_by_Gabriel_Faur%C3%A9_op56_no1.ogg), [Burgmüller Berceuse](https://commons.wikimedia.org/wiki/File:Berceuse_Burgmuller.ogg), two [PDSounds music-box recordings](https://commons.wikimedia.org/wiki/File:Lullaby_wound_up_clock.ogg) | Public Domain / CC BY-SA 3.0 / CC BY 3.0 |

The figures above reflect the generated catalog and are rounded. The update script fetches Internet Archive metadata, selects the approved files, and records upstream SHA-1 values. Instrument families and ensemble types are explicit catalog metadata so future curation can measure actual timbral diversity instead of inferring it from titles. The retained diversity selections include solo cello, cello with strings, and chamber-orchestra textures; expansion remains deliberately incremental because a nominally slow movement may still contain unsuitable dynamic peaks. The lullaby collection is an explicitly reviewed selection from Wikimedia Commons: each file page identifies the performer, recordist, or artist and the recording license. The two PDSounds files preserve the original record number and recordist in their Commons metadata. Their originals contain an Ogg Skeleton stream before the Vorbis stream, which Android's platform player rejects on some devices; the catalog therefore uses Wikimedia's stable MP3 transcodes of the same recordings. The selected files are instrumental and were checked for calm, uninterrupted playback without speech, advertising, applause, or abrupt high-impact passages. It does not download the audio during catalog generation.

## Rebuilding the catalog

```powershell
./tools/Update-MediaCatalog.ps1
dotnet test tests/OpenSleepMusic.Core.Tests/OpenSleepMusic.Core.Tests.csproj --configuration Release
```

Catalog tests require unique identifiers and filenames, HTTPS URLs, attribution and license metadata, positive durations, valid SHA-1 formatting, at least 134 entries and 15 hours of audio, and no storm/thunder titles.

## Catalog maintenance policy

1. Keep the source page, creator, recording license, license URL, and direct media URL together.
2. Accept only MP3, Ogg/Vorbis, FLAC, WAV, or MP4/M4A signatures after download; never trust a filename or HTTP `Content-Type` alone.
3. Download to a `.part` file and move it into the library only after validation.
4. Treat 404, other HTTP errors, timeouts, HTML bodies, and invalid media as an unavailable track, not a failed collection.
5. Desktop apps log skipped entries as JSON Lines in the app-data `logs/downloads.jsonl` file.
6. Replace broken URLs in a normal application/catalog update. A temporarily smaller library is acceptable.


## Persistent curation blacklist

The remaining Chopin mazurkas provisionally play at two-thirds speed (150% listening time) for another listening review. Their original recording durations remain in the catalog metadata and source totals above. This tempo adjustment is not a final suitability approval; further unsuitable recordings will be blacklisted after review.

On 2026-10-02, the maintainer requested removal of all 20 blocked track IDs from field feedback: five Quiet Classics recordings, fourteen Gentle Rain recordings, and one lullaby. Their IDs are retained in tools/media-catalog-blacklist.json; the generator excludes them from every source. Catalog tests prevent their reintroduction. The raw report is not committed. The harp/recorder pilot and Another Lullaby are no longer included. The minimum count is now 134 to reflect this deliberate removal; the 15-hour duration floor remains unchanged.

## Preselection review collection

Preview releases include a separate **Vorauswahl (Preselection)** research collection with 70 instrumental recording candidates. Stable releases contain only the six production worlds; they never include Preselection. Its source of truth is `tools/preselection-candidates.json`; `tools/Update-MediaCatalog.ps1` generates the embedded `src/OpenSleepMusic.Core/Catalog/preselection-candidates.json`. The first 50 now have stable header-checked audio delivery and are ordinary downloadable `AudioTrack` entries in the Preview-only Preselection world. Twenty further candidates remain source-page references without suitable download delivery. Source file existence and metadata were checked through the Wikimedia Commons API on 2026-10-02. Tempo/title/instrument metadata guided the preliminary selection; complete listening reviews have not yet been performed. The original 50-candidate selection has 33 piano candidates, 9 guitar candidates, 2 harp candidates, 1 pan-flute candidate, 1 flute/harp/orchestra candidate, 1 saxophone/piano candidate, 2 string-quartet candidates, and 1 wind-ensemble candidate provisionally tagged orchestra. Instrument tags are intentionally broad and provisional.

Every candidate has an explicit `licenseReviewStatus` (`unchecked`, `verified`, or `rejected`). All 70 start as `unchecked`; upstream declared license metadata is only a research lead and does not establish permission for the specific recording. This distinction matters where composition and recording licensing differ. Preview downloads the 50 delivery-ready candidates on request through the normal validated `.part` pipeline and plays them offline; it supports favorites/blocking and includes ratings under world ID `preselection` in the explicit feedback export. A secondary references button opens the 20 remaining artist-hosted pages. Feedback never transmits automatically. Hosting pages are external services with their own policies and require a network connection.

A recording may enter a production sleep world only after a documented recording-rights review, an accepted `approvedLicense`, its HTTPS `approvedLicenseUri`, and an HTTPS `licenseEvidenceUri` are recorded. The preview Core promotion guard and the catalog generator reject candidates lacking those checks. The generator matches candidate IDs **and source pages**, so renaming a candidate ID does not bypass the check. Full sleep-suitability review, stable direct audio delivery, signatures, positive duration, upstream checksums when provided, and attribution remain required for production admission. The persistent blacklist also applies to candidate IDs. Existing mazurkas remain in Quiet Classics at the requested speed for listening review.

The Wienerwald zither excerpt still has an unidentified performer and insufficiently established recording provenance. The initial harp-ensemble leads contained only 30-second excerpts; neither was used to pad the 50-candidate selection. The requested older familiar instrumental direction remains the curation goal, and the preview feedback will refine it.

A further requested melody is probably Carl Loewe's Die Uhr, op. 123 no. 3, with text by Johann Gabriel Seidl ([publisher identification](https://www.schott-music.com/en/die-uhr-noc42612.html)). A gentle instrumental recording with verifiable compatible recording rights has not yet been found; it remains a research candidate and is not part of the downloadable catalog.

## Preview-only packaging

Stable builds are the default, including ordinary Release builds. Use `-p:OpenSleepMusicPreview=true` only for preview artifacts. The GitHub release workflow passes the actual `release.prerelease` boolean to both Windows and Android publishing. Tag names do not determine the channel.

The generator also writes `src/OpenSleepMusic.Core/Catalog/media-catalog.production.json`, which excludes the Preselection world. Stable builds embed this production manifest under the normal catalog resource name, exclude `preselection-candidates.json`, and exclude the Preselection page from compilation. Preview builds embed the full catalog and candidate resource. Resource-level tests and the CI build/test matrices cover both values of the flag. The curation registry remains in the repository so generation can enforce licensing checks for candidates promoted to production, even when their IDs are renamed.

```powershell
dotnet test tests/OpenSleepMusic.Core.Tests/OpenSleepMusic.Core.Tests.csproj --configuration Release -p:OpenSleepMusicPreview=false
dotnet test tests/OpenSleepMusic.Core.Tests/OpenSleepMusic.Core.Tests.csproj --configuration Release -p:OpenSleepMusicPreview=true
```
## Lullaby audition expansion (2026-10-02)

Twenty additional source-page candidates target the Lullabies for little ones world, bringing Preview Preselection to 70 candidates. The production lullaby world still contains four reviewed downloads. `intendedWorldId: lullabies` records the future destination; `selectionKind` separates nine traditional cradle-song/bedtime melodies, one gentle arrangement (Ode to Joy, not an original lullaby), and ten modern lullaby or gentle piano leads. This is an audition pool, not a promise that twenty recordings will pass review.

Traditional leads include Guter Mond, Sandman Is Here, Hush Little Baby, Rock-a-bye Baby, Sleep Baby Sleep, Brahms' cradle song, Twinkle Twinkle Little Star, The Moon Has Risen and Schubert D 498. Modern leads include five tracks from [Wonderful Piano Lullabies Vol. 1](https://wonderfullullabies.bandcamp.com/album/wonderful-piano-lullabies-vol-1-2), four longer piano pieces from [Little Lullabies for Little People](https://thebackgroundmusicians.bandcamp.com/album/little-lullabies-for-little-people), and Peder B. Helland's [Bedtime Lullaby (Radio Edit)](https://soothingrelaxation.com/products/bedtime-lullaby-radio-edit-single). Individual artist-hosted track pages and public player metadata were checked; very short interludes and repeated hour-long versions were not selected. All listening assessments remain preliminary, based on the artists' instrumentation and bedtime descriptions; full listening, vocal absence, level changes and transitions have not been verified. Instrument tags are provisional, including music-box and synthesized orchestral accompaniment.

All twenty retain `licenseReviewStatus: unchecked`. Bandcamp sources declare all rights reserved; the Helland storefront sells personal listening and offers separate project licensing. Neither establishes a compatible redistribution grant. Schubert's modern piano arrangement has separate arrangement and recording rights to check. These pages are directional listening references only, accessed in the external browser. No media is downloaded or redistributed, and no candidate is promoted to production. The existing promotion and stable-package exclusions remain in force. Artist pages may impose streaming limits or offer only a preview.

## Downloadable Preview audition collection (2026-10-03)

The build channel, not a manifest flag or title, controls admission of Preselection. Preview uses the normal collection download, validation, library, playback and feedback flows; Stable packages exclude the world, candidate resource and references page. Dynamic catalog handling must enforce the same channel restriction so a remote catalog cannot enable this collection in a Stable app. Production promotion guards remain mandatory outside the Preselection world.

`tools/Resolve-PreselectionDownloads.ps1` resolves Commons item metadata and probes 128 audio bytes without storing media. It records HTTPS direct delivery, duration, original SHA-1 when available and the check timestamp in the candidate source. Sources beginning with Ogg Skeleton use the stable Commons MP3 transcode instead, without reusing the original checksum. Oga originals are saved under portable .ogg destination names. The generator materializes only candidates carrying this delivery evidence. The fifty technical delivery checks do not establish recording rights or sleep suitability; `licenseReviewStatus` remains unchecked and track details explicitly say the review is pending. The twenty artist-page references are preserved rather than treating expiring streaming endpoints or purchase offers as offline download grants. No media is bundled with app packages.
