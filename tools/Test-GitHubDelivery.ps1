param(
    [string]$CatalogDirectory = (Join-Path $PSScriptRoot '../src/OpenSleepMusic.Core/Catalog'),
    [string[]]$ManifestPaths = @()
)
$ErrorActionPreference = 'Stop'
function Assert-ContentUri([string]$Value, [string]$Location) {
    if ($Value -notmatch '^https://github\.com/jochenwezel/open-sleep-music/releases/download/[^/?#]+/[^?#]+$' -and
        $Value -notmatch '^https://raw\.githubusercontent\.com/jochenwezel/open-sleep-music/[a-f0-9]{40}/[^?#]+$') {
        throw "Non-GitHub runtime content at ${Location}: $Value"
    }
}
function Test-Node($Node, [string]$Location) {
    if ($Node -is [Collections.IDictionary]) {
        foreach ($key in $Node.Keys) {
            $value = $Node[$key]
            if ($key -in @('downloadUri','artworkUri','songMotifUri','fallbackMotifUri') -and $null -ne $value) {
                Assert-ContentUri ([string]$value) "$Location.$key"
            }
            Test-Node $value "$Location.$key"
        }
    } elseif ($Node -is [Collections.IEnumerable] -and $Node -isnot [string]) {
        foreach ($value in $Node) { Test-Node $value $Location }
    }
}
if (!$ManifestPaths.Count) {
    $ManifestPaths = @('media-catalog.json','media-catalog.production.json','preselection-candidates.json') | ForEach-Object { Join-Path $CatalogDirectory $_ }
}
foreach ($path in $ManifestPaths) {
    $document = Get-Content -LiteralPath $path -Raw -Encoding utf8 | ConvertFrom-Json -AsHashtable
    Test-Node $document (Split-Path -Leaf $path)
}
Write-Host 'All embedded/runtime catalog content URLs use project GitHub delivery; provenance links are preserved separately.'
