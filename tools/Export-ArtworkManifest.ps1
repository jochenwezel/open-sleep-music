param(
    [string]$CatalogPath = (Join-Path $PSScriptRoot '..\src\OpenSleepMusic.Core\Catalog\media-catalog.json'),
    [string]$OutputPath = (Join-Path $PSScriptRoot '..\artifacts\artwork-catalog.json')
)

$ErrorActionPreference = 'Stop'
$catalog = Get-Content -Raw -LiteralPath $CatalogPath | ConvertFrom-Json
$tracks = @($catalog.sleepWorlds.tracks | ForEach-Object {
    [ordered]@{
        trackId = $_.id
        artworkUri = $_.artworkUri
        artworkFileName = $_.artworkFileName
        artworkSha256 = $_.artworkSha256
        songMotifUri = $_.songMotifUri
        songMotifFileName = $_.songMotifFileName
        songMotifSha256 = $_.songMotifSha256
        fallbackMotifUri = $_.fallbackMotifUri
        fallbackMotifFileName = $_.fallbackMotifFileName
        fallbackMotifSha256 = $_.fallbackMotifSha256
    }
})
$manifest = [ordered]@{ schemaVersion = 1; tracks = $tracks }
$directory = Split-Path -Parent $OutputPath
New-Item -ItemType Directory -Force -Path $directory | Out-Null
$manifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $OutputPath -Encoding utf8
Write-Host "Wrote $($tracks.Count) artwork entries to $OutputPath"
