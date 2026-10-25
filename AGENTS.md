
# PoVPlus Modding Workspace

## Project
This repository supports Valheim mod development
using BepInEx, Harmony, and C#.

## Primary goals
- Analyze existing mod configurations.
- Identify genuine compatibility problems.
- Develop custom BepInEx plugins.
- Preserve Path of Valheim progression and balance.
- Maintain compatibility with installed mods.

## Rules
1. Do not assume overlapping configuration settings
   represent conflicts.
2. Treat mod-author compatibility patches as intentional
   unless evidence demonstrates otherwise.
3. Never overwrite existing configurations without
   explaining the changes.
4. Prefer standalone compatibility patches over
   modifying third-party assemblies.
5. Preserve original files and document modifications.
6. Do not invent configuration keys or plugin APIs.
7. Verify assembly references and API signatures
   before writing compatibility patches.
8. Do not redistribute third-party DLLs without
   checking their licenses.

## Development
- Language: C#
- Framework: BepInEx and Harmony
- Target: Valheim
- Platform: Windows
- Verify the installed BepInEx version and target
  framework before building.

## Persistent engineering knowledge
- Before substantial development or investigation, read Knowledge/INDEX.md and relevant linked records.
- Check prior investigations, decisions, evidence, and applicability to current assembly/configuration versions before repeating research.
- During meaningful work, distinguish verified facts, hypotheses, decisions, and unresolved questions; cite precise evidence.
- After meaningful work, update canonical records and related links, record tests/outcomes and remaining questions, and add a concise session entry when useful.
- Run python Scripts/knowledge.py validate; review drift and revalidation notices. Do not change accepted snapshots merely to silence warnings.
- Treat the development installation as read-only unless explicitly authorized; knowledge maintenance does not authorize deployment, configuration changes, or game launch.
