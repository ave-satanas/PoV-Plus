[CmdletBinding()]
param([string]$Output='.dev-state/dev-008/persistence-il.json')
$ErrorActionPreference='Stop'
$repo=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$outputFull=[IO.Path]::GetFullPath((Join-Path $repo $Output))
$state=[IO.Path]::GetFullPath((Join-Path $repo '.dev-state'))+[IO.Path]::DirectorySeparatorChar
if(-not $outputFull.StartsWith($state,[StringComparison]::OrdinalIgnoreCase)){throw 'Evidence output must be repository-local .dev-state'}
function Assert-NoOutputReparse {
 $candidate=$outputFull
 while($candidate){
  if(Test-Path -LiteralPath $candidate){
   if((Get-Item -LiteralPath $candidate -Force).Attributes -band [IO.FileAttributes]::ReparsePoint){throw 'Evidence output crosses a reparse point'}
  }
  $parent=Split-Path -Parent $candidate
  if($parent -eq $candidate){break}
  $candidate=$parent
 }
}
Assert-NoOutputReparse
# Metadata only. Never load target game/plugin assemblies for execution.
$dev='S:\Valheim-Modding\PoVPlus-Dev'
Add-Type -LiteralPath (Join-Path $dev 'BepInEx/core/Mono.Cecil.dll')
function Walk($types){foreach($t in $types){$t;Walk $t.NestedTypes}}
$paths=@('valheim_Data/Managed/assembly_valheim.dll','valheim_Data/Managed/assembly_utils.dll','BepInEx/core/BepInEx.dll')
$paths+=@(Get-ChildItem -LiteralPath (Join-Path $dev 'valheim_Data/Managed') -Filter '*Splatform*.dll' | ForEach-Object {'valheim_Data/Managed/'+$_.Name})
$paths+=@(Get-ChildItem -LiteralPath (Join-Path $dev 'BepInEx/plugins') -Filter '*.dll' -Recurse | ForEach-Object {$_.FullName.Substring($dev.Length+1)})
$images=@(foreach($rel in $paths){
 $path=Join-Path $dev $rel
 $hash=(Get-FileHash -LiteralPath $path).Hash
 $a=[Mono.Cecil.AssemblyDefinition]::ReadAssembly($path)
 try{
 $isGame=$rel -match 'assembly_(valheim|utils)\.dll$'
  $selected=@();$io=@();$strings=@();$callSites=@()
  foreach($t in (Walk $a.MainModule.Types)){foreach($m in $t.Methods){if(-not $m.HasBody){continue}
   $hits=@($m.Body.Instructions|Where-Object {
     ($_.Operand -is [Mono.Cecil.MemberReference] -and $_.Operand.FullName -match 'System.IO.(File|Directory|Path|FileStream)|GetFolderPath|persistentDataPath|PlayerPrefs::|SteamRemoteStorage|FileHelpers::|Utils::GetSaveDataPath') -or
     ($_.OpCode.Code -eq 'Ldstr' -and $_.Operand -match 'savedir|characters_local|worlds_local|AppData|LocalLow')
   })
   if($hits.Count){$io+= [ordered]@{method=$m.FullName;token=$m.MetadataToken.ToUInt32();hits=@($hits|ForEach-Object ToString);il=@($m.Body.Instructions|ForEach-Object ToString)}}
   if(($isGame -and ($t.FullName -match '^(FileHelpers|Utils|SaveSystem|SaveCollection|SaveWithBackups|SaveFile|PlayerProfile|World|FejdStartup|SteamManager|Game|Settings|ZNet)(/|$)' -and
      ($m.Name -match 'Save|Load|Path|Cloud|Awake|Init|Start|File|Move|Backup|Remote|Preference|Steam|Source|UpdateProfile|ParseArguments|ParseServerArguments|\.cctor') -or $t.FullName -eq 'FileHelpers')) -or
      ($rel -eq 'BepInEx/core/BepInEx.dll' -and $t.FullName -eq 'BepInEx.Paths')){
      $selected+= [ordered]@{method=$m.FullName;token=$m.MetadataToken.ToUInt32();il=@($m.Body.Instructions|ForEach-Object ToString)}
   }
   foreach($i in $m.Body.Instructions){
    if($i.Operand -is [Mono.Cecil.MemberReference] -and $i.Operand.FullName -match '::(ParseServerArguments|SetSaveDataPath|GetSaveDataPath)|Utils::persistantDataPath|RestartAppIfNecessary'){
     $callSites+=[ordered]@{method=$m.FullName;token=$m.MetadataToken.ToUInt32();instruction=$i.ToString()}
    }
    if($i.OpCode.Code -eq 'Ldstr' -and $i.Operand -match 'savedir|characters_local|worlds_local|AppData|LocalLow'){
     $strings+= [ordered]@{method=$m.FullName;token=$m.MetadataToken.ToUInt32();instruction=$i.ToString()}
   }}
  }}
  [ordered]@{path=$rel;sha256=$hash;identity=$a.Name.FullName;mvid=$a.MainModule.Mvid.ToString();methods=$selected;io=$io;strings=$strings;callSites=$callSites}
 }finally{$a.Dispose()}
 if((Get-FileHash -LiteralPath $path).Hash -ne $hash){throw "Image changed during read: $rel"}
})
[IO.Directory]::CreateDirectory((Split-Path -Parent $outputFull))|Out-Null
Assert-NoOutputReparse
$images|ConvertTo-Json -Depth 10|Set-Content -LiteralPath $outputFull -Encoding UTF8
Write-Host "Exported metadata for $($images.Count) images to $outputFull; no target code executed."
