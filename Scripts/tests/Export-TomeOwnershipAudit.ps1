[CmdletBinding()]
param([string]$ValheimDevPath='S:\Valheim-Modding\PoVPlus-Dev',
 [string]$DiagnosticDll='S:\Valheim-Modding\PoV-Plus\.dev-state\c1-observer\builds\c1\PoVPlus.dll',
 [string]$Output='S:\Valheim-Modding\PoV-Plus\.dev-state\c1-audit\metadata.json')
$ErrorActionPreference='Stop'
$DiagnosticDll=(Resolve-Path -LiteralPath $DiagnosticDll).Path
# Cecil reads metadata/IL. None of the target assemblies are loaded for execution.
Add-Type -LiteralPath (Join-Path $ValheimDevPath 'BepInEx\core\Mono.Cecil.dll')
function Walk($types){foreach($t in $types){$t;Walk $t.NestedTypes}}
$specs=@(
 @('MS','BepInEx\plugins\Dreanegade-Magic_Supremacy\Magic_Supremacy.dll','^Magic_Supremacy.CustomSlotSystem','^(GetRuntimeData|GetEquippedItem|SetEquippedItem|TryClaimSystemOwner|Log|GetSavedEquippedGuid|SetSavedEquippedGuid|ClearSavedEquippedGuid|EquipSlotInternal|UnequipSlotInternal|EnsureDefinitionsInitialized|\.cctor|\.ctor)$'),
 @('UI','BepInEx\plugins\seneaL-SeneaL_UI\SeneaL-UI\SeneaL-UI.dll','^SeneaLUI\.(Inv\.(CustomStore|InventoryView)|Patches.InventoryGuiPatches)','^(Claimed|get_Inv|get_LinkedShown|Awake|TickLinked|\.cctor|\.ctor)$'),
 @('API','BepInEx\plugins\seneaL-SeneaL_API\SeneaL-API\SeneaL-API.dll','SlotDef','^get_'),
 @('PoV','BepInEx\plugins\treextr-PathOfValheiman\PathOfValheiman\PathOfValheiman.dll','^(PassiveTree.(Gear.State|BeltSlot)|Valheiman.Api.Equipment)$','^(Get|Item|Bound|WornTrinket|Read|Peek|Owner|get_Cells|\.cctor)$'),
 @('Game','valheim_Data\Managed\assembly_valheim.dll','^(Game|Player|Humanoid|Inventory|ItemDrop/ItemData|SEMan|ZNet|ZNetView|ZDO)$','^(get_instance|GetPlayerName|GetWorldName|GetInventory|GetAllItems|GetEquippedItems|GetStatusEffects|IsValid|GetZDO|GetString|\.cctor)$'),
 @('Unity','valheim_Data\Managed\UnityEngine.CoreModule.dll','^(UnityEngine.(Object|Transform|Component|GameObject|Time)|UnityEngine.Object/MarshalledUnityObject|UnityEngine.Bindings.Unmarshal)$','^(Marshal|MarshalNotNull|MarshalFromInstanceId|UnmarshalUnityObject|Find|FindRelativeTransformWithPath|GetName|get_name|get_realtimeSinceStartup|get_transform|get_gameObject|get_activeInHierarchy|op_Implicit|op_Equality|CompareBaseObjects|IsNativeObjectAlive|GetCachedPtr|DoesObjectWithInstanceIDExist)$'),
 @('Mono','valheim_Data\Managed\mscorlib.dll','^(System.Runtime.CompilerServices.ConditionalWeakTable`2|System.Reflection.RuntimeFieldInfo)$','^(TryGetValue|GetOrCreateValue|GetValue|GetValueInternal)$'),
 @('Harmony','BepInEx\core\0Harmony.dll','^HarmonyLib.(Harmony|PatchProcessor|Public.Patching.PatchManager|GeneralExtensions)$','^(GetPatchInfo|GetValueSafe|GetState|GetOrCreateSharedState|\.cctor)$'),
 @('BepInEx','BepInEx\core\BepInEx.dll','^BepInEx.Logging.(ManualLogSource|Logger|Logger/LogListenerCollection)$','^(Log|LogInfo|InternalLogEvent|SendLogEvent|\.cctor)$'),
 @('Observer',$DiagnosticDll,'^PoVPlus.Diagnostics.(TomeLifecycleObserver|LifecycleRead|LifecycleWindow|LifecycleEvidence)(/|$)','.*')
)
$results=@()
foreach($spec in $specs){
 $path=if([IO.Path]::IsPathRooted($spec[1])){$spec[1]}else{Join-Path $ValheimDevPath $spec[1]}
 if(-not (Test-Path -LiteralPath $path)){throw "Audit source missing: $path"}
 $before=(Get-FileHash -LiteralPath $path).Hash
 $a=[Mono.Cecil.AssemblyDefinition]::ReadAssembly($path)
 try {
  $types=@(foreach($t in (Walk $a.MainModule.Types | Where-Object FullName -match $spec[2])){
   [ordered]@{name=$t.FullName;token=$t.MetadataToken.ToUInt32();beforeFieldInit=$t.IsBeforeFieldInit;
    fields=@($t.Fields | ForEach-Object {[ordered]@{name=$_.FullName;token=$_.MetadataToken.ToUInt32();static=$_.IsStatic}});
    methods=@($t.Methods | Where-Object Name -match $spec[3] | ForEach-Object {
     [ordered]@{name=$_.FullName;token=$_.MetadataToken.ToUInt32();body=$_.HasBody;
      il=@(if($_.HasBody){$_.Body.Instructions | ForEach-Object {$_.ToString()}})}
    })}
  })
  $results += [ordered]@{label=$spec[0];path=$path;sha256=$before;identity=$a.Name.FullName;mvid=$a.MainModule.Mvid.ToString();types=$types}
 }finally{$a.Dispose()}
 if((Get-FileHash -LiteralPath $path).Hash -ne $before){throw "Source changed while reading: $path"}
}
$parent=Split-Path -Parent $Output
[IO.Directory]::CreateDirectory($parent)|Out-Null
$results | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath $Output -Encoding UTF8
Write-Host "Exported $($results.Count) pinned metadata/IL sources to $Output. Native calls and live detours remain unproved."
