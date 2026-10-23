param([string]$MusicRoot = (Join-Path $PSScriptRoot '../music'))

$ErrorActionPreference = 'Stop'
$resolvedRoot = [IO.Path]::GetFullPath($MusicRoot)
$songFiles = @(Get-ChildItem -LiteralPath $resolvedRoot -Filter song.json -Recurse -File)
if ($songFiles.Count -eq 0) { throw 'No music publication manifests found.' }
$ids = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
$downloadUris = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)

function Resolve-SongFile([string]$SongRoot, [string]$RelativePath) {
    $path = [IO.Path]::GetFullPath((Join-Path $SongRoot $RelativePath))
    if (-not $path.StartsWith($SongRoot + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Publication path escapes song directory: $RelativePath"
    }
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "Missing publication file: $path" }
    return $path
}

foreach ($manifest in $songFiles) {
    $songRoot = $manifest.Directory.FullName
    $song = Get-Content -LiteralPath $manifest.FullName -Raw | ConvertFrom-Json
    if (-not $ids.Add($song.id)) { throw "Duplicate song ID: $($song.id)" }
    if ($song.id -ne $manifest.Directory.Name) { throw 'Song ID must match its directory.' }
    foreach ($uri in @($song.licenseUri, $song.sourcePageUri, $song.archiveDownloadUri)) {
        if ($uri -notmatch '^https://') { throw "Expected HTTPS source/license/download URI: $uri" }
    }
    foreach ($name in @('README.md', 'checksums.sha256', $song.sourceEvidencePath)) {
        Resolve-SongFile $songRoot $name | Out-Null
    }
    foreach ($folder in @('audio', 'midi', 'sources', 'licenses')) {
        if (-not (Test-Path -LiteralPath (Join-Path $songRoot $folder) -PathType Container)) {
            throw "Missing publication folder: $folder"
        }
    }
    $listed = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    foreach ($line in Get-Content -LiteralPath (Join-Path $songRoot 'checksums.sha256')) {
        if ($line -notmatch '^([0-9a-f]{64})  (.+)$') { throw 'Invalid SHA-256 manifest line.' }
        $expected = $Matches[1]; $relative = $Matches[2]
        if (-not $listed.Add($relative)) { throw "Duplicate checksum path: $relative" }
        $path = Resolve-SongFile $songRoot $relative
        if ((Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant() -ne $expected) {
            throw "Published file hash mismatch: $relative"
        }
    }
    $files = @(Get-ChildItem -LiteralPath $songRoot -File -Recurse | Where-Object Name -NE 'checksums.sha256')
    if ($files.Count -ne $listed.Count) { throw 'Checksum list must cover every publication file exactly once.' }
    $variantIds = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    $variantPaths = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    foreach ($variant in $song.variants) {
        if (-not $variantIds.Add($variant.id)) { throw 'Duplicate variant ID.' }
        if ($variant.durationSeconds -le 0 -or $variant.tempoFactor -le 0) { throw 'Invalid duration or tempo.' }
        if ($variant.downloadUri -notmatch '^https://github\.com/[^/]+/[^/]+/releases/download/music-[^/]+/[^/]+\.mp3$') {
            throw 'Use a fixed-version music release MP3 URL.'
        }
        if (-not $downloadUris.Add($variant.downloadUri)) { throw 'Duplicate audio download URL.' }
        foreach ($kind in @('audio', 'midi')) {
            $relative = $variant.($kind + 'Path')
            if (-not $variantPaths.Add($relative)) { throw "Variant file reused: $relative" }
            $path = Resolve-SongFile $songRoot $relative
            $bytes = [IO.File]::ReadAllBytes($path)
            $expected = $variant.($kind + 'Sha256')
            if ((Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant() -ne $expected) {
                throw "Variant hash mismatch: $relative"
            }
            if ($kind -eq 'midi' -and [Text.Encoding]::ASCII.GetString($bytes, 0, 4) -ne 'MThd') { throw 'Invalid MIDI header.' }
            if ($kind -eq 'audio') {
                if ([Text.Encoding]::ASCII.GetString($bytes, 0, 3) -ne 'ID3' -and -not ($bytes[0] -eq 255 -and ($bytes[1] -band 224) -eq 224)) {
                    throw 'Invalid MP3 signature.'
                }
                if ($bytes.Length -ne $variant.audioBytes) { throw 'Audio byte length mismatch.' }
            }
        }
    }
    $audioCount = @(Get-ChildItem -LiteralPath (Join-Path $songRoot 'audio') -File).Count
    $midiCount = @(Get-ChildItem -LiteralPath (Join-Path $songRoot 'midi') -File).Count
    if ($audioCount -ne $song.variants.Count -or $midiCount -ne $song.variants.Count) {
        throw 'Keep exactly the selected MP3/MIDI variants in their publication folders.'
    }
    Write-Host "Verified $($song.id): $($song.variants.Count) variants, $($listed.Count) checksummed files."
}
