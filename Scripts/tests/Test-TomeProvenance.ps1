[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$repo=(Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$state=Join-Path $repo '.dev-state/exp005-correction-20261026/fixtures'
[IO.Directory]::CreateDirectory($state)|Out-Null
$source=Join-Path $repo 'Source/PoVPlus/Diagnostics/PassiveOwnershipProof.cs'
$harness=Join-Path $PSScriptRoot 'TomeProvenanceHarness.cs'
function Build-Run([string]$name,[string]$code,[string]$tests,[bool]$mutant,[bool]$legacy){
 $dir=Join-Path $state $name; [IO.Directory]::CreateDirectory($dir)|Out-Null
 $copy=Join-Path $dir 'Proof.cs'; [IO.File]::WriteAllText($copy,$code)
 $includes=@($copy,$tests)|ForEach-Object {'<Compile Include="'+[Security.SecurityElement]::Escape($_)+'" />'}
 $project='<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net48</TargetFramework><OutputType>Exe</OutputType><AutomaticallyUseReferenceAssemblyPackages>false</AutomaticallyUseReferenceAssemblyPackages><EnableDefaultCompileItems>false</EnableDefaultCompileItems><LangVersion>7.3</LangVersion><DefineConstants>POVPLUS_TOME_PROVENANCE_POC;POVPLUS_PROVENANCE_FIXTURE</DefineConstants></PropertyGroup><ItemGroup>'+($includes -join '')+'</ItemGroup></Project>'
 $path=Join-Path $dir 'Proof.csproj'; [IO.File]::WriteAllText($path,$project)
 & dotnet restore $path --source $repo --nologo
 if($LASTEXITCODE){throw 'Offline fixture restore failed'}
 & dotnet build $path --no-restore -c Release --nologo
 if($LASTEXITCODE){throw 'Fixture compile failed (not a killed mutant)'}
 $exe=Join-Path $dir 'bin/Release/net48/Proof.exe'
 $output=& $exe 2>&1; $result=$LASTEXITCODE
 $output|Set-Content -LiteralPath (Join-Path $dir 'run.log')
 $output|ForEach-Object { Write-Host $_ }
 if($mutant){
  if($result -ne 1){throw 'Mutant escaped independent assertions'}
  Write-Host "PASS killed disposable mutant: $name"
  $auditDir=Join-Path $dir 'artifact';[IO.Directory]::CreateDirectory($auditDir)|Out-Null
  $auditProject='<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net48</TargetFramework><OutputType>Library</OutputType><AssemblyName>PoVPlus</AssemblyName><AutomaticallyUseReferenceAssemblyPackages>false</AutomaticallyUseReferenceAssemblyPackages><EnableDefaultCompileItems>false</EnableDefaultCompileItems><LangVersion>7.3</LangVersion><DefineConstants>POVPLUS_TOME_PROVENANCE_POC</DefineConstants></PropertyGroup><ItemGroup><Compile Include="'+[Security.SecurityElement]::Escape($copy)+'" /></ItemGroup></Project>'
  $auditPath=Join-Path $auditDir 'PoVPlus.csproj';[IO.File]::WriteAllText($auditPath,$auditProject)
  & dotnet restore $auditPath --source $repo --nologo
  if($LASTEXITCODE){throw 'Artifact mutant restore failed'}
  & dotnet build $auditPath --no-restore -c Release --nologo
  if($LASTEXITCODE){throw 'Artifact mutant compile failed, not a rejected artifact'}
  $auditLog=Join-Path $auditDir 'audit.log'
  & pwsh -NoProfile -File (Join-Path $PSScriptRoot 'Test-TomeProvenanceArtifact.ps1') -Enabled -Dll (Join-Path $auditDir 'bin/Release/net48/PoVPlus.dll') *> $auditLog
  if($LASTEXITCODE -ne 1){throw 'Artifact audit did not independently reject regression mutant'}
  $message=Get-Content -LiteralPath $auditLog -Raw
  if($name -eq 'unsafe-collection') {if($message -notmatch 'Concrete collection guard missing'){throw 'Artifact mutant failed for an unrelated reason'}}
  elseif($name -eq 'string-budget') {if($message -notmatch 'Collection string bound missing'){throw 'String-budget audit failed for an unrelated reason'}}
  elseif($name -eq 'storage-budget') {if($message -notmatch 'Traversal bound missing'){throw 'Storage-budget audit failed for an unrelated reason'}}
  elseif($name -eq 'alias-context') {if($message -notmatch 'Missing observation scope comparison'){throw 'Alias-context audit failed for an unrelated reason'}}
  elseif($message -notmatch 'Unsupported claim promotion'){throw 'Artifact mutant failed for an unrelated reason'}
  Write-Host "PASS independent compiled-IL audit rejects $name";return
 }
 if($result){throw 'Provenance fixture failed'}
 if($legacy){
  $counterexample=& $exe --falsify; $result=$LASTEXITCODE
  $counterexample|Set-Content -LiteralPath (Join-Path $dir 'counterexample.txt')
  if($result -ne 2 -or -not ($counterexample -match 'independent driver saw two writes') -or
      -not ($counterexample -match 'CorroboratedNoOp history=True') -or
      -not ($counterexample -match 'foreign enumerator executed 2 times, complete=True')){throw 'Original three failures did not independently reproduce'}
  Write-Host 'PASS original three defects reproduced, exit2; frozen baseline unchanged.';return
 }
 $json=& $exe --json; if($LASTEXITCODE){throw 'JSON export failed'}
 [IO.File]::WriteAllText((Join-Path $dir 'evidence.json'),$json)
 $parsed=$json|ConvertFrom-Json
 if($parsed.schema -ne 2 -or $parsed.runtimePatchInstalled -ne $false -or $parsed.neutralCertification -ne 'unknown' -or
     $parsed.localReadValid -ne $true -or $parsed.endpointsAgree -ne $true -or $parsed.sampledValuesAgree -ne $true -or
     $parsed.atomicity -ne 'unknown' -or $parsed.originValidity -ne 'unknown' -or $parsed.authoritativeState -ne 'unknown' -or $parsed.hostStorageField -notin @('count','_count') -or $parsed.recordCount -ne 2 -or $parsed.firstSequence -ne 1 -or $parsed.lastSequence -ne 2 -or $parsed.classification -ne 'EquipEndpointAgreement' -or $parsed.coverage -ne 'unknown' -or $parsed.initialState -ne 'unknown' -or
     $parsed.continuity -ne 'unknown' -or $parsed.rollback -ne 'unknown' -or $parsed.historyComplete -ne $false){throw 'Independent JSON interpretation failed'}
}
$legacy=Join-Path $PSScriptRoot 'Fixtures/EXP005Original'
$manifest=Get-Content -LiteralPath (Join-Path $legacy 'manifest.json') -Raw|ConvertFrom-Json
foreach($entry in $manifest.files){if((Get-FileHash -LiteralPath (Join-Path $legacy $entry.path)).Hash -cne $entry.sha256){throw 'Frozen original changed'}}
Build-Run 'original' ([IO.File]::ReadAllText((Join-Path $legacy 'PassiveOwnershipProof.cs'))) (Join-Path $legacy 'TomeProvenanceHarness.cs') $false $true
$original=[IO.File]::ReadAllText($source)
Build-Run 'corrected' $original $harness $false $false
$target='if (value == null || value.GetType() != exactType)'
if(-not $original.Contains($target)){throw 'Concrete-type mutant target drift'}
Build-Run 'unsafe-collection' ($original.Replace($target,'if (value == null)')) $harness $true $false
$target='internal string Continuity { get { return "unknown"; } }'
$index=$original.IndexOf($target)
if($index -lt 0){throw 'Continuity mutant target drift'}
# Replace only Snapshot.Continuity, leaving the separate ledger property intact.
$mutant=$original.Remove($index,$target.Length).Insert($index,'internal string Continuity { get { return EndpointsAgree ? "continuous" : "unknown"; } }')
Build-Run 'endpoint-continuity' $mutant $harness $true $false
foreach($claim in @('Coverage','Initial')){
 $property=if($claim -eq 'Coverage'){'TransitionCoverage'}else{'InitialStateValidity'}
 $argument=if($claim -eq 'Coverage'){'claimedCoverage'}else{'claimedInitial'}
 $target='internal string '+$property+' { get { return "unknown"; } }'
 if(-not $original.Contains($target)){throw 'Claim promotion mutant target drift'}
 $mutant=$original.Replace('private bool _invalid;','private bool _invalid; private bool _untrustedClaim;').Replace('_current = snapshot;',('_current = snapshot; _untrustedClaim = '+$argument+';')).Replace($target,('internal string '+$property+' { get { return _untrustedClaim ? "verified" : "unknown"; } }'))
 Build-Run ('caller-'+$claim.ToLowerInvariant()) $mutant $harness $true $false
}
$target='!ReferenceEquals(_current.ReaderIdentity, e.State.ReaderIdentity)'
if(-not $original.Contains($target)){throw 'Alias-context mutant target drift'}
Build-Run 'alias-context' ($original.Replace($target,'false')) $harness $true $false
$target='if (used < 0 || used > 64)'
if(-not $original.Contains($target)){throw 'Storage-budget mutant target drift'}
Build-Run 'storage-budget' ($original.Replace($target,'if (used < 0)')) $harness $true $false
$target='key == null || key.Length > 128 || (pair.Value is string && ((string)pair.Value).Length > 4096)'
if(-not $original.Contains($target)){throw 'String-budget mutant target drift'}
Build-Run 'string-budget' ($original.Replace($target,'key == null')) $harness $true $false
if([IO.File]::ReadAllText($source) -cne $original){throw 'Experimental source changed during mutation tests'}
Write-Host 'PASS frozen original, corrected fixtures, independent JSON, seven killed mutants; no runtime proof implied.'
exit 0
