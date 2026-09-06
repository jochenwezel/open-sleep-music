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
    'quiet-classics' = [ordered]@{ name = 'Ruhige Klassik'; description = 'Nocturnes, Mazurken und sanfte Klavierminiaturen von Frédéric Chopin.'; icon = '🎹'; tracks = [Collections.Generic.List[object]]::new() }
    rain = [ordered]@{ name = 'Sanfter Regen'; description = 'Leichter bis kräftiger Regen, ohne ausgewählte Gewitterspitzen.'; icon = '🌧️'; tracks = [Collections.Generic.List[object]]::new() }
    forest = [ordered]@{ name = 'Wald'; description = 'Lange Wald- und Regenwaldaufnahmen mit Wind, Wasser und Vögeln.'; icon = '🌲'; tracks = [Collections.Generic.List[object]]::new() }
    waves = [ordered]@{ name = 'Wasser & Wellen'; description = 'Meereswellen, Strand, Bachplätschern und gleichmäßige Wassergeräusche.'; icon = '🌊'; tracks = [Collections.Generic.List[object]]::new() }
    fireplace = [ordered]@{ name = 'Kaminfeuer'; description = 'Ruhiges Knistern eines Kaminfeuers.'; icon = '🔥'; tracks = [Collections.Generic.List[object]]::new() }
    'brown-noise' = [ordered]@{ name = 'Braunes Rauschen'; description = 'Tiefes, gleichmäßiges Rauschen ohne plötzliche Spitzen.'; icon = '🟤'; tracks = [Collections.Generic.List[object]]::new() }
}

$chopin = Get-ArchiveMetadata 'musopen-chopin-complete-works-flac'
$calmChopin = 'Nocturne|Mazurka|Berceuse|Cantabile|Largo|Albumleaf|Andantino|Cello Sonata.*III\. Largo'
$chopin.files |
    Where-Object { $_.name -match '\.mp3$' } |
    Where-Object { $_.name -match $calmChopin } |
    Sort-Object name |
    ForEach-Object { $worlds['quiet-classics'].tracks.Add((New-ArchiveTrack $chopin $_ 'Various artists; produced by Musopen')) }

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
Get-OriginalMp3 $fire | ForEach-Object { $worlds.fireplace.tracks.Add((New-ArchiveTrack $fire $_ 'inchadney (Freesound)')) }

function New-CommonsTrack(
    [string]$Id, [string]$Title, [string]$Creator, [string]$CommonsFile,
    [string]$SourcePage, [string]$License, [string]$LicenseUrl,
    [string]$FileName, [double]$DurationSeconds, [string]$Sha1 = $null) {
    [ordered]@{
        id = $Id; title = $Title; creator = $Creator
        downloadUri = "https://commons.wikimedia.org/wiki/Special:Redirect/file/$([Uri]::EscapeDataString($CommonsFile))"
        sourcePageUri = $SourcePage; license = $License; licenseUri = $LicenseUrl
        fileName = $FileName; durationSeconds = $DurationSeconds; sha1 = $Sha1
    }
}

$worlds['quiet-classics'].tracks.Add((New-CommonsTrack 'liszt-consolation-no-3' 'Franz Liszt – Consolation Nr. 3, Lento placido' 'Piano: Benedict Kramer; recording: Tobias Lohner' 'Franz Liszt - Consolation No. 3, Lento placido.ogg' 'https://commons.wikimedia.org/wiki/File:Franz_Liszt_-_Consolation_No._3,_Lento_placido.ogg' 'CC BY-SA 4.0' 'https://creativecommons.org/licenses/by-sa/4.0/' 'liszt-consolation-no-3.ogg' 226))
$worlds['quiet-classics'].tracks.Add((New-CommonsTrack 'liszt-consolation-no-5' 'Franz Liszt – Consolation Nr. 5' 'Piano: Constantin Stephan' 'Liszt-ConsolationNo5.ogg' 'https://commons.wikimedia.org/wiki/File:Liszt-ConsolationNo5.ogg' 'CC BY-SA 4.0' 'https://creativecommons.org/licenses/by-sa/4.0/' 'liszt-consolation-no-5.ogg' 177))
$worlds['quiet-classics'].tracks.Add((New-CommonsTrack 'liszt-romance-s-169' 'Franz Liszt – Romance S.169' 'Piano: Constantin Stephan' 'Franz Liszt, Romance S.169.ogg' 'https://commons.wikimedia.org/wiki/File:Franz_Liszt,_Romance_S.169.ogg' 'CC BY-SA 4.0' 'https://creativecommons.org/licenses/by-sa/4.0/' 'liszt-romance-s-169.ogg' 235))
$worlds['quiet-classics'].tracks.Add((New-CommonsTrack 'liszt-au-bord-d-une-source' "Franz Liszt – Au bord d'une source" 'Piano: Randolph Hokanson' 'Liszt- au bord d une.ogg' 'https://commons.wikimedia.org/wiki/File:Liszt-_au_bord_d_une.ogg' 'CC BY-SA 1.0' 'https://creativecommons.org/licenses/by-sa/1.0/' 'liszt-au-bord-d-une-source.ogg' 298.34 '9585bb47222ac319eded8fbcd4193364c6854113'))
$worlds.waves.tracks.Add((New-CommonsTrack 'lake-ontario-waves' 'Waves' 'Dsw4' 'Waves.ogg' 'https://commons.wikimedia.org/wiki/File:Waves.ogg' 'Public Domain' 'https://creativecommons.org/publicdomain/mark/1.0/' 'waves.ogg' 287))
$worlds['brown-noise'].tracks.Add((New-CommonsTrack 'brown-noise' 'Brownian noise' 'Kieff / LucasVB' 'Brownnoise.ogg' 'https://commons.wikimedia.org/wiki/File:Brownnoise.ogg' 'Public Domain (not copyrightable)' 'https://creativecommons.org/publicdomain/mark/1.0/' 'brown-noise.ogg' 10 '30994e449ca187db99e87c16f853af487ad16944'))

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
