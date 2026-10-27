# Metadata only; output only in this worktree's ignored scratch. No target code execution.
$ErrorActionPreference='Stop'
$repo=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$out=Join-Path $repo '.dev-state/crossreview-a/installed.json'
$dev='S:\Valheim-Modding\PoVPlus-Dev'
Add-Type -LiteralPath (Join-Path $dev 'BepInEx/core/Mono.Cecil.dll')
function Walk($types){foreach($t in $types){$t;Walk $t.NestedTypes}}
$rows=@(foreach($relative in @('valheim_Data/Managed/assembly_valheim.dll','BepInEx/plugins/blacks7ar-MagicPlugin/MagicPlugin.dll','BepInEx/plugins/Dreanegade-Magic_Supremacy/Magic_Supremacy.dll','BepInEx/core/BepInEx.dll')){
 $path=Join-Path $dev $relative;$hash=(Get-FileHash -LiteralPath $path).Hash
 $a=[Mono.Cecil.AssemblyDefinition]::ReadAssembly($path)
 try{
  $types=@(Walk $a.MainModule.Types)
  $item=$a.MainModule.GetType('ItemDrop/ItemData')
  $methods=@(foreach($t in $types){foreach($m in $t.Methods){if(-not $m.HasBody){continue}
   $refs=@($m.Body.Instructions|Where-Object {$_.Operand -is [Mono.Cecil.MemberReference] -and $_.Operand.FullName -match '_enableTomeSlot|ValidateSingleSlot'})
   if($refs.Count -or ($t.FullName -match '^MagicPlugin.Plugin$' -and $m.Name -match '^config') -or
     ($t.FullName -match '^BepInEx.Configuration.(ConfigFile|ConfigEntryBase|ConfigEntry`1)$' -and $m.Name -match 'Reload|set_Value|set_BoxedValue') -or
     ($t.FullName -eq 'Magic_Supremacy.CustomSlotSystem' -and $m.Name -in @('ValidateSingleSlot','FindInventoryItemByGuid','IsItemValidForSlot','IsValidPlayer','GetRuntimeData')) -or
     ($t.FullName -eq 'Magic_Supremacy.CustomSlotSystem/HumanoidCustomSlotData' -and $m.Name -eq '.ctor')){
    [ordered]@{name=$m.FullName;token=$m.MetadataToken.ToUInt32();il=@($m.Body.Instructions|ForEach-Object ToString)}
   }
  }})
  [ordered]@{path=$relative;sha256=$hash;identity=$a.Name.FullName;
   shapes=@($types|Where-Object FullName -in @('Player','Magic_Supremacy.CustomSlotSystem/HumanoidCustomSlotData')|ForEach-Object {[ordered]@{name=$_.FullName;token=$_.MetadataToken.ToUInt32();sealed=$_.IsSealed}});
   item=if($item){[ordered]@{sealed=$item.IsSealed;token=$item.MetadataToken.ToUInt32();equals=@($item.Methods|Where-Object Name -eq Equals|ForEach-Object FullName)}}else{$null};methods=$methods}
 }finally{$a.Dispose()}
 if((Get-FileHash -LiteralPath $path).Hash -ne $hash){throw 'Image changed'}
})
[IO.Directory]::CreateDirectory((Split-Path -Parent $out))|Out-Null
$rows|ConvertTo-Json -Depth 9|Set-Content -LiteralPath $out -Encoding UTF8
Write-Host "PASS read-only metadata exported to $out"
