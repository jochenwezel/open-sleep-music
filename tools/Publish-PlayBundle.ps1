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
    # Validate the delivered native libraries, rather than inferring compatibility
    # from the build properties. ARM32 must remain available beside both 64-bit ABIs.
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $taskArchive = [IO.Compression.ZipFile]::OpenRead($taskSigned[0].FullName)
    try {
        $taskAbis = @($taskArchive.Entries | ForEach-Object {
            if ($_.FullName -match '^base/lib/([^/]+)/[^/]+\.so$') { $Matches[1] }
        } | Sort-Object -Unique)
        $taskExpectedAbis = @('arm64-v8a', 'armeabi-v7a', 'x86_64')
        if (Compare-Object $taskExpectedAbis $taskAbis) {
            throw "Unexpected Play bundle ABIs: $($taskAbis -join ', '). Expected ARM32, ARM64 and x86-64."
        }
        foreach ($taskAbi in $taskExpectedAbis) {
            foreach ($taskRuntimeLibrary in @('libmonodroid.so', 'libmonosgen-2.0.so')) {
                if (-not $taskArchive.GetEntry("base/lib/$taskAbi/$taskRuntimeLibrary")) {
                    throw "Play bundle is missing $taskRuntimeLibrary for $taskAbi."
                }
            }
        }
    } finally {
        $taskArchive.Dispose()
    }
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
        supportedAbis = $taskAbis
        note = 'Package creation only; no Play Console upload or rollout.'
    }
    $taskManifest | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $taskOutput 'build-info.json') -Encoding utf8
    Write-Output "Created signed Play bundle $taskVersion ($Channel); SHA-256: $taskDigest"
} finally {
    Pop-Location
}
