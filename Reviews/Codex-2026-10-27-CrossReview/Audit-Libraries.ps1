# Calls the immutable baseline auditor on locally compiled synthetic libraries only.
$ErrorActionPreference='Stop'
$repo=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$state=Join-Path $repo '.dev-state/crossreview-a'
$audit=Join-Path $state 'immutable/Scripts/tests/Test-TomeProvenanceArtifact.ps1'
$expected=[ordered]@{
 'corrected'='PASS';'unsafe-collection'='Concrete collection guard missing';
 'endpoint-continuity'='Unsupported claim promotion: Snapshot.Continuity';
 'caller-coverage'='Unsupported claim promotion: ProvenanceLedger.TransitionCoverage';
 'caller-initial'='Unsupported claim promotion: ProvenanceLedger.InitialStateValidity';
 'alias-context'='Missing observation scope comparison';'storage-budget'='Traversal bound missing';
 'string-budget'='Collection string bound missing';'escape-idictionary-add'='PASS';'escape-array-clear'='PASS';
 'reachable-add'='PASS';'reachable-clear'='PASS'
}
$rows=@(foreach($name in $expected.Keys){
 $dll=Join-Path $state "builds/lib-$name/bin/Release/net48/PoVPlus.dll"
 try{$null=& $audit -Enabled -Dll $dll 6>&1;$observed='PASS'}catch{$observed=$_.Exception.Message}
 if($observed -cne $expected[$name]){throw "Unexpected $name audit=$observed"}
 [ordered]@{name=$name;sha256=(Get-FileHash -LiteralPath $dll).Hash;verdict=$observed}
})
$rows|ConvertTo-Json|Set-Content -LiteralPath (Join-Path $state 'audit-results.json') -Encoding UTF8
$rows|ForEach-Object {Write-Host "$($_.name): $($_.verdict)"}
