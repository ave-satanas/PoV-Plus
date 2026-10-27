"""Independent DEV-008 exported-IL assertions; no target assembly execution."""
import json
from pathlib import Path
import sys


def verify(path):
    images = json.loads(Path(path).read_text(encoding="utf-8-sig"))
    by_path = {x["path"].replace("\\", "/"): x for x in images}
    pins = {
        "valheim_Data/Managed/assembly_valheim.dll": "94998885B103D6E5F5480123EAEE0A093E82AF2DDEEEE37536C15BF555F24F3A",
        "valheim_Data/Managed/assembly_utils.dll": "771D59CBF3EF704007267E8EAF89AAC231C172B052BD93F9C273A18381BD6D59",
        "valheim_Data/Managed/Splatform.dll": "AB1753F05F5FAE8C8614B729DE852C3282C5F174EB6E672F76480579B1F41DCE",
        "valheim_Data/Managed/Splatform.Steam.dll": "1DF9DF6D3368D5413D0027916A186B39785FBF422AC249EC7CD9207DAB8FB5E6",
        "BepInEx/core/BepInEx.dll": "36BDB9AF2B8F2EDA4C911D2A49C2A05A4F643637AD724B876D292A9A5FCD8AE6",
    }
    for name, sha in pins.items():
        if by_path[name]["sha256"] != sha:
            raise AssertionError("changed primary image: " + name)

    def body(image, member, token=None):
        matches = [x for x in by_path[image]["methods"] + by_path[image]["io"] if member == x["method"]]
        if not matches or (token is not None and matches[0]["token"] != token):
            raise AssertionError("missing/changed method: " + member)
        return "\n".join(matches[0]["il"])

    game = "valheim_Data/Managed/assembly_valheim.dll"
    utils = "valheim_Data/Managed/assembly_utils.dll"
    steam = "valheim_Data/Managed/Splatform.Steam.dll"
    required = [
        (game, "System.Void FejdStartup::Awake()", 0x060015e1, ["IL_0007: call System.Void FejdStartup::ParseArguments()"]),
        (game, "System.Boolean FejdStartup::ParseServerArguments()", 0x060015f2, ['ldstr "-savedir"', "IL_010f: call System.Void Utils::SetSaveDataPath"]),
        (utils, "System.String Utils::GetSaveDataPath(FileHelpers/FileSource)", 0x0600030d, ["get_CloudStorageSupportedAndEnabled", "IsAutoOrCloud", 'IL_000f: ldstr ""', "IL_0015: ldsfld System.String Utils::m_saveDataOverride"]),
        (utils, "System.Void Utils::.cctor()", 0x06000386, ["get_persistentDataPath", "Utils::persistantDataPath"]),
        (game, "System.String SaveSystem::GetWorldsSaveRootPath(FileHelpers/FileSource)", 0x060018a4, ['"/worlds_local"']),
        (game, "System.String SaveSystem::GetCharacterFolderPath(FileHelpers/FileSource)", 0x060018a5, ['"/characters_local/"']),
        (game, "System.Collections.Generic.List`1<System.String> World::GetSavePaths()", 0x06001977, ["m_chunkedSave", "World::GetDBPath", "World::GetMetaPath"]),
        (game, "System.Void SteamManager::Awake()", 0x060014e9, ["RestartAppIfNecessary", "SteamAPI::Init"]),
        (steam, "System.Boolean Splatform.Steam.SteamCloud::get_IsEnabled()", 0x06000013, ["IsCloudEnabledForAccount", "IsCloudEnabledForApp"]),
        (steam, "System.Boolean Splatform.Steam.SteamCloud::WriteFile(System.String,System.Byte[],Splatform.CloudStorageFileGrouping,System.Boolean)", 0x06000020, ["FileWriteStreamOpen", "FileWriteStreamWriteChunk", "FileWriteStreamClose"]),
    ]
    for image, member, token, snippets in required:
        il = body(image, member, token)
        for snippet in snippets:
            if snippet not in il:
                raise AssertionError("missing IL: " + snippet)
    client = body(game, "System.Void FejdStartup::ParseArguments()", 0x060015ee)
    if "-savedir" in client or "ParseServerArguments" in client:
        raise AssertionError("client argument parser changed")
    calls = [x for image in images for x in image["callSites"] if "::ParseServerArguments(" in x["instruction"]]
    if calls:
        raise AssertionError("new static caller; review rather than assuming client support")
    for dll, member, literal in [
        ("treextr-PathOfValheiman/PathOfValheiman/PathOfValheiman.dll", "System.String PassiveTree.BuildLibrary::get_FilePath()", '"builds.json"'),
        ("RandyKnapp-EpicLoot/EpicLoot.dll", "System.String Common.ConfigUIFirstRun::get_RecordPath()", '"ModQuickConfig"'),
    ]:
        il = body("BepInEx/plugins/" + dll, member)
        if "get_persistentDataPath" not in il or literal not in il:
            raise AssertionError("mod persistence evidence changed")
    print(f"PASS: 5 image pins, 10 method contracts, client parser/caller boundaries, 2 independent mod-path contracts; {len(images)} images inspected")


if __name__ == "__main__":
    verify(sys.argv[1])
