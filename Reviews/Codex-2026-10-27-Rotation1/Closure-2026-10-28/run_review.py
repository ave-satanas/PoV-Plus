"""Bounded independent review runner. No checkout, installation writes or game process calls.

Exports exact Git objects into ignored local scratch; runs separate PowerShell processes.
Results retain actual failing assertions and exit codes, including the seven expected old failures.
"""
from pathlib import Path
import argparse
import hashlib
import json
import os
import subprocess
import sys
import time
import uuid

HERE=Path(__file__).resolve().parent
REPO=HERE.parents[2]
BASE=REPO/'.dev-state/rotation1/closure-review-20261028'
COMMITS={'original':'633ddb2941942fd0136bc892a37895d358f4fb87','revision':'d1a1761016a69c84603cea0ada66eb441aa905c7'}
PS7=Path.home()/'.cache/codex-runtimes/codex-primary-runtime/dependencies/native/powershell/pwsh.exe'

def sha(data):return hashlib.sha256(data).hexdigest()

def prepare():
    for p in [BASE,*BASE.parents]:
        if p.exists():assert not p.is_symlink() and not p.stat().st_file_attributes & 0x400
    BASE.mkdir(parents=True,exist_ok=True)
    for label,c in COMMITS.items():
        dest=BASE/label;dest.mkdir(exist_ok=True);rows=[]
        names=subprocess.check_output(['git','ls-tree','-r','--name-only',c,'--','Scripts','Source'],cwd=REPO,text=True).splitlines()
        for name in names:
            data=subprocess.check_output(['git','show',c+':'+name],cwd=REPO)
            target=dest/name;target.parent.mkdir(parents=True,exist_ok=True)
            if target.exists():assert target.read_bytes()==data,('snapshot changed',name)
            else:target.write_bytes(data)
            rows.append({'path':name,'sha256':sha(data)})
        (dest/'.git').write_text('fixture marker; not a worktree',encoding='ascii')
        (dest/'blob-manifest.json').write_text(json.dumps({'commit':c,'files':rows},indent=2),encoding='utf-8')
    # Author's new harness is an explicit overlay on the old sources, not independent evidence.
    name='Scripts/tests/Test-Rotation1Closure.ps1'
    data=subprocess.check_output(['git','show',COMMITS['revision']+':'+name],cwd=REPO)
    target=BASE/'original'/name
    if target.exists():assert target.read_bytes()==data
    else:target.write_bytes(data)

def main():
    ap=argparse.ArgumentParser();ap.add_argument('--author',action='store_true');ap.add_argument('--short-temp',action='store_true');ap.add_argument('--host',choices=['ps5','ps7','both'],default='both');args=ap.parse_args()
    prepare()
    run=BASE/('run-'+uuid.uuid4().hex);run.mkdir()
    temp=REPO/'.dev-state/rotation1'/('tc-'+uuid.uuid4().hex[:8]) if args.short_temp else run/'temp'
    temp.mkdir()
    env=os.environ.copy();env['TEMP']=str(temp);env['TMP']=str(temp);env['PYTHONDONTWRITEBYTECODE']='1'
    hosts={'ps5':'powershell.exe','ps7':str(PS7)}
    results=[]
    for h,host in hosts.items():
        if args.host not in ('both',h):continue
        for label in COMMITS:
            fixture=run/(h+'-'+label+'-fixture');output=run/(h+'-'+label+'.json')
            if args.author:
                command=[host,'-NoProfile','-NonInteractive','-File',str(BASE/label/'Scripts/tests/Test-Rotation1Closure.ps1'),'-Classes','D,C' if label=='original' else 'D,C,X']
            else:
                command=[host,'-NoProfile','-NonInteractive','-File',str(HERE/'Independent-Closure.ps1'),'-Snapshot',str(BASE/label),'-FixtureRoot',str(fixture),'-Results',str(output)]
            started=time.monotonic()
            p=subprocess.run(command,cwd=REPO,env=env,stdout=subprocess.PIPE,stderr=subprocess.STDOUT,timeout=240)
            log=run/(h+'-'+label+'.txt');log.write_bytes(p.stdout)
            row={'host':h,'checkpoint':label,'commit':COMMITS[label],'command':command,'exit':p.returncode,'elapsed_seconds':round(time.monotonic()-started,3),'log':str(log.relative_to(REPO)),'log_sha256':sha(p.stdout)}
            if output.exists():
                d=json.loads(output.read_text(encoding='utf-8-sig'));row['cases']=d['cases'];row['version']=d['host']
                expected=7 if label=='original' else 0
                row['expected_exit']=expected;row['matches_expected']=p.returncode==expected
            results.append(row)
            print(h,label,'exit',p.returncode,'log',log.relative_to(REPO),flush=True)
            print(p.stdout.decode('utf-8',errors='replace')[-1000:],flush=True)
    summary={'kind':'author-harness-reproduction' if args.author else 'independent-acceptance-reproduction','commits':COMMITS,'sources':{n:sha((HERE/n).read_bytes()) for n in ['Independent-Closure.ps1','run_review.py']},'runs':results}
    (run/'summary.json').write_text(json.dumps(summary,indent=2),encoding='utf-8')
    print('SUMMARY',run.relative_to(REPO)/'summary.json')

if __name__=='__main__':main()
