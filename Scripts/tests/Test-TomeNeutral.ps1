[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$repo=(Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..\..')).Path
$state=Join-Path $repo '.dev-state\c1-audit\neutral'
[IO.Directory]::CreateDirectory($state)|Out-Null
$includes=(@('Source/PoVPlus/Diagnostics/DiagnosticSession.cs','Source/PoVPlus/Diagnostics/LifecycleEvidence.cs','Scripts/tests/TomeNeutralHarness.cs')|ForEach-Object {'<Compile Include="'+[Security.SecurityElement]::Escape((Join-Path $repo $_))+'" />'})-join "`n"
$project=@"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net48</TargetFramework><OutputType>Exe</OutputType>
<AutomaticallyUseReferenceAssemblyPackages>false</AutomaticallyUseReferenceAssemblyPackages><DefineConstants>POVPLUS_TOME_LIFECYCLE_DIAGNOSTIC</DefineConstants>
<DefineConstants Condition="'`$(Mutant)' == 'true'">POVPLUS_TOME_LIFECYCLE_DIAGNOSTIC;MUTATE_UNKNOWN_TO_EMPTY</DefineConstants>
<EnableDefaultCompileItems>false</EnableDefaultCompileItems><LangVersion>7.3</LangVersion></PropertyGroup><ItemGroup>$includes</ItemGroup></Project>
"@
$path=Join-Path $state 'TomeNeutralHarness.csproj'
[IO.File]::WriteAllText($path,$project)
& dotnet restore $path --source $repo --nologo
if($LASTEXITCODE){throw 'Neutral fixture restore failed'}
& dotnet build $path --no-restore -c Release --nologo
if($LASTEXITCODE){throw 'Neutral fixture build failed'}
& (Join-Path $state 'bin\Release\net48\TomeNeutralHarness.exe')
if($LASTEXITCODE){throw 'Neutral fixtures failed'}
$mutant=Join-Path $state 'mutant'
& dotnet build $path --no-restore -c Release -p:Mutant=true "-p:OutputPath=$mutant/" --nologo
if($LASTEXITCODE){throw 'Mutant did not compile; not a killed mutant'}
$process=Start-Process -FilePath (Join-Path $mutant 'TomeNeutralHarness.exe') -Wait -PassThru -WindowStyle Hidden -RedirectStandardOutput (Join-Path $state 'mutant.stdout.txt') -RedirectStandardError (Join-Path $state 'mutant.stderr.txt')
$errorText=[IO.File]::ReadAllText((Join-Path $state 'mutant.stderr.txt'))
if($process.ExitCode -eq 0 -or $errorText -notmatch 'missing ownership dictionary is not empty'){throw 'Unknown-to-empty mutant survived or failed for unrelated reason'}
Write-Host 'PASS deliberate unknown-to-empty mutation killed by missing-ownership assertion (exit1)'
