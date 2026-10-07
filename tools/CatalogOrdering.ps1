function Update-CatalogTrackOrder {
    param(
        [string[]]$ExistingOrder = @(),
        [Parameter(Mandatory)][AllowEmptyCollection()][string[]]$TrackIds,
        [scriptblock]$ChoosePosition = { param($Limit) Get-Random -Minimum 0 -Maximum $Limit }
    )

    if ($null -eq $ExistingOrder) { $ExistingOrder = @() }
    if (@($ExistingOrder | Sort-Object -Unique).Count -ne $ExistingOrder.Count -or
        @($TrackIds | Sort-Object -Unique).Count -ne $TrackIds.Count -or
        @(@($ExistingOrder) + @($TrackIds) | Where-Object { [string]::IsNullOrWhiteSpace($_) }).Count) {
        throw 'Catalog order IDs must be nonempty and unique.'
    }

    $result = [Collections.Generic.List[string]]::new()
    foreach ($id in $ExistingOrder) {
        if ($id -in $TrackIds) { $result.Add($id) }
    }
    foreach ($id in $TrackIds) {
        if ($result.Contains($id)) { continue }
        # Initial insertion produces a random permutation. Later additions preserve
        # the relative order of existing tracks and may occupy any position.
        $position = & $ChoosePosition ($result.Count + 1)
        if ($position -isnot [int] -or $position -lt 0 -or $position -gt $result.Count) {
            throw 'Catalog insertion position must be an integer within the collection.'
        }
        $result.Insert($position, $id)
    }
    return $result.ToArray()
}
