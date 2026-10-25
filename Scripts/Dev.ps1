[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'Medium')]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [ValidateSet('Build', 'Deploy', 'Launch', 'Diagnose', 'Rollback', 'Status')]
    [string]$Command,
    [ValidateSet('Debug', 'Release')][string]$Configuration = 'Release',
    [string]$ValheimDevPath = 'S:\Valheim-Modding\PoVPlus-Dev',
    [string]$BackupId,
    [switch]$DryRun,
    [switch]$Authorized
)
$ErrorActionPreference = 'Stop'
Import-Module (Join-Path $PSScriptRoot 'DevWorkflow.psm1') -Force
$repoRoot = Split-Path -Parent $PSScriptRoot
$context = New-DevContext -Repository $repoRoot -Installation $ValheimDevPath

switch ($Command) {
    'Build' {
        Write-Host "Build: $repoRoot\Scripts\Build.ps1 -Configuration $Configuration -ValheimDevPath $($context.Installation)"
        if (-not $DryRun -and $PSCmdlet.ShouldProcess($repoRoot, 'Run existing build script')) {
            & (Join-Path $PSScriptRoot 'Build.ps1') -Configuration $Configuration -ValheimDevPath $context.Installation
        }
    }
    'Status' { Get-DevStatus $context $Configuration | Format-List }
    'Diagnose' {
        $report = Get-DevDiagnostic $context
        Write-Host $report.Summary
        Write-Host "Report: $($report.Path)"
        if (-not $DryRun -and $PSCmdlet.ShouldProcess($report.Path, 'Write repository-local diagnostic report')) {
            Save-DevReport $context $report
        }
    }
    'Deploy' {
        $plan = New-DeploymentPlan $context $Configuration
        Show-DevPlan $plan
        if ($plan.Unchanged) { Write-Host 'DLL already matches; no deployment or backup needed.'; return }
        if ($DryRun) { return }
        if ($PSCmdlet.ShouldProcess($context.Target, 'Back up and deploy only PoVPlus.dll')) {
            if (-not $Authorized) { throw 'Real deployment requires explicit user authorization and -Authorized. Use -WhatIf to review.' }
            Invoke-DevTransition $context $plan -Confirm:$false
        }
    }
    'Rollback' {
        $backups = @(Get-DevBackups $context)
        $backups | Select-Object Id, Operation, CreatedUtc, Status, PreviousVersion | Format-Table -AutoSize | Out-Host
        if (-not $backups.Count) { Write-Host 'No workflow backups exist. Nothing to roll back.'; return }
        $plan = New-RollbackPlan $context $BackupId
        Show-DevPlan $plan
        if ($plan.Unchanged) { Write-Host 'Previous state already restored; no changes needed.'; return }
        if ($DryRun) { return }
        if ($PSCmdlet.ShouldProcess($context.Target, "Restore state before operation $($plan.Restores)")) {
            if (-not $Authorized) { throw 'Real rollback requires explicit user authorization and -Authorized.' }
            if (-not $PSCmdlet.ShouldContinue('Replace only the managed PoVPlus.dll with the verified previous state?', 'Confirm PoVPlus rollback')) { return }
            Invoke-DevTransition $context $plan -Confirm:$false
        }
    }
    'Launch' {
        Assert-DevStopped $context
        $arguments = @('-screen-fullscreen', '0', '-screen-width', '2560', '-screen-height', '1440', '-popupwindow', '-console')
        Write-Warning 'This development client may access the SAME character/world saves as the normal game. -savedir is not relied upon for isolation.'
        Write-Host "Launch: $($context.Installation)\valheim.exe $($arguments -join ' ')"
        Write-Host "Working directory: $($context.Installation)"
        Write-Host "CREATE/USE local launch/deployment lock: $($context.State)\workflow.lock"
        if (-not $DryRun -and $PSCmdlet.ShouldProcess($context.Installation, 'Launch development Valheim (shared saves warning above)')) {
            if (-not $Authorized) { throw 'Launching requires separate explicit user authorization and -Authorized.' }
            Start-DevGame $context $arguments -Confirm:$false
        }
    }
}
