param([Parameter(Mandatory=$true)][string]$Snapshot,[Parameter(Mandatory=$true)][string]$Root)
$ErrorActionPreference='Stop';$WarningPreference='SilentlyContinue'
$repo=Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$allowed=Join-Path $repo '.dev-state\rotation1\closure-review-20261028'
if(-not $Root.StartsWith($allowed+'\',[StringComparison]::OrdinalIgnoreCase) -or (Test-Path $Root)){throw 'Private fresh fixture required'}
[IO.Directory]::CreateDirectory($Root)|Out-Null
$ws=Import-Module (Join-Path $Snapshot 'Scripts\WorkspaceSafety.psm1') -Force -PassThru
$dev=Import-Module (Join-Path $Snapshot 'Scripts\DevWorkflow.psm1') -Force -PassThru
& $dev{
 function script:Read-PluginMetadata($Path,$Context){[IO.File]::ReadAllText($Path)|ConvertFrom-Json}
 function script:Get-DevProcesses{@()}
}
function Must($ok,$s){if(-not $ok){throw "ASSERT: $s"}}
function Ctx{
 $b=Join-Path $Root ([guid]::NewGuid().ToString('N'));$r=Join-Path $b 'repo';$i=Join-Path $b 'install'
 [IO.Directory]::CreateDirectory($r)|Out-Null
 foreach($n in @('BepInEx\core','BepInEx\config','BepInEx\plugins\PoVPlus')){[IO.Directory]::CreateDirectory((Join-Path $i $n))|Out-Null}
 foreach($n in @('valheim.exe','BepInEx\core\BepInEx.dll','BepInEx\core\Mono.Cecil.dll')){[IO.File]::WriteAllText((Join-Path $i $n),'synthetic')}
 New-DevContext -Repository $r -Installation $i -ApprovedInstallation $i -LeaseRoot (Join-Path $b 'leases')
}
function Plugin($p,$v){[IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($p))|Out-Null;[IO.File]::WriteAllText($p,(@{Assembly='PoVPlus';Plugins=@(@{Guid='povplus.core';Name='PoVPlus';Version=$v;Base='BepInEx.BaseUnityPlugin';Type='PoVPlus.Plugin'})}|ConvertTo-Json -Depth 5))}
function Token($s){if($s -match "-AcknowledgeUncleanLease '([^']+)'"){$Matches[1]}}
function Rejected($c){$l=$null;try{$l=Enter-ResourceLease -Identity $c.Resource -LeaseRoot $c.LeaseRoot -Operation 'Additional';''}catch{$_.Exception.Message}finally{if($l){Exit-ResourceLease $l 'no-op'}}}
$out=New-Object Collections.Generic.List[object]
function Run($id,[scriptblock]$f){try{& $f;$r=@{id=$id;status='PASS'}}catch{$r=@{id=$id;status='FAIL';error=$_.Exception.Message.Replace($Root,'<fixture>')}};$out.Add($r);[Console]::WriteLine(($r|ConvertTo-Json -Compress))}

Run 'A1-frame-parser-agreement' {
 $c=Ctx;$l=Enter-ResourceLease -Identity $c.Resource -LeaseRoot $c.LeaseRoot -Operation 'Additional'
 Exit-ResourceLease $l 'completed';$p=$l.Path;$original=[IO.File]::ReadAllBytes($p)
 $samples=@([byte[]]@(),[byte[]](80,111,86),[byte[]]($original+[byte]32),[byte[]]$original[0..100])
 foreach($bytes in $samples){
  [IO.File]::WriteAllBytes($p,$bytes);$before=(Get-FileHash $p).Hash
  $state=Get-ResourceLeaseStatus $c.Resource $c.LeaseRoot;$err=Rejected $c
  Must ($state.State -ceq 'unreadable-or-empty' -and (Token $err) -and (Get-FileHash $p).Hash -eq $before) 'malformed/truncated/trailing bytes must agree across Status and acquisition, remain unchanged and block'
 }
 # Independently corrupt a payload byte without adjusting its declared checksum.
 $bad=[byte[]]$original.Clone();$bad[$bad.Length-3]=$bad[$bad.Length-3] -bxor 1
 [IO.File]::WriteAllBytes($p,$bad)
 Must ((Rejected $c) -match 'hash mismatch') 'checksum must reject altered payload'
}
Run 'A2-pin-through-deploy-rollback-receipts' {
 $c=Ctx;Plugin $c.Target '0.1.0';$before=(Get-FileHash $c.Target).Hash
 $build=Join-Path $c.Repository 'Source\PoVPlus\bin\Release\net48\PoVPlus.dll';Plugin $build '0.2.0'
 & $dev{
  $script:OriginalJournal=${function:Write-DevJson};$script:PinSamples=New-Object Collections.Generic.List[object]
  function script:Write-DevJson($p,$value){
   $moved=$false
   try{[IO.Directory]::Move($value.Installation,$value.Installation+'-unexpected');$moved=$true;[IO.Directory]::Move($value.Installation+'-unexpected',$value.Installation)}catch{}
   $script:PinSamples.Add(@{moved=$moved;state=$value.Status;operation=$value.Operation})
   & $script:OriginalJournal $p $value
  }
 }
 try{
  Invoke-DevTransition $c (New-DeploymentPlan $c) -Confirm:$false 6>$null
  $rollback=New-RollbackPlan $c;Invoke-DevTransition $c $rollback -Confirm:$false 6>$null
 }finally{& $dev{Set-Item Function:script:Write-DevJson $script:OriginalJournal}}
 $samples=@(& $dev{$script:PinSamples.ToArray()})
 Must ($samples.Count -eq 4 -and -not @($samples|Where-Object moved).Count) 'root pin must remain held at prepared and completed receipts for both deploy and rollback'
 Must ((Get-FileHash $c.Target).Hash -eq $before) 'rollback must restore exact synthetic bytes'
 foreach($entry in (Get-DevBackups $c)){Must ($entry.Manifest.ResourceId -ceq $c.ResourceId -and $entry.Manifest.Status -ceq 'completed') 'receipt must bind correct identity and completed state'}
 [IO.Directory]::Move($c.Installation,$c.Installation+'-released')
}
Run 'A3-launch-boundary-and-release' {
 $c=Ctx
 & $dev{
  $script:LaunchPinBlocked=$false;$script:LaunchCalls=0
  function script:Start-Process{param($FilePath,$WorkingDirectory,$ArgumentList,$WindowStyle,[switch]$PassThru)
   $script:LaunchCalls++
   try{[IO.Directory]::Move($WorkingDirectory,$WorkingDirectory+'-unexpected');[IO.Directory]::Move($WorkingDirectory+'-unexpected',$WorkingDirectory)}catch{$script:LaunchPinBlocked=$true}
   [pscustomobject]@{Id=111111}
  }
 }
 Start-DevGame $c @('-synthetic') -Confirm:$false 6>$null
 Must (& $dev{$script:LaunchPinBlocked -and $script:LaunchCalls -eq 1}) 'pin must cover actual final process-start seam'
 [IO.Directory]::Move($c.Installation,$c.Installation+'-released')
}
Run 'A4-publication-detaches-hardlinks' {
 $c=Ctx;[IO.Directory]::CreateDirectory($c.State)|Out-Null;$target=Join-Path $c.State 'manifest.json';$alias=Join-Path $Root 'prior-manifest'
 [IO.File]::WriteAllText($target,'prior');New-Item -ItemType HardLink -Path $alias -Target $target|Out-Null
 & $dev{param($p)Write-DevJson $p @{independent=1}} $target
 Must ([IO.File]::ReadAllText($alias) -ceq 'prior' -and ([IO.File]::ReadAllText($target)|ConvertFrom-Json).independent -eq 1) 'atomic publication replaces a name without writing through prior alias'
 New-Item -ItemType HardLink -Path ($target+'.tmp') -Target $alias|Out-Null
 $err='';try{& $dev{param($p)Write-DevJson $p @{independent=2}} $target}catch{$err=$_.Exception.Message}
 Must ($err -match 'Leftover temporary' -and [IO.File]::ReadAllText($alias) -ceq 'prior') 'exclusive temporary create must refuse existing hardlink without truncating it'
}

[IO.File]::WriteAllText((Join-Path $Root 'additional-results.json'),(@{host=$PSVersionTable.PSVersion.ToString();cases=@($out.ToArray())}|ConvertTo-Json -Depth 8))
exit @($out|Where-Object status -eq 'FAIL').Count
