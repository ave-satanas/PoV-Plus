[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$ValheimDevPath = 'S:\Valheim-Modding\PoVPlus-Dev'
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $repoRoot 'Source\PoVPlus.sln'

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw 'Install a .NET SDK and the .NET Framework 4.8 targeting pack before building.'
}

# Restore only this package-free solution, using an existing local directory as
# the sole package source. This build never needs to contact a NuGet feed.
& dotnet restore $solution --source $repoRoot "-p:ValheimDevPath=$ValheimDevPath" --nologo
if ($LASTEXITCODE -ne 0) { throw "Restore failed (exit $LASTEXITCODE)." }

& dotnet build $solution --no-restore --configuration $Configuration "-p:ValheimDevPath=$ValheimDevPath" --nologo
if ($LASTEXITCODE -ne 0) { throw "Build failed (exit $LASTEXITCODE)." }

$outputPath = Join-Path $repoRoot "Source\PoVPlus\bin\$Configuration\net48"
$unexpectedDlls = @(Get-ChildItem -LiteralPath $outputPath -Filter '*.dll' |
    Where-Object { $_.Name -ne 'PoVPlus.dll' })
if ($unexpectedDlls.Count -gt 0) {
    throw "Unexpected dependency DLLs in build output: $($unexpectedDlls.Name -join ', '). Do not deploy this output."
}
Write-Host "Build verified: $outputPath\PoVPlus.dll"
Write-Host 'No files were deployed. The game was not launched.'
