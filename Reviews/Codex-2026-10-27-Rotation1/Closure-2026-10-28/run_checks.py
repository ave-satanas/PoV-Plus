"""Relevant bounded regression/build checks against immutable revision sources in own scratch."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys
import time
import uuid

HERE=Path(__file__).resolve().parent
REPO=HERE.parents[2]
BASE=REPO/'.dev-state/rotation1/closure-review-20261028'
SNAP=BASE/'revision'
PS7=str(Path.home()/'.cache/codex-runtimes/codex-primary-runtime/dependencies/native/powershell/pwsh.exe')

def main():
    parser=argparse.ArgumentParser();parser.add_argument('--build-only',action='store_true');parser.add_argument('--no-git-query',action='store_true');args=parser.parse_args()
    run=BASE/('checks-'+uuid.uuid4().hex);run.mkdir()
    temp=REPO/'.dev-state/rotation1'/('t-'+uuid.uuid4().hex[:8]);temp.mkdir()
    env=os.environ.copy();env.update(TEMP=str(temp),TMP=str(temp),PYTHONDONTWRITEBYTECODE='1',DOTNET_CLI_TELEMETRY_OPTOUT='1')
    # Exact Git blobs use a marker for path fixtures, not a real checkout. This documented
    # SDK property disables only source-revision metadata queries for this disposable build.
    if args.no_git_query:env['EnableSourceControlManagerQueries']='false'
    checks=[]
    def execute(label,command,cwd=SNAP):
        start=time.monotonic();p=subprocess.run(command,cwd=cwd,env=env,stdout=subprocess.PIPE,stderr=subprocess.STDOUT,timeout=240)
        log=run/(label+'.txt');log.write_bytes(p.stdout)
        checks.append({'check':label,'command':command,'exit':p.returncode,'seconds':round(time.monotonic()-start,3),'log':str(log.relative_to(REPO)),'sha256':hashlib.sha256(p.stdout).hexdigest()})
        (run/'checks.json').write_text(json.dumps(checks,indent=2),encoding='utf-8')
        print(label,'exit',p.returncode,p.stdout.decode('utf-8',errors='replace')[-700:],flush=True)
        return p.returncode
    if not args.build_only:
        for label,host in [('ps5','powershell.exe'),('ps7',PS7)]:
            execute('additional-'+label,[host,'-NoProfile','-NonInteractive','-File',str(HERE/'Additional-Review.ps1'),'-Snapshot',str(SNAP),'-Root',str(run/('additional-'+label))],REPO)
        execute('workflow-ps5',['powershell.exe','-NoProfile','-NonInteractive','-File',str(SNAP/'Scripts/tests/Test-DevWorkflow.ps1')])
        execute('paths-ps5',['powershell.exe','-NoProfile','-NonInteractive','-File',str(SNAP/'Scripts/tests/Test-WorkspacePaths.ps1')])
        execute('python-revision',[sys.executable,'-B','-m','unittest','discover','-s','Scripts/tests','-v'])
    built=execute('release-build',['powershell.exe','-NoProfile','-NonInteractive','-File',str(SNAP/'Scripts/Build.ps1'),'-Configuration','Release'])
    if built==0:
        dll=SNAP/'Source/PoVPlus/bin/Release/net48/PoVPlus.dll'
        for name in ['Test-TomeDiagnostic','Test-TomeLifecycleArtifact','Test-TomeProvenanceArtifact','Test-TomeBridgeArtifact']:
            execute(name,['powershell.exe','-NoProfile','-NonInteractive','-File',str(SNAP/'Scripts/tests'/(name+'.ps1')),'-Dll',str(dll)])
        (run/'artifact.json').write_text(json.dumps({'path':str(dll.relative_to(REPO)),'sha256':hashlib.sha256(dll.read_bytes()).hexdigest(),'dlls':[p.name for p in dll.parent.glob('*.dll')]}),encoding='utf-8')
    execute('strict-knowledge-own',[sys.executable,'-B','Scripts/knowledge.py','validate','--strict'],REPO)
    execute('git-whitespace',['git','diff','--check'],REPO)
    print('CHECKS',run.relative_to(REPO)/'checks.json')

if __name__=='__main__':main()
