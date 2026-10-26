[CmdletBinding()]
param(
    [switch]$Enabled,
    [switch]$StageB,
    [string]$Dll = (Join-Path $PSScriptRoot '..\..\Source\PoVPlus\bin\Release\net48\PoVPlus.dll'),
    [string]$ValheimDevPath = 'S:\Valheim-Modding\PoVPlus-Dev'
)
$ErrorActionPreference = 'Stop'
if ($StageB -and -not $Enabled) { throw 'Stage B artifact audit also requires -Enabled.' }
Add-Type -LiteralPath (Join-Path $ValheimDevPath 'BepInEx\core\Mono.Cecil.dll')
$assembly = [Mono.Cecil.AssemblyDefinition]::ReadAssembly((Resolve-Path -LiteralPath $Dll).Path)
function Get-AllTypes($Types) { foreach ($type in $Types) { $type; Get-AllTypes $type.NestedTypes } }
try {
    $diagnostic = $assembly.MainModule.GetType('PoVPlus.Diagnostics.TomeDiagnostic')
    if (-not $Enabled) {
        if ($diagnostic) { throw 'Default build contains temporary diagnostic code.' }
        if (@($assembly.MainModule.Types | Where-Object Namespace -eq 'PoVPlus.Diagnostics').Count) {
            throw 'Default build contains diagnostic support types.'
        }
        if ($assembly.MainModule.GetType('PoVPlus.Plugin').Methods.Name -contains 'Update') {
            throw 'Default plugin contains a polling callback.'
        }
        Write-Host 'PASS: ordinary build excludes diagnostic type and polling callback.'
        return
    }
    if (-not $diagnostic) { throw 'Diagnostic build is missing its probe.' }
    $probe = $assembly.MainModule.GetType('PoVPlus.Diagnostics.TomeStageBProbe')
    if ([bool]$probe -ne [bool]$StageB) { throw 'Stage B probe inclusion does not match the explicit expected build mode.' }
    if ($StageB) {
        $ownLookups = @($probe.Methods | Where-Object HasBody | ForEach-Object { $_.Body.Instructions } |
            Where-Object { $_.Operand -is [Mono.Cecil.MethodReference] -and $_.Operand.DeclaringType.FullName -eq 'BepInEx.Bootstrap.Chainloader' })
        if ($ownLookups.Count) { throw 'Stage B probe must not query own Chainloader registration during Awake.' }
        $ctor = $diagnostic.Methods | Where-Object IsConstructor
        if ($ctor.Parameters.Count -ne 2 -or $ctor.Parameters[1].ParameterType.FullName -ne 'System.Nullable`1<System.Boolean>') { throw 'Missing explicit immutable setting snapshot constructor.' }
        $awake = $assembly.MainModule.GetType('PoVPlus.Plugin').Methods | Where-Object Name -eq 'Awake'
        $newProbe = @($awake.Body.Instructions | Where-Object { $_.OpCode.Code -eq 'Newobj' -and $_.Operand.DeclaringType.FullName -eq 'PoVPlus.Diagnostics.TomeDiagnostic' })
        if ($newProbe.Count -ne 1 -or $newProbe[0].Previous.Operand.DeclaringType.FullName -ne 'System.Nullable`1<System.Boolean>' -or
            $newProbe[0].Previous.Previous.Operand.Name -ne 'get_Value') { throw 'Awake must pass bound setting value directly to diagnostic.' }
        $calls = @($diagnostic.Methods | Where-Object HasBody | ForEach-Object { $_.Body.Instructions } |
            Where-Object { $_.Operand -is [Mono.Cecil.MethodReference] } | ForEach-Object { $_.Operand.FullName })
        foreach ($required in @('CheckStartup','RequireInitializedRegistry','CaptureInternal','CheckVisible','RequireReviewed')) {
            if (-not ($calls -match "TomeStageBProbe::$required\(")) { throw "Missing Stage B observation guard: $required" }
        }
    }
    $methods = @((Get-AllTypes @($assembly.MainModule.Types | Where-Object Namespace -eq 'PoVPlus.Diagnostics')) | ForEach-Object { $_.Methods })
    $checked = 0
    foreach ($method in $methods) {
        if (-not $method.HasBody) { continue }
        foreach ($instruction in $method.Body.Instructions) {
            $operand = $instruction.Operand
            if ($instruction.OpCode.Code -in @('Stfld', 'Stsfld') -and $operand -is [Mono.Cecil.FieldReference]) {
                if ($operand.DeclaringType.FullName -notlike 'PoVPlus.Diagnostics.*') {
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
