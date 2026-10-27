# Synthetic demonstration of the exact File.Open/FileShare.None primitive, not a deployment.
$ErrorActionPreference='Stop'
$repo=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$root=Join-Path $repo ('.dev-state/crossreview-a/locks-'+[Guid]::NewGuid().ToString('N'))
$a=Join-Path $root 'worktree-a/.dev-state/workflow.lock'
$b=Join-Path $root 'worktree-b/.dev-state/workflow.lock'
[IO.Directory]::CreateDirectory((Split-Path -Parent $a))|Out-Null
[IO.Directory]::CreateDirectory((Split-Path -Parent $b))|Out-Null
$child=Join-Path $root 'TryLock.ps1'
@'
param([string]$Path)
try{$f=[IO.File]::Open($Path,[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None);$f.Dispose();exit 0}catch{exit 23}
'@ | Set-Content -LiteralPath $child -Encoding UTF8
$handle=[IO.File]::Open($a,[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)
try{
 & powershell -NoProfile -File $child -Path $a
 if($LASTEXITCODE -ne 23){throw 'Same resource primitive did not exclude child'}
 & powershell -NoProfile -File $child -Path $b
 if($LASTEXITCODE -ne 0){throw 'Separate worktree lock unexpectedly excluded child'}
 Write-Host 'PASS same-path second process blocked; different worktree path acquired while first held'
}finally{$handle.Dispose()}
& powershell -NoProfile -File $child -Path $a
if($LASTEXITCODE -ne 0){throw 'Released handle not reacquirable'}
Write-Host 'PASS released handle reacquirable despite lock file persistence; file presence is not lease ownership'
