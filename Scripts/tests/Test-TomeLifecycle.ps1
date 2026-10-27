[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$repo=(Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..\..')).Path
$state=Join-Path $repo '.dev-state\c1-observer\harness'
[IO.Directory]::CreateDirectory($state) | Out-Null
$sources=@('Source/PoVPlus/Diagnostics/DiagnosticSession.cs','Source/PoVPlus/Diagnostics/LifecycleEvidence.cs','Scripts/tests/TomeLifecycleHarness.cs')
$includes=($sources | ForEach-Object {'<Compile Include="'+[Security.SecurityElement]::Escape((Join-Path $repo $_))+'" />'}) -join "`n"
$project=@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net48</TargetFramework><OutputType>Exe</OutputType>
<AutomaticallyUseReferenceAssemblyPackages>false</AutomaticallyUseReferenceAssemblyPackages><DefineConstants>POVPLUS_TOME_LIFECYCLE_DIAGNOSTIC</DefineConstants>
<EnableDefaultCompileItems>false</EnableDefaultCompileItems><LangVersion>7.3</LangVersion></PropertyGroup><ItemGroup>$includes</ItemGroup></Project>
"@
$path=Join-Path $state 'TomeLifecycleHarness.csproj'
[IO.File]::WriteAllText($path,$project)
& dotnet restore $path --source $repo --nologo
if($LASTEXITCODE){throw 'Lifecycle fixture restore failed'}
& dotnet build $path --no-restore -c Release --nologo
if($LASTEXITCODE){throw 'Lifecycle fixture build failed'}
& (Join-Path $state 'bin\Release\net48\TomeLifecycleHarness.exe')
if($LASTEXITCODE){throw 'Lifecycle fixtures failed'}

$exe=Join-Path $state 'bin\Release\net48\TomeLifecycleHarness.exe'
$first=& $exe --json
$second=& $exe --json
if($LASTEXITCODE -or $first -cne $second){throw 'Serialization is not deterministic'}
$decoded=$first | ConvertFrom-Json
if($decoded.empty.status -ne 'empty' -or $decoded.missing.status -ne 'unknown' -or $decoded.missing.reason -ne 'No existing entry' -or $null -ne $decoded.empty.value){throw 'JSON schema/unknown states corrupted'}
Write-Host 'PASS independent JSON parsing and byte-identical fixture serialization'
