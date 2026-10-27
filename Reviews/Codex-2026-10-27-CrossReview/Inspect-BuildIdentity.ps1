$ErrorActionPreference='Stop'
$repo=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$state=Join-Path $repo '.dev-state/crossreview-a'
Add-Type -LiteralPath 'S:\Valheim-Modding\PoVPlus-Dev\BepInEx/core/Mono.Cecil.dll'
function Walk($types){foreach($t in $types){$t;Walk $t.NestedTypes}}
$rows=@(foreach($entry in (Get-Content -LiteralPath (Join-Path $state 'identity-results.json') -Raw|ConvertFrom-Json)){
 $module=[Mono.Cecil.ModuleDefinition]::ReadModule((Join-Path $repo $entry.path))
 try{
  $il=@(foreach($t in (Walk $module.Types)){foreach($m in $t.Methods){if($m.HasBody){$m.FullName;foreach($i in $m.Body.Instructions){$i.ToString()}}}}) -join "`n"
  $sha=[Security.Cryptography.SHA256]::Create()
  try{$ilhash=[BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($il))).Replace('-','')}finally{$sha.Dispose()}
  $debug=@(foreach($e in $module.GetDebugHeader().Entries){if($e.Directory.Type.ToString() -eq 'CodeView'){[Text.Encoding]::UTF8.GetString($e.Data,24,$e.Data.Length-25)}})
  [ordered]@{mode=$entry.mode;root=$entry.root;sha256=$entry.sha256;mvid=$module.Mvid.ToString();codeview=$debug;method_il_sha256=$ilhash}
 }finally{$module.Dispose()}
})
$rows|ConvertTo-Json -Depth 5|Set-Content -LiteralPath (Join-Path $state 'identity-audit.json') -Encoding UTF8
$rows|ForEach-Object {Write-Host "$($_.mode) $($_.root) IL=$($_.method_il_sha256) CodeView=$($_.codeview)"}
