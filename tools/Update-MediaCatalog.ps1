param(
    [string]$OutputPath = (Join-Path $PSScriptRoot '..\src\OpenSleepMusic.Core\Catalog\media-catalog.json')
)

$ErrorActionPreference = 'Stop'
$headers = @{ 'User-Agent' = 'OpenSleepMusicCatalog/0.1 (+https://github.com/jochenwezel/open-sleep-music)' }
$cc0 = 'CC0 1.0'
$cc0Url = 'https://creativecommons.org/publicdomain/zero/1.0/'

function Get-ArchiveMetadata([string]$Identifier) {
    Invoke-RestMethod -Uri "https://archive.org/metadata/$Identifier" -Headers $headers
}

function Convert-ToSlug([string]$Value) {
    $normalized = $Value.Normalize([Text.NormalizationForm]::FormD)
    $withoutMarks = -join ($normalized.ToCharArray() | Where-Object {
        [Globalization.CharUnicodeInfo]::GetUnicodeCategory($_) -ne [Globalization.UnicodeCategory]::NonSpacingMark
    })
    (($withoutMarks.ToLowerInvariant() -replace '[^a-z0-9]+', '-').Trim('-'))
}

function Convert-ToFileName([string]$ItemId, [string]$RemoteName) {
    $extension = [IO.Path]::GetExtension($RemoteName).ToLowerInvariant()
    "$(Convert-ToSlug "$ItemId-$([IO.Path]::GetFileNameWithoutExtension($RemoteName))")$extension"
}

function New-ArchiveTrack($Metadata, $File, [string]$Creator) {
    $itemId = [string]$Metadata.metadata.identifier
    $remoteName = [string]$File.name
    [ordered]@{
        id = Convert-ToSlug "$itemId-$remoteName"
        title = [IO.Path]::GetFileNameWithoutExtension($remoteName).Trim()
        creator = $Creator
        downloadUri = "https://archive.org/download/$itemId/$([Uri]::EscapeDataString($remoteName))"
        sourcePageUri = "https://archive.org/details/$itemId"
        license = $cc0
        licenseUri = $cc0Url
        fileName = Convert-ToFileName $itemId $remoteName
        durationSeconds = Convert-ToSeconds ([string]$File.length)
        sha1 = if ($File.sha1) { [string]$File.sha1 } else { $null }
    }
}

function Get-OriginalMp3($Metadata) {
    @($Metadata.files | Where-Object { $_.name -match '\.mp3$' -and $_.source -eq 'original' })
}

function Convert-ToSeconds([string]$Length) {
    if ($Length -match '^(\d+):(\d+(?:\.\d+)?)$') {
        return [Math]::Round(([double]$Matches[1] * 60) + [double]$Matches[2], 2)
    }
    [Math]::Round([double]$Length, 2)
}

$worlds = [ordered]@{
    'quiet-classics' = [ordered]@{ name = 'Ruhige Klassik'; description = 'Sanfte Klavier-, Harfen-, Cello- und Streicheraufnahmen mit ruhiger Dynamik.'; icon = '🎼'; tracks = [Collections.Generic.List[object]]::new() }
    rain = [ordered]@{ name = 'Sanfter Regen'; description = 'Leichter bis kräftiger Regen, ohne ausgewählte Gewitterspitzen.'; icon = '🌧️'; tracks = [Collections.Generic.List[object]]::new() }
    forest = [ordered]@{ name = 'Wald'; description = 'Lange Wald- und Regenwaldaufnahmen mit Wind, Wasser und Vögeln.'; icon = '🌲'; tracks = [Collections.Generic.List[object]]::new() }
    waves = [ordered]@{ name = 'Wasser & Wellen'; description = 'Meereswellen, Strand, Bachplätschern und gleichmäßige Wassergeräusche.'; icon = '🌊'; tracks = [Collections.Generic.List[object]]::new() }
    fireplace = [ordered]@{ name = 'Kaminfeuer'; description = 'Ruhiges Knistern eines Kaminfeuers.'; icon = '🔥'; tracks = [Collections.Generic.List[object]]::new() }
    lullabies = [ordered]@{ name = 'Schlaflieder für Kleine'; description = 'Sanfte Klavier-, Spieluhr- und Instrumentalstücke ohne Gesang.'; icon = '🌙'; tracks = [Collections.Generic.List[object]]::new() }
}

