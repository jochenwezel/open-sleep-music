$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'CatalogOrdering.ps1')

function Assert-Order([string[]]$Actual, [string[]]$Expected) {
    if (($Actual -join '|') -cne ($Expected -join '|')) {
        throw "Unexpected order: $($Actual -join ',') (expected $($Expected -join ','))"
    }
}

function Assert-Rejected([scriptblock]$Action) {
    $rejected = $false
    try { & $Action | Out-Null } catch { $rejected = $true }
    if (!$rejected) { throw 'Invalid order input was accepted.' }
}

# Rebuilding preserves existing order even when input enumeration changes.
Assert-Order @(Update-CatalogTrackOrder -ExistingOrder @('c','a','b') -TrackIds @('a','b','c') `
    -ChoosePosition { throw 'An unchanged collection must not draw random positions.' }) @('c','a','b')

# A new title can occupy every position while retaining the old relative order.
foreach ($slot in 0..3) {
    $choose = { param($Limit) $slot }.GetNewClosure()
    $expected = [Collections.Generic.List[string]]::new([string[]]@('a','b','c'))
    $expected.Insert($slot, 'new')
    Assert-Order @(Update-CatalogTrackOrder -ExistingOrder @('a','b','c') -TrackIds @('a','b','c','new') `
        -ChoosePosition $choose) $expected.ToArray()
}

# Removing a title does not rearrange survivors; multiple newcomers are inserted.
Assert-Order @(Update-CatalogTrackOrder -ExistingOrder @('c','a','removed','b') -TrackIds @('a','b','c','new-1','new-2') `
    -ChoosePosition { param($Limit) 1 }) @('c','new-2','new-1','a','b')
Assert-Order @(Update-CatalogTrackOrder -TrackIds @('a','b','c') -ChoosePosition { param($Limit) 0 }) @('c','b','a')
Assert-Order @(Update-CatalogTrackOrder -ExistingOrder @('removed') -TrackIds @()) @()

Assert-Rejected { Update-CatalogTrackOrder -ExistingOrder @('a','a') -TrackIds @('a') }
Assert-Rejected { Update-CatalogTrackOrder -TrackIds @('a','a') }
Assert-Rejected { Update-CatalogTrackOrder -TrackIds @('a',' ') }
Assert-Rejected { Update-CatalogTrackOrder -TrackIds @('a') -ChoosePosition { -1 } }
Assert-Rejected { Update-CatalogTrackOrder -TrackIds @('a') -ChoosePosition { param($Limit) $Limit } }
Assert-Rejected { Update-CatalogTrackOrder -TrackIds @('a') -ChoosePosition { 0.5 } }
Write-Host 'Catalog ordering checks passed.'
