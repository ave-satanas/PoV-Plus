[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..\..')).Path
$state = Join-Path $repo '.dev-state\offline-tome\harness'
[IO.Directory]::CreateDirectory($state) | Out-Null
$policy = [Security.SecurityElement]::Escape((Join-Path $repo 'Source\PoVPlus\Diagnostics\DiagnosticSession.cs'))
$tests = [Security.SecurityElement]::Escape((Join-Path $PSScriptRoot 'TomeDiagnosticHarness.cs'))
$project = @"
<Project Sdk="Microsoft.NET.Sdk">
 <PropertyGroup><TargetFramework>net48</TargetFramework><OutputType>Exe</OutputType>
 <AutomaticallyUseReferenceAssemblyPackages>false</AutomaticallyUseReferenceAssemblyPackages>
 <EnableDefaultCompileItems>false</EnableDefaultCompileItems><LangVersion>7.3</LangVersion></PropertyGroup>
 <ItemGroup><Compile Include="$policy" /><Compile Include="$tests" /></ItemGroup>
</Project>
"@
$projectPath = Join-Path $state 'TomeDiagnosticHarness.csproj'
[IO.File]::WriteAllText($projectPath, $project)
& dotnet restore $projectPath --source $repo --nologo
if ($LASTEXITCODE -ne 0) { throw 'Offline harness restore failed' }
& dotnet build $projectPath --no-restore --configuration Release --nologo
if ($LASTEXITCODE -ne 0) { throw 'Offline harness compile failed' }
& (Join-Path $state 'bin\Release\net48\TomeDiagnosticHarness.exe')
if ($LASTEXITCODE -ne 0) { throw 'Offline diagnostic policy tests failed' }
