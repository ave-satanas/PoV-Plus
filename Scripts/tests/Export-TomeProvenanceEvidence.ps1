[CmdletBinding()]
param([string]$Output='.dev-state/exp005-correction-20261026/primary.json')
$ErrorActionPreference='Stop'
$dev='S:\Valheim-Modding\PoVPlus-Dev'
Add-Type -LiteralPath (Join-Path $dev 'BepInEx/core/Mono.Cecil.dll')
$path=Join-Path $dev 'BepInEx/plugins/Dreanegade-Magic_Supremacy/Magic_Supremacy.dll'
$hash='8BAFE7DAEFB9B6AF057FC3B10534764ED9CCEBA430B526A014B2DD12CFABA787'
if((Get-FileHash -LiteralPath $path).Hash -cne $hash){throw 'Pinned MS image changed'}
$a=[Mono.Cecil.AssemblyDefinition]::ReadAssembly($path)
function Walk($types){foreach($t in $types){$t;Walk $t.NestedTypes}}
function IL($m){@($m.Body.Instructions|ForEach-Object ToString)}
try {
 $t=$a.MainModule.GetType('Magic_Supremacy.CustomSlotSystem')
 $get=@($t.Methods|Where-Object Name -eq GetRuntimeData)
 if($get.Count -ne 1 -or -not $get[0].IsStatic -or $get[0].Parameters.Count -ne 1 -or $get[0].Parameters[0].ParameterType.FullName -ne 'Humanoid' -or $get[0].ReturnType.FullName -ne ($t.FullName+'/HumanoidCustomSlotData')){throw 'Capture target contract differs'}
 if((IL $get[0]) -join "`n" -notmatch 'GetOrCreateValue'){throw 'Return origin differs'}
 $data=$a.MainModule.GetType($get[0].ReturnType.FullName)
 $field=@($data.Fields|Where-Object Name -eq EquippedItems)
 if($field.Count -ne 1 -or $field[0].IsStatic -or -not $field[0].IsInitOnly -or $field[0].FieldType.FullName -ne 'System.Collections.Generic.Dictionary`2<System.String,ItemDrop/ItemData>'){throw 'Ownership field contract differs'}
 $callers=@(foreach($ty in (Walk $a.MainModule.Types)){foreach($m in $ty.Methods){if($m.HasBody){foreach($i in $m.Body.Instructions){if($i.Operand -is [Mono.Cecil.MemberReference] -and $i.Operand.FullName -match 'CustomSlotSystem::(GetRuntimeData|SetEquippedItem)|HumanoidCustomSlotData::EquippedItems'){
  [ordered]@{method=$m.FullName;token=$m.MetadataToken.ToUInt32();instruction=$i.ToString()}
 }}}}})
 $methods=@(foreach($ty in (Walk @($t))){foreach($m in $ty.Methods){if($m.Name -match '^(GetRuntimeData|SetEquippedItem|GetEquippedItem|IsItemEquippedInCustomSlot|EquipSlotInternal|UnequipSlotInternal|ValidateSingleSlot|RestoreOrValidateCustomSlots|SetSavedEquippedGuid|ClearSavedEquippedGuid|GetOrCreateItemGuid|Prefix|Postfix|\.cctor|\.ctor)$'){
  [ordered]@{method=$m.FullName;token=$m.MetadataToken.ToUInt32();attributes=$m.Attributes.ToString();impl=$m.ImplAttributes.ToString();il=if($m.HasBody){IL $m}else{@()};handlers=@($m.Body.ExceptionHandlers|ForEach-Object HandlerType);annotations=@($m.CustomAttributes|ForEach-Object {$_.AttributeType.FullName+':'+($_.ConstructorArguments -join ',')})}
 }}})
 $result=[ordered]@{path=$path;sha256=$hash;identity=$a.Name.FullName;mvid=$a.MainModule.Mvid.ToString();holderBeforeFieldInit=$t.IsBeforeFieldInit;
   dataType=$data.FullName;dataToken=$data.MetadataToken.ToUInt32();dataVisibility=$data.Attributes.ToString();field=$field[0].FullName;fieldToken=$field[0].MetadataToken.ToUInt32();fieldAttributes=$field[0].Attributes.ToString();methods=$methods;callers=$callers;
   patchLimit='Declared annotations only; dynamic patches, inlining and effective ordering not established'}
 # Full field-reference list proves the pinned constructor assignment path within this image;
 # it cannot prove absence of external reflection or runtime patch writers.
 $result.fieldReferences=@(foreach($ty in (Walk $a.MainModule.Types)){foreach($m in $ty.Methods){if($m.HasBody){foreach($i in $m.Body.Instructions){if($i.Operand -is [Mono.Cecil.FieldReference] -and $i.Operand.FullName -eq $field[0].FullName){
  [ordered]@{method=$m.FullName;token=$m.MetadataToken.ToUInt32();opcode=$i.OpCode.Code.ToString();instruction=$i.ToString()}
 }}}}})
 $monoPath=Join-Path $dev 'valheim_Data/Managed/mscorlib.dll'
 $monoHash='5ED1180FC8CB409D57952296C7F573F2B1B3D3D2A2CD37258EF407498A17DD4D'
 if((Get-FileHash -LiteralPath $monoPath).Hash -cne $monoHash){throw 'Pinned Mono BCL changed'}
 $mono=[Mono.Cecil.AssemblyDefinition]::ReadAssembly($monoPath)
 try {
  $dict=$mono.MainModule.GetType('System.Collections.Generic.Dictionary`2')
  $enum=$dict.NestedTypes|Where-Object Name -eq Enumerator
  $bodies=@(foreach($ty in @($dict,$enum)){foreach($m in $ty.Methods){if($m.Name -match '(GetEnumerator|get_Count|MoveNext|get_Entry|get_Current|^\.ctor$)'){
   [ordered]@{method=$m.FullName;token=$m.MetadataToken.ToUInt32();attributes=$m.Attributes.ToString();il=if($m.HasBody){IL $m}else{@()}}
  }}})
  $result.bcl=[ordered]@{path=$monoPath;sha256=$monoHash;identity=$mono.Name.FullName;mvid=$mono.MainModule.Mvid.ToString();methods=$bodies;fields=@($dict.Fields|ForEach-Object {[ordered]@{name=$_.FullName;token=$_.MetadataToken.ToUInt32();attributes=$_.Attributes.ToString()}});
   limit='Installed managed bodies only. Host fixture uses desktop CLR; Mono/native detours and atomicity remain unproved.'}
 }finally{$mono.Dispose()}
 if((Get-FileHash -LiteralPath $monoPath).Hash -cne $monoHash){throw 'Mono source changed during read'}
 $gamePath=Join-Path $dev 'valheim_Data/Managed/assembly_valheim.dll'
 $gameHash='94998885B103D6E5F5480123EAEE0A093E82AF2DDEEEE37536C15BF555F24F3A'
 if((Get-FileHash -LiteralPath $gamePath).Hash -cne $gameHash){throw 'Pinned game metadata changed'}
 $game=[Mono.Cecil.AssemblyDefinition]::ReadAssembly($gamePath)
 try {
  $result.game=[ordered]@{path=$gamePath;sha256=$gameHash;mvid=$game.MainModule.Mvid.ToString();
   types=@(foreach($name in @('Player','ItemDrop/ItemData')){
    $ty=$game.MainModule.GetType($name)
    [ordered]@{name=$ty.FullName;beforeFieldInit=$ty.IsBeforeFieldInit;
     fields=@($ty.Fields|Where-Object Name -in @('m_customData','m_equipped')|ForEach-Object {[ordered]@{name=$_.FullName;token=$_.MetadataToken.ToUInt32();attributes=$_.Attributes.ToString()}});
     constructors=@($ty.Methods|Where-Object IsConstructor|ForEach-Object {[ordered]@{name=$_.FullName;token=$_.MetadataToken.ToUInt32();il=if($_.HasBody){IL $_}else{@()}}})}
   })}
 }finally{$game.Dispose()}
 if((Get-FileHash -LiteralPath $gamePath).Hash -cne $gameHash){throw 'Game source changed during read'}
 $parent=Split-Path -Parent $Output;[IO.Directory]::CreateDirectory($parent)|Out-Null
 $result|ConvertTo-Json -Depth 10|Set-Content -LiteralPath $Output -Encoding UTF8
 if((Get-FileHash -LiteralPath $path).Hash -cne $hash){throw 'Assembly changed during read'}
 Write-Host "PASS primary metadata: $($methods.Count) method bodies; $($callers.Count) references; pinned existing-return and instance-field contracts. $Output"
}finally{$a.Dispose()}
