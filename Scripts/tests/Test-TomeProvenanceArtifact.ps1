[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$Dll,[switch]$Enabled,[string]$EvidenceOutput)
$ErrorActionPreference='Stop'
Add-Type -LiteralPath 'S:\Valheim-Modding\PoVPlus-Dev\BepInEx/core/Mono.Cecil.dll'
$a=[Mono.Cecil.AssemblyDefinition]::ReadAssembly((Resolve-Path -LiteralPath $Dll).Path)
function Walk($types){foreach($t in $types){$t;Walk $t.NestedTypes}}
try {
 $all=@(Walk $a.MainModule.Types)
 $types=@($all|Where-Object FullName -like 'PoVPlus.Diagnostics.Provenance.*')
 if($EvidenceOutput){
  $repo=(Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
  $target=[IO.Path]::GetFullPath($EvidenceOutput)
  $allowed=[IO.Path]::GetFullPath((Join-Path $repo '.dev-state'))+[IO.Path]::DirectorySeparatorChar
  if(-not $target.StartsWith($allowed,[StringComparison]::OrdinalIgnoreCase)){throw 'Compiled evidence must remain under repository .dev-state'}
  $evidence=[ordered]@{path=(Resolve-Path -LiteralPath $Dll).Path;sha256=(Get-FileHash -LiteralPath $Dll).Hash;
    mvid=$a.MainModule.Mvid.ToString();references=@($a.MainModule.AssemblyReferences|ForEach-Object FullName);
    types=@(foreach($t in $types){[ordered]@{name=$t.FullName;fields=@($t.Fields|ForEach-Object FullName);
     methods=@(foreach($m in $t.Methods){[ordered]@{name=$m.FullName;token=$m.MetadataToken.ToUInt32();il=@(if($m.HasBody){$m.Body.Instructions|ForEach-Object ToString})}})}})}
  [IO.Directory]::CreateDirectory((Split-Path -Parent $target))|Out-Null
  $evidence|ConvertTo-Json -Depth 10|Set-Content -LiteralPath $target -Encoding UTF8
 }
 if(-not $Enabled){if($types.Count){throw 'Ordinary artifact includes provenance experiment'};Write-Host 'PASS ordinary artifact excludes all provenance types';return}
 $expected=@('ReturnedRoot','Snapshot','ExistingStateReader','Kind','Phase','Transition','ProvenanceLedger')
 if($types.Count -ne $expected.Count -or @($types|Where-Object Name -notin $expected).Count){throw "Unexpected PoC type inventory: $($types.Count)"}
 # Prove the concrete-type rejection branch dominates every source interface access.
 $reader=$types|Where-Object Name -eq ExistingStateReader
 $copy=$reader.Methods|Where-Object Name -eq Copy
 if(-not $copy.IsPrivate -or $copy.Parameters[0].ParameterType.FullName -ne 'System.Object' -or
     $copy.Parameters[1].ParameterType.FullName -ne 'System.Type'){throw 'Unexpected guarded copy boundary'}
 $instructions=@($copy.Body.Instructions)
 $guard=@($instructions|Where-Object { $_.Operand -is [Mono.Cecil.MethodReference] -and $_.Operand.DeclaringType.FullName -eq 'System.Type' -and $_.Operand.Name -eq 'op_Inequality' })
 if($guard.Count -ne 1){throw 'Concrete collection guard missing'}
 $g=$guard[0]
 if($g.Previous.OpCode.Code -ne 'Ldarg_1' -or $g.Previous.Previous.Operand.Name -ne 'GetType' -or
     $g.Previous.Previous.Previous.OpCode.Code -ne 'Ldarg_0' -or $g.Next.OpCode.Code -notin @('Brfalse','Brfalse_S')){throw 'Concrete guard does not compare object runtime type to expected type'}
 $success=$g.Next.Operand
 foreach($h in $copy.Body.ExceptionHandlers){if($h.TryStart.Offset -le $g.Offset -and $h.TryEnd.Offset -gt $g.Offset){throw 'Guard may be bypassed by exception recovery'}}
 $sinks=@($instructions|Where-Object { $_.Operand -is [Mono.Cecil.MethodReference] -and $_.Operand.DeclaringType.FullName -match '^System.Collections.I' })
 if(-not ($sinks.Operand.Name -contains 'GetEnumerator') -or -not ($sinks.Operand.Name -contains 'get_Count')){throw 'Expected source enumeration missing'}
 $queue=[Collections.Generic.Queue[Mono.Cecil.Cil.Instruction]]::new()
 $seen=[Collections.Generic.HashSet[int]]::new();$queue.Enqueue($instructions[0])
 while($queue.Count){
  $i=$queue.Dequeue()
  if($i -eq $success -or -not $seen.Add($i.Offset)){continue}
  if($sinks -contains $i){throw 'Unsafe interface path bypasses concrete guard'}
  $flow=$i.OpCode.FlowControl.ToString()
  if($flow -in @('Return','Throw')){continue}
  if($flow -in @('Branch','Cond_Branch')){foreach($target in @($i.Operand)) { if($target -is [Mono.Cecil.Cil.Instruction]){$queue.Enqueue($target)} }}
  if($flow -ne 'Branch' -and $i.Next){$queue.Enqueue($i.Next)}
 }
 $storage=$reader.Methods|Where-Object Name -eq StorageCount
 if(-not $storage.IsPrivate -or -not @($storage.Body.Instructions|Where-Object { $_.Operand -eq 64 }).Count){throw 'Traversal bound missing'}
 $storageCalls=@($instructions|Where-Object { $_.Operand -is [Mono.Cecil.MethodReference] -and $_.Operand.Name -eq 'StorageCount' })
 if($storageCalls.Count -ne 2 -or $storageCalls[0].Offset -gt ($sinks|Where-Object {$_.Operand.Name -eq 'GetEnumerator'}|Select-Object -First 1).Offset){throw 'Dictionary traversal work is unbounded'}
 if(-not @($instructions|Where-Object {$_.Operand -eq 128}).Count -or -not @($instructions|Where-Object {$_.Operand -eq 4096}).Count){throw 'Collection string bound missing'}
 $read=$reader.Methods|Where-Object Name -eq Read
 $copies=@($read.Body.Instructions|Where-Object { $_.Operand -is [Mono.Cecil.MethodReference] -and $_.Operand.DeclaringType.FullName -eq $reader.FullName -and $_.Operand.Name -eq 'Copy' })
 if($copies.Count -ne 6){throw 'Unexpected copy/read path count'}
 foreach($call in $copies){if($call.Previous.Operand.Name -ne 'get_FieldType' -or
    $call.Previous.Previous.Operand.Name -notin @('_map','_itemData','_bindings')){throw 'Copy expected type is not from validated metadata field'}}
 foreach($t in $types){foreach($m in $t.Methods){if($m.HasBody){foreach($i in $m.Body.Instructions){
  if($i.Operand -is [Mono.Cecil.MethodReference] -and $i.Operand.DeclaringType.FullName -match '^System.Collections.I' -and $m -ne $copy){throw 'Alternate source interface path outside guarded copy'}
 }}}}
 # Unsupported claims must be literal Unknown, not upgraded by arguments or sequence arithmetic.
 foreach($t in ($types|Where-Object Name -in @('ReturnedRoot','Snapshot','ProvenanceLedger'))){
  foreach($name in @('OriginValidity','Atomicity','Continuity','AuthoritativeState','TransitionCoverage','InitialStateValidity','NeutralState','RollbackSafety')){
   $m=$t.Methods|Where-Object Name -eq ('get_'+$name)
   if($m){$body=@($m.Body.Instructions);if($body.Count -ne 2 -or $body[0].OpCode.Code -ne 'Ldstr' -or $body[0].Operand -cne 'unknown' -or $body[1].OpCode.Code -ne 'Ret'){throw "Unsupported claim promotion: $($t.Name).$name"}}
  }
 }
 $add=($types|Where-Object Name -eq ProvenanceLedger).Methods|Where-Object Name -eq Add
 # Roslyn lowers Object.ReferenceEquals on these object fields to CLI beq, not a call.
 # Match the two exact readonly scope fields and prove the equality-success target
 # dominates record publication. Do not mistake an unrelated beq for this guard.
 $scope=@($add.Body.Instructions|Where-Object { $_.OpCode.Code -in @('Beq','Beq_S') -and $_.Previous.Operand.Name -eq 'ReaderIdentity' -and $_.Previous.Previous.Previous.Previous.Operand.Name -eq 'ReaderIdentity' })
 if($scope.Count -ne 1 -or $add.Body.ExceptionHandlers.Count){throw 'Missing observation scope comparison'}
 $published=@($add.Body.Instructions|Where-Object { $_.Operand -is [Mono.Cecil.MethodReference] -and $_.Operand.Name -eq 'Add' -and $_.Operand.DeclaringType.FullName -like 'System.Collections.Generic.List*' })
 if($published.Count -ne 1){throw 'Unexpected record publication path'}
 $queue=[Collections.Generic.Queue[Mono.Cecil.Cil.Instruction]]::new();$seen=[Collections.Generic.HashSet[int]]::new();$queue.Enqueue($add.Body.Instructions[0])
 while($queue.Count){
  $i=$queue.Dequeue();if($i -eq $scope[0].Operand -or -not $seen.Add($i.Offset)){continue}
  if($i -eq $published[0]){throw 'Record publication bypasses observation scope equality'}
  $flow=$i.OpCode.FlowControl.ToString();if($flow -in @('Return','Throw')){continue}
  if($flow -in @('Branch','Cond_Branch')){foreach($target in @($i.Operand)){if($target -is [Mono.Cecil.Cil.Instruction]){$queue.Enqueue($target)}}}
  if($flow -ne 'Branch' -and $i.Next){$queue.Enqueue($i.Next)}
 }
 $history=($types|Where-Object Name -eq ProvenanceLedger).Methods|Where-Object Name -eq get_HistoryComplete
 if($history.Body.Instructions.Count -ne 2 -or $history.Body.Instructions[0].OpCode.Code -ne 'Ldc_I4_0'){throw 'History completeness can be promoted'}
 $enumerations=@(foreach($t in $types){foreach($m in $t.Methods){if($m.HasBody){foreach($i in $m.Body.Instructions){if($i.Operand -is [Mono.Cecil.MethodReference] -and $i.Operand.Name -eq 'GetEnumerator'){
  [pscustomobject]@{Method=$m;Call=$i.Operand}
 }}}}})
 if($enumerations.Count -ne 3){throw 'Unexpected alternate enumeration path'}
 foreach($entry in $enumerations){
  if($entry.Method.DeclaringType.FullName -ne $reader.FullName -or $entry.Method.Name -notin @('Copy','Same','Read')){throw 'Unexpected enumeration caller'}
  if($entry.Method.Name -ne 'Copy' -and $entry.Call.DeclaringType.FullName -ne 'System.Collections.Generic.Dictionary`2<System.String,System.Object>'){throw 'Alternate enumeration not over observer-owned copy'}
 }
 $calls=0
 foreach($t in $types){
  if(@($t.CustomAttributes|Where-Object {$_.AttributeType.FullName -like 'Harmony*'}).Count){throw 'Unexpected Harmony attribute'}
  if(@($t.Fields.Name | Where-Object { $_ -in @('DuringRead','EnumerationStep') }).Count){throw 'Fixture adversary leaked into diagnostic artifact'}
  foreach($m in $t.Methods){
   if(@($m.CustomAttributes|Where-Object {$_.AttributeType.FullName -like 'Harmony*'}).Count){throw 'Unexpected Harmony method attribute'}
   if(-not $m.HasBody){continue}
   foreach($i in $m.Body.Instructions){
    $o=$i.Operand
    if($i.OpCode.Code -in @('Stfld','Stsfld') -and $o.DeclaringType.FullName -notlike 'PoVPlus.Diagnostics.Provenance.*'){throw "Foreign store: $m $i"}
    if($i.OpCode.Code -in @('Ldsfld','Ldsflda')){throw "Unexpected static root read: $m $i"}
    if($o -is [Mono.Cecil.MethodReference]){
     $calls++
     if($o.DeclaringType.FullName -match '^System.Collections.(Generic.Dictionary|IDictionary)' -and $o.Name -in @('set_Item','Remove','Clear')){throw "Unexpected dictionary mutation: $m $i"}
     if($o.DeclaringType.FullName -like 'System.Collections.Generic.Dictionary*' -and $o.Name -eq 'Add' -and $m -ne $copy){throw 'Dictionary Add outside observer-copy construction'}
     if($o.DeclaringType.Scope.Name -notin @('mscorlib','PoVPlus.dll')){throw "Unexpected call scope: $m $i"}
     if($o.Name -match '^(Invoke|SetValue|GetOrCreateValue|GetRuntimeData|GetEquippedItem|EquipItem|UnequipItem|GetValueInternal|Patch|PatchAll|Unpatch|Save|Load|SetData|WriteAllText|WriteAllBytes)$'){throw "Forbidden call: $m $i"}
     if($o.Name -eq 'GetValue' -and ($o.DeclaringType.FullName -ne 'System.Reflection.FieldInfo' -or $m.Name -notin @('Read','StorageCount'))){throw 'Unreviewed reflective field read'}
    }
   }
  }
 }
 # No Plugin/old observer/bridge method may activate or call this experiment.
 foreach($t in ($all|Where-Object FullName -notlike 'PoVPlus.Diagnostics.Provenance.*')){foreach($m in $t.Methods){if($m.HasBody){foreach($i in $m.Body.Instructions){if($i.Operand -is [Mono.Cecil.MemberReference] -and $i.Operand.DeclaringType.FullName -like 'PoVPlus.Diagnostics.Provenance.*'){throw "Unexpected activation: $m"}}}}}
 Write-Host "PASS compiled opt-in PoC: $($types.Count) types, $calls calls, no fixture hook/static roots/foreign stores/installer/activation. Concrete guard dominates interface reads; six copies use validated field types; unsupported claims remain literal unknown. Reflection/runtime boundaries still require the evidence review."
}finally{$a.Dispose()}
