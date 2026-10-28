param([Parameter(Mandatory=$true)][string]$Snapshot,[Parameter(Mandatory=$true)][string]$FixtureRoot,[Parameter(Mandatory=$true)][string]$Results)
# Reviewer-owned tests. Only snapshot sources and fresh synthetic installation trees are used.
$ErrorActionPreference='Stop';$WarningPreference='SilentlyContinue'
$reviewRepo=Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$allowed=Join-Path $reviewRepo '.dev-state\rotation1\closure-review-20261028'
foreach($p in @($Snapshot,$FixtureRoot,$Results)){if(-not ([IO.Path]::GetFullPath($p)).StartsWith($allowed+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'Review path outside private scratch'}}
if(Test-Path -LiteralPath $FixtureRoot){throw 'Fixture must be fresh'}
[IO.Directory]::CreateDirectory($FixtureRoot)|Out-Null
$ws=Import-Module (Join-Path $Snapshot 'Scripts\WorkspaceSafety.psm1') -PassThru -Force
$dev=Import-Module (Join-Path $Snapshot 'Scripts\DevWorkflow.psm1') -PassThru -Force
& $dev {
 function script:Read-PluginMetadata($Path,$Context){[IO.File]::ReadAllText($Path)|ConvertFrom-Json}
 function script:Get-DevProcesses{@()}
 function script:Start-Process{throw 'Unstubbed launch forbidden by review'}
}
$all=New-Object Collections.Generic.List[object]
function Check($ok,[string]$assertion){if(-not $ok){throw "ASSERT: $assertion"}}
function Case([string]$id,[string]$class,[scriptblock]$body){
 try{& $body;$r=@{test=$id;class=$class;status='PASS';assertion=$null}}
 catch{$r=@{test=$id;class=$class;status='FAIL';assertion=$_.Exception.Message.Replace($FixtureRoot,'<fixture>')}}
 $all.Add($r);[Console]::WriteLine(($r|ConvertTo-Json -Compress))
}
function NewTree([string]$install){
 foreach($n in @('BepInEx\core','BepInEx\plugins\PoVPlus','BepInEx\config')){[IO.Directory]::CreateDirectory((Join-Path $install $n))|Out-Null}
 foreach($n in @('valheim.exe','BepInEx\core\BepInEx.dll','BepInEx\core\Mono.Cecil.dll')){[IO.File]::WriteAllText((Join-Path $install $n),'not executable')}
}
function Context{
 $base=Join-Path $FixtureRoot ([guid]::NewGuid().ToString('N'))
 $repo=Join-Path $base 'repository';$install=Join-Path $base 'installation'
 [IO.Directory]::CreateDirectory($repo)|Out-Null;NewTree $install
 New-DevContext -Repository $repo -Installation $install -ApprovedInstallation $install -LeaseRoot (Join-Path $base 'lease-store')
}
function LeasePath($c){Join-Path $c.LeaseRoot ($c.ResourceId+'.lease')}
function Empty($c){[IO.Directory]::CreateDirectory($c.LeaseRoot)|Out-Null;[IO.File]::WriteAllBytes((LeasePath $c),[byte[]]@())}
function Take($c,[string]$ack){Enter-ResourceLease -Identity $c.Resource -LeaseRoot $c.LeaseRoot -Operation 'IndependentTest' -Repository $c.Repository -AcknowledgeUncleanLease $ack}
function Attempt($c,[string]$ack){
 $l=$null
 try{$l=Take $c $ack;[pscustomobject]@{accepted=$true;error='';record=$l.Record}}
 catch{[pscustomobject]@{accepted=$false;error=$_.Exception.Message;record=$null}}
 finally{if($l){Exit-ResourceLease $l 'no-op'}}
}
function Token([string]$s){if($s -match "-AcknowledgeUncleanLease '([^']+)'"){$Matches[1]}}
function WriteOutput($repo,$relative,[string]$text){
 $path=Resolve-WorkspaceOutput -Repository $repo -Path $relative -Overwrite
 New-WorkspaceOutputParent $repo $path
 [IO.File]::WriteAllText($path,$text)
}
function ErrorOf([scriptblock]$body){try{& $body|Out-Null;''}catch{$_.Exception.Message}}
function NewPlugin([string]$path,[string]$version){
 [IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($path))|Out-Null
 $data=@{Assembly='PoVPlus';Plugins=@(@{Guid='povplus.core';Name='PoVPlus';Version=$version;Base='BepInEx.BaseUnityPlugin';Type='PoVPlus.Plugin'})}|ConvertTo-Json -Depth 5
 [IO.File]::WriteAllText($path,$data)
}
function SetupPlan($c){
 NewPlugin $c.Target '0.1.0'
 $build=Join-Path $c.Repository 'Source\PoVPlus\bin\Release\net48\PoVPlus.dll'
 NewPlugin $build '0.2.0'
 New-DeploymentPlan $c
}
function ReplaceRoot($c){
 $prior=$c.Installation+'-prior';[IO.Directory]::Move($c.Installation,$prior);NewTree $c.Installation
 [IO.File]::Copy((Join-Path $prior 'BepInEx\plugins\PoVPlus\PoVPlus.dll'),$c.Target)
}
function BytesHash([string]$p){(Get-FileHash -LiteralPath $p -Algorithm SHA256).Hash}
function Facts($c){
 # Independent record inspection: never call the candidate parser for this oracle.
 $bytes=[IO.File]::ReadAllBytes((LeasePath $c));$s=[Text.Encoding]::UTF8.GetString($bytes)
 if($s.StartsWith('PoVPlusLease/2 ')){
  $nl=[Array]::IndexOf($bytes,[byte]10);$head=[Text.Encoding]::ASCII.GetString($bytes,0,$nl)
  Check ($head -match '^PoVPlusLease/2 (\d+) ([a-f0-9]{64})$') 'independent frame header parses'
  $len=[int]$Matches[1];$expected=$Matches[2]
  $payload=New-Object byte[] ($bytes.Length-$nl-1);[Array]::Copy($bytes,$nl+1,$payload,0,$payload.Length)
  $sha=[Security.Cryptography.SHA256]::Create();try{$hash=([BitConverter]::ToString($sha.ComputeHash($payload))).Replace('-','').ToLowerInvariant()}finally{$sha.Dispose()}
  Check ($len -eq $payload.Length -and $hash -ceq $expected) 'independent frame length and digest verify'
  $s=[Text.Encoding]::UTF8.GetString($payload)
 }
 $s|ConvertFrom-Json
}

Case 'T-R1-a' 'D' {
 $c=Context;Empty $c;$r=Attempt $c
 Check (-not $r.accepted -and $r.error -match 'unreadable|unknown' -and (Token $r.error)) 'existing zero-length record must refuse as unreadable/unknown and offer token'
 Check ((Get-Item (LeasePath $c)).Length -eq 0) 'refusal must preserve empty file'
}
Case 'T-R1-b' 'D' {
 $c=Context;Empty $c;$first=Attempt $c;$token=Token $first.error
 Check ($token) 'empty-record refusal must offer acknowledgement token'
 $before=BytesHash (LeasePath $c);$second=Attempt $c
 Check ((Token $second.error) -ceq $token -and (BytesHash (LeasePath $c)) -eq $before) 'repeat refusal must preserve token and bytes'
 $r=Attempt $c $token
 Check ($r.accepted -and $r.record.AcknowledgedPrevious -ceq $token) 'correct token must acquire and be recorded'
 Check (-not (Attempt $c $token).accepted) 'consumed token must not authorize another acquisition'
 Check ((Facts $c).AcknowledgedPrevious -ceq $token) 'independent on-disk record must contain acknowledgement'
}
Case 'T-R1-c' 'D' {
 $c=Context;Empty $c;$r=Attempt $c 'sha256:1111111111111111'
 Check (-not $r.accepted -and (Token $r.error)) 'wrong empty-record token must refuse and offer correct token'
}
Case 'T-R1-d' 'D' {
 $c=Context;$l=Take $c;$expected=$l.Record.LeaseId
 & $ws {
  $script:ReviewWriter=${function:Write-LeaseRecord}
  function script:Write-LeaseRecord($s,$record){
   $proxy=[pscustomobject]@{File=$s}
   $proxy|Add-Member ScriptProperty Position {$this.File.Position} {param($v)$this.File.Position=$v}
   $proxy|Add-Member ScriptMethod SetLength {param($n)$this.File.SetLength($n)}
   $proxy|Add-Member ScriptMethod Flush {param($disk)if($null -eq $disk){$this.File.Flush()}else{$this.File.Flush([bool]$disk)}}
   $proxy|Add-Member ScriptMethod Write {param($b,$start,$count)throw 'independent deterministic publication fault'}
   & $script:ReviewWriter $proxy $record
  }
 }
 try{Exit-ResourceLease $l 'completed'}finally{& $ws{Set-Item Function:script:Write-LeaseRecord $script:ReviewWriter}}
 $r=Attempt $c
 Check (-not $r.accepted -and (Token $r.error)) 'write failure must leave next acquisition blocked with acknowledgement token'
 Check ($r.error.Contains($expected)) 'zero-byte failed release must preserve previous held lease evidence'
}
Case 'T-R1-e' 'C' {
 $c=Context;Check (-not (Test-Path $c.LeaseRoot)) 'fixture lease root initially absent'
 $r=Attempt $c;Check ($r.accepted -and -not $r.record.AcknowledgedPrevious) 'first-ever acquisition must succeed without acknowledgement'
}
Case 'T-R1-f' 'C' {
 $c=Context;Check (Attempt $c).accepted 'first acquisition succeeds';$r=Attempt $c
 Check ($r.accepted -and $r.record.PreviousState -ceq 'released' -and -not $r.record.AcknowledgedPrevious) 'released record reacquires without acknowledgement'
}
Case 'T-R1-g' 'C' {
 $c=Context;Empty $c;$id='lease-abcdefabcdefabcdefabcdefabcdefab'
 [IO.File]::WriteAllText((LeasePath $c),(@{Schema=1;State='held';LeaseId=$id;ResourceId=$c.ResourceId}|ConvertTo-Json))
 $one=Take $c $id
 $child=Join-Path $FixtureRoot 'Independent-Contender.ps1';$signal=Join-Path $FixtureRoot 'contender.json'
 @'
param($Module,$Installation,$Store,$Ack,$Signal)
$ErrorActionPreference='Stop';$WarningPreference='SilentlyContinue'
Import-Module $Module
$lease=$null
try{$lease=Enter-ResourceLease -Identity (Get-ResourceIdentity $Installation) -LeaseRoot $Store -Operation 'IndependentChild' -AcknowledgeUncleanLease $Ack;$v=@{acquired=$true;pid=$PID}}
catch{$v=@{acquired=$false;error=$_.Exception.Message;pid=$PID}}
finally{if($lease){Exit-ResourceLease $lease 'no-op'}}
[IO.File]::WriteAllText($Signal,($v|ConvertTo-Json))
'@|Set-Content -LiteralPath $child -Encoding UTF8
 $hostExe=(Get-Process -Id $PID).Path
 $args=@('-NoProfile','-NonInteractive','-File',"`"$child`"",'-Module',"`"$(Join-Path $Snapshot 'Scripts\WorkspaceSafety.psm1')`"",'-Installation',"`"$($c.Installation)`"",'-Store',"`"$($c.LeaseRoot)`"",'-Ack',$id,'-Signal',"`"$signal`"")
 try{
  $p=Start-Process -FilePath $hostExe -ArgumentList $args -PassThru -WindowStyle Hidden
  Check ($p.WaitForExit(30000)) 'contender must terminate within 30 seconds'
  $r=[IO.File]::ReadAllText($signal)|ConvertFrom-Json
  Check (-not $r.acquired -and $r.error -match 'busy' -and $r.pid -eq $p.Id) 'exactly one independent process acquires while first remains held'
 }finally{Exit-ResourceLease $one 'no-op'}
}
Case 'T-R2-a' 'D' {
 $c=Context;$canary=Join-Path $FixtureRoot 'output-canary';[IO.File]::WriteAllText($canary,'original')
 [IO.Directory]::CreateDirectory($c.State)|Out-Null;$alias=Join-Path $c.State 'alias.json'
 New-Item -ItemType HardLink -Path $alias -Target $canary|Out-Null
 $before=BytesHash $canary;$message=ErrorOf {WriteOutput $c.Repository '.dev-state\alias.json' 'replacement'}
 Check ($message -match 'Multi-link' -and (BytesHash $canary) -eq $before) 'hardlinked output must refuse for multi-link reason without changing external canary'
}
Case 'T-R2-b' 'C' {
 $c=Context;[IO.Directory]::CreateDirectory($c.State)|Out-Null;$f=Join-Path $c.State 'plain.json';[IO.File]::WriteAllText($f,'old')
 WriteOutput $c.Repository '.dev-state\plain.json' 'replacement'
 Check ([IO.File]::ReadAllText($f) -ceq 'replacement') 'ordinary existing output must overwrite successfully'
}
Case 'T-R2-c' 'C' {
 $c=Context;[IO.Directory]::CreateDirectory($c.State)|Out-Null;$outside=Join-Path $FixtureRoot 'junction-destination';[IO.Directory]::CreateDirectory($outside)|Out-Null
 $link=Join-Path $c.State 'redirect';New-Item -ItemType Junction -Path $link -Target $outside|Out-Null
 try{$message=ErrorOf {WriteOutput $c.Repository '.dev-state\redirect\output' 'bad'};Check ($message -match 'Reparse' -and -not (Test-Path (Join-Path $outside 'output'))) 'junction output must refuse without escaped write'}finally{[IO.Directory]::Delete($link)}
}
Case 'T-R2-d' 'D' {
 $c=Context;$canary=Join-Path $FixtureRoot 'lease-canary';[IO.File]::WriteAllBytes($canary,[byte[]]@())
 [IO.Directory]::CreateDirectory($c.LeaseRoot)|Out-Null;New-Item -ItemType HardLink -Path (LeasePath $c) -Target $canary|Out-Null
 $before=BytesHash $canary;$r=Attempt $c
 Check (-not $r.accepted -and $r.error -match 'Multi-link' -and (BytesHash $canary) -eq $before) 'hardlinked lease must refuse before protected storage changes'
}
Case 'T-R2-e' 'C' {
 $c=Context;WriteOutput $c.Repository '.dev-state\new-parent\fresh' 'new'
 Check ([IO.File]::ReadAllText((Join-Path $c.State 'new-parent\fresh')) -ceq 'new') 'fresh tool-created output must succeed'
}
Case 'T-R3-a' 'D' {
 $c=Context;$plan=SetupPlan $c;ReplaceRoot $c;$before=BytesHash $c.Target
 $message=ErrorOf {Invoke-DevTransition $c $plan -Confirm:$false}
 Check ($message -match 'replaced or moved' -and (BytesHash $c.Target) -eq $before -and -not (Test-Path $c.State)) 'stale installation must refuse as replaced/moved before target or repository state write'
 Check ($message -match 'not lease contention') 'replacement error must distinguish lease contention'
 [IO.Directory]::Move($c.Installation,$c.Installation+'-cleanup-proof')
}
Case 'T-R3-b' 'C' {
 $c=Context;$plan=SetupPlan $c;ReplaceRoot $c
 $fresh=New-DevContext -Repository $c.Repository -Installation $c.Installation -ApprovedInstallation $c.Installation -LeaseRoot $c.LeaseRoot
 $plan=New-DeploymentPlan $fresh;Invoke-DevTransition $fresh $plan -Confirm:$false 6>$null
 Check ((BytesHash $fresh.Target) -eq (BytesHash $plan.Source)) 'fresh context after reinstall must deploy exact source bytes'
}
Case 'T-R3-c' 'C' {
 $c=Context;$plan=SetupPlan $c;Invoke-DevTransition $c $plan -Confirm:$false 6>$null
 Check ((BytesHash $c.Target) -eq (BytesHash $plan.Source)) 'unchanged installation must deploy exact source bytes'
 Check ((Facts $c).State -ceq 'released') 'normal transaction releases its lease'
}

$out=@{host=$PSVersionTable.PSVersion.ToString();snapshot=(Split-Path $Snapshot -Leaf);cases=@($all.ToArray());fixture=(Split-Path $FixtureRoot -Leaf)}
[IO.File]::WriteAllText($Results,($out|ConvertTo-Json -Depth 8),(New-Object Text.UTF8Encoding($false)))
exit @($all|Where-Object status -eq 'FAIL').Count
