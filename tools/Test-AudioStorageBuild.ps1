param(
    [Parameter(Mandatory)][string]$AssemblyPath,
    [switch]$ExpectLegacyMigration
)
$ErrorActionPreference = 'Stop'
$stream = [IO.File]::OpenRead((Resolve-Path -LiteralPath $AssemblyPath).Path)
$reader = [System.Reflection.PortableExecutable.PEReader]::new($stream)
try {
    $metadata = [System.Reflection.Metadata.PEReaderExtensions]::GetMetadataReader($reader)
    $storageTypes = @($metadata.TypeDefinitions | Where-Object {
        $metadata.GetString($metadata.GetTypeDefinition($_).Name) -eq 'AudioLibraryStorage'
    })
    if ($storageTypes.Count -ne 1) { throw 'AudioLibraryStorage was not found in the compiled application.' }
    $methods = @($metadata.GetTypeDefinition($storageTypes[0]).GetMethods() | ForEach-Object {
        $metadata.GetString($metadata.GetMethodDefinition($_).Name)
    })
    $hasMigration = $methods -contains 'MigrateLegacyAudioAsync'
    if ($hasMigration -ne $ExpectLegacyMigration.IsPresent) {
        throw "Compiled legacy migration presence ($hasMigration) does not match release policy."
    }
    Write-Host "Audio storage build verified. Legacy Music-folder migration included: $hasMigration"
}
finally {
    $reader.Dispose()
    $stream.Dispose()
}
