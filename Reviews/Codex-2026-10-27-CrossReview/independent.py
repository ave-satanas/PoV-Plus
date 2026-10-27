"""Independent extensions to, and falsification of, Claude's review (offline only)."""
import importlib.util
import json
from pathlib import Path
import subprocess

here=Path(__file__).resolve().parent
spec=importlib.util.spec_from_file_location('reproduction',here/'reproduce.py')
r=importlib.util.module_from_spec(spec);spec.loader.exec_module(r)
source=r.PROOF.read_text()
harness=here/'Independent.cs'
results=[]
cases=['subclass-entry','subclass-tracked','subclass-root','virtual-equals','dead-root-latch','distinct-ordered-stale','occupied-same-item-swap','same-count-foreign-change','reachable-add','reachable-clear']
for case in cases:
    row=r.build('independent-control-'+case,r.PROOF,harness,[case]);results.append(row)
    assert row['exit']==0,row
for mutant,case in [('read-entry-type','subclass-entry'),('read-ownitem-type','subclass-tracked'),('read-roottype','subclass-root'),('read-same-refeq','virtual-equals'),('borrow-dead','dead-root-latch')]:
    row=r.build('independent-'+mutant,r.STATE/'sweep'/(mutant+'.cs'),harness,[case]);results.append(row)
    assert row['exit']==1 and 'INDEPENDENT FAILURE:' in row['output'],row
for name,old,new,case in [
    ('reachable-add','int count = map.Count;','int count = map.Count; if (count == 47) map.Add("__crossreview", null);','reachable-add'),
    ('reachable-clear','int used = (int)field.GetValue(value);','int used = (int)field.GetValue(value); if (used == 47) Array.Clear((Array)(exactType.GetField("_entries", BindingFlags.Instance | BindingFlags.NonPublic) ?? exactType.GetField("entries", BindingFlags.Instance | BindingFlags.NonPublic)).GetValue(value), 0, 1);','reachable-clear')]:
    assert source.count(old)==1
    path=r.STATE/(name+'.cs');path.write_text(source.replace(old,new))
    row=r.build(name+'-original-tests',path,r.HARNESS);results.append(row);assert row['exit']==0,row
    row=r.build(name+'-independent-test',path,harness,[case]);results.append(row);assert row['exit']==1,row
    results.append(r.build('lib-'+name,path))
row=r.build('original-clear-trigger',r.STATE/'escape/escape-array-clear.cs',harness,['original-clear-trigger']);results.append(row);assert row['exit']==0,row
row=r.build('original-add-trigger',r.STATE/'escape/escape-idictionary-add.cs',harness,['original-add-trigger']);results.append(row);assert row['exit']==0,row
(r.STATE/'independent-results.json').write_text(json.dumps(results,indent=2))
print('PASS independent controls, five distinguished survivors, reachable mutating audit fixtures, and original-clear malformed trigger')
for row in results:
    if 'output' in row:print(row['name'],row['exit'],row['output'].strip())
