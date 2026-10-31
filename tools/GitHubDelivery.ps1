# Runtime delivery is independent of original source/rights evidence.
$script:GitHubDeliveryRegistry = $null
function Get-GitHubDeliveryRegistry {
    if (!$script:GitHubDeliveryRegistry) {
        $path = Join-Path $PSScriptRoot 'github-media-delivery.json'
        $manifest = Get-Content -LiteralPath $path -Raw -Encoding utf8 | ConvertFrom-Json -AsHashtable
        if ($manifest.schemaVersion -ne 1) { throw 'Unsupported GitHub media delivery schema.' }
        $script:GitHubDeliveryRegistry = @{}
        foreach ($entry in $manifest.tracks) {
            if ($script:GitHubDeliveryRegistry.ContainsKey($entry.id)) { throw "Duplicate mirror ID: $($entry.id)" }
            $script:GitHubDeliveryRegistry[$entry.id] = $entry
        }
    }
    return $script:GitHubDeliveryRegistry
}

function Set-GitHubTrackDelivery($Track) {
    $registry = Get-GitHubDeliveryRegistry
    $entry = $registry[$Track.id]
    $original = if ($Track.originalDownloadUri) { [string]$Track.originalDownloadUri } else { [string]$Track.downloadUri }
    if (!$entry -or $entry.originalDownloadUri -cne $original -or $entry.fileName -cne $Track.fileName -or
        $entry.license -cne $Track.license -or $entry.licenseUri -cne [string]$Track.licenseUri -or
        $entry.licenseReviewStatus -ne 'verified' -or
        $entry.sha1 -notmatch '^[a-f0-9]{40}$' -or $entry.sha256 -notmatch '^[a-f0-9]{64}$' -or $entry.bytes -lt 128 -or
        ($Track.sha1 -and $Track.sha1 -ne $entry.sha1) -or
        $entry.downloadUri -notmatch '^https://github\.com/jochenwezel/open-sleep-music/releases/download/media-[a-z0-9.-]+/[^/?#]+$') {
        throw "Missing or mismatched verified GitHub mirror for '$($Track.id)'. Publish exact bytes first; never fall back to the original host."
    }
    $Track['originalDownloadUri'] = $original
    $Track['downloadUri'] = $entry.downloadUri
    $Track['sha1'] = $entry.sha1
}
