"""Read only this worktree's immutable baseline scripts; no candidate/installation access."""
import hashlib
import json
from pathlib import Path
import re
import subprocess

REPO = Path(__file__).resolve().parents[2]
BASE = 'b06cdfcac75c48349cfc26a09f5c4f1671947e83'
names = subprocess.check_output(['git', 'ls-tree', '-r', '--name-only', BASE, '--', 'Scripts'],
                                cwd=REPO, text=True).splitlines()
rows = []
pattern = re.compile(r'workflow\.lock|FileShare|Start-Process|OutputPath|DiagnosticDll|Metadata=|EvidenceOutput|\$Output=|S:[/\\]Valheim-Modding[/\\]PoV-Plus|\.dev-state|manifest\.Repository')
for name in names:
    if Path(name).suffix not in {'.ps1', '.psm1', '.py'}:
        continue
    data = subprocess.check_output(['git', 'show', BASE + ':' + name], cwd=REPO)
    hits = [dict(line=i, text=line) for i, line in enumerate(data.decode('utf-8-sig').splitlines(), 1)
            if pattern.search(line)]
    if hits:
        rows.append(dict(path=name, sha256=hashlib.sha256(data).hexdigest(), hits=hits))
state = REPO / '.dev-state/rotation1'
state.mkdir(parents=True, exist_ok=True)
(state / 'baseline-inventory.json').write_text(json.dumps(dict(commit=BASE, files=rows), indent=2) + '\n')
print('PASS pinned baseline inventory; no candidate code or shared installation read; matched', len(rows), 'scripts')
