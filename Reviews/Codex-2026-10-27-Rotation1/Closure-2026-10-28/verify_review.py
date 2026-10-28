"""Validate retained review evidence and export portable results; no game access."""
from pathlib import Path
import argparse
import ast
import collections
import hashlib
import json
import re
import subprocess

HERE=Path(__file__).resolve().parent
REPO=HERE.parents[2]
BASE=REPO/'.dev-state/rotation1/closure-review-20261028'
REV='d1a1761016a69c84603cea0ada66eb441aa905c7'
CRITERIA='b301ceebf0a0592d8ea405e77d641f4f1b91124b'
def sha(data):return hashlib.sha256(data).hexdigest()
def blob(c,p):return subprocess.check_output(['git','show',c+':'+p],cwd=REPO)

def validation_view():
    dest=BASE/'knowledge-validation-view';dest.mkdir(exist_ok=True)
    snapshot=json.loads(blob(REV,'Knowledge/Dependencies/environment-snapshot.json'))
    expected={x['path'].replace('\\','/'):x['sha256'].lower() for x in snapshot['repository_sources']}
    rows=[]
    names=subprocess.check_output(['git','ls-tree','-r','--name-only',REV,'--','Scripts','Source','Knowledge','AGENTS.md'],cwd=REPO,text=True).splitlines()
    for name in names:
        data=blob(REV,name);original=sha(data);kind='git-blob'
        if name in expected and original!=expected[name]:
            data=data.replace(b'\r\n',b'\n').replace(b'\n',b'\r\n')
            assert sha(data)==expected[name],('unexpected source drift',name)
            kind='working-tree-CRLF'
        p=dest/name;p.parent.mkdir(parents=True,exist_ok=True)
        if p.exists():assert p.read_bytes()==data
        else:p.write_bytes(data)
        if name in expected:rows.append({'path':name,'git_blob_sha256':original,'materialized_sha256':sha(data),'encoding':kind})
    (dest/'materialization.json').write_text(json.dumps(rows,indent=2),encoding='utf-8')
    return subprocess.run(['python','-B',str(dest/'Scripts/knowledge.py'),'validate','--strict'],cwd=REPO).returncode

def main():
    ap=argparse.ArgumentParser();ap.add_argument('--validate-snapshot',action='store_true');a=ap.parse_args()
    if a.validate_snapshot:raise SystemExit(validation_view())
    criteria=blob(CRITERIA,'Reviews/Codex-2026-10-27-Rotation1/Rotation1-Closure-Acceptance-Criteria-v2.md')
    assert sha(criteria).upper()=='A2397865E2A025587ADA8BFCEC810687E55F5EE91F9AF7F1415031C5241DAE9A'
    tests=dict(re.findall(r'^\| (T-R\d-[a-z]) \| \*\*([DC])\*\*',criteria.decode(),re.M));assert len(tests)==15
    independent=json.loads((BASE/'run-42e0181751e14cbabbaa888b77861071/summary.json').read_text())
    for r in independent['runs']:
        assert r['exit']==(7 if r['checkpoint']=='original' else 0)
        assert len(r['cases'])==15
        for case in r['cases']:
            assert tests[case['test']]==case['class']
            expected='FAIL' if r['checkpoint']=='original' and case['class']=='D' else 'PASS'
            assert case['status']==expected,case
        assert independent['sources']['Independent-Closure.ps1']==sha((HERE/'Independent-Closure.ps1').read_bytes())
        assert sha((REPO/r['log']).read_bytes())==r['log_sha256']
    for label in ['original','revision']:
        p=BASE/label;manifest=json.loads((p/'blob-manifest.json').read_text())
        for f in manifest['files']:assert sha((p/f['path']).read_bytes())==f['sha256']
    def portable(value):
        text=json.dumps(value).replace(str(REPO).replace('\\','\\\\'),'${REPO}').replace(str(Path.home()).replace('\\','\\\\'),'${USERPROFILE}')
        text=text.replace(REPO.as_posix(),'${REPO}').replace(Path.home().as_posix(),'${USERPROFILE}')
        return json.loads(text)
    suite_runs=[]
    for dirname in ['run-5f1b38b856804f7b8d68ee01ba85d23b','run-266bfdb6d66c4cc78fbfa272c42b0ac5']:
        summary=json.loads((BASE/dirname/'summary.json').read_text())
        for r in summary['runs']:
            data=(REPO/r['log']).read_bytes();assert sha(data)==r['log_sha256']
            cases=re.findall(r'RESULT\|([^|]+)\|([DCX])\|([^|]+)\|(PASS|FAIL)\|([^\r\n]+)',data.decode('utf-8',errors='replace'))
            r['parsed_cases']=[dict(zip(['test','class','criteria','status','name'],v)) for v in cases]
            if dirname.endswith('42b0ac5'):
                assert r['exit']==(7 if r['checkpoint']=='original' else 0)
                assert len(cases)==(15 if r['checkpoint']=='original' else 28)
                assert len({v[0] for v in cases})==len(cases)
                for test,cls,_,status,_ in cases:
                    assert status==('FAIL' if r['checkpoint']=='original' and cls=='D' else 'PASS')
            suite_runs.append(r)
    checks=[]
    for dirname in ['checks-eab29810948a454bb340932699990961','checks-76e0262830414c3fb05b6ed2f4e6d05d']:
        for row in json.loads((BASE/dirname/'checks.json').read_text()):
            assert sha((REPO/row['log']).read_bytes())==row['sha256'];checks.append(row)
    for f in HERE.glob('*.py'):ast.parse(f.read_text(encoding='utf-8-sig'))
    result={'schema':1,'review':'Rotation 1 R1-R3 closure','recommendation':'ACCEPT WITH DOCUMENTED LIMITATIONS','criteria_commit':CRITERIA,'criteria_sha256':sha(criteria),'implementation_commit':REV,'frozen_test_classes':dict(collections.Counter(tests.values())),'independent':portable(independent),'author_harness_reproduction':portable(suite_runs),'regression_checks':portable(checks),'current_review_sources':{p.name:sha(p.read_bytes()) for p in HERE.iterdir() if p.suffix in ['.py','.ps1']},'limits':['No runtime isolation or Tome lifecycle certification','Hardlink checks remain check-then-use','Directory pin ends at lease release; child files are not pinned','Six fixed-path Tome build harnesses remain outside scoped writer coverage']}
    (HERE/'EVIDENCE.json').write_bytes((json.dumps(result,indent=2)+'\n').encode('utf-8'))
    print('PASS 60 independent matrix observations, immutable blobs, retained log hashes and source syntax')

if __name__=='__main__':main()
