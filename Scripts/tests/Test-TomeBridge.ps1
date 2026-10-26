[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..\..')).Path
$state = Join-Path $repo '.dev-state\tome-bridge\harness'
[IO.Directory]::CreateDirectory($state) | Out-Null
$sources = @('Source/PoVPlus/Compatibility/TomeBridgePolicy.cs',
    'Source/PoVPlus/Compatibility/MagicPluginTomeBridge.cs',
    'Scripts/tests/TomeBridgeStubs.cs', 'Scripts/tests/TomeBridgeHarness.cs')
$includes = ($sources | ForEach-Object {
    '<Compile Include="' + [Security.SecurityElement]::Escape((Join-Path $repo $_)) + '" />'
}) -join "`n"
$project = @"
<Project Sdk="Microsoft.NET.Sdk">
 <PropertyGroup><TargetFramework>net48</TargetFramework><OutputType>Exe</OutputType>
 <AutomaticallyUseReferenceAssemblyPackages>false</AutomaticallyUseReferenceAssemblyPackages>
 <EnableDefaultCompileItems>false</EnableDefaultCompileItems><LangVersion>7.3</LangVersion></PropertyGroup>
 <ItemGroup>$includes</ItemGroup>
</Project>
"@
$projectPath = Join-Path $state 'TomeBridgeHarness.csproj'
[IO.File]::WriteAllText($projectPath, $project)
& dotnet restore $projectPath --source $repo --nologo
if ($LASTEXITCODE -ne 0) { throw 'Bridge harness offline restore failed.' }
& dotnet build $projectPath --no-restore --configuration Release --nologo
if ($LASTEXITCODE -ne 0) { throw 'Bridge harness build failed.' }
& (Join-Path $state 'bin\Release\net48\TomeBridgeHarness.exe')
if ($LASTEXITCODE -ne 0) { throw 'Bridge harness tests failed.' }
