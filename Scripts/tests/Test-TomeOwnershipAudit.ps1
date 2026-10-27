[CmdletBinding()]
param([string]$Metadata='S:\Valheim-Modding\PoV-Plus\.dev-state\c1-audit-continuation-20261026\metadata.json')
$ErrorActionPreference='Stop'
# Contract assertions against a freshly exported, hash-verified Cecil data set. No mod execution.
$rows=Get-Content -LiteralPath $Metadata -Raw | ConvertFrom-Json
$passed=0
function Check([bool]$ok,[string]$label){if(-not $ok){throw "Ownership audit failed: $label"};$script:passed++;Write-Host "PASS $label"}
function AuditType($label,$name){@($rows|Where-Object label -eq $label)[0].types|Where-Object name -eq $name}
function AuditMethod($t,$name){@($t.methods|Where-Object {$_.name -match ('::'+[regex]::Escape($name)+'\(')})[0]}
Check ($rows.Count -eq 10) 'ten required assembly sources exported'
foreach($a in $rows){Check ((Get-FileHash -LiteralPath $a.path).Hash -ceq $a.sha256) "source unchanged since IL read: $($a.label)"}
$ms=AuditType MS 'Magic_Supremacy.CustomSlotSystem'
Check $ms.beforeFieldInit 'MS is beforefieldinit'
$claim=AuditMethod $ms TryClaimSystemOwner
Check (($claim.il -join "`n") -match 'SetData' -and ($claim.il -join "`n") -notmatch 'ldsfld') 'owner marker setter does not require a static-field read'
Check (((AuditMethod $ms Log).il -join '') -eq 'IL_0000: ret') 'MS Log supplies no initialization evidence'
Check (((AuditMethod $ms '.cctor').il -join "`n") -match 'stsfld.*::RuntimeData') 'MS initializer creates runtime ownership table'
Check (((AuditMethod $ms GetEquippedItem).il -join "`n") -match '::GetRuntimeData') 'equipped getter reaches lazy data getter'
Check (((AuditMethod $ms GetRuntimeData).il -join "`n") -match '::GetOrCreateValue') 'data getter can create foreign state'
$ui=AuditType UI 'SeneaLUI.Patches.InventoryGuiPatches'
Check ($ui.beforeFieldInit -and ((AuditMethod $ui '.cctor').il -join "`n") -match 'stsfld') 'UI static holder has state-changing initialization'
Check (((AuditMethod (AuditType UI 'SeneaLUI.Inv.CustomStore') Claimed).il -join "`n") -match 'stsfld.*::_plainBelt') 'Claimed writes cache'
$observer=AuditType Observer 'PoVPlus.Diagnostics.TomeLifecycleObserver'
$static=AuditMethod $observer InitializedField
Check (($static.il -join "`n") -match 'LifecycleRead::Field' -and ($static.il -join "`n") -notmatch 'FieldInfo::GetValue') 'compiled candidate static paths route through refusal'
$field=AuditMethod (AuditType Observer 'PoVPlus.Diagnostics.LifecycleRead') Field
$il=$field.il -join "`n"
Check ($il -match 'get_IsStatic' -and $il.IndexOf('get_IsStatic') -lt $il.IndexOf('FieldInfo::GetValue') -and $il -match 'throw') 'field reader checks static status before raw read'
$calls=@($observer.methods | ForEach-Object {$_.il} | Where-Object {$_ -match ' (call|callvirt|newobj) '}) -join "`n"
Check ($calls -notmatch '::(GetEquippedItem|GetRuntimeData|Claimed|EquipItem|UnequipItem|SetEquippedItem|GetOrCreateValue|PatchAll|Patch)\(') 'observer does not call forbidden equipment/cache APIs'
Check ($calls -notmatch '::add_') 'observer subscribes to no events'
Check ($calls -notmatch 'PoVPlus.Compatibility.*::') 'observer does not invoke bridge policy or lifecycle'
Write-Host "All $passed ownership IL/hash assertions passed. Native calls, live patches and collection completeness remain unproved."
