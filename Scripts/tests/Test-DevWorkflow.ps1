[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$module = Import-Module (Join-Path $repo 'Scripts\DevWorkflow.psm1') -Force -PassThru
$fixtureRoot = Join-Path ([IO.Path]::GetTempPath()) ('PoVPlus-workflow-tests-' + [guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($fixtureRoot) | Out-Null
$script:Passed = 0

function Assert-True($Value, [string]$Message) { if (-not $Value) { throw "Assertion failed: $Message" } }
function Assert-Throws([scriptblock]$Action, [string]$Pattern) {
    try { & $Action } catch { if ($_.Exception.Message -notmatch $Pattern) { throw "Unexpected failure: $($_.Exception.Message) (expected $Pattern)" }; return }
    throw "Expected failure matching $Pattern"
}
function Test-Case([string]$Name, [scriptblock]$Action) {
    & $Action
    $script:Passed++
    Write-Host "PASS $Name"
}
function New-Fixture {
    $case = Join-Path $fixtureRoot ([guid]::NewGuid().ToString('N'))
    $repoPath = Join-Path $case 'repo'
    $install = Join-Path $case 'dev'
    foreach ($relative in @('BepInEx\core','BepInEx\plugins\PoVPlus','BepInEx\config')) { [IO.Directory]::CreateDirectory((Join-Path $install $relative)) | Out-Null }
    [IO.Directory]::CreateDirectory((Join-Path $repoPath 'Source\PoVPlus\bin\Release\net48')) | Out-Null
    foreach ($relative in @('valheim.exe','BepInEx\core\BepInEx.dll','BepInEx\core\Mono.Cecil.dll')) { [IO.File]::WriteAllText((Join-Path $install $relative), 'fixture, not executable') }
    New-DevContext -Repository $repoPath -Installation $install -ApprovedInstallation $install
}
function Write-FakePlugin([string]$Path, [string]$Version, [string]$Guid = 'povplus.core') {
    [IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($Path)) | Out-Null
    @{ Assembly='PoVPlus'; Plugins=@(@{ Guid=$Guid; Name='PoVPlus'; Version=$Version; Base='BepInEx.BaseUnityPlugin'; Type='PoVPlus.Plugin' }) } | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $Path
}
function Build-Path($Context) { Join-Path $Context.Repository 'Source\PoVPlus\bin\Release\net48\PoVPlus.dll' }

# Metadata and process probes are mocked INSIDE this isolated module instance. All
# DLLs below are JSON fixtures; no third-party DLLs are copied, loaded or executed.
& $module {
    function script:Read-PluginMetadata([string]$Path, $Context) { Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json }
    function script:Get-DevProcesses { @() }
}

try {
    Test-Case 'reject unexpected development path and repository overlap' {
        $c = New-Fixture
        Assert-Throws { New-DevContext -Repository $c.Repository -Installation $c.Installation } 'Unexpected installation'
        Assert-Throws { New-DevContext -Repository $c.Installation -Installation $c.Installation -ApprovedInstallation $c.Installation } 'separate directories'
        Assert-Throws { New-DevContext -Repository $c.Repository -Installation (Join-Path $c.Repository 'dev') -ApprovedInstallation (Join-Path $c.Repository 'dev') } 'separate directories'
        Assert-Throws { New-DevContext -Repository $c.Repository -Installation 'S:relative' } 'absolute'
    }
    Test-Case 'reject missing installation marker' {
        $c = New-Fixture
        [IO.File]::Delete((Join-Path $c.Installation 'valheim.exe'))
        Assert-Throws { New-DevContext -Repository $c.Repository -Installation $c.Installation -ApprovedInstallation $c.Installation } 'Missing development'
    }
    Test-Case 'planning is read-only and permits only one managed DLL' {
        $c = New-Fixture
        Write-FakePlugin (Build-Path $c) '0.2.0'
        $plan = New-DeploymentPlan $c
        Assert-True (-not $plan.Before.Exists -and $plan.After.Exists) 'first deployment plan'
        Assert-True (-not (Test-Path $c.State) -and -not (Test-Path $c.Target)) 'no plan writes'
        [IO.File]::WriteAllText((Join-Path ([IO.Path]::GetDirectoryName((Build-Path $c))) 'dependency.dll'), '{}')
        Assert-Throws { New-DeploymentPlan $c } 'Unexpected build dependency'
    }
    Test-Case 'reject foreign target and renamed duplicate GUID' {
        $c = New-Fixture
        Write-FakePlugin (Build-Path $c) '0.2.0'
        Write-FakePlugin $c.Target '0.1.0' 'foreign.mod'
        Assert-Throws { New-DeploymentPlan $c } 'Not the expected'
        [IO.File]::Delete($c.Target)
        Write-FakePlugin (Join-Path $c.Installation 'BepInEx\plugins\Other\renamed.dll') '0.1.0'
        Assert-Throws { New-DeploymentPlan $c } 'Duplicate povplus.core'
    }
    Test-Case 'running or unidentifiable game processes block writes' {
        $c = New-Fixture
        Write-FakePlugin (Build-Path $c) '0.2.0'
        & $module { function script:Get-DevProcesses { @([pscustomobject]@{ ProcessId=42; ExecutablePath=$null }) } }
        Assert-Throws { New-DeploymentPlan $c } 'must be stopped'
        & $module { function script:Get-DevProcesses { @() } }
    }
    Test-Case 'verified deployment backup and rollback leave unrelated files intact' {
        $c = New-Fixture
        Write-FakePlugin $c.Target '0.1.0'
        Write-FakePlugin (Build-Path $c) '0.2.0'
        $unrelated = Join-Path ([IO.Path]::GetDirectoryName($c.Target)) 'settings.cfg'
        [IO.File]::WriteAllText($unrelated, 'preserve exactly')
        $oldHash = (Get-FileHash $c.Target).Hash
        $plan = New-DeploymentPlan $c
        Invoke-DevTransition $c $plan
        Assert-True ((Get-FileHash $c.Target).Hash -eq $plan.After.Hash) 'deployed hash'
        Assert-True ((Get-FileHash (Join-Path $plan.Backup 'PoVPlus.dll')).Hash -eq $oldHash) 'backup hash'
        $rollback = New-RollbackPlan $c
        Assert-True ($rollback.After.Hash -eq $oldHash) 'restore previous DLL'
        Show-DevPlan $rollback
        $beforeDryRun = (Get-FileHash $c.Target).Hash
        Invoke-DevTransition $c $rollback -WhatIf
        Assert-True ((Get-FileHash $c.Target).Hash -eq $beforeDryRun -and @(Get-DevBackups $c).Count -eq 1) 'rollback WhatIf writes nothing'
        Invoke-DevTransition $c $rollback
        Assert-True ((Get-FileHash $c.Target).Hash -eq $oldHash) 'rollback hash'
        Assert-True ([IO.File]::ReadAllText($unrelated) -ceq 'preserve exactly') 'unrelated config untouched'
        Assert-True (@(Get-DevBackups $c).Count -eq 2) 'both operations auditable'
    }
    Test-Case 'rollback restores absence only for workflow-managed first deployment' {
        $c = New-Fixture
        Write-FakePlugin (Build-Path $c) '0.2.0'
        Invoke-DevTransition $c (New-DeploymentPlan $c)
        $plan = New-RollbackPlan $c
        Assert-True (-not $plan.After.Exists) 'first deploy prior state absent'
        Invoke-DevTransition $c $plan
        Assert-True (-not (Test-Path $c.Target)) 'managed DLL removed'
    }
    Test-Case 'rollback rejects tampered backup or manually changed deployment' {
        $c = New-Fixture
        Write-FakePlugin $c.Target '0.1.0'
        Write-FakePlugin (Build-Path $c) '0.2.0'
        $plan = New-DeploymentPlan $c
        Invoke-DevTransition $c $plan
        Write-FakePlugin (Join-Path $plan.Backup 'PoVPlus.dll') '9.0.0'
        Assert-Throws { New-RollbackPlan $c } 'Backup hash mismatch'
        Write-FakePlugin $c.Target '9.0.0'
        Assert-Throws { New-RollbackPlan $c } 'changed since'
    }
    Test-Case 'candidate drift fails before replacing original' {
        $c = New-Fixture
        Write-FakePlugin $c.Target '0.1.0'
        Write-FakePlugin (Build-Path $c) '0.2.0'
        $plan = New-DeploymentPlan $c
        Write-FakePlugin (Build-Path $c) '0.3.0'
        Assert-Throws { Invoke-DevTransition $c $plan } 'Candidate changed'
        Assert-True ((Get-FileHash $c.Target).Hash -eq $plan.Before.Hash) 'original preserved'
    }
    Test-Case 'post-replacement failure automatically restores verified original' {
        $c = New-Fixture
        Write-FakePlugin $c.Target '0.1.0'
        Write-FakePlugin (Build-Path $c) '0.2.0'
        $plan = New-DeploymentPlan $c
        & $module {
            $script:OriginalSetState = ${function:Set-ManagedFileState}
            $script:FailOnce = $true
            function script:Set-ManagedFileState($Context, [string]$Stage, $After) {
                & $script:OriginalSetState $Context $Stage $After
                if ($script:FailOnce) { $script:FailOnce=$false; throw 'fixture failure after atomic replacement' }
            }
        }
        Assert-Throws { Invoke-DevTransition $c $plan } 'fixture failure'
        Assert-True ((Get-FileHash $c.Target).Hash -eq $plan.Before.Hash) 'original automatically recovered'
        Assert-True ((Get-DevBackups $c | Select-Object -First 1).Status -eq 'failed-recovered') 'recovery audited'
        & $module { Set-Item Function:script:Set-ManagedFileState -Value $script:OriginalSetState }
        $recoveryPreview = New-RollbackPlan $c $plan.Id
        Assert-True $recoveryPreview.Unchanged 'failed receipt recognizes already recovered state'
    }
    Test-Case 'session parser ignores stale messages and does not infer failed Awake' {
        & $module {
            $lines = @('[Message:   BepInEx] BepInEx 5.4.23.5', '[Info   :   PoVPlus] PoVPlus 0.1.0 loaded.', '[Message:   BepInEx] BepInEx 5.4.23.5', '[Info   :   BepInEx] Loading [PoVPlus 0.1.0] (povplus.core)')
            $parsed = Convert-DevLog $lines
            if ($parsed.Startup -or $parsed.Failure -or -not $parsed.Loading) { throw 'Incorrect session isolation or failure classification' }
            $failure = Convert-DevLog @('[Message:   BepInEx] BepInEx 5.4.23.5','[Error  :   BepInEx] Error loading [PoVPlus 0.1.0] (povplus.core) : failure')
            if (-not $failure.Failure) { throw 'Load failure not detected' }
            $unknown = Convert-DevLog @('[Info   :   PoVPlus] PoVPlus 0.1.0 loaded.')
            if ($unknown.Boundary -or $unknown.Startup) { throw 'Unbounded log treated as current session' }
            $empty = Convert-DevLog @()
            if ($empty.Boundary) { throw 'Empty log treated as bounded session' }
            $pathOnly = Convert-DevLog @('[Message:   BepInEx] BepInEx 5.4.23.5', '[Info   :Other] Read S:\Valheim-Modding\PoVPlus-Dev\unrelated.txt')
            if (@($pathOnly.Lines).Count) { throw 'Installation pathname mistaken for plugin event' }
        }
    }
    Test-Case 'report correlates only referenced profiler session and keeps cfg unchanged' {
        $c = New-Fixture
        $log = Join-Path $c.Installation 'BepInEx\LogOutput.log'
        @('[Message:   BepInEx] BepInEx 5.4.23.5','[Info   :LoadTimeProfiler] Writing startup and connection profiles to X:\anything\2026-10-24_12-00-00.log.','[Info   :   PoVPlus] PoVPlus 0.1.0 loaded.') | Set-Content -LiteralPath $log
        $profileDir = Join-Path $c.Installation 'BepInEx\config\LoadTimeProfiler'
        [IO.Directory]::CreateDirectory($profileDir) | Out-Null
        [IO.File]::WriteAllText((Join-Path $profileDir '2026-10-24_12-00-00.log'), 'Logging at end: default groups=Errors, Warnings; mods=[povplus.core=Information]')
        [IO.File]::WriteAllText((Join-Path $profileDir '2026-10-24_13-00-00.log'), 'WRONG SESSION')
        $report = Get-DevDiagnostic $c
        Assert-True ($report.Parsed.Startup -and $report.Content -notmatch 'WRONG SESSION') 'correct session evidence'
        Assert-True (-not (Test-Path $c.State)) 'diagnostic collection read-only until report save'
        Save-DevReport $c $report
        Assert-True (Test-Path $report.Path) 'local report saved'
    }
    Test-Case 'reject directory junctions including destination redirection' {
        $c = New-Fixture
        $redirect = Join-Path $c.Repository 'redirect'
        New-Item -ItemType Junction -Path $redirect -Target $c.Installation | Out-Null
        Assert-Throws { & $module { param($p) Assert-PlainPath $p } $redirect } 'Reparse points'
        # Remove only the verified junction itself; never recurse into its target.
        [IO.Directory]::Delete($redirect)
    }
    Test-Case 'launcher WhatIf has no side effects and real launch primitive is mocked' {
        $c = New-Fixture
        & $module {
            $script:LaunchCall = $null
            function script:Start-Process {
                param($FilePath,$WorkingDirectory,$ArgumentList,$WindowStyle,[switch]$PassThru)
                $script:LaunchCall = @{ File=$FilePath; Directory=$WorkingDirectory; Arguments=$ArgumentList }
                [pscustomobject]@{ Id=999 }
            }
        }
        $arguments = @('-screen-fullscreen','0','-screen-width','2560','-screen-height','1440','-popupwindow','-console')
        Start-DevGame $c $arguments -WhatIf
        Assert-True (-not (Test-Path $c.State)) 'launch WhatIf writes no lock or state'
        Start-DevGame $c $arguments
        $call = & $module { $script:LaunchCall }
        Assert-True ($call.File -eq (Join-Path $c.Installation 'valheim.exe') -and $call.Directory -eq $c.Installation) 'exact executable and working directory'
        Assert-True (($call.Arguments -join ' ') -ceq ($arguments -join ' ')) 'launcher argument list'
    }
    Write-Host "All $script:Passed workflow fixture tests passed. Real installation was never accessed."
} finally {
    # Every cleanup target was created beneath this unique temporary fixture root.
    $resolved = [IO.Path]::GetFullPath($fixtureRoot)
    $tempRoot = [IO.Path]::GetFullPath([IO.Path]::GetTempPath()).TrimEnd('\') + '\'
    if (-not $resolved.StartsWith($tempRoot, [StringComparison]::OrdinalIgnoreCase) -or [IO.Path]::GetFileName($resolved) -notlike 'PoVPlus-workflow-tests-*') { throw 'Unsafe fixture cleanup target.' }
    # Junction test removes its own link before cleanup; refuse any remaining links.
    $links = @(Get-ChildItem -LiteralPath $resolved -Recurse -Force | Where-Object { $_.Attributes -band [IO.FileAttributes]::ReparsePoint })
    if (-not $links.Count) { Remove-Item -LiteralPath $resolved -Recurse -Force }
    else { Write-Warning "Fixture cleanup skipped due to remaining reparse point: $resolved" }
}
