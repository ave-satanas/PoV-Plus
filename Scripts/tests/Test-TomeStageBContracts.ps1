[CmdletBinding()]
param([string]$ValheimDevPath = 'S:\Valheim-Modding\PoVPlus-Dev')
$ErrorActionPreference = 'Stop'
Add-Type -LiteralPath (Join-Path $ValheimDevPath 'BepInEx\core\Mono.Cecil.dll')
$path = Join-Path $ValheimDevPath 'BepInEx\plugins\Dreanegade-Magic_Supremacy\Magic_Supremacy.dll'
if ((Get-FileHash -LiteralPath $path).Hash -ne '8BAFE7DAEFB9B6AF057FC3B10534764ED9CCEBA430B526A014B2DD12CFABA787') { throw 'Unreviewed MS binary' }
$a = [Mono.Cecil.AssemblyDefinition]::ReadAssembly($path)
try {
    $t = $a.MainModule.GetType('Magic_Supremacy.CustomSlotSystem')
    $getter = $t.Methods | Where-Object Name -eq 'GetSharedSlotRegistry'
    $gate = $t.Methods | Where-Object Name -eq 'IsItemValidForSlot'
    $il = @($getter.Body.Instructions)
    if (-not $getter.IsPrivate -or -not $getter.IsStatic -or $getter.Parameters.Count -ne 0) { throw 'Getter signature drift' }
    if (($il | Where-Object Offset -eq 0x16).Operand.Offset -ne 0x1a -or
        ($il | Where-Object Offset -eq 0x18).OpCode.Code -ne 'Ldloc_0' -or
        ($il | Where-Object Offset -eq 0x19).OpCode.Code -ne 'Ret') { throw 'Existing registry fast-return drift' }
    if (@($il | Where-Object { $_.Operand -is [Mono.Cecil.MethodReference] -and $_.Operand.Name -eq 'SetData' }).Count -ne 1 -or
        ($il | Where-Object Offset -eq 0x30).Operand.Name -ne 'SetData') { throw 'Conditional initializer drift' }
    if (@($gate.Body.Instructions | Where-Object { $_.OpCode.Code -in @('Stfld','Stsfld') }).Count) { throw 'Validator now writes fields' }
    $calls = @($gate.Body.Instructions | Where-Object { $_.Operand -is [Mono.Cecil.MethodReference] } | ForEach-Object { $_.Operand.Name })
    foreach ($name in $calls) { if ($name -notin @('op_Equality','IsNullOrWhiteSpace','GetSharedSlotRegistry','TryGetValue','get_name','Contains')) { throw "Unreviewed validator call: $name" } }
    Write-Host 'PASS Stage B purity contract: pinned MS; initialized dictionary returns before SetData; validator calls only reviewed reads.'
    Write-Host 'Runtime must also require existing native control, unpatched helper, and the exact two reviewed bridge postfixes.'
} finally { $a.Dispose() }