$chopin = Get-ArchiveMetadata 'musopen-chopin-complete-works-flac'
$calmChopin = 'Nocturne|Mazurka|Berceuse|Cantabile|Largo|Albumleaf|Andantino|Cello Sonata.*III\. Largo'
$chopin.files |
    Where-Object { $_.name -match '\.mp3$' } |
    Where-Object { $_.name -match $calmChopin } |
    Sort-Object name |
    ForEach-Object {
        $track = New-ArchiveTrack $chopin $_ 'Various artists; produced by Musopen'
        if ($_.name -match 'Cello Sonata') {
            $track.instrumentation = @('cello', 'piano')
            $track.ensembleType = 'duo'
        } else {
            $track.instrumentation = @('piano')
            $track.ensembleType = 'solo'
        }
        $worlds['quiet-classics'].tracks.Add($track)
    }

$rainCollection = Get-ArchiveMetadata 'relaxingrainsounds'
Get-OriginalMp3 $rainCollection | Sort-Object name | ForEach-Object {
    if ($_.name -match 'Thunder|thunder|storm|Storm') { return }
    $target = if ($_.name -match 'Forest|Rainforest|Birds|Tropical Rain\.mp3') { 'forest' } else { 'rain' }
    $worlds[$target].tracks.Add((New-ArchiveTrack $rainCollection $_ 'Internet Archive contributor; CC0 collection'))
}

$natureCollection = Get-ArchiveMetadata 'naturesounds-soundtheraphy'
Get-OriginalMp3 $natureCollection | Sort-Object name | ForEach-Object {
    if ($_.name -match 'Thunder|thunder|storm|Storm') { return }
    $target = if ($_.name -match 'Ocean|Sea|Stream') { 'waves' } elseif ($_.name -match 'Bird') { 'forest' } else { 'rain' }
    $worlds[$target].tracks.Add((New-ArchiveTrack $natureCollection $_ 'Internet Archive contributor; CC0 collection'))
}

$oceanCollection = Get-ArchiveMetadata 'ocean-sea-sounds'
Get-OriginalMp3 $oceanCollection |
    Where-Object { $_.name -notmatch 'Thunder|thunder|storm|Storm' } |
    Sort-Object name |
    ForEach-Object { $worlds.waves.tracks.Add((New-ArchiveTrack $oceanCollection $_ 'Internet Archive contributor; CC0 collection')) }

$extraRain = Get-ArchiveMetadata 'rain-sounds-gentle-rain-thunderstorms'
Get-OriginalMp3 $extraRain |
    Where-Object {
        $_.name -notmatch 'Thunder|thunder|storm|Storm|Birds in the Rain Part 1|Light Gentle Rain Part 1|Rain Sounds, Gentle Rain, Thunderstorms'
    } |
    Sort-Object name |
    ForEach-Object { $worlds.rain.tracks.Add((New-ArchiveTrack $extraRain $_ 'cl0udn0te and credited source recordists')) }

$fire = Get-ArchiveMetadata 'FireFavorite'
Get-OriginalMp3 $fire | ForEach-Object {
    $track = New-ArchiveTrack $fire $_ 'inchadney (Freesound)'
    $track['volumeGain'] = 6.0
    $worlds.fireplace.tracks.Add($track)
}

