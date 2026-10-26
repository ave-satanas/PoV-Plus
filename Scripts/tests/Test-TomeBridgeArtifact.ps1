[CmdletBinding()]
param(
    [string]$Dll,
    [string]$ValheimDevPath = 'S:\Valheim-Modding\PoVPlus-Dev'
)
$ErrorActionPreference = 'Stop'
if (-not $Dll) { $Dll = Join-Path $PSScriptRoot '..\..\Source\PoVPlus\bin\Release\net48\PoVPlus.dll' }
Add-Type -LiteralPath (Join-Path $ValheimDevPath 'BepInEx\core\Mono.Cecil.dll')
$assembly = [Mono.Cecil.AssemblyDefinition]::ReadAssembly((Resolve-Path -LiteralPath $Dll).Path)
function Get-AllTypes($Types) { foreach ($type in $Types) { $type; Get-AllTypes $type.NestedTypes } }
try {
    $types = @(Get-AllTypes @($assembly.MainModule.Types | Where-Object Namespace -eq 'PoVPlus.Compatibility'))
    if ($types.Count -lt 3) { throw 'Ordinary build lacks bridge policy/activation/adapter.' }
    $plugin = $assembly.MainModule.GetType('PoVPlus.Plugin')
    $awake = $plugin.Methods | Where-Object Name -eq 'Awake'
    $instructions = @($awake.Body.Instructions)
    $bind = @($instructions | Where-Object { $_.Operand -is [Mono.Cecil.MethodReference] -and $_.Operand.Name -eq 'Bind' })
    if ($bind.Count -ne 1) { throw 'Expected one explicit feature configuration binding.' }
    $idx = [Array]::IndexOf($instructions, $bind[0])
    if ($instructions[$idx-1].OpCode.Code -ne 'Ldstr' -or $instructions[$idx-2].OpCode.Code -ne 'Ldc_I4_0' -or
        $instructions[$idx-3].Operand -ne 'EnableMagicPluginTomeBridge' -or $instructions[$idx-4].Operand -ne 'Compatibility.MagicSupremacy') {
        throw 'Feature configuration is not the reviewed section/key/default false.'
    }
    $bridge = $assembly.MainModule.GetType('PoVPlus.Compatibility.MagicPluginTomeBridge')
    foreach ($name in @('DescriptorPostfix','EquipmentPostfix')) {
        $method = $bridge.Methods | Where-Object Name -eq $name
        if (-not $method.IsPrivate -or -not $method.IsStatic -or $method.Parameters[-1].ParameterType.FullName -ne 'System.Boolean&') {
            throw "Unexpected postfix shape: $name"
        }
    }
    $checked = 0
    foreach ($method in @($types | ForEach-Object Methods)) {
        if (-not $method.HasBody) { continue }
        foreach ($instruction in $method.Body.Instructions) {
            $operand = $instruction.Operand
            if ($instruction.OpCode.Code -in @('Stfld','Stsfld') -and $operand -is [Mono.Cecil.FieldReference] -and
                $operand.DeclaringType.FullName -notlike 'PoVPlus.Compatibility.*') { throw "Foreign field write: $instruction" }
            if ($operand -is [Mono.Cecil.MethodReference]) {
                $checked++
                if ($operand.FullName -match '::(EquipItem|UnequipItem|AddItem|RemoveItem|AddSlot|RegisterSlot|RemoveSlot|SetEquippedItem|EquipSlotInternal|AddStatusEffect|RemoveStatusEffect|SetSavedEquippedGuid|SetCustomVisualState|Instantiate|Destroy|SetData|SetValue|set_Value|Bind|Save|WriteAllText|WriteAllBytes|CreateDirectory|PatchAll|SetEitr)\(') {
                    throw "Unauthorized bridge operation: $instruction"
                }
            }
        }
    }
    # No direct mod references: optional integrations remain behind the verified reflection boundary.
    if ($assembly.MainModule.AssemblyReferences.Name -match 'MagicPlugin|Magic_Supremacy|SeneaL|PathOfValheiman') {
        throw 'Unexpected hard third-party mod assembly reference.'
    }
    Write-Host "PASS bridge artifact: default false binding, two postfixes, $checked audited member calls; no direct equipment/slot/effect/config mutations."
    Write-Host 'This static audit and mocks do not prove runtime equipment, Harmony ordering or save correctness.'
} finally { $assembly.Dispose() }
