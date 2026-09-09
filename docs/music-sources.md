# Reviewed media catalog

The built-in catalog contains 151 entries in six sleep worlds, totaling about 18.9 hours. The exact per-file download URL, source page, creator, license, duration, file name, and SHA-1 (when supplied upstream) are stored in `src/OpenSleepMusic.Core/Catalog/media-catalog.json`.

A composition being in the public domain does **not** automatically make a modern recording public domain. The catalog therefore uses recordings whose collection pages explicitly declare CC0 or public-domain status. It deliberately excludes tracks whose names indicate thunder or storms.

## Collection sources

| Sleep world | Entries | Approx. duration | Source collection | Declared license |
| --- | ---: | ---: | --- | --- |
| Quiet Classics | 86 | 4.75 h | [Musopen – Complete Works of Frédéric Chopin](https://archive.org/details/musopen-chopin-complete-works-flac) | CC0 1.0 |
| Quiet Classics | 4 | 15.6 min | [Liszt audio files on Wikimedia Commons](https://commons.wikimedia.org/wiki/Category:Audio_files_of_music_by_Franz_Liszt): Consolations 3 and 5, Romance S.169, Au bord d'une source | CC BY-SA 4.0 / 1.0 |
| Gentle Rain / Forest | 13 | 5.40 h | [Relaxing Rain Sounds](https://archive.org/details/relaxingrainsounds) | CC0 1.0 |
| Rain / Forest / Waves | 4 | 1.66 h | [Nature Sounds (Birds, Rain, Water)](https://archive.org/details/naturesounds-soundtheraphy) | CC0 1.0 |
| Gentle Rain | 28 | 2.64 h | [Rain Sounds, Gentle Rain, Thunderstorms](https://archive.org/details/rain-sounds-gentle-rain-thunderstorms) (only non-storm selections) | CC0 1.0 |
| Water & Waves | 10 | 4.70 h | [Ocean and Sea Sounds](https://archive.org/details/ocean-sea-sounds) (only non-storm selections) | CC0 1.0 |
| Fireplace | 1 | 4 min | [FireFavorite](https://archive.org/details/FireFavorite) / inchadney (Freesound); playback gain 6× compensates for the unusually quiet source without exceeding the player's full-volume ceiling | CC0 1.0 |
| Water & Waves | 1 | 4.8 min | [Waves by Dsw4](https://commons.wikimedia.org/wiki/File:Waves.ogg) | Public Domain |
| Lullabies for little ones | 6 | 12.8 min | [Fauré Berceuse](https://commons.wikimedia.org/wiki/File:Berceuse_by_Gabriel_Faur%C3%A9_op56_no1.ogg), [Burgmüller Berceuse](https://commons.wikimedia.org/wiki/File:Berceuse_Burgmuller.ogg), two [PDSounds music-box recordings](https://commons.wikimedia.org/wiki/File:Lullaby_wound_up_clock.ogg), [Another Lullaby](https://commons.wikimedia.org/wiki/File:Another_Lullaby_%28Antti_Luode%29.mp3), and [Ailsa's Lullaby](https://commons.wikimedia.org/wiki/File:Axle_-_02_-_Ailsas_Lullaby.ogg) | Public Domain / CC BY-SA 3.0 / CC BY 3.0 / CC BY 4.0 |

The figures above reflect the generated catalog and are rounded. The update script fetches Internet Archive metadata, selects the approved files, and records upstream SHA-1 values. The lullaby collection is an explicitly reviewed selection from Wikimedia Commons: each file page identifies the performer, recordist, or artist and the recording license. The two PDSounds files preserve the original record number and recordist in their Commons metadata. The selected files are instrumental and were checked for calm, uninterrupted playback without speech, advertising, applause, or abrupt high-impact passages. It does not download the audio during catalog generation.

## Rebuilding the catalog

```powershell
./tools/Update-MediaCatalog.ps1
dotnet test tests/OpenSleepMusic.Core.Tests/OpenSleepMusic.Core.Tests.csproj --configuration Release
```

Catalog tests require unique identifiers and filenames, HTTPS URLs, attribution and license metadata, positive durations, valid SHA-1 formatting, at least 140 entries and 15 hours of audio, and no storm/thunder titles.

## Catalog maintenance policy

1. Keep the source page, creator, recording license, license URL, and direct media URL together.
2. Accept only MP3, Ogg/Vorbis, FLAC, WAV, or MP4/M4A signatures after download; never trust a filename or HTTP `Content-Type` alone.
3. Download to a `.part` file and move it into the library only after validation.
4. Treat 404, other HTTP errors, timeouts, HTML bodies, and invalid media as an unavailable track, not a failed collection.
5. Desktop apps log skipped entries as JSON Lines in the app-data `logs/downloads.jsonl` file.
6. Replace broken URLs in a normal application/catalog update. A temporarily smaller library is acceptable.
