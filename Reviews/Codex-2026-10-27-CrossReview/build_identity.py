"""Compile the same immutable ordinary plugin in two private roots; never deploy/run it."""
import hashlib,json,subprocess
from pathlib import Path
BASE='0c1f3d1a909e0c40a7bcf36425f8efc6a1c09722'
repo=Path(__file__).resolve().parents[2]
state=repo/'.dev-state/crossreview-a'
files=subprocess.check_output(['git','ls-tree','-r','--name-only',BASE,'--','Source'],cwd=repo,text=True).splitlines()
rows=[]
for mode in ['ordinary','pathmap','pathmap-native','ci']:
    for name in ['a','b']:
        root=state/'identity'/name
        for path in files:
            target=root/path;target.parent.mkdir(parents=True,exist_ok=True)
            target.write_bytes(subprocess.check_output(['git','show',BASE+':'+path],cwd=repo))
        project=root/'Source/PoVPlus/PoVPlus.csproj'
        output=root/'artifacts'/mode
        extra=['-p:OutputPath='+str(output).replace('\\','/')+'/']
        if mode=='pathmap':extra+=['-p:PathMap='+str(root).replace('\\','/')+'=/_/']
        if mode=='pathmap-native':extra+=['-p:PathMap='+str(root)+'=/_/']
        if mode=='ci':extra+=['-p:ContinuousIntegrationBuild=true']
        for phase,args in [('restore',['dotnet','restore',str(project),'--source',str(root),'--nologo']),
                           ('build',['dotnet','build',str(project),'--no-restore','-t:Rebuild','-c','Release','--nologo',*extra])]:
            p=subprocess.run(args,cwd=repo,stdout=subprocess.PIPE,stderr=subprocess.STDOUT,text=True,encoding='utf-8',errors='replace')
            (state/(mode+'-'+name+'-'+phase+'.txt')).write_text(p.stdout)
            if p.returncode:raise RuntimeError(p.stdout)
        dll=output/'PoVPlus.dll'
        rows.append(dict(mode=mode,root=name,path=str(dll.relative_to(repo)),sha256=hashlib.sha256(dll.read_bytes()).hexdigest()))
    print(mode,rows[-2]['sha256'],rows[-1]['sha256'],'equal',rows[-2]['sha256']==rows[-1]['sha256'],flush=True)
(state/'identity-results.json').write_text(json.dumps(rows,indent=2))
