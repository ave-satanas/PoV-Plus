[CmdletBinding()]
param([string]$ValheimDevPath = 'S:\Valheim-Modding\PoVPlus-Dev')
# Static, read-only Mono.Cecil checks. Never loads/executes mod code or accesses saves.
$ErrorActionPreference = 'Stop'
Add-Type -LiteralPath (Join-Path $ValheimDevPath 'BepInEx\core\Mono.Cecil.dll')
$opened = @()
$passed = 0
function Check([bool]$Condition, [string]$Label) {
    if (-not $Condition) { throw "C1 contract drift: $Label" }
    $script:passed++; Write-Host "PASS $Label"
}
function Open-Pinned([string]$Relative, [string]$Hash) {
    $path = Join-Path $ValheimDevPath $Relative
    Check ((Get-FileHash -LiteralPath $path).Hash -eq $Hash) "pinned $Relative"
    $a = [Mono.Cecil.AssemblyDefinition]::ReadAssembly($path)
    $script:opened += $a
    return $a
}
function Method($Type, [string]$Name, [int]$ParameterCount = -1) {
    $found = @($Type.Methods | Where-Object { $_.Name -ceq $Name -and ($ParameterCount -lt 0 -or $_.Parameters.Count -eq $ParameterCount) })
    if ($found.Count -ne 1 -or -not $found[0].HasBody) { throw "Unknown method $($Type.FullName).$Name" }
    return $found[0]
}
function Calls($Method) {
    @($Method.Body.Instructions | Where-Object { $_.Operand -is [Mono.Cecil.MethodReference] } | ForEach-Object { $_.Operand.Name })
}
function IntValue($Instruction) {
    $code = [string]$Instruction.OpCode.Code
    if ($code -in @('Ldc_I4','Ldc_I4_S')) { return [int]$Instruction.Operand }
    if ($code -match '^Ldc_I4_([0-8])$') { return [int]$Matches[1] }
    throw "Unreviewed integer instruction: $Instruction"
}
function Resources($Method, [int]$Start = 0, [int]$End = 2147483647) {
    $il = @($Method.Body.Instructions); $result = @{}
    for ($i=0; $i -lt $il.Count-3; $i++) {
        if ($il[$i].Offset -lt $Start -or $il[$i].Offset -gt $End -or $il[$i].OpCode.Code -ne 'Ldstr') { continue }
        $call = $il[$i+3].Operand
        if ($call -is [Mono.Cecil.MethodReference] -and $call.DeclaringType.FullName -eq 'ItemManager.RequiredResourceList' -and $call.Name -eq 'Add') {
            if ((IntValue $il[$i+2]) -ne 0) { throw 'Unexpected upgrade requirement' }
            $result[[string]$il[$i].Operand] = IntValue $il[$i+1]
        }
    }
    return $result
}
function SameResources($Actual,$Expected) {
    if ($Actual.Count -ne $Expected.Count) { return $false }
    foreach($key in $Expected.Keys) { if (-not $Actual.ContainsKey($key) -or $Actual[$key] -ne $Expected[$key]) { return $false } }
    return $true
}
function ConfigSection([string]$Text,[string]$Name) {
    $match = [regex]::Match($Text, '(?ms)^\['+[regex]::Escape($Name)+'\]\r?\n(.*?)(?=^\[|\z)')
    if(-not $match.Success){throw "Missing section $Name"}
    $values=@{}
    foreach($line in ($match.Groups[1].Value -split '\r?\n')) {
        if($line -match '^([^#=]+?)\s*=\s*(.*)$') { $values[$Matches[1].Trim()]=$Matches[2].Trim() }
    }
    return $values
}
try {
    $mp=Open-Pinned 'BepInEx\plugins\blacks7ar-MagicPlugin\MagicPlugin.dll' '6504C2FF9CEAAE7B317820E41EF84D8C28D33FEDBF32B7D0F4FDDFD680BDA795'
    $ms=Open-Pinned 'BepInEx\plugins\Dreanegade-Magic_Supremacy\Magic_Supremacy.dll' '8BAFE7DAEFB9B6AF057FC3B10534764ED9CCEBA430B526A014B2DD12CFABA787'
    $game=Open-Pinned 'valheim_Data\Managed\assembly_valheim.dll' '94998885B103D6E5F5480123EAEE0A093E82AF2DDEEEE37536C15BF555F24F3A'
    $ui=Open-Pinned 'BepInEx\plugins\seneaL-SeneaL_UI\SeneaL-UI\SeneaL-UI.dll' '53B0F675BFFD0BAC5FCCA711AF04AA1EF00BB5BECD543D702EBAA146E5C487CD'
    $enum=$mp.MainModule.GetType('ItemManager.CraftingTable')
    Check ((@($enum.Fields | Where-Object Name -eq 'Workbench')[0]).Constant -eq 2) 'MP Workbench enum maps to registered value2'
    $config=[IO.File]::ReadAllText((Join-Path $ValheimDevPath 'BepInEx\config\blacks7ar.MagicPlugin.cfg'))
    $cases=@(
        @{Method='BeginnersBook';Section='Beginners Magic Book';Prefab='BMP_BeginnersMagicBook';Costs=@{GreydwarfEye=6;Resin=4;Feathers=6;Wood=12}},
        @{Method='AdvanceBook';Section='Advance Magic Book';Prefab='BMP_AdvanceMagicBook';Costs=@{YmirRemains=2;SurtlingCore=5;Crystal=52;AncientSeed=12}},
        @{Method='DruidsTome';Section='Druids Tome';Prefab='BMP_DruidsTome';Costs=@{GoblinTotem=3;LoxPelt=12;LinenThread=16;ElderBark=8}}
    )
    foreach($case in $cases) {
        $m=Method ($mp.MainModule.GetType('MagicPlugin.Functions.PrefabsSetup')) $case.Method
        Check (SameResources (Resources $m) $case.Costs) "$($case.Prefab) installed registration costs"
        $il=@($m.Body.Instructions)
        $add=@($il | Where-Object { $_.Operand -is [Mono.Cecil.MethodReference] -and $_.Operand.DeclaringType.FullName -eq 'ItemManager.CraftingStationList' -and $_.Operand.Name -eq 'Add' })
        Check ($add.Count -eq 1 -and (IntValue $add[0].Previous.Previous) -eq 2 -and (IntValue $add[0].Previous) -eq 1) "$($case.Prefab) registered Workbench1"
        $section=ConfigSection $config $case.Section; $costs=@{}
        foreach($entry in ($section['Crafting Costs'] -split ',')) {$pair=$entry -split ':';$costs[$pair[0]]=[int]$pair[1]}
        Check ($section['Crafting Station'] -eq 'Workbench' -and $section['Crafting Station Level'] -eq '1' -and $section['Require only one resource'] -eq 'Off' -and (SameResources $costs $case.Costs)) "$($case.Prefab) disk configuration agrees; no live proof"
    }
    $awake=Method ($ms.MainModule.GetType('Magic_Supremacy.Magic_SupremacyPlugin')) 'Awake'
    Check (SameResources (Resources $awake 0x35ef 0x3631) @{PageArcane_DO=4;SpiritMushroom_DO=6;DimEitr_DO=4;MushroomYellow=4}) 'native Mushroomcaller registered costs'
    $il=@($awake.Body.Instructions)
    Check (($il | Where-Object Offset -eq 0x35e3).Operand -eq 'MagicWeaver_DO' -and (IntValue ($il | Where-Object Offset -eq 0x35e8)) -eq 4) 'native Tome registered MagicWeaver4'
    $system=$ms.MainModule.GetType('Magic_Supremacy.CustomSlotSystem')
    Check ((Calls (Method $system 'GetRuntimeData')) -contains 'GetOrCreateValue') 'MS runtime getter is potentially creating: forbidden to observer'
    Check ((Calls (Method $system 'GetEquippedItem')) -contains 'GetRuntimeData') 'MS GetEquippedItem is not an unconditional pure read'
    Check ((Calls (Method $system 'GetDefinitionBySlotId')) -contains 'EnsureDefinitionsInitialized') 'MS definition getter may initialize: forbidden to observer'
    Check ($system.Fields.Name -contains 'RuntimeData' -and ($system.NestedTypes | Where-Object Name -eq 'HumanoidCustomSlotData').Fields.Name -contains 'EquippedItems') 'existing MS runtime table/ownership fields available for TryGetValue design'
    $store=$ui.MainModule.GetType('SeneaLUI.Inv.CustomStore')
    $claimed=Method $store 'Claimed'
    Check (@($claimed.Body.Instructions | Where-Object { $_.OpCode.Code -eq 'Stsfld' -and $_.Operand.Name -eq '_plainBelt' }).Count -eq 1) 'SeneaL Claimed mutates cache: do not call from observer'
    Check ($store.Fields.Name -contains '<Inv>k__BackingField' -and $store.Fields.Name -contains 'Last') 'SeneaL existing inventory/last fields available; correlation still required'
    foreach($case in @(@('Humanoid','GetInventory','m_inventory'),@('Player','GetEitr','m_eitr'),@('Player','GetMaxEitr','m_maxEitr'),@('SEMan','GetStatusEffects','m_statusEffects'),@('Inventory','GetAllItems','m_inventory'))) {
        $m=Method ($game.MainModule.GetType($case[0])) $case[1] 0
        Check ($m.Body.Instructions.Count -eq 3 -and $m.Body.Instructions[1].OpCode.Code -eq 'Ldfld' -and $m.Body.Instructions[1].Operand.Name -eq $case[2]) "$($case[0]).$($case[1]) direct-field getter (patch chain still requires runtime review)"
    }
    Write-Host "All $passed Stage C1 installed static contracts passed. No mod execution, lifecycle proof or runtime probe implementation."
} finally { foreach($a in $opened){$a.Dispose()} }
