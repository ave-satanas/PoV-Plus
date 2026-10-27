"""Independent edit verification and saved-output comparison, using immutable blobs."""
import ast
import hashlib
import json
from pathlib import Path
import subprocess
import sys

repo=Path(__file__).resolve().parents[2]
state=repo/'.dev-state/crossreview-a'
package=state/'immutable/Reviews/Claude-2026-10-27'
proof=state/'immutable/Source/PoVPlus/Diagnostics/PassiveOwnershipProof.cs'
text=proof.read_text()
tree=ast.parse((package/'tests/sweep_mutants.py').read_text())
assignment=next(x for x in tree.body if isinstance(x,ast.Assign) and any(isinstance(t,ast.Name) and t.id=='M' for t in x.targets))
assert not any(isinstance(x,ast.Call) for x in ast.walk(assignment.value))
edits=eval(compile(ast.Expression(assignment.value),'<immutable mutation table>','eval'),{'NL':'\n','__builtins__':{}})
assert len(edits)==77
manifest=[]
for name,before,after in edits:
    assert text.count(before)==1,(name,text.count(before))
    actual=(state/'sweep'/(name+'.cs')).read_text()
    assert actual==text.replace(before,after) and actual!=text,name
    manifest.append(dict(name=name,before=before,after=after,sha256=hashlib.sha256(actual.encode()).hexdigest()))
(state/'mutation-edit-manifest.json').write_text(json.dumps(manifest,indent=2))
for newline,label in [('\n','lf'),('\r\n','crlf')]:
    source=state/('generator-'+label+'.cs');source.write_bytes(text.replace('\n',newline).encode())
    output=state/('generator-'+label);output.mkdir(exist_ok=True)
    p=subprocess.run([sys.executable,'-I',str(package/'tests/sweep_mutants.py'),str(source),str(output)],capture_output=True,text=True)
    assert p.returncode==0 and len((output/'names.txt').read_text().splitlines())==77
results=json.loads((state/'results.json').read_text())
for name,file in [('adversarial','adversarial.txt'),('discriminating','discriminating-corrected.txt'),('falsify','original-falsify.txt')]:
    actual=next(x['output'] for x in results if x['name']==name)
    saved=(package/'results'/file).read_text(encoding='utf-8-sig')
    assert actual.strip()==saved.strip(),(name,'saved output differs')
for prefix,saved_file in [('s-','sweep-vs-53-fixtures.txt'),('d-','survivors-vs-discriminating.txt')]:
    saved=(package/'results'/saved_file).read_text().splitlines()
    actual=['['+x['name']+'] exit='+str(x['exit'])+' :: '+x['output'].splitlines()[-1][:200] for x in results if x['name'].startswith(prefix)]
    assert sorted(saved)==sorted(actual),saved_file
print('PASS all77 exact unique declared source edits; LF/CRLF generation77; five saved result sets match independent runs')
