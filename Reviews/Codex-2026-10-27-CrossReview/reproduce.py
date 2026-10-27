"""Reproduce immutable Claude review fixtures in THIS worktree's ignored scratch.

No game execution, installation writes, other-worktree reads/writes or live adapters.
Run from the primary repository: python -B Reviews/Codex-2026-10-27-CrossReview/reproduce.py
Compiler/process output is retained under .dev-state/crossreview-a. Target code is synthetic.
"""
import concurrent.futures
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys
import xml.sax.saxutils

REPO=Path(__file__).resolve().parents[2]
STATE=REPO/'.dev-state/crossreview-a'
ROOT=STATE/'immutable'
PACKAGE=ROOT/'Reviews/Claude-2026-10-27'
PROOF=ROOT/'Source/PoVPlus/Diagnostics/PassiveOwnershipProof.cs'
HARNESS=ROOT/'Scripts/tests/TomeProvenanceHarness.cs'
BASE='0c1f3d1a909e0c40a7bcf36425f8efc6a1c09722'
CLAUDE='d84903d8f383d8dac24c635fc9fa675b5e87264b'


def command(args, directory, log):
    completed=subprocess.run(args,cwd=directory,stdout=subprocess.PIPE,stderr=subprocess.STDOUT,
                             text=True,encoding='utf-8',errors='replace',timeout=180)
    log.write_text(completed.stdout,encoding='utf-8')
    return completed.returncode,completed.stdout


def build(name, source, harness=None, arguments=()):
    directory=STATE/'builds'/name
    directory.mkdir(parents=True,exist_ok=True)
    (directory/'Proof.cs').write_bytes(Path(source).read_bytes())
    includes=['Proof.cs']
    if harness:
        (directory/'Harness.cs').write_bytes(Path(harness).read_bytes());includes+=['Harness.cs']
    project=directory/'PoVPlus.csproj'
    symbols='POVPLUS_TOME_PROVENANCE_POC'+(';POVPLUS_PROVENANCE_FIXTURE' if harness else '')
    project.write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>'
        '<TargetFramework>net48</TargetFramework><OutputType>'+('Exe' if harness else 'Library')+'</OutputType>'
        '<AssemblyName>PoVPlus</AssemblyName><AutomaticallyUseReferenceAssemblyPackages>false</AutomaticallyUseReferenceAssemblyPackages>'
        '<EnableDefaultCompileItems>false</EnableDefaultCompileItems><LangVersion>7.3</LangVersion>'
        '<DefineConstants>'+symbols+'</DefineConstants></PropertyGroup><ItemGroup>'+
        ''.join('<Compile Include="'+xml.sax.saxutils.escape(p)+'" />' for p in includes)+'</ItemGroup></Project>')
    for label,args in [('restore',['dotnet','restore',str(project),'--source',str(ROOT),'--nologo']),
                       ('compile',['dotnet','build',str(project),'--no-restore','-c','Release','--nologo'])]:
        code,output=command(args,REPO,directory/(label+'.txt'))
        if code: raise RuntimeError(name+' '+label+' failed: '+output[-1200:])
    artifact=directory/'bin/Release/net48'/('PoVPlus.exe' if harness else 'PoVPlus.dll')
    result=dict(name=name,source_sha256=hashlib.sha256(Path(source).read_bytes()).hexdigest(),
                harness_sha256=hashlib.sha256(Path(harness).read_bytes()).hexdigest() if harness else None,
                artifact=str(artifact.relative_to(REPO)),artifact_sha256=hashlib.sha256(artifact.read_bytes()).hexdigest())
    if harness:
        code,output=command([str(artifact),*arguments],REPO,directory/'run.txt')
        result.update(exit=code,output=output)
    return result


