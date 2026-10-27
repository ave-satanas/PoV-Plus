"""Create compact review evidence; generated code/binaries remain ignored."""
import hashlib,json,subprocess
from pathlib import Path
repo=Path(__file__).resolve().parents[2]
state=repo/'.dev-state/crossreview-a'
review=Path(__file__).resolve().parent
get=lambda p:json.loads((state/p).read_text(encoding='utf-8-sig'))
main=get('results.json'); independent=get('independent-results.json');audits=get('audit-results.json')
assert len(main)==156 and len(independent)==23 and len(audits)==12
summary=dict(
 base='0c1f3d1a909e0c40a7bcf36425f8efc6a1c09722',
 reviewed='d84903d8f383d8dac24c635fc9fa675b5e87264b',
 head='27ba868f7bcc82d6f5bbef0c6b6acf23a6deac6f',
 main_builds=156,independent_builds=23,
 sweep=dict(edits=77,killed53=22,survived53=55,killed36=36,remaining19=19,new_distinctions=5),
 audits=audits,identity=get('identity-audit.json'),
 independent_cases=[{k:v for k,v in x.items() if k in ['name','exit','source_sha256','harness_sha256','artifact_sha256']} for x in independent],
 installation=get('installation.json'),
 evidence_files={name:hashlib.sha256((state/name).read_bytes()).hexdigest() for name in [
    'inputs.json','mutation-edit-manifest.json','results.json','independent-results.json',
    'installed.json','audit-results.json','identity-audit.json','python-tests.txt','powershell51-exact.txt','locks.txt']},
 limits='Offline synthetic/metadata evidence only; no current-source mutation, runtime isolation or ownership certification established')
assert subprocess.check_output(['git','rev-parse','HEAD'],cwd=repo,text=True).strip()==summary['head']
assert not subprocess.check_output(['git','diff','--name-only'],cwd=repo).strip()
assert not subprocess.check_output(['git','diff','--cached','--name-only'],cwd=repo).strip()
for path in review.rglob('*'):
    if path.is_file() and path.suffix in ['.py','.ps1','.cs','.md']:
        for number,line in enumerate(path.read_text().splitlines(),1):
            assert line.rstrip()==line,(path,number)
out=review/'results/summary.json';out.parent.mkdir(exist_ok=True)
out.write_text(json.dumps(summary,indent=2)+'\n')
print('PASS compact summary written; existing tracked/index content unchanged; review text whitespace clean')
