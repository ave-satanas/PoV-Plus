"""Render reviewable mod inventory blocks from the accepted snapshot; preserve prose."""
import datetime as dt
import json
from pathlib import Path
import re
import sys

sys.path.insert(0, str(Path(__file__).resolve().parent))
from knowledge import KNOWLEDGE, read_record, write_record

START = "<!-- inventory:start -->"
END = "<!-- inventory:end -->"


def record_name(folder):
    slug = re.sub(r"[^a-z0-9-]+", "-", folder.lower()).strip("-")
    return "MOD-" + slug


def update_block(path, meta, block):
    generated = START + "\n" + block.rstrip() + "\n" + END
    if path.exists():
        old_meta, body, _ = read_record(path)
        if START not in body or END not in body:
            raise ValueError(f"refusing to overwrite record without inventory markers: {path}")
        before, rest = body.split(START, 1)
        _, after = rest.split(END, 1)
        # Metadata verification stays with the reviewer; rendering is not verification.
        write_record(path, old_meta, before + generated + after)
    else:
        write_record(path, meta, "# " + meta["title"] + "\n\n" + generated +
                     "\n\n## Engineering notes\n\nIntegration behavior and authorship beyond metadata remain unreviewed. "
                     "Add verified evidence here; this section survives inventory rendering.\n")


def render():
    snapshot = json.loads((KNOWLEDGE / "Dependencies/environment-snapshot.json").read_text(encoding="utf-8-sig"))
    date = dt.datetime.fromisoformat(snapshot["captured_utc"].replace("Z", "+00:00")).astimezone().date().isoformat()
    packages = {p["folder"]: p for p in snapshot["packages"]}
    groups = {}
    for assembly in snapshot["assemblies"]:
        parts = assembly["path"].split("/")
        if parts[:2] in (["BepInEx", "plugins"], ["BepInEx", "patchers"]):
            groups.setdefault(parts[2], []).append(assembly)
    for folder in packages:
        groups.setdefault(folder, [])
    guid_map = {p["guid"]: record_name(folder) for folder, assemblies in groups.items()
                for a in assemblies for p in a["plugins"]}
    assembly_map = {}
    for folder, assemblies in groups.items():
        for a in assemblies:
            assembly_map.setdefault(a["name"], set()).add(record_name(folder))
    index_rows = []
    for folder, assemblies in sorted(groups.items()):
        name = record_name(folder)
        package = packages.get(folder)
        plugins = [p for a in assemblies for p in a["plugins"]]
        versions = {p["guid"]: p["version"] for p in plugins}
        if package:
            versions["package"] = package["version"]
        configs = [c for c in snapshot["configurations"] if any(
            Path(c["path"]).stem.casefold() == p["guid"].casefold() for p in plugins)]
        related = {"MOD-000-Inventory", "DEP-001-Environment", "CFG-001-Configuration-map", "COMP-001-Warnings"}
        lines = ["Metadata verified from the accepted snapshot; behavior coverage is **partial**.", "",
                 "Source: `Dependencies/environment-snapshot.json`, select entries by `path`.", ""]
        if package:
            lines += [f"- Manifest: `{package['path']}`; package version **{package['version']}**.",
                      f"- Package publisher inferred from folder: **{package['publisher_from_folder']}**. This is not verified personal authorship."]
            if package["website"]:
                lines += [f"- Manifest-declared website (not independently endorsed/verified): {package['website']}"]
            lines += ["- Manifest dependency requirements: " + (", ".join(f"`{d}`" for d in package["dependencies"]) or "none declared") + "."]
        else:
            lines += ["- No package manifest found in this group; do not infer package version or author."]
        lines += ["", "## Plugin identities", ""]
        if not plugins:
            lines += ["No BepInPlugin attributes found. This may be a preloader patcher, library, or content package; do not infer it is an ordinary plugin."]
        for plugin in plugins:
            lines += [f"- **{plugin['name']} {plugin['version']}** — GUID `{plugin['guid']}`; type `{plugin['type']}`; base `{plugin['base_type']}`."]
            for dep in plugin["dependencies"]:
                guid = dep[0] if dep else "unresolved"
                value = dep[1] if len(dep) > 1 else "unspecified"
                target = guid_map.get(guid)
                if target:
                    related.add(target)
                lines += [f"  - BepInDependency `{guid}`; second constructor argument `{value}`"
                          + (f"; [[{target}]]." if target else "; target not mapped in this snapshot.")]
        lines += ["", "Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. "
                  "Manifest requirements and assembly references are separate dependency layers.", "", "## Assemblies and patch declarations", ""]
        for assembly in assemblies:
            lines += [f"- `{assembly['path']}` — assembly `{assembly['name']} {assembly['version']}`; "
                      f"target `{assembly['target_framework'] or 'not declared'}`; "
                      f"{len(assembly['declared_patch_attributes'])} HarmonyPatch attributes.",
                      f"  - AssemblyCompany metadata: `{assembly['company_metadata'] or 'absent'}` (not proof of authorship)."]
            targets = set()
            for ref in assembly["references"]:
                targets.update(assembly_map.get(ref["name"], set()))
            targets.discard(name)
            related.update(targets)
            if targets:
                lines += ["  - Local mod assembly reference matches: " + ", ".join(f"[[{t}]]" for t in sorted(targets)) + "."]
        lines += ["", "The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.",
                  "", "## Configuration mapping", ""]
        if configs:
            lines += [f"- `{c['path']}` (filename matches plugin GUID; association heuristic, not proof of ownership)." for c in configs]
        else:
            lines += ["No GUID-named cfg matched. Embedded/custom/YAML configuration remains possible."]
        lines += ["", "Related: " + ", ".join(f"[[{r}]]" for r in sorted(related)) + "."]
        meta = dict(id=name, title=folder, type="mod", status="partial", created=date, last_verified=date,
                    versions=versions, evidence=["Dependencies/environment-snapshot.json: assemblies/packages selected by path"],
                    related=[f"[[{r}]]" for r in sorted(related)], confidence="mixed",
                    watch=["dll:" + a["path"] for a in assemblies] + ["cfg:" + c["path"] for c in configs] + (["pkg:" + package["path"]] if package else []),
                    claims={}, contradicts=[], supersedes=[])
        update_block(KNOWLEDGE / "Mods" / (name + ".md"), meta, "\n".join(lines))
        identity = "; ".join(f"`{p['guid']}` {p['version']}" for p in plugins) or "content/library; no plugin attribute"
        index_rows.append(f"| [[{name}|{folder}]] | {package['version'] if package else 'no manifest'} | {identity} |")
    overview = KNOWLEDGE / "Mods/MOD-000-Inventory.md"
    meta, body, _ = read_record(overview)
    block = (f"{len(groups)} plugin/patcher-directory groups; {len(packages)} plugin manifests; "
             f"{sum(len(a['plugins']) for a in snapshot['assemblies'])} BepInPlugin identities. "
             "Counts differ because libraries/content packages and multi-plugin DLLs exist.\n\n"
             "| Package / group | Manifest version | Plugin GUID and version |\n|---|---|---|\n" + "\n".join(index_rows))
    update_block(overview, meta, block)
    print(f"Rendered {len(groups)} mod records and inventory. Existing review metadata and manual sections preserved.")


if __name__ == "__main__":
    render()