def initialize():
    STATE.mkdir(parents=True,exist_ok=True)
    paths=subprocess.check_output(['git','ls-tree','-r','--name-only',CLAUDE,'--','Reviews/Claude-2026-10-27'],cwd=REPO,text=True).splitlines()
    extra=['Source/PoVPlus/Diagnostics/PassiveOwnershipProof.cs','Scripts/tests/TomeProvenanceHarness.cs','Scripts/tests/Test-TomeProvenanceArtifact.ps1','Scripts/tests/Test-TomeProvenance.ps1']
    extra+=['Scripts/tests/Fixtures/EXP005Original/'+p for p in ['PassiveOwnershipProof.cs','TomeProvenanceHarness.cs','manifest.json']]
    rows=[]
    for sha,files in [(CLAUDE,paths),(BASE,extra)]:
        for path in files:
            content=subprocess.check_output(['git','show',sha+':'+path],cwd=REPO)
            target=ROOT/path;target.parent.mkdir(parents=True,exist_ok=True);target.write_bytes(content)
            rows.append(dict(commit=sha,path=path,sha256=hashlib.sha256(content).hexdigest()))
    (STATE/'inputs.json').write_text(json.dumps(rows,indent=2))
    frozen=ROOT/'Scripts/tests/Fixtures/EXP005Original'
    for row in json.loads((frozen/'manifest.json').read_text())['files']:
        assert hashlib.sha256((frozen/row['path']).read_bytes()).hexdigest().upper()==row['sha256']
    for generator,folder in [('sweep_mutants.py','sweep'),('escape_mutants.py','escape'),('regression_mutants.py','regression')]:
        out=STATE/folder;out.mkdir(exist_ok=True)
        code,log=command([sys.executable,'-I',str(PACKAGE/'tests'/generator),str(PROOF),str(out)],REPO,STATE/(generator+'.txt'))
        assert code==0,log


def main():
    initialize()
    results=[]
    frozen=ROOT/'Scripts/tests/Fixtures/EXP005Original'
    for name,source,harness,args in [
        ('original',frozen/'PassiveOwnershipProof.cs',frozen/'TomeProvenanceHarness.cs',()),
        ('falsify',frozen/'PassiveOwnershipProof.cs',frozen/'TomeProvenanceHarness.cs',('--falsify',)),
        ('corrected',PROOF,HARNESS,()),
        ('adversarial',PROOF,PACKAGE/'tests/ClaudeAdversarial.cs',()),
        ('discriminating',PROOF,PACKAGE/'tests/Discriminating.cs',())]:
        row=build(name,source,harness,args);results.append(row);print(name,row['exit'],row['output'].splitlines()[-1],flush=True)
    assert [r['exit'] for r in results]==[0,2,0,0,0]
    names=(STATE/'sweep/names.txt').read_text().splitlines()
    assert len(names)==77 and len(set(names))==77
    with concurrent.futures.ThreadPoolExecutor(max_workers=6) as pool:
        rows=list(pool.map(lambda n:build('s-'+n,STATE/'sweep'/(n+'.cs'),HARNESS),names))
    assert all(r['exit'] in [0,1] for r in rows)
    killed=[r for r in rows if r['exit']==1];survivors=[r['name'][2:] for r in rows if r['exit']==0]
    print('sweep: killed',len(killed),'survived',len(survivors),flush=True)
    results+=rows;(STATE/'results.json').write_text(json.dumps(results,indent=2))
    assert len(killed)==22 and len(survivors)==55
    with concurrent.futures.ThreadPoolExecutor(max_workers=6) as pool:
        rows=list(pool.map(lambda n:build('d-'+n,STATE/'sweep'/(n+'.cs'),PACKAGE/'tests/Discriminating.cs'),survivors))
    for row in rows:
        assert row['exit'] in [0,1]
        if row['exit']==1:
            failures=[line for line in row['output'].splitlines() if line.startswith('FAIL')]
            assert len(failures)==1 and '[discriminates '+row['name'][2:]+']' in failures[0],row
    print('discriminating:',sum(r['exit']==1 for r in rows),'killed',sum(r['exit']==0 for r in rows),'survived',flush=True)
    assert sum(r['exit']==1 for r in rows)==36
    results+=rows
    for group in ['regression','escape']:
        for source in sorted((STATE/group).glob('*.cs')):
            row=build(group+'-'+source.stem,source,HARNESS);results.append(row)
            assert row['exit']==(1 if group=='regression' else 0),row
            results.append(build('lib-'+source.stem,source))
    results.append(build('lib-corrected',PROOF))
    (STATE/'results.json').write_text(json.dumps(results,indent=2))
    print('PASS exact immutable reproduction; all compile/restore steps succeeded',flush=True)


if __name__=='__main__':
    main()
