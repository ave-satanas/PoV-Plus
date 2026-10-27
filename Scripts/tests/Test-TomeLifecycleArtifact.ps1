[CmdletBinding()]
param([switch]$Enabled,[string]$Dll='S:\Valheim-Modding\PoV-Plus\Source\PoVPlus\bin\Release\net48\PoVPlus.dll',[string]$ValheimDevPath='S:\Valheim-Modding\PoVPlus-Dev')
$ErrorActionPreference='Stop'
Add-Type -LiteralPath (Join-Path $ValheimDevPath 'BepInEx\core\Mono.Cecil.dll')
$a=[Mono.Cecil.AssemblyDefinition]::ReadAssembly((Resolve-Path -LiteralPath $Dll).Path)
function Walk($Types){foreach($t in $Types){$t;Walk $t.NestedTypes}}
try {
 $observer=$a.MainModule.GetType('PoVPlus.Diagnostics.TomeLifecycleObserver')
 $policy=$a.MainModule.GetType('PoVPlus.Diagnostics.LifecycleWindow')
 $plugin=$a.MainModule.GetType('PoVPlus.Plugin')
 if(-not $Enabled){
  if($observer -or $policy -or @($a.MainModule.Types | Where-Object Namespace -eq 'PoVPlus.Diagnostics').Count -or $plugin.Methods.Name -contains 'Update'){throw 'Ordinary artifact contains diagnostic code/polling'}
  Write-Host 'PASS ordinary artifact excludes all diagnostics and polling';return
 }
 if(-not $observer -or -not $policy){throw 'Missing C1 observer/policy'}
 $ctor=$observer.Methods | Where-Object IsConstructor | Where-Object {-not $_.IsStatic}
 if($ctor.Parameters.Count -ne 2 -or $ctor.Parameters[1].ParameterType.FullName -ne 'System.Boolean'){throw 'C1 requires bound immutable setting snapshot'}
 $awake=$plugin.Methods | Where-Object Name -eq 'Awake'
 $news=@($awake.Body.Instructions | Where-Object {$_.OpCode.Code -eq 'Newobj' -and $_.Operand -is [Mono.Cecil.MethodReference] -and $_.Operand.DeclaringType.FullName -like 'PoVPlus.Diagnostics.*'})
 if($news.Count -ne 1 -or $news[0].Operand.DeclaringType.FullName -ne $observer.FullName -or $news[0].Previous.Operand.Name -ne 'get_Value'){throw 'C1 initialization must exclusively pass bound setting directly'}
 $types=@(Walk $a.MainModule.Types | Where-Object { $_.FullName -match '^PoVPlus.Diagnostics.(TomeLifecycleObserver|LifecycleEvidence|LifecycleWindow|LifecycleRead)(/|$)' })
 $calls=0
 foreach($t in $types){foreach($m in $t.Methods){if(-not $m.HasBody){continue};foreach($i in $m.Body.Instructions){
  $o=$i.Operand
  if($i.OpCode.Code -in @('Stfld','Stsfld') -and $o -is [Mono.Cecil.FieldReference] -and $o.DeclaringType.FullName -notmatch '^PoVPlus.Diagnostics.(TomeLifecycleObserver|LifecycleEvidence|LifecycleWindow|LifecycleRead)(/|$)'){throw "Foreign field write: $m $i"}
  if($o -is [Mono.Cecil.MethodReference]){
   $calls++
   if($i.OpCode.Code -eq 'Newobj' -and $o.DeclaringType.FullName -notmatch '^(System\.|PoVPlus\.Diagnostics\.)'){throw "Foreign object construction: $m $i"}
   if($o.Name -eq 'Invoke' -and $o.DeclaringType.FullName -like 'System.Reflection.*' -and -not ($t.FullName -eq 'PoVPlus.Diagnostics.LifecycleRead' -and $m.Name -eq 'ExistingEntry')){throw "Unreviewed reflective invocation: $m $i"}
   if($o.Name -match '^(EquipItem|UnequipItem|GetEquippedItem|GetRuntimeData|GetDefinitionBySlotId|Claimed|GetOrCreateValue|GetValue|SetValue|set_Value|GetOrCreateItemGuid|AddItem|RemoveItem|AddStatusEffect|RemoveStatusEffect|Instantiate|Destroy|SetData|Save|WriteAllText|WriteAllBytes|CreateDirectory|Patch|PatchAll|Unpatch|UnpatchAll)$'){
    # FieldInfo.GetValue is a raw CLR read, not ConditionalWeakTable.GetValue(factory).
    if(-not ($o.Name -eq 'GetValue' -and $o.DeclaringType.FullName -eq 'System.Reflection.FieldInfo')){throw "Prohibited observation call: $m $i"}
   }
   if($o.DeclaringType.FullName -eq 'System.Reflection.PropertyInfo' -and $o.Name -eq 'GetValue'){throw 'Property invocation prohibited'}
  }
 }} }
 $read=$a.MainModule.GetType('PoVPlus.Diagnostics.LifecycleRead')
 $existing=$read.Methods | Where-Object Name -eq 'ExistingEntry'
 $strings=@($existing.Body.Instructions | Where-Object OpCode -match 'ldstr' | ForEach-Object Operand)
 if($strings -notcontains 'TryGetValue' -or $strings -contains 'GetOrCreateValue'){throw 'Noncreating lookup contract missing'}
 $field=$read.Methods | Where-Object Name -eq 'Field'
 $staticRead=$observer.Methods | Where-Object Name -eq 'InitializedField'
 if(@($staticRead.Body.Instructions | Where-Object {$_.Operand -is [Mono.Cecil.MethodReference] -and $_.Operand.Name -eq 'GetValue'}).Count){throw 'Unproven foreign static read can trigger a type initializer'}
 if(-not @($staticRead.Body.Instructions | Where-Object {$_.Operand -is [Mono.Cecil.MethodReference] -and $_.Operand.FullName -like '*LifecycleRead::Field*'}).Count){throw 'Foreign static read must use rejecting field policy'}
 if(@($field.Body.Instructions | Where-Object {$_.Operand -is [Mono.Cecil.MethodReference] -and $_.Operand.Name -eq 'GetProperty'}).Count){throw 'Field reader accesses properties'}
 $capture=$observer.Methods | Where-Object Name -eq 'Capture'
 foreach($key in @('neutral.certification','equipment.exclusivity','inventory.seneaLCustom','equipment.povUtilityOwners','eitr.actualRegenRate')){
  if(-not @($capture.Body.Instructions | Where-Object {$_.OpCode.Code -eq 'Ldstr' -and $_.Operand -eq $key}).Count){throw "Missing explicit limited-evidence field $key"}
 }
 if(@($types.Methods | Where-Object HasBody | ForEach-Object {$_.Body.Instructions} | Where-Object {$_.Operand -is [Mono.Cecil.MethodReference] -and $_.Operand.DeclaringType.FullName -like 'PoVPlus.Compatibility.*'}).Count){throw 'Observer must not invoke production bridge behavior'}
 Write-Host "PASS C1 compiled wiring/read-only policy audit: $calls member calls, no foreign field stores or forbidden mutation/creating getters. Reflection/runtime provenance still require manual review."
} finally {$a.Dispose()}
