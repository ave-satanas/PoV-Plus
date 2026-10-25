Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
$script:ApprovedInstallation = 'S:\Valheim-Modding\PoVPlus-Dev'

function Get-FullDevPath([string]$Path) {
    if ($Path -notmatch '^[A-Za-z]:[\\/]') { throw "An absolute local Windows path is required: $Path" }
    [IO.Path]::GetFullPath($Path).TrimEnd('\', '/')
}

function Assert-PlainPath([string]$Path) {
    $cursor = Get-FullDevPath $Path
    while ($cursor) {
        if (Test-Path -LiteralPath $cursor) {
            $item = Get-Item -LiteralPath $cursor -Force
            if ($item.Attributes -band [IO.FileAttributes]::ReparsePoint) { throw "Reparse points/junctions are not allowed: $cursor" }
        }
        $cursor = [IO.Path]::GetDirectoryName($cursor)
    }
}

function Assert-Within([string]$Path, [string]$Root) {
    $full = Get-FullDevPath $Path
    $base = Get-FullDevPath $Root
    if ($full -ne $base -and -not $full.StartsWith($base + '\', [StringComparison]::OrdinalIgnoreCase)) { throw "Path escaped allowed root: $full" }
    Assert-PlainPath $full
}

function New-DevContext {
    param([string]$Repository, [string]$Installation, [string]$ApprovedInstallation = $script:ApprovedInstallation)
    $repo = Get-FullDevPath $Repository
    $dev = Get-FullDevPath $Installation
    if ($dev -ne (Get-FullDevPath $ApprovedInstallation)) { throw "Unexpected installation. Only $ApprovedInstallation is configured." }
    if ([IO.Path]::GetPathRoot($repo) -ine [IO.Path]::GetPathRoot($dev)) { throw 'Repository-local staging and installation must share a volume for atomic replacement.' }
    if ($dev -eq $repo -or $dev.StartsWith($repo + '\', [StringComparison]::OrdinalIgnoreCase) -or $repo.StartsWith($dev + '\', [StringComparison]::OrdinalIgnoreCase)) { throw 'Repository and installation must be separate directories, neither containing the other.' }
    Assert-PlainPath $repo
    Assert-PlainPath $dev
    foreach ($relative in @('valheim.exe', 'BepInEx\core\BepInEx.dll', 'BepInEx\core\Mono.Cecil.dll')) {
        $path = Join-Path $dev $relative
        Assert-Within $path $dev
        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "Missing development installation file: $path" }
    }
    foreach ($relative in @('BepInEx\plugins', 'BepInEx\config')) {
        $path = Join-Path $dev $relative
        Assert-Within $path $dev
        if (-not (Test-Path -LiteralPath $path -PathType Container)) { throw "Missing installation directory: $path" }
    }
    $target = Join-Path $dev 'BepInEx\plugins\PoVPlus\PoVPlus.dll'
    $state = Join-Path $repo '.dev-state'
    Assert-Within $target $dev
    Assert-Within $state $repo
    [pscustomobject]@{ Repository=$repo; Installation=$dev; Target=$target; State=$state }
}

function Get-DevProcesses {
    @(Get-CimInstance Win32_Process -Filter "Name = 'valheim.exe' OR Name = 'valheim_server.exe'" -ErrorAction Stop)
}

function Get-DevRuntime($Context) {
    foreach ($process in (Get-DevProcesses)) {
        if (-not $process.ExecutablePath) { [pscustomobject]@{ Id=$process.ProcessId; State='Unknown executable path; mutation blocked' }; continue }
        $path = Get-FullDevPath $process.ExecutablePath
        if ($path.StartsWith($Context.Installation + '\', [StringComparison]::OrdinalIgnoreCase)) {
            [pscustomobject]@{ Id=$process.ProcessId; State='Development game/server running' }
        }
    }
}

function Assert-DevStopped($Context) {
    $running = @(Get-DevRuntime $Context)
    if ($running.Count) { throw "Game/server must be stopped (or process location could not be verified): $($running.Id -join ', ')" }
}

function Read-PluginMetadata([string]$Path, $Context) {
    Assert-PlainPath $Path
    if (-not ('Mono.Cecil.AssemblyDefinition' -as [type])) { Add-Type -LiteralPath (Join-Path $Context.Installation 'BepInEx\core\Mono.Cecil.dll') }
    $assembly = $null
    try {
        $assembly = [Mono.Cecil.AssemblyDefinition]::ReadAssembly($Path)
        $plugins = @()
        $pending = New-Object 'Collections.Generic.Stack[Mono.Cecil.TypeDefinition]'
        foreach ($type in $assembly.MainModule.Types) { $pending.Push($type) }
        while ($pending.Count) {
            $type = $pending.Pop()
            foreach ($nested in $type.NestedTypes) { $pending.Push($nested) }
            foreach ($attribute in $type.CustomAttributes) {
                if ($attribute.AttributeType.FullName -eq 'BepInEx.BepInPlugin') {
                    if ($attribute.ConstructorArguments.Count -ne 3) { throw "Unreadable BepInPlugin identity in $Path" }
                    $plugins += [pscustomobject]@{ Guid=[string]$attribute.ConstructorArguments[0].Value; Name=[string]$attribute.ConstructorArguments[1].Value; Version=[string]$attribute.ConstructorArguments[2].Value; Base=[string]$type.BaseType; Type=$type.FullName }
                }
            }
        }
        [pscustomobject]@{ Assembly=$assembly.Name.Name; Plugins=$plugins }
    } catch { throw "Cannot safely inspect assembly $Path : $($_.Exception.Message)" }
    finally { if ($assembly) { $assembly.Dispose() } }
}

function Assert-PoVPlusIdentity([string]$Path, $Context) {
    $metadata = Read-PluginMetadata $Path $Context
    $plugins = @($metadata.Plugins)
    if ($metadata.Assembly -ne 'PoVPlus' -or $plugins.Count -ne 1 -or $plugins[0].Guid -cne 'povplus.core' -or $plugins[0].Name -cne 'PoVPlus' -or $plugins[0].Base -ne 'BepInEx.BaseUnityPlugin') { throw "Not the expected standalone PoVPlus plugin: $Path" }
    $plugins[0]
}

function Get-PlainFiles([string]$Root) {
    Assert-PlainPath $Root
    $pending = New-Object 'Collections.Generic.Stack[string]'
    $pending.Push($Root)
    while ($pending.Count) {
        foreach ($item in (Get-ChildItem -LiteralPath $pending.Pop() -Force)) {
            Assert-Within $item.FullName $Root
            if ($item.PSIsContainer) { $pending.Push($item.FullName) } else { $item }
        }
    }
}

function Assert-NoDuplicatePlugin($Context) {
    foreach ($file in (Get-PlainFiles (Join-Path $Context.Installation 'BepInEx\plugins'))) {
        if ($file.Extension -ine '.dll' -or $file.FullName -ieq $Context.Target) { continue }
        $metadata = Read-PluginMetadata $file.FullName $Context
        if (@($metadata.Plugins | Where-Object { $_.Guid -ceq 'povplus.core' }).Count) { throw "Duplicate povplus.core installation: $($file.FullName)" }
    }
}

function Get-ManagedState($Context) {
    Assert-Within $Context.Target $Context.Installation
    if (-not (Test-Path -LiteralPath $Context.Target)) { return [pscustomobject]@{ Exists=$false; Hash=$null; Version=$null } }
    $identity = Assert-PoVPlusIdentity $Context.Target $Context
    [pscustomobject]@{ Exists=$true; Hash=(Get-FileHash -LiteralPath $Context.Target -Algorithm SHA256).Hash; Version=$identity.Version }
}

function Assert-StateEqual($Expected, $Actual) {
    if ($Expected.Exists -ne $Actual.Exists -or $Expected.Hash -ne $Actual.Hash) { throw 'Managed DLL changed since planning/backup. Stop and inspect; no blind overwrite allowed.' }
}

function New-TransitionPlan($Context, [string]$Operation, [string]$Source, $After, [string]$Restores) {
    Assert-DevStopped $Context
    Assert-NoDuplicatePlugin $Context
    $before = Get-ManagedState $Context
    $id = [DateTime]::UtcNow.ToString('yyyyMMddTHHmmssfffffffZ') + '-' + [guid]::NewGuid().ToString('N')
    [pscustomobject]@{ Id=$id; Operation=$Operation; Source=$Source; Target=$Context.Target; Before=$before; After=$After; Restores=$Restores; Backup=(Join-Path $Context.State "backups\$id"); Stage=(Join-Path $Context.State "staging\$id\PoVPlus.dll"); Unchanged=($before.Exists -eq $After.Exists -and $before.Hash -eq $After.Hash) }
}

function New-DeploymentPlan($Context, [string]$Configuration = 'Release') {
    $output = Join-Path $Context.Repository "Source\PoVPlus\bin\$Configuration\net48"
    Assert-Within $output $Context.Repository
    $dll = Join-Path $output 'PoVPlus.dll'
    if (-not (Test-Path -LiteralPath $dll -PathType Leaf)) { throw "Build first: missing $dll" }
    foreach ($file in (Get-PlainFiles $output)) {
        if ($file.Extension -ieq '.dll' -and $file.FullName -ine $dll) { throw "Unexpected build dependency DLL: $($file.FullName)" }
    }
    $identity = Assert-PoVPlusIdentity $dll $Context
    $after = [pscustomobject]@{ Exists=$true; Hash=(Get-FileHash -LiteralPath $dll -Algorithm SHA256).Hash; Version=$identity.Version }
    New-TransitionPlan $Context 'Deploy' $dll $after ''
}

function Get-DevBackups($Context) {
    $root = Join-Path $Context.State 'backups'
    Assert-Within $root $Context.State
    if (-not (Test-Path -LiteralPath $root)) { return }
    foreach ($dir in (Get-ChildItem -LiteralPath $root -Directory | Sort-Object Name -Descending)) {
        Assert-Within $dir.FullName $root
        $path = Join-Path $dir.FullName 'manifest.json'
        Assert-Within $path $root
        if (Test-Path -LiteralPath $path) {
            $entry = Get-Content -LiteralPath $path -Raw | ConvertFrom-Json
            [pscustomobject]@{ Id=$dir.Name; Operation=$entry.Operation; CreatedUtc=$entry.CreatedUtc; Status=$entry.Status; PreviousVersion=$entry.Before.Version; Manifest=$entry; Directory=$dir.FullName }
        }
    }
}

function New-RollbackPlan($Context, [string]$BackupId) {
    $entries = @(Get-DevBackups $Context)
    if ($BackupId) {
        if ($BackupId -notmatch '^[A-Za-z0-9-]+$') { throw 'Invalid backup identifier.' }
        $entries = @($entries | Where-Object { $_.Id -ceq $BackupId })
    } else {
        $entries = @($entries | Where-Object { $_.Status -eq 'completed' })
    }
    if (-not $entries.Count) { throw 'No matching completed workflow backup.' }
    $entry = $entries[0]
    $manifest = $entry.Manifest
    if ($manifest.Schema -ne 1 -or $manifest.Installation -ine $Context.Installation -or $manifest.Target -ine $Context.Target -or $manifest.Repository -ine $Context.Repository -or $manifest.ManagedFile -cne 'PoVPlus.dll') { throw 'Backup belongs to a different installation/repository or managed file.' }
    if ($manifest.Id -cne $entry.Id -or $manifest.Operation -notin @('Deploy','Rollback')) { throw 'Malformed backup receipt identity.' }
    foreach ($state in @($manifest.Before, $manifest.After)) {
        if ($state.Exists -isnot [bool] -or ($state.Exists -and $state.Hash -notmatch '^[A-Fa-f0-9]{64}$') -or (-not $state.Exists -and $state.Hash)) { throw 'Malformed backup receipt state/hash.' }
    }
    $current = Get-ManagedState $Context
    # Explicit failed/prepared receipts support recovery only from a recognized state.
    if ($manifest.Status -ne 'completed' -and $current.Exists -eq $manifest.Before.Exists -and $current.Hash -eq $manifest.Before.Hash) {
        Assert-StateEqual $manifest.Before $current
    } else { Assert-StateEqual $manifest.After $current }
    $source = ''
    if ($manifest.Before.Exists) {
        $source = Join-Path $entry.Directory 'PoVPlus.dll'
        Assert-Within $source (Join-Path $Context.State 'backups')
        $identity = Assert-PoVPlusIdentity $source $Context
        if ((Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash -ne $manifest.Before.Hash) { throw 'Backup hash mismatch. Refusing rollback.' }
    }
    New-TransitionPlan $Context 'Rollback' $source $manifest.Before $entry.Id
}

function Show-DevPlan($Plan) {
    Write-Host "Operation: $($Plan.Operation); previous version: $($Plan.Before.Version); proposed version: $($Plan.After.Version)"
    Write-Host "Before SHA256: $($Plan.Before.Hash)"
    Write-Host "After  SHA256: $($Plan.After.Hash)"
    if ($Plan.Unchanged) { Write-Host "UNCHANGED $($Plan.Target)"; return }
    if ($Plan.Before.Exists) { Write-Host "CREATE backup $($Plan.Backup)\PoVPlus.dll" }
    Write-Host "CREATE audit $($Plan.Backup)\manifest.json (prepared/completed/failure state)"
    Write-Host "CREATE transient audit $($Plan.Backup)\manifest.json.tmp (consumed by atomic journal updates)"
    Write-Host "CREATE/USE repository lock $([IO.Path]::GetDirectoryName([IO.Path]::GetDirectoryName($Plan.Backup)))\workflow.lock"
    if ($Plan.After.Exists) { Write-Host "CREATE transient staging $($Plan.Stage) from $($Plan.Source); consumed on success" }
    $action = if (-not $Plan.After.Exists) { 'REMOVE managed DLL' } elseif ($Plan.Before.Exists) { 'REPLACE managed DLL' } else { 'CREATE managed DLL' }
    Write-Host "$action $($Plan.Target)"
    Write-Host 'Create parent directories if missing; keep all unrelated files, PDBs and configs unchanged.'
}

function Write-DevJson([string]$Path, $Value) {
    $temporary = $Path + '.tmp'
    [IO.File]::WriteAllText($temporary, ($Value | ConvertTo-Json -Depth 10), (New-Object Text.UTF8Encoding($false)))
    if (Test-Path -LiteralPath $Path) { [IO.File]::Replace($temporary, $Path, [NullString]::Value) }
    else { [IO.File]::Move($temporary, $Path) }
}

function Set-ManagedFileState($Context, [string]$Stage, $After) {
    Assert-Within $Context.Target $Context.Installation
    if ($After.Exists) {
        if (Test-Path -LiteralPath $Context.Target) { [IO.File]::Replace($Stage, $Context.Target, [NullString]::Value) }
        else { [IO.File]::Move($Stage, $Context.Target) }
    } else { [IO.File]::Delete($Context.Target) }
}

function Invoke-DevTransition {
    [CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'Medium')]
    param($Context, $Plan)
    # Internal transaction primitive. Public Dev.ps1 supplies authorization/ShouldProcess gates.
    if ($Plan.Unchanged) { return }
    if (-not $PSCmdlet.ShouldProcess($Context.Target, "$($Plan.Operation) managed PoVPlus DLL with verified backup")) { return }
    Assert-Within $Context.State $Context.Repository
    Assert-Within $Plan.Backup $Context.State
    Assert-Within $Plan.Stage $Context.State
    [IO.Directory]::CreateDirectory($Context.State) | Out-Null
    $lock = $null
    $manifest = $null
    $manifestPath = Join-Path $Plan.Backup 'manifest.json'
    try {
        $lock = [IO.File]::Open((Join-Path $Context.State 'workflow.lock'), [IO.FileMode]::OpenOrCreate, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
        Assert-DevStopped $Context
        Assert-NoDuplicatePlugin $Context
        Assert-StateEqual $Plan.Before (Get-ManagedState $Context)
        if ($Plan.After.Exists) {
            $null = Assert-PoVPlusIdentity $Plan.Source $Context
            if ((Get-FileHash -LiteralPath $Plan.Source -Algorithm SHA256).Hash -ne $Plan.After.Hash) { throw 'Candidate changed after planning.' }
        }
        [IO.Directory]::CreateDirectory($Plan.Backup) | Out-Null
        if ($Plan.Before.Exists) {
            [IO.File]::Copy($Context.Target, (Join-Path $Plan.Backup 'PoVPlus.dll'), $false)
            if ((Get-FileHash -LiteralPath (Join-Path $Plan.Backup 'PoVPlus.dll')).Hash -ne $Plan.Before.Hash) { throw 'Backup verification failed; target untouched.' }
        }
        $manifest = [pscustomobject]@{ Schema=1; Id=$Plan.Id; Operation=$Plan.Operation; CreatedUtc=[DateTime]::UtcNow.ToString('o'); Repository=$Context.Repository; Installation=$Context.Installation; Target=$Context.Target; ManagedFile='PoVPlus.dll'; Before=$Plan.Before; After=$Plan.After; Restores=$Plan.Restores; Status='prepared'; Error=$null }
        Write-DevJson $manifestPath $manifest
        if ($Plan.After.Exists) {
            [IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($Plan.Stage)) | Out-Null
            [IO.File]::Copy($Plan.Source, $Plan.Stage, $false)
            if ((Get-FileHash -LiteralPath $Plan.Stage).Hash -ne $Plan.After.Hash) { throw 'Staging hash mismatch; target untouched.' }
        }
        Assert-DevStopped $Context
        Assert-NoDuplicatePlugin $Context
        Assert-StateEqual $Plan.Before (Get-ManagedState $Context)
        Assert-Within ([IO.Path]::GetDirectoryName($Context.Target)) $Context.Installation
        [IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($Context.Target)) | Out-Null
        Set-ManagedFileState $Context $Plan.Stage $Plan.After
        Assert-StateEqual $Plan.After (Get-ManagedState $Context)
        $manifest.Status = 'completed'
        Write-DevJson $manifestPath $manifest
        Write-Host "$($Plan.Operation) verified. Audit/backup: $manifestPath"
    } catch {
        $failure = $_.Exception.Message
        if ($manifest) {
            try {
                $current = Get-ManagedState $Context
                if ($current.Exists -eq $Plan.Before.Exists -and $current.Hash -eq $Plan.Before.Hash) {
                    $manifest.Status = 'failed-original-intact'
                } elseif ($current.Exists -eq $Plan.After.Exists -and $current.Hash -eq $Plan.After.Hash) {
                    Assert-DevStopped $Context
                    $restoreStage = Join-Path $Plan.Backup 'recovery-staged.dll'
                    if ($Plan.Before.Exists) {
                        [IO.File]::Copy((Join-Path $Plan.Backup 'PoVPlus.dll'), $restoreStage, $false)
                        if ((Get-FileHash -LiteralPath $restoreStage).Hash -ne $Plan.Before.Hash) { throw 'Recovery backup hash mismatch.' }
                    }
                    Set-ManagedFileState $Context $restoreStage $Plan.Before
                    Assert-StateEqual $Plan.Before (Get-ManagedState $Context)
                    $manifest.Status = 'failed-recovered'
                } else { $manifest.Status = 'failed-manual-recovery-required' }
            } catch { $manifest.Status = 'failed-manual-recovery-required' }
            $manifest.Error = $failure
            Write-DevJson $manifestPath $manifest
        }
        throw "Operation failed: $failure. Previous DLL, if present, is preserved at $($Plan.Backup)\PoVPlus.dll when backup completed. Inspect $manifestPath; stop the game and use the documented recovery procedure. Never overwrite an unrecognized current DLL."
    } finally { if ($lock) { $lock.Dispose() } }
}

function Get-DevStatus($Context, [string]$Configuration = 'Release') {
    $build = Join-Path $Context.Repository "Source\PoVPlus\bin\$Configuration\net48\PoVPlus.dll"
    Assert-Within $build $Context.Repository
    $backups = @(Get-DevBackups $Context)
    $runtime = @(Get-DevRuntime $Context)
    $log = Join-Path $Context.Installation 'BepInEx\LogOutput.log'
    $profiles = Join-Path $Context.Installation 'BepInEx\config\LoadTimeProfiler'
    Assert-Within $log $Context.Installation
    Assert-Within $profiles $Context.Installation
    $latest = if (Test-Path -LiteralPath $profiles) { Get-ChildItem -LiteralPath $profiles -File -Filter '*.log' | Sort-Object LastWriteTimeUtc -Descending | Select-Object -First 1 }
    $deployed = Get-ManagedState $Context
    [pscustomobject]@{
        Repository=$Context.Repository; Installation=$Context.Installation; Build=$build
        BuildSHA256=$(if (Test-Path -LiteralPath $build) { (Get-FileHash -LiteralPath $build).Hash } else { 'not built' })
        Deployed=$Context.Target; DeployedSHA256=$deployed.Hash; DeployedVersion=$deployed.Version
        Runtime=$(if ($runtime.Count) { $runtime.State -join '; ' } else { 'Stopped' })
        LatestBackup=$(if ($backups.Count) { "$($backups[0].Id) [$($backups[0].Status)] previous=$($backups[0].PreviousVersion)" } else { 'none' })
        BepInExLogUtc=$(if (Test-Path -LiteralPath $log) { (Get-Item -LiteralPath $log).LastWriteTimeUtc.ToString('o') } else { 'absent' })
        LatestProfilerLog=$(if ($latest) { "$($latest.FullName) [$($latest.LastWriteTimeUtc.ToString('o'))] (not necessarily the same session)" } else { 'absent' })
    }
}

function Start-DevGame {
    [CmdletBinding(SupportsShouldProcess = $true)]
    param($Context, [string[]]$Arguments)
    if (-not $PSCmdlet.ShouldProcess($Context.Installation, 'Start approved development client')) { return }
    Assert-Within $Context.State $Context.Repository
    Assert-Within (Join-Path $Context.Installation 'valheim.exe') $Context.Installation
    [IO.Directory]::CreateDirectory($Context.State) | Out-Null
    $lock = $null
    try {
        $lock = [IO.File]::Open((Join-Path $Context.State 'workflow.lock'), [IO.FileMode]::OpenOrCreate, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
        Assert-DevStopped $Context
        $process = Start-Process -FilePath (Join-Path $Context.Installation 'valheim.exe') -WorkingDirectory $Context.Installation -ArgumentList $Arguments -WindowStyle Normal -PassThru
        Write-Host "Development process started: $($process.Id)"
    } finally { if ($lock) { $lock.Dispose() } }
}

function Convert-DevLog([string[]]$Lines) {
    $Lines = @($Lines)
    $start = -1
    for ($i=0; $i -lt $Lines.Count; $i++) { if ($Lines[$i] -match '^\[Message:\s*BepInEx\] BepInEx \d') { $start=$i } }
    if ($start -lt 0) { return [pscustomobject]@{ Boundary=$false; Lines=@(); Startup=$false; Failure=$false; Profile=$null; Loading=$false } }
    $selected = @()
    for ($i=$start; $i -lt $Lines.Count; $i++) {
        if ($Lines[$i] -match '\[.*:\s*PoVPlus\]|povplus\.core|PoVPlus\.Plugin|PoVPlus \d|Chainloader startup complete|\[Error|Exception|\[Warning.*Harmony|compatibility.*unavailable|Writing startup and connection profiles') {
            $selected += [pscustomobject]@{ Number=$i+1; Text=$Lines[$i] }
        }
    }
    $session = @($Lines[$start..($Lines.Count-1)])
    $profile = $null
    foreach ($line in $session) { if ($line -match 'Writing startup and connection profiles.*?(\d{4}-\d{2}-\d{2}_\d{2}-\d{2}-\d{2}\.log)') { $profile=$Matches[1] } }
    [pscustomobject]@{
        Boundary=$true; Lines=$selected; Profile=$profile
        Startup=(@($session | Where-Object { $_ -match '^\[Info\s*:\s*PoVPlus\] PoVPlus .+ loaded\.$' }).Count -gt 0)
        Loading=(@($session | Where-Object { $_ -match 'Loading \[PoVPlus .+\] \(povplus\.core\)' }).Count -gt 0)
        Failure=(@($session | Where-Object { $_ -match '^\[Error\s*:\s*BepInEx\].*Error loading \[PoVPlus' }).Count -gt 0)
    }
}

function Protect-DevExcerpt([string]$Text) {
    $safe = $Text -replace '(?i)[A-Z]:\\Users\\[^\\\s]+', '<user-profile>'
    $safe = $safe -replace '(?i)(token|password|secret|apikey|api_key)\s*[=:]\s*[^\s,;]+', '$1=<redacted>'
    if ($safe.Length -gt 400) { $safe=$safe.Substring(0,400) + ' ...' }
    $safe.Replace('`', "'")
}

function Get-DevDiagnostic($Context) {
    $log = Join-Path $Context.Installation 'BepInEx\LogOutput.log'
    $config = Join-Path $Context.Installation 'BepInEx\config\sighsorry.LoadTimeProfiler.cfg'
    $bepConfig = Join-Path $Context.Installation 'BepInEx\config\BepInEx.cfg'
    foreach ($path in @($log,$config,$bepConfig)) { Assert-Within $path $Context.Installation }
    $lines = if (Test-Path -LiteralPath $log) { @(Get-Content -LiteralPath $log) } else { @() }
    $parsed = Convert-DevLog $lines
    $profilePath = $null
    $profileEvidence = @()
    if ($parsed.Profile) {
        $profilePath = Join-Path $Context.Installation ('BepInEx\config\LoadTimeProfiler\' + $parsed.Profile)
        Assert-Within $profilePath $Context.Installation
        if (Test-Path -LiteralPath $profilePath) {
            $profileEvidence = @(Select-String -LiteralPath $profilePath -Pattern '^Session:|^Result:|^Logging |PoVPlus' | Select-Object -First 12 | ForEach-Object { "$($_.LineNumber): $(Protect-DevExcerpt $_.Line)" })
        }
    }
    $currentRule = 'No explicit povplus.core rule; documented default is Errors, Warnings. Current cfg is not proof of past effective policy.'
    if (Test-Path -LiteralPath $config) {
        $section = ''
        foreach ($line in (Get-Content -LiteralPath $config)) {
            if ($line -match '^\s*\[([^\]]+)\]') { $section=$Matches[1] }
            if ($section -ceq 'Logging.Mods' -and $line -cmatch '^\s*povplus\.core\s*=\s*(.*)$') { $currentRule='Current cfg: povplus.core = ' + $Matches[1] }
        }
    }
    $summary = if (-not $parsed.Boundary) { 'Session boundary unavailable; initialization remains unverified.' } elseif ($parsed.Failure) { 'BepInEx reports a PoVPlus loading failure; inspect the selected session evidence.' } elseif ($parsed.Startup) { 'PoVPlus own startup message observed in the selected session.' } else { 'PoVPlus startup message not observed; initialization remains unverified. Per-plugin filtering may explain absence.' }
    $reportPath = Join-Path $Context.State ('reports\' + [DateTime]::UtcNow.ToString('yyyyMMddTHHmmssfffffffZ') + '-' + [guid]::NewGuid().ToString('N') + '.md')
    $body = @('# PoVPlus development diagnostic', '', "Collected UTC: $([DateTime]::UtcNow.ToString('o'))", '', $summary, '', "Source: $log", "Log LastWriteTimeUtc: $(if (Test-Path -LiteralPath $log) { (Get-Item -LiteralPath $log).LastWriteTimeUtc.ToString('o') } else { 'absent' })", '', 'Only the last BepInEx startup boundary is selected. This may be a prior run, not a currently running process.', '', '## Logging policy', '', $currentRule, "Configuration source: $config", 'LoadTimeProfiler may filter before BepInEx disk/console listeners; Info allowance there is insufficient.', '', "Correlated profile source: $(if ($profilePath) { $profilePath } else { 'none; latest-by-time profiles are not mixed into this session' })", '')
    $body += $profileEvidence
    $body += @('', "BepInEx listener configuration source: $bepConfig", '')
    if (Test-Path -LiteralPath $bepConfig) {
        $body += @(Select-String -LiteralPath $bepConfig -Pattern '^\[Logging|^Enabled\s*=|^LogLevels\s*=' | ForEach-Object { "$($_.LineNumber): $(Protect-DevExcerpt $_.Line)" })
    }
    $body += @('', '## Selected log excerpts (at most 30; original line numbers)', '')
    $errors = @($parsed.Lines | Where-Object { $_.Text -match '\[Error|Exception' })
    $initialization = @($parsed.Lines | Where-Object { $_.Text -match '\[.*:\s*PoVPlus\]|povplus\.core|PoVPlus\.Plugin|PoVPlus \d|Chainloader startup complete|Writing startup' })
    $warnings = @($parsed.Lines | Where-Object { $_.Text -match '\[Warning' })
    $body += "Selected-session counts: $($errors.Count) error/exception lines; $($warnings.Count) matching warning lines. These are line counts, not distinct defects."
    $excerpts = @(@($errors | Select-Object -First 10) + @($initialization | Select-Object -First 10) + @($warnings | Select-Object -First 10) | Sort-Object Number -Unique)
    $body += @($excerpts | ForEach-Object { "$($_.Number): $(Protect-DevExcerpt $_.Text)" })
    $body += @('', '## Interpretation and unresolved questions', '', '- A loading announcement precedes AddComponent and does not prove Awake execution.', '- A missing/filtered event is distinct from a confirmed loading failure. Profile combined timing is not instruction-level proof.', '- Current cfg may differ from startup policy; prefer the correlated profiler snapshot.', '- Errors/exceptions require attribution; Harmony API-probe warnings do not automatically imply a conflict.', '- Active patch ownership/order, warnings outside selected excerpts, and gameplay effects remain unverified.', '- Reports are local and ignored; inspect/redact further before sharing. No logging settings were changed.')
    [pscustomobject]@{ Path=$reportPath; Summary=$summary; Content=($body -join [Environment]::NewLine); Parsed=$parsed }
}

function Save-DevReport($Context, $Report) {
    Assert-Within $Report.Path $Context.State
    [IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($Report.Path)) | Out-Null
    [IO.File]::WriteAllText($Report.Path, $Report.Content, (New-Object Text.UTF8Encoding($false)))
}

Export-ModuleMember -Function New-DevContext, Assert-DevStopped, New-DeploymentPlan, New-RollbackPlan, Show-DevPlan, Invoke-DevTransition, Get-DevBackups, Get-DevStatus, Get-DevDiagnostic, Save-DevReport, Start-DevGame
