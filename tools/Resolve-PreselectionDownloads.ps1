param([string]$CandidatePath = (Join-Path $PSScriptRoot 'preselection-candidates.json'))
$ErrorActionPreference = 'Stop'
$manifest = Get-Content -LiteralPath $CandidatePath -Raw | ConvertFrom-Json
$client = [Net.Http.HttpClient]::new()
$client.DefaultRequestHeaders.UserAgent.ParseAdd('OpenSleepMusicResearch/0.1 (+https://github.com/jochenwezel/open-sleep-music)')
function Test-AudioHeader([string]$Url) {
    Start-Sleep -Seconds 2
    $request = [Net.Http.HttpRequestMessage]::new([Net.Http.HttpMethod]::Get, $Url)
    $request.Headers.Range = [Net.Http.Headers.RangeHeaderValue]::new(0,127)
    $response = $client.SendAsync($request, [Net.Http.HttpCompletionOption]::ResponseHeadersRead).GetAwaiter().GetResult()
    if ([int]$response.StatusCode -eq 429) {
        $response.Dispose()
        Start-Sleep -Seconds 30
        $request.Dispose()
        $request = [Net.Http.HttpRequestMessage]::new([Net.Http.HttpMethod]::Get, $Url)
        $request.Headers.Range = [Net.Http.Headers.RangeHeaderValue]::new(0,127)
        $response = $client.SendAsync($request, [Net.Http.HttpCompletionOption]::ResponseHeadersRead).GetAwaiter().GetResult()
    }
    try {
        $response.EnsureSuccessStatusCode() | Out-Null
        $stream = $response.Content.ReadAsStreamAsync().GetAwaiter().GetResult()
        try {
            $bytes = [byte[]]::new(128)
            $count = $stream.ReadAtLeast($bytes, 128, $false)
            $ascii = [Text.Encoding]::ASCII.GetString($bytes, 0, $count)
            return $count -ge 12 -and (($ascii.StartsWith('OggS') -and !$ascii.Contains('fishead')) -or
                $ascii.StartsWith('ID3') -or ($bytes[0] -eq 255 -and ($bytes[1] -band 224) -eq 224) -or
                $ascii.StartsWith('fLaC') -or ($ascii.StartsWith('RIFF') -and $ascii.Substring(8,4) -eq 'WAVE'))
        } finally { $stream.Dispose() }
    } finally { $response.Dispose(); $request.Dispose() }
}
try {
    $candidates = @($manifest.candidates | Where-Object {
        $_.sourcePageUri -like 'https://commons.wikimedia.org/wiki/File:*' -and
        !($_.downloadUri -and $_.deliveryCheckedAtUtc)
    })
    if (!$candidates.Count) { return }
    $titles = $candidates | ForEach-Object { [Uri]::UnescapeDataString(([Uri]$_.sourcePageUri).AbsolutePath.Substring(6)).Replace('_',' ') }
    $api = 'https://commons.wikimedia.org/w/api.php?action=query&format=json&prop=imageinfo&iiprop=url%7Csize%7Csha1%7Cextmetadata&titles=' + [Uri]::EscapeDataString(($titles -join '|'))
    $metadata = $client.GetStringAsync($api).GetAwaiter().GetResult() | ConvertFrom-Json
    foreach ($candidate in $candidates) {
        if ($candidate.downloadUri -and $candidate.deliveryCheckedAtUtc) { continue }
        $title = [Uri]::UnescapeDataString(([Uri]$candidate.sourcePageUri).AbsolutePath.Substring(6)).Replace('_',' ')
        $page = $metadata.query.pages.PSObject.Properties.Value | Where-Object { $_.title.Replace('_',' ') -eq $title }
        $info = $page.imageinfo[0]
        if (!$info -or $info.duration -le 0) { throw "No audio metadata for $title" }
        $url = ([Uri]$info.url).GetLeftPart([UriPartial]::Path)
        $extension = [IO.Path]::GetExtension(([Uri]$url).AbsolutePath).ToLowerInvariant()
        if ($extension -eq '.oga') { $extension = '.ogg' }
        if ($extension -notin @('.mp3','.ogg','.flac','.wav','.m4a','.mp4')) { throw "Unsupported audio type for $title" }
        $sha1 = $info.sha1
        if (!(Test-AudioHeader $url)) {
            if ($extension -ne '.ogg') { throw "Invalid audio header for $title" }
            $index = $url.LastIndexOf('/')
            $name = $url.Substring($index + 1)
            $url = $url.Substring(0, $index).Replace('/commons/', '/commons/transcoded/') + '/' + $name + '/' + $name + '.mp3'
            if (!(Test-AudioHeader $url)) { throw "No playable transcode for $title" }
            $extension = '.mp3'
            $sha1 = $null # Original checksum does not describe the transcode.
        }
        $candidate | Add-Member -Force NoteProperty downloadUri $url
        $candidate | Add-Member -Force NoteProperty fileName ($candidate.id + $extension)
        $candidate | Add-Member -Force NoteProperty durationSeconds ([Math]::Round($info.duration, 3))
        $candidate | Add-Member -Force NoteProperty sha1 $sha1
        $licenseUrl = $info.extmetadata.LicenseUrl.value
        if ($licenseUrl -like 'http://*') { $licenseUrl = 'https://' + $licenseUrl.Substring(7) }
        $candidate | Add-Member -Force NoteProperty declaredLicenseUri $licenseUrl
        $candidate | Add-Member -Force NoteProperty deliveryCheckedAtUtc ([DateTimeOffset]::UtcNow.ToString('O'))
        [IO.File]::WriteAllText([IO.Path]::GetFullPath($CandidatePath), ($manifest | ConvertTo-Json -Depth 8) + [Environment]::NewLine, [Text.UTF8Encoding]::new($false))
        Write-Host "Audio verified: $($candidate.id)"
    }
    [IO.File]::WriteAllText([IO.Path]::GetFullPath($CandidatePath), ($manifest | ConvertTo-Json -Depth 8) + [Environment]::NewLine, [Text.UTF8Encoding]::new($false))
} finally { $client.Dispose() }