function New-CommonsTrack(
    [string]$Id, [string]$Title, [string]$Creator, [string]$CommonsFile,
    [string]$SourcePage, [string]$License, [string]$LicenseUrl,
    [string]$FileName, [double]$DurationSeconds, [string]$Sha1 = $null,
    [string]$DownloadUri = $null) {
    [ordered]@{
        id = $Id; title = $Title; creator = $Creator
        downloadUri = if ($DownloadUri) { $DownloadUri } else { "https://commons.wikimedia.org/wiki/Special:Redirect/file/$([Uri]::EscapeDataString($CommonsFile))" }
        sourcePageUri = $SourcePage; license = $License; licenseUri = $LicenseUrl
        fileName = $FileName; durationSeconds = $DurationSeconds
        sha1 = if ($Sha1) { $Sha1 } else { $null }
    }
}

$worlds['quiet-classics'].tracks.Add((New-CommonsTrack 'liszt-consolation-no-3' 'Franz Liszt – Consolation Nr. 3, Lento placido' 'Piano: Benedict Kramer; recording: Tobias Lohner' 'Franz Liszt - Consolation No. 3, Lento placido.ogg' 'https://commons.wikimedia.org/wiki/File:Franz_Liszt_-_Consolation_No._3,_Lento_placido.ogg' 'CC BY-SA 4.0' 'https://creativecommons.org/licenses/by-sa/4.0/' 'liszt-consolation-no-3.ogg' 226))
$worlds['quiet-classics'].tracks.Add((New-CommonsTrack 'liszt-consolation-no-5' 'Franz Liszt – Consolation Nr. 5' 'Piano: Constantin Stephan' 'Liszt-ConsolationNo5.ogg' 'https://commons.wikimedia.org/wiki/File:Liszt-ConsolationNo5.ogg' 'CC BY-SA 4.0' 'https://creativecommons.org/licenses/by-sa/4.0/' 'liszt-consolation-no-5.ogg' 177))
$worlds['quiet-classics'].tracks.Add((New-CommonsTrack 'liszt-romance-s-169' 'Franz Liszt – Romance S.169' 'Piano: Constantin Stephan' 'Franz Liszt, Romance S.169.ogg' 'https://commons.wikimedia.org/wiki/File:Franz_Liszt,_Romance_S.169.ogg' 'CC BY-SA 4.0' 'https://creativecommons.org/licenses/by-sa/4.0/' 'liszt-romance-s-169.ogg' 235))
$worlds['quiet-classics'].tracks.Add((New-CommonsTrack 'liszt-au-bord-d-une-source' "Franz Liszt – Au bord d'une source" 'Piano: Randolph Hokanson' 'Liszt- au bord d une.ogg' 'https://commons.wikimedia.org/wiki/File:Liszt-_au_bord_d_une.ogg' 'CC BY-SA 1.0' 'https://creativecommons.org/licenses/by-sa/1.0/' 'liszt-au-bord-d-une-source.ogg' 298.34 '9585bb47222ac319eded8fbcd4193364c6854113'))
$worlds['quiet-classics'].tracks | Where-Object { $_.id -like 'liszt-*' } | ForEach-Object {
    $_.instrumentation = @('piano')
    $_.ensembleType = 'solo'
}

$harpPilot = New-CommonsTrack 'frank-schroeter-medieval-dream' 'Frank Schröter – Medieval Dream' 'Frank Schröter' 'Medieval Dream by Frank Schröter.ogg' 'https://commons.wikimedia.org/wiki/File:Medieval_Dream_by_Frank_Schr%C3%B6ter.ogg' 'CC BY 4.0' 'https://creativecommons.org/licenses/by/4.0/' 'frank-schroeter-medieval-dream.ogg' 140.527 '336b920b9c4df4cee00d829d50f338a8d6c6fee1'
$harpPilot.instrumentation = @('harp', 'recorder')
$harpPilot.ensembleType = 'duo'
$worlds['quiet-classics'].tracks.Add($harpPilot)

