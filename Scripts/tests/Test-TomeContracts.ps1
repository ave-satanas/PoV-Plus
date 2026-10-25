[CmdletBinding()]
param([string]$ValheimDevPath = 'S:\Valheim-Modding\PoVPlus-Dev')
$ErrorActionPreference = 'Stop'
Add-Type -LiteralPath (Join-Path $ValheimDevPath 'BepInEx\core\Mono.Cecil.dll')
$specs = @{
    magic=@('BepInEx/plugins/blacks7ar-MagicPlugin/MagicPlugin.dll','6504C2FF9CEAAE7B317820E41EF84D8C28D33FEDBF32B7D0F4FDDFD680BDA795')
    ms=@('BepInEx/plugins/Dreanegade-Magic_Supremacy/Magic_Supremacy.dll','8BAFE7DAEFB9B6AF057FC3B10534764ED9CCEBA430B526A014B2DD12CFABA787')
    pov=@('BepInEx/plugins/treextr-PathOfValheiman/PathOfValheiman/PathOfValheiman.dll','83BD50F23D9C301FE469B5FBE2C430FD35C928D7A2CCFD99AF5A1040558C9D72')
    api=@('BepInEx/plugins/seneaL-SeneaL_API/SeneaL-API/SeneaL-API.dll','2EF9EB8DD3AB7DC72E7F3A908EFF843C4C7FAD19570CB46C3565BC36841E3574')
    ui=@('BepInEx/plugins/seneaL-SeneaL_UI/SeneaL-UI/SeneaL-UI.dll','53B0F675BFFD0BAC5FCCA711AF04AA1EF00BB5BECD543D702EBAA146E5C487CD')
    game=@('valheim_Data/Managed/assembly_valheim.dll','94998885B103D6E5F5480123EAEE0A093E82AF2DDEEEE37536C15BF555F24F3A')
}
$assemblies=@{}
$script:checks=0
function Assert-Contract([bool]$Condition,[string]$Name) {
    if (-not $Condition) { throw "Installed contract failed: $Name" }
    $script:checks++; Write-Host "PASS $Name"
}
function Get-Method($Key,$Type,$Name) {
    $t=$assemblies[$Key].MainModule.GetType($Type)
    if (-not $t) { throw "Missing type $Type" }
    $found=@($t.Methods | Where-Object Name -eq $Name)
    if ($found.Count -ne 1) { throw "Ambiguous/missing method $Type::$Name" }
    $found[0]
}
function IL($Method) { ($Method.Body.Instructions | ForEach-Object ToString) -join "`n" }
try {
    foreach ($key in $specs.Keys) {
        $path=Join-Path $ValheimDevPath $specs[$key][0]
        Assert-Contract ((Get-FileHash -LiteralPath $path).Hash -eq $specs[$key][1]) "$key audited fingerprint"
        $assemblies[$key]=[Mono.Cecil.AssemblyDefinition]::ReadAssembly($path)
    }
    $mp=IL (Get-Method magic 'MagicPlugin.Functions.MagicSlot' 'IsTomeItem')
    Assert-Contract ($mp.Contains('_enableTomeSlot') -and $mp.Contains('m_name') -and $mp.Contains('_magicbook')) 'MP classifier is toggle/shared-name based'
    $food=IL (Get-Method magic 'MagicPlugin.Patches.PlayerPatch' 'GetTotalFoodValue_Postfix')
    $eitr=IL (Get-Method magic 'MagicPlugin.Patches.PlayerPatch' 'SetEitr')
    Assert-Contract ($food.Contains('bmp_beginners_magicbook') -and $eitr.Contains('HaveStatusEffect') -and -not $food.Contains('IsTomeItem')) 'MP max Eitr uses original status effect, not classifier'
    Assert-Contract ((IL (Get-Method magic 'MagicPlugin.Patches.PlayerPatch' 'GetEquipmentEitrRegenModifier_Postfix')).Contains('IsTomeItem')) 'MP regen remains classifier-dependent'
    $ms=IL (Get-Method ms 'Magic_Supremacy.Magic_SupremacyPlugin' 'IsTomeItem')
    $strings=@((Get-Method ms 'Magic_Supremacy.Magic_SupremacyPlugin' 'IsTomeItem').Body.Instructions |
        Where-Object {$_.OpCode.Code -eq 'Ldstr'} | ForEach-Object Operand)
    Assert-Contract ($strings.Count -eq 7 -and $strings -contains 'ArmorMushroomcallerTome_DO' -and -not ($strings -match '^BMP_')) 'MS descriptor whitelist has seven native prefabs, no BMP books'
    $valid=Get-Method ms 'Magic_Supremacy.CustomSlotSystem' 'IsItemValidForSlot'
    Assert-Contract ($valid.IsPrivate -and (IL $valid).Contains('Contains')) 'MS lifecycle has a separate private registry gate'
    Assert-Contract ((IL (Get-Method ms 'Magic_Supremacy.CustomSlotSystem' 'GetMatchingSlotDefinition')).Contains('::IsValid')) 'MS matching invokes definition predicate'
    $equip=IL (Get-Method ms 'Magic_Supremacy.CustomSlotSystem' 'EquipSlotInternal')
    Assert-Contract ($equip.Contains('ApplyStatusEffect') -and $equip.Contains('SetSavedEquippedGuid') -and $equip.Contains('UnequipSlotInternal')) 'MS lifecycle owns prior removal, effect application and saved GUID'
    Assert-Contract ((IL (Get-Method ms 'Magic_Supremacy.CustomSlotSystem' 'SetSavedEquippedGuid')).Contains('m_customData')) 'MS saves equipment binding in player custom data'
    $restore=IL (Get-Method ms 'Magic_Supremacy.CustomSlotSystem' 'RestoreOrValidateCustomSlots')
    $validate=IL (Get-Method ms 'Magic_Supremacy.CustomSlotSystem' 'ValidateSingleSlot')
    $find=IL (Get-Method ms 'Magic_Supremacy.CustomSlotSystem' 'FindInventoryItemByGuid')
    Assert-Contract ($restore.Contains('ValidateSingleSlot') -and $validate.Contains('FindInventoryItemByGuid') -and $find.Contains('::IsValid')) 'MS restore reaches definition predicate through validate/find chain'
    $msType=$assemblies.ms.MainModule.GetType('Magic_Supremacy.CustomSlotSystem')
    Assert-Contract (-not ($msType.Methods | Where-Object {$_.IsPublic -and $_.Name -match '^Register'})) 'No public MS registration method on inspected equipment class'
    $stub=IL (Get-Method api 'SeneaL.API.StubHost' 'Emulate')
    Assert-Contract ($stub.Contains('AzuEPI.emulated') -and $stub.Contains('CustomSlots::Add')) 'SeneaL creates emulated descriptors'
    Assert-Contract ((IL (Get-Method ui 'SeneaLUI.Inv.Slots' 'Accepts')).Contains('CustomStore::Claimed')) 'SeneaL belt double-equipment safeguard remains present'
    Assert-Contract ((IL (Get-Method pov 'PassiveTree.Gear.State' 'UtilitySlotFor')).Contains('MagicSlots::IsTome')) 'PoV native route delegates to MP bridge'
    $name=IL (Get-Method game 'Player' 'GetPlayerName')
    $world=IL (Get-Method game 'ZNet' 'GetWorldName')
    Assert-Contract ($name.Contains('s_playerName') -and $name.Contains('"..."') -and $world.Contains('World::m_name')) 'Identity APIs read ZDO player name and world name'
    $loading=$assemblies.game.MainModule.GetType('Player').Fields | Where-Object Name -eq 'm_isLoading'
    Assert-Contract ($loading -and $loading.IsPrivate) 'Loading readiness requires audited private-field read'
    Write-Host "All $script:checks installed contract checks passed. Static metadata only; no runtime proof."
} finally { foreach ($assembly in $assemblies.Values) { $assembly.Dispose() } }
