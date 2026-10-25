[CmdletBinding()]
param(
    [string]$ValheimDevPath = 'S:\Valheim-Modding\PoVPlus-Dev',
    [string]$OutputPath
)
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
if (-not $OutputPath) { $OutputPath = Join-Path $repoRoot 'Knowledge\Dependencies\environment-snapshot.json' }
$destination = [IO.Path]::GetFullPath($OutputPath)
if (-not $destination.StartsWith($repoRoot.TrimEnd('\') + '\', [StringComparison]::OrdinalIgnoreCase)) {
    throw 'Snapshot output must remain inside the repository.'
}
# Read metadata with Cecil; never load or execute game/plugin code.
Add-Type -LiteralPath (Join-Path $ValheimDevPath 'BepInEx\core\Mono.Cecil.dll')
$resolver = New-Object Mono.Cecil.DefaultAssemblyResolver
$resolver.AddSearchDirectory((Join-Path $ValheimDevPath 'BepInEx\core'))
$resolver.AddSearchDirectory((Join-Path $ValheimDevPath 'valheim_Data\Managed'))
$reader = New-Object Mono.Cecil.ReaderParameters
$reader.AssemblyResolver = $resolver
function Get-AllTypes($types) {
    foreach ($type in $types) { $type; Get-AllTypes $type.NestedTypes }
}
function Get-AttributeValues($attribute) {
    @($attribute.ConstructorArguments | ForEach-Object { [string]$_.Value })
}
function Get-RelativeDevPath($path) { $path.Substring($ValheimDevPath.TrimEnd('\').Length + 1).Replace('\', '/') }
$assemblies = @()
foreach ($area in @('BepInEx\core', 'BepInEx\plugins', 'BepInEx\patchers', 'valheim_Data\Managed')) {
    foreach ($file in (Get-ChildItem -LiteralPath (Join-Path $ValheimDevPath $area) -Recurse -Filter '*.dll' | Sort-Object FullName)) {
        $assembly = $null
        try {
            $assembly = [Mono.Cecil.AssemblyDefinition]::ReadAssembly($file.FullName, $reader)
            $plugins = @()
            $patches = @()
            foreach ($type in (Get-AllTypes $assembly.MainModule.Types)) {
                foreach ($attribute in $type.CustomAttributes) {
                    if ($attribute.AttributeType.FullName -eq 'BepInEx.BepInPlugin') {
                        $values = Get-AttributeValues $attribute
                        $dependencies = @($type.CustomAttributes | Where-Object { $_.AttributeType.FullName -eq 'BepInEx.BepInDependency' } | ForEach-Object { ,(Get-AttributeValues $_) })
                        $plugins += [ordered]@{ type=$type.FullName; guid=$values[0]; name=$values[1]; version=$values[2]; base_type=[string]$type.BaseType; dependencies=$dependencies; methods=@($type.Methods | Where-Object { $_.Name -in @('Awake','Start','OnEnable') } | ForEach-Object { $_.FullName }) }
                    }
                }
                if ($area -notin @('BepInEx\core','valheim_Data\Managed')) {
                    foreach ($member in (@($type) + @($type.Methods))) {
                        foreach ($attribute in $member.CustomAttributes) {
                            if ($attribute.AttributeType.FullName -eq 'HarmonyLib.HarmonyPatch') {
                                $patches += [ordered]@{ member=$member.FullName; arguments=@(Get-AttributeValues $attribute) }
                            }
                        }
                    }
                }
            }
            $framework = @($assembly.CustomAttributes | Where-Object { $_.AttributeType.Name -eq 'TargetFrameworkAttribute' } | ForEach-Object { $_.ConstructorArguments[0].Value }) -join ''
            $company = @($assembly.CustomAttributes | Where-Object { $_.AttributeType.Name -eq 'AssemblyCompanyAttribute' } | ForEach-Object { $_.ConstructorArguments[0].Value }) -join ''
            $assemblies += [ordered]@{
                path=Get-RelativeDevPath $file.FullName; name=$assembly.Name.Name; version=[string]$assembly.Name.Version
                sha256=(Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
                target_framework=$framework; company_metadata=$company; plugins=$plugins
                references=@($assembly.MainModule.AssemblyReferences | ForEach-Object { [ordered]@{ name=$_.Name; version=[string]$_.Version } })
                declared_patch_attributes=$patches
            }
        } finally { if ($assembly) { $assembly.Dispose() } }
    }
}
$packages = @(Get-ChildItem -LiteralPath (Join-Path $ValheimDevPath 'BepInEx\plugins') -Recurse -Filter manifest.json | Sort-Object FullName | ForEach-Object {
    $manifest = Get-Content -LiteralPath $_.FullName -Raw | ConvertFrom-Json
    $folder = $_.Directory.Name
    [ordered]@{ path=Get-RelativeDevPath $_.FullName; sha256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash; folder=$folder; publisher_from_folder=($folder -split '-',2)[0]; name=$manifest.name; version=$manifest.version_number; website=$manifest.website_url; dependencies=@($manifest.dependencies) }
})
$configs = @(Get-ChildItem -LiteralPath (Join-Path $ValheimDevPath 'BepInEx\config') -Recurse -Filter '*.cfg' | Sort-Object FullName | ForEach-Object {
    [ordered]@{ path=Get-RelativeDevPath $_.FullName; sha256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash }
})
$sourceHashes = @(foreach ($relative in @('AGENTS.md','Source\PoVPlus\Plugin.cs','Source\PoVPlus\PoVPlus.csproj','Scripts\Build.ps1')) {
    [ordered]@{ path=$relative.Replace('\','/'); sha256=(Get-FileHash -LiteralPath (Join-Path $repoRoot $relative) -Algorithm SHA256).Hash }
})
$snapshot = [ordered]@{ schema_version=1; captured_utc=[DateTime]::UtcNow.ToString('o'); installation_root=$ValheimDevPath; assemblies=$assemblies; packages=$packages; configurations=$configs; repository_sources=$sourceHashes }
$json = $snapshot | ConvertTo-Json -Depth 20 -Compress
[IO.File]::WriteAllText($destination, $json + [Environment]::NewLine, (New-Object Text.UTF8Encoding($false)))
Write-Host "Captured $($assemblies.Count) assemblies, $($packages.Count) manifests, $($configs.Count) cfg fingerprints. No configuration values or logs copied."
$resolver.Dispose()