$haydnCello = New-CommonsTrack 'haydn-cello-concerto-no-1-adagio' 'Joseph Haydn – Cellokonzert Nr. 1: II. Adagio' 'Metropolitan Chamber Orchestra' "The Metropolitan Chamber Orchestra - Haydn's Cello Concerto No. 1 in C major, Hob.VIIb-1 - II. Adagio.ogg" 'https://commons.wikimedia.org/wiki/File:The_Metropolitan_Chamber_Orchestra_-_Haydn%27s_Cello_Concerto_No._1_in_C_major,_Hob.VIIb-1_-_II._Adagio.ogg' 'Public Domain Dedication' 'https://web.archive.org/web/20230926203737/https://creativecommons.org/licenses/publicdomain/' 'haydn-cello-concerto-no-1-adagio.ogg' 500.12 '19da8d41b3d8b902a27478e8681942750261ef18'
$haydnCello.instrumentation = @('cello', 'strings')
$haydnCello.ensembleType = 'chamber-orchestra'
$worlds['quiet-classics'].tracks.Add($haydnCello)

$bachCelloSarabande = New-CommonsTrack 'bach-cello-suite-1-sarabande-john-michel' 'Johann Sebastian Bach – Cellosuite Nr. 1: Sarabande' 'Cello: John Michel' 'JOHN MICHEL CELLO-J S BACH CELLO SUITE 1 in G Sarabande.ogg' 'https://commons.wikimedia.org/wiki/File:JOHN_MICHEL_CELLO-J_S_BACH_CELLO_SUITE_1_in_G_Sarabande.ogg' 'CC BY-SA 3.0' 'https://creativecommons.org/licenses/by-sa/3.0/' 'bach-cello-suite-1-sarabande-john-michel.ogg' 123.82040816326531 '99a0efd629203283480cde7ef11b53c94e7726d2'
$bachCelloSarabande.instrumentation = @('cello')
$bachCelloSarabande.ensembleType = 'solo'
$worlds['quiet-classics'].tracks.Add($bachCelloSarabande)

