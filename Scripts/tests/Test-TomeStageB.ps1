[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..\..')).Path
$state = Join-Path $repo '.dev-state\stage-b\harness'
[IO.Directory]::CreateDirectory($state) | Out-Null
$sources = @('Source/PoVPlus/Compatibility/TomeBridgePolicy.cs', 'Source/PoVPlus/Compatibility/MagicPluginTomeBridge.cs',
    'Source/PoVPlus/Diagnostics/TomeStageBProbe.cs', 'Scripts/tests/TomeBridgeStubs.cs',
    'Scripts/tests/TomeStageBStubs.cs', 'Scripts/tests/TomeStageBHarness.cs')
$includes = ($sources | ForEach-Object { '<Compile Include="' + [Security.SecurityElement]::Escape((Join-Path $repo $_)) + '" />' }) -join "`n"
$project = @"
<Project Sdk="Microsoft.NET.Sdk">
 <PropertyGroup><TargetFramework>net48</TargetFramework><OutputType>Exe</OutputType>
 <AutomaticallyUseReferenceAssemblyPackages>false</AutomaticallyUseReferenceAssemblyPackages>
 <DefineConstants>POVPLUS_TOME_STAGE_B_DIAGNOSTIC</DefineConstants>
 <EnableDefaultCompileItems>false</EnableDefaultCompileItems><LangVersion>7.3</LangVersion></PropertyGroup>
 <ItemGroup>$includes</ItemGroup>
</Project>
"@
$path = Join-Path $state 'TomeStageBHarness.csproj'
[IO.File]::WriteAllText($path, $project)
& dotnet restore $path --source $repo --nologo
if ($LASTEXITCODE -ne 0) { throw 'Stage B harness offline restore failed.' }
& dotnet build $path --no-restore --configuration Release --nologo
if ($LASTEXITCODE -ne 0) { throw 'Stage B harness build failed.' }
& (Join-Path $state 'bin\Release\net48\TomeStageBHarness.exe')
if ($LASTEXITCODE -ne 0) { throw 'Stage B fixtures failed.' }
