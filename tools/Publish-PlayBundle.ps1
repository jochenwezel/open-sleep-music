param(
    [Parameter(Mandatory)][string]$KeyStorePath,
    [Parameter(Mandatory)][string]$KeyAlias,
    [Parameter(Mandatory)][string]$PasswordFile,
    [Parameter(Mandatory)][ValidatePattern('^\d{4}-\d{2}-\d{2}$')][string]$ReleaseDate,
    [ValidateSet('stable', 'preview')][string]$Channel = 'stable',
    [string]$OutputDirectory = 'artifacts/google-play'
)

$ErrorActionPreference = 'Stop'
$taskRepository = Split-Path -Parent $PSScriptRoot
$taskKeyStore = (Resolve-Path -LiteralPath $KeyStorePath).Path
$taskPassword = (Resolve-Path -LiteralPath $PasswordFile).Path
$taskProjectPath = Join-Path $taskRepository 'src/OpenSleepMusic.App/OpenSleepMusic.App.csproj'
$taskProject = [xml][IO.File]::ReadAllText($taskProjectPath)
$taskCode = [int]$taskProject.SelectSingleNode('//ApplicationVersion').InnerText
$taskVersion = "0.1.$taskCode"
$taskOutput = [IO.Path]::GetFullPath($OutputDirectory, $taskRepository)
$taskPackage = Join-Path $taskOutput "open-sleep-music-v$taskVersion-play.aab"
if (Test-Path -LiteralPath $taskPackage) { throw 'The named bundle already exists; preserve it and choose a new output directory.' }

Push-Location $taskRepository
try {
    & git diff --quiet HEAD -- src/OpenSleepMusic.App src/OpenSleepMusic.Core Directory.Build.props Directory.Build.targets docs/google-play/privacy-notice.de.md
    if ($LASTEXITCODE -ne 0) { throw 'Commit the tested application inputs before publishing.' }
    $taskUntracked = & git ls-files --others --exclude-standard -- src/OpenSleepMusic.App src/OpenSleepMusic.Core
    if ($taskUntracked) { throw 'Application source contains untracked files.' }
    $taskCommit = (& git rev-parse HEAD).Trim()
    $taskPreview = ($Channel -eq 'preview').ToString().ToLowerInvariant()
    & dotnet publish $taskProjectPath --framework net10.0-android --configuration Release `
        "-p:OpenSleepMusicPreview=$taskPreview" "-p:OpenSleepMusicReleaseDate=$ReleaseDate" `
        '-p:AndroidPackageFormats=aab' '-p:AndroidKeyStore=true' `
        "-p:AndroidSigningKeyStore=$taskKeyStore" "-p:AndroidSigningKeyAlias=$KeyAlias" `
        "-p:AndroidSigningKeyPass=file:$taskPassword" "-p:AndroidSigningStorePass=file:$taskPassword" `
        --output $taskOutput
    if ($LASTEXITCODE -ne 0) { throw 'Android App Bundle publishing failed.' }
    $taskSigned = @(Get-ChildItem -LiteralPath $taskOutput -Filter '*-Signed.aab' -File)
    if ($taskSigned.Count -ne 1) { throw 'Expected exactly one signed Android App Bundle.' }
    Copy-Item -LiteralPath $taskSigned[0].FullName -Destination $taskPackage
    $taskDigest = (Get-FileHash -LiteralPath $taskPackage -Algorithm SHA256).Hash.ToLowerInvariant()
    [IO.File]::WriteAllText("$taskPackage.sha256", "$taskDigest`n", [Text.UTF8Encoding]::new($false))
    $taskManifest = [ordered]@{
        version = $taskVersion
        versionCode = $taskCode
        commit = $taskCommit
        runtimeChannel = $Channel
        releaseDate = $ReleaseDate
        fileName = [IO.Path]::GetFileName($taskPackage)
        bytes = (Get-Item -LiteralPath $taskPackage).Length
        sha256 = $taskDigest
        note = 'Package creation only; no Play Console upload or rollout.'
    }
    $taskManifest | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $taskOutput 'build-info.json') -Encoding utf8
    Write-Output "Created signed Play bundle $taskVersion ($Channel); SHA-256: $taskDigest"
} finally {
    Pop-Location
}
