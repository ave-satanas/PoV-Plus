[CmdletBinding()]
param(
    [switch]$Enabled,
    [string]$Dll = (Join-Path $PSScriptRoot '..\..\Source\PoVPlus\bin\Release\net48\PoVPlus.dll'),
    [string]$ValheimDevPath = 'S:\Valheim-Modding\PoVPlus-Dev'
)
$ErrorActionPreference = 'Stop'
Add-Type -LiteralPath (Join-Path $ValheimDevPath 'BepInEx\core\Mono.Cecil.dll')
$assembly = [Mono.Cecil.AssemblyDefinition]::ReadAssembly((Resolve-Path -LiteralPath $Dll).Path)
function Get-AllTypes($Types) { foreach ($type in $Types) { $type; Get-AllTypes $type.NestedTypes } }
try {
    $diagnostic = $assembly.MainModule.GetType('PoVPlus.Diagnostics.TomeDiagnostic')
    if (-not $Enabled) {
        if ($diagnostic) { throw 'Default build contains temporary diagnostic code.' }
        if ($assembly.MainModule.GetType('PoVPlus.Plugin').Methods.Name -contains 'Update') {
            throw 'Default plugin contains a polling callback.'
        }
        Write-Host 'PASS: ordinary build excludes diagnostic type and polling callback.'
        return
    }
    if (-not $diagnostic) { throw 'Diagnostic build is missing its probe.' }
    $methods = @((Get-AllTypes @($diagnostic)) | ForEach-Object { $_.Methods })
    $checked = 0
    foreach ($method in $methods) {
        if (-not $method.HasBody) { continue }
        foreach ($instruction in $method.Body.Instructions) {
            $operand = $instruction.Operand
            if ($instruction.OpCode.Code -in @('Stfld', 'Stsfld') -and $operand -is [Mono.Cecil.FieldReference]) {
                if ($operand.DeclaringType.FullName -notlike 'PoVPlus.Diagnostics.TomeDiagnostic*') {
                    throw "Foreign state write in $method : $instruction"
                }
            }
            if ($operand -is [Mono.Cecil.MethodReference]) {
                $checked++
                if ($operand.FullName -match '::(EquipItem|UnequipItem|AddItem|RemoveItem|AddStatusEffect|RemoveStatusEffect|Instantiate|Destroy|SetData|SetValue|set_Value|Bind|Save|WriteAllText|WriteAllBytes|CreateDirectory|Patch|PatchAll)\(') {
                    throw "Forbidden direct mutation call in $method : $instruction"
                }
            }
        }
    }
    Write-Host "PASS: enabled probe scanned $checked member calls; no foreign field stores or forbidden direct mutation calls."
    Write-Host 'Reflection targets and predicate purity still require the documented installed-IL review.'
} finally { $assembly.Dispose() }