$schubertOctetAdagio = New-CommonsTrack 'schubert-octet-d803-adagio' 'Franz Schubert – Oktett D 803: II. Adagio' 'Monica Huggett, Rob Diggins, Vicki Gunn, Sarah Freiberg, Curtis Daily, William McColl, R. J. Kelley, Charles Kaufman' 'Franz Schubert - Octet - 2. Adagio.ogg' 'https://commons.wikimedia.org/wiki/File:Franz_Schubert_-_Octet_-_2._Adagio.ogg' 'CC BY-SA 2.0' 'https://creativecommons.org/licenses/by-sa/2.0/' 'schubert-octet-d803-adagio.ogg' 661.599977324263 '3091872e878d5bd930c811972bf1e8d1ef90c822'
$schubertOctetAdagio.instrumentation = @('violin', 'viola', 'cello', 'double-bass', 'clarinet', 'horn', 'bassoon')
$schubertOctetAdagio.ensembleType = 'octet'
$worlds['quiet-classics'].tracks.Add($schubertOctetAdagio)
$worlds.waves.tracks.Add((New-CommonsTrack 'lake-ontario-waves' 'Waves' 'Dsw4' 'Waves.ogg' 'https://commons.wikimedia.org/wiki/File:Waves.ogg' 'Public Domain' 'https://creativecommons.org/publicdomain/mark/1.0/' 'waves.ogg' 287))
$worlds.lullabies.tracks.Add((New-CommonsTrack 'faure-berceuse-op-56-no-1' 'Gabriel Fauré – Berceuse op. 56 Nr. 1' 'Piano: Brian M. Jones' 'Berceuse by Gabriel Fauré op56 no1.ogg' 'https://commons.wikimedia.org/wiki/File:Berceuse_by_Gabriel_Faur%C3%A9_op56_no1.ogg' 'CC BY 3.0' 'https://creativecommons.org/licenses/by/3.0/' 'faure-berceuse-op-56-no-1.ogg' 214.622 '16f742aed631bbd30a78e3298fb77672655bf306'))
$worlds.lullabies.tracks.Add((New-CommonsTrack 'burgmuller-berceuse-op-109-no-7' 'Friedrich Burgmüller – Berceuse op. 109 Nr. 7' 'Piano: BastienM' 'Berceuse Burgmuller.ogg' 'https://commons.wikimedia.org/wiki/File:Berceuse_Burgmuller.ogg' 'CC BY-SA 3.0' 'https://creativecommons.org/licenses/by-sa/3.0/' 'burgmuller-berceuse-op-109-no-7.ogg' 84.578 '2701c3802dae189dd59b4980aa363a1d467a9007'))
$worlds.lullabies.tracks.Add((New-CommonsTrack 'music-box-schlafe-mein-prinzchen' 'Spieluhr – Schlafe, mein Prinzchen' 'Recordist: stephan (PDSounds)' 'Lullaby wound up clock.ogg' 'https://commons.wikimedia.org/wiki/File:Lullaby_wound_up_clock.ogg' 'Public Domain' 'https://creativecommons.org/publicdomain/mark/1.0/' 'music-box-schlafe-mein-prinzchen.mp3' 85.238 $null 'https://upload.wikimedia.org/wikipedia/commons/transcoded/e/e4/Lullaby_wound_up_clock.ogg/Lullaby_wound_up_clock.ogg.mp3'))
$worlds.lullabies.tracks.Add((New-CommonsTrack 'music-box-guten-abend-gute-nacht' 'Spieluhr – Guten Abend, gute Nacht' 'Recordist: stephan (PDSounds)' 'Lullaby wound up clock guten abend gute nacht.ogg' 'https://commons.wikimedia.org/wiki/File:Lullaby_wound_up_clock_guten_abend_gute_nacht.ogg' 'Public Domain' 'https://creativecommons.org/publicdomain/mark/1.0/' 'music-box-guten-abend-gute-nacht.mp3' 46.446 $null 'https://upload.wikimedia.org/wikipedia/commons/transcoded/c/cf/Lullaby_wound_up_clock_guten_abend_gute_nacht.ogg/Lullaby_wound_up_clock_guten_abend_gute_nacht.ogg.mp3'))
$worlds.lullabies.tracks.Add((New-CommonsTrack 'antti-luode-another-lullaby' 'Antti Luode – Another Lullaby' 'Antti Luode' 'Another Lullaby (Antti Luode).mp3' 'https://commons.wikimedia.org/wiki/File:Another_Lullaby_(Antti_Luode).mp3' 'CC BY 3.0' 'https://creativecommons.org/licenses/by/3.0/' 'antti-luode-another-lullaby.mp3' 139.337 'a32d3ce29635b421981ba72347e4082ca497c09a'))
$worlds.lullabies.tracks | ForEach-Object {
    if ($_.id -like 'music-box-*') { $_.instrumentation = @('music-box'); $_.ensembleType = 'solo' }
    elseif ($_.id -in @('faure-berceuse-op-56-no-1', 'burgmuller-berceuse-op-109-no-7')) { $_.instrumentation = @('piano'); $_.ensembleType = 'solo' }
    else { $_.instrumentation = @('instrumental'); $_.ensembleType = 'solo' }
}
foreach ($track in $worlds.lullabies.tracks) { $track.playbackSpeed = [Math]::Round(1 / 1.2, 6) }

$artworkReleaseBase = 'https://github.com/jochenwezel/open-sleep-music/releases/download/artwork-v2'
$legacyArtworkReleaseBase = 'https://github.com/jochenwezel/open-sleep-music/releases/download/artwork-v1'
$artwork = @{
    'quiet-classics' = @{ file = 'background_quiet_classics.png'; sha256 = '0711d561294bf5085be5c2b70ab4cde9ca8750cec4aa98568b2932cf08453a2e' }
    rain = @{ file = 'background_rain.png'; sha256 = '7d5811077c43d119d45751adc8f269debd47a0a5aabc839ed7b323fb763afc35' }
    forest = @{ file = 'background_forest.png'; sha256 = '9c242462d21698f8baa298062ba1747803d306f30b589780211e3a982b173454' }
    waves = @{ file = 'background_waves.png'; sha256 = 'ba1f932cef14fffa1b49ae34c86e8f858fa2303cb362a74e9d341a85e0917bc9' }
    fireplace = @{ file = 'background_fireplace.png'; sha256 = 'e084796e82d373a37a262262a8e0dc6db2cda8a063f8a5bb6259792b557891fb' }
    lullabies = @{ file = 'background_lullabies.png'; sha256 = '71360dc46ea8da5895ed37f1e044ad5019151bedd692f87e27b2aede9e179c36' }
}
$fallbackArtwork = @{
    'quiet-classics' = @{ file = 'fallback_quiet_classics.png'; sha256 = '35b4ef983fc4b067ecfb20359036665482a770b0cc2d8eef40d38727e257b05d' }
    rain = @{ file = 'fallback_rain.png'; sha256 = '90763629c0dfdabce6302992eb94306d9ef36b78afecf9a914d480c1cad18620' }
    forest = @{ file = 'fallback_forest.png'; sha256 = 'cd9d764c9412b52a9f47e2c094c7f8e3ea1f300dc0e2e3e2d2279b91b82d9328' }
    waves = @{ file = 'fallback_waves.png'; sha256 = '764e5eec2b6b53f72f24b00065042ae8b205a9ad94d6e69e4bfb193aa398b65f' }
    fireplace = @{ file = 'fallback_fireplace.png'; sha256 = '5f61496b481ec977cb30999b98f4251af43193dbef011f5d41393eed06ae1271' }
    lullabies = @{ file = 'fallback_lullabies.png'; sha256 = '24d629f9d802f91f76642f52557edd58c59a58ab65c3778fa7465d587283fd67' }
}
$forestArtwork = @{
    'relaxingrainsounds-a-tropical-rain-forest-1-mp3' = @{ file = 'forest_tropical_rain_forest_1.png'; sha256 = '449d3b1c1038d89bf1926d99ad6a03697cd0ad1284fe88422a4bd392187aa630' }
    'relaxingrainsounds-a-tropical-rain-forest-2-mp3' = @{ file = 'forest_tropical_rain_forest_2.png'; sha256 = '606dc39ef6512eb7bbfc7932ad1e39313e5b755502907f29ef84788c22051882' }
    'relaxingrainsounds-birds-in-the-rain-part-1-mp3' = @{ file = 'forest_birds_in_rain_1.png'; sha256 = '5d47cc5e4d9f4646859c9c1c05ed3b459c1a4102e9d67c07e44651f50974ecd8' }
    'relaxingrainsounds-birds-in-the-rain-part-2-mp3' = @{ file = 'forest_birds_in_rain_2.png'; sha256 = '71f578bb76b9d6841690c7a48c524db0ed2ac2c1421e66b262fecdfd7261b080' }
    'relaxingrainsounds-rainforest-part-1-mp3' = @{ file = 'forest_rainforest_1.png'; sha256 = 'e988f3cb74dcb97acaa96cabfdfb64598d322940fe4bd42137ec48ab284a00b6' }
    'relaxingrainsounds-rainforest-part-2-mp3' = @{ file = 'forest_rainforest_2.png'; sha256 = 'f8d54346e253688484bb8aa08554baa25758c52469e68967c20a168fd520433b' }
    'relaxingrainsounds-tropical-rain-mp3' = @{ file = 'forest_tropical_rain.png'; sha256 = 'bc651f9b18aae7bdf0836fc8b1c49253c133a613843581a70f6d5c47284a65cf' }
    'naturesounds-soundtheraphy-relaxing-nature-sounds-birdsong-sound-mp3' = @{ file = 'forest_birdsong.png'; sha256 = 'f1a6a78271a643c763fe98506ef2d7ceebe2e0b1bb8b7617baf7eb5923ebabaa' }
}
$lullabyArtwork = @{
    'faure-berceuse-op-56-no-1' = @{ file = 'motif_lullaby_sleepy_child.png'; sha256 = '19b304a437287e6ce4cfab313225237bd2623438777cf905f311a0c27cf5aa55' }
    'burgmuller-berceuse-op-109-no-7' = @{ file = 'motif_lullaby_vine.png'; sha256 = '5daba8633f8e450ad70969a85fc969d70c9686686bcba4228f3b6d81a99cdc31' }
    'music-box-schlafe-mein-prinzchen' = @{ file = 'sleepy_bear_moon.png'; sha256 = '12039a2dbc713d6281fb4bc6c171cc7821843aec587a2c16d5c948e513f0a455' }
    'music-box-guten-abend-gute-nacht' = @{ file = 'sleepy_lamb_star.png'; sha256 = 'dda54da1b91f317ab83061af55ccf00e1b161c1fc367fe8e626181e563b41b06' }
    'antti-luode-another-lullaby' = @{ file = 'motif_lullaby_boat.png'; sha256 = '5f12ded5d87122b8714eb88d29c35ee3082a5b9554545a131d019c4a4a26216f' }
}
foreach ($worldEntry in $worlds.GetEnumerator()) {
    foreach ($track in $worldEntry.Value.tracks) {
        $trackArtwork = $artwork[$worldEntry.Key]
        $track['artworkUri'] = "$artworkReleaseBase/$($trackArtwork.file)"
        $track['artworkFileName'] = $trackArtwork.file
        $track['artworkSha256'] = $trackArtwork.sha256
        $fallbackMotif = $fallbackArtwork[$worldEntry.Key]
        $track['fallbackMotifUri'] = "$artworkReleaseBase/$($fallbackMotif.file)"
        $track['fallbackMotifFileName'] = $fallbackMotif.file
        $track['fallbackMotifSha256'] = $fallbackMotif.sha256
        if ($worldEntry.Key -eq 'forest' -and $forestArtwork.ContainsKey($track.id)) {
            $songMotif = $forestArtwork[$track.id]
            $track['songMotifUri'] = "$artworkReleaseBase/$($songMotif.file)"
            $track['songMotifFileName'] = $songMotif.file
            $track['songMotifSha256'] = $songMotif.sha256
        }
        if ($worldEntry.Key -eq 'lullabies' -and $lullabyArtwork.ContainsKey($track.id)) {
            $songMotif = $lullabyArtwork[$track.id]
            $track['songMotifUri'] = "$legacyArtworkReleaseBase/$($songMotif.file)"
            $track['songMotifFileName'] = $songMotif.file
            $track['songMotifSha256'] = $songMotif.sha256
        }
    }
}

$manifestWorlds = foreach ($entry in $worlds.GetEnumerator()) {
    [ordered]@{
        id = $entry.Key
        name = $entry.Value.name
        description = $entry.Value.description
        icon = $entry.Value.icon
        tracks = @($entry.Value.tracks)
    }
}

$manifest = [ordered]@{
    schemaVersion = 1
    generatedAtUtc = [DateTimeOffset]::UtcNow.ToString('O')
    sleepWorlds = @($manifestWorlds)
}

$json = $manifest | ConvertTo-Json -Depth 8
[IO.File]::WriteAllText((Resolve-Path (Split-Path $OutputPath)).Path + '\' + (Split-Path $OutputPath -Leaf), $json + [Environment]::NewLine, [Text.UTF8Encoding]::new($false))

$tracks = @($manifestWorlds | ForEach-Object { @($_.tracks) })
$seconds = ($tracks | ForEach-Object { [double]$_['durationSeconds'] } | Measure-Object -Sum).Sum
$hours = $seconds / 3600
Write-Host "Wrote $($tracks.Count) tracks across $($manifestWorlds.Count) sleep worlds ($([Math]::Round($hours, 2)) hours) to $OutputPath"
