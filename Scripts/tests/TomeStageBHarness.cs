using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using PoVPlus.Compatibility;
using PoVPlus.Diagnostics;

internal static class TomeStageBHarness
{
    private const BindingFlags All = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
    private static int _passed;
    private static bool? _setting;
    private static readonly List<string> Rows = new List<string>();
    public sealed class Entry { public bool Value { get; set; } }
    public sealed class Settings { public Entry Entry = new Entry { Value = true }; public Entry this[string section, string key] { get { return Entry; } } }
    public sealed class OwnPlugin : UnityEngine.Object { public Settings Config { get; set; } }
    public sealed class NativeToggle { public int Value { get; set; } }
    private static MethodInfo Gate(int gate) { return gate == 0 ? typeof(Magic_Supremacy.Magic_SupremacyPlugin).GetMethod("IsTomeItem", All) : typeof(Magic_Supremacy.CustomSlotSystem).GetMethod("IsItemValidForSlot", All); }
    private static void Check(bool value, string reason) { if (!value) throw new Exception(reason); }
    private static void Reject(Action action)
    {
        bool rejected = false; try { action(); } catch (InvalidOperationException) { rejected = true; }
        Check(rejected, "Expected safe rejection");
    }
    private static void Prepare()
    {
        _setting = true; Rows.Clear(); Harmony.TestPatches.Clear(); ObjectDB.instance = new ObjectDB();
        Magic_Supremacy.CustomSlotSystem.Initializations = 0;
        AppDomain.CurrentDomain.SetData("DO_CustomSlotSystem_SlotPrefabs", new Dictionary<string, HashSet<string>> {
            { "tome", new HashSet<string> { "ArmorMushroomcallerTome_DO" } } });
        BepInEx.Bootstrap.Chainloader.PluginInfos.Clear();
        BepInEx.Bootstrap.Chainloader.PluginInfos.Add("povplus.core", new BepInEx.Bootstrap.PluginInfo { Instance = new OwnPlugin { Config = new Settings() } });
        var type = typeof(MagicPluginTomeBridge); var bridge = new MagicPluginTomeBridge(new ManualLogSource());
        type.GetField("_current", All).SetValue(null, bridge);
        var activation = (BridgeActivation)type.GetField("_activation", All).GetValue(bridge);
        activation.Start(true, () => { }, _ => { }, () => { }, () => { });
        var toggle = new NativeToggle(); type.GetField("_nativeToggle", All).SetValue(bridge, toggle);
        type.GetField("_toggleValue", All).SetValue(bridge, typeof(NativeToggle).GetProperty("Value"));
        for (int gate = 0; gate < 2; gate++)
        {
            var info = new Patches(); info.Owners.Add("povplus.core.magicplugin-tomes");
            info.Postfixes.Add(new Patch { owner = "povplus.core.magicplugin-tomes", PatchMethod = type.GetMethod(gate == 0 ? "DescriptorPostfix" : "EquipmentPostfix", All) });
            Harmony.TestPatches[Gate(gate)] = info;
        }
    }
    private static void Start() { TomeStageBProbe.CheckStartup(typeof(Magic_Supremacy.Magic_SupremacyPlugin).Assembly, Rows.Add, _setting); }
    private static void Test(string label, Action test) { Prepare(); test(); _passed++; Console.WriteLine("PASS " + label); }
    private static ItemDrop.ItemData Prefab(string id)
    {
        var item = new ItemDrop.ItemData { m_dropPrefab = new UnityEngine.GameObject { name = id }, m_shared = new ItemDrop.SharedData() };
        string effect = TomeBridgePolicy.EffectFor(id);
        if (effect != null) item.m_shared.m_equipStatusEffect = new StatusEffect { name = effect };
        item.m_dropPrefab.Component = new ItemDrop { m_itemData = item }; ObjectDB.instance.Items[id] = item.m_dropPrefab; return item;
    }
    private static int Main()
    {
        try
        {
            Test("enabled setting and active pair with exact owners/methods accepted", () => { Start(); Check(Rows.Count == 3 && Rows[0].Contains("priority=400"), "Metadata evidence"); });
            Test("false own configuration refuses stage B", () => { _setting = false; Reject(Start); });
            Test("own Chainloader entry absent during Awake is safe with explicit setting", () => {
                BepInEx.Bootstrap.Chainloader.PluginInfos.Remove("povplus.core"); Start();
                Check(Rows.Exists(x => x.Contains("bothPostfixesVerified=True")), "Explicit setting reached probe");
            });
            Test("own Instance unassigned during Awake is safe", () => {
                BepInEx.Bootstrap.Chainloader.PluginInfos["povplus.core"].Instance = null; Start();
            });
            Test("missing configuration snapshot fails closed with useful error", () => {
                _setting = null;
                try { Start(); throw new Exception("Missing snapshot accepted"); }
                catch (InvalidOperationException error) { Check(error.Message.Contains("configuration snapshot"), "Useful failure"); }
                Check(Rows.Count == 0, "No successful startup evidence on failure");
            });
            Test("inactive bridge refuses stage B", () => { typeof(MagicPluginTomeBridge).GetField("_current", All).SetValue(null, null); Reject(Start); });
            Test("missing target patch refuses stage B", () => { Harmony.TestPatches.Remove(Gate(1)); Reject(Start); });
            Test("foreign postfix owner rejected", () => { Harmony.TestPatches[Gate(0)].Postfixes[0].owner = "other"; Reject(Start); });
            Test("same owner with wrong method rejected", () => { Harmony.TestPatches[Gate(0)].Postfixes[0].PatchMethod = typeof(TomeStageBHarness).GetMethod("Start", All); Reject(Start); });
            foreach (string kind in new[] { "Prefixes", "Transpilers", "Finalizers", "ILManipulators" })
            {
                string collection = kind;
                Test("unexpected " + kind + " rejected", () => {
                    ((List<Patch>)typeof(Patches).GetField(collection).GetValue(Harmony.TestPatches[Gate(0)])).Add(new Patch()); Reject(Start);
                });
            }
            Test("patched registry getter rejected", () => { var info = new Patches(); info.Owners.Add("other"); Harmony.TestPatches[typeof(Magic_Supremacy.CustomSlotSystem).GetMethod("GetSharedSlotRegistry", All)] = info; Reject(Start); });
            Test("patched supporting prefab lookup rejected", () => { var info = new Patches(); info.Owners.Add("other"); Harmony.TestPatches[typeof(ObjectDB).GetMethod("GetItemPrefab", new[] { typeof(string) })] = info; Reject(Start); });
            Test("absent registry refused without initializing helper", () => { Start(); AppDomain.CurrentDomain.SetData("DO_CustomSlotSystem_SlotPrefabs", null); Reject(() => TomeStageBProbe.CaptureInternal(Prefab("BMP_DruidsTome"), "BMP_DruidsTome", Rows.Add)); Check(Magic_Supremacy.CustomSlotSystem.Initializations == 0 && AppDomain.CurrentDomain.GetData("DO_CustomSlotSystem_SlotPrefabs") == null, "No initialization"); });
            Test("missing native control refused", () => { Start(); ((Dictionary<string, HashSet<string>>)AppDomain.CurrentDomain.GetData("DO_CustomSlotSystem_SlotPrefabs"))["tome"].Clear(); Reject(() => TomeStageBProbe.RequireInitializedRegistry()); });
            Test("three books control and unrelated material exercise actual diagnostic and adapter with doubles", () => {
                Start(); foreach (string id in new[] { "BMP_BeginnersMagicBook", "BMP_AdvanceMagicBook", "BMP_DruidsTome", "ArmorMushroomcallerTome_DO", "Wood" }) {
                    var item = Prefab(id); var shared = item.m_shared;
                    TomeStageBProbe.CaptureInternal(item, id, Rows.Add);
                    Check(ReferenceEquals(shared, item.m_shared), "No shared-data mutation");
                }
                Check(Rows.Exists(x => x.Contains("prefab=Wood") && x.Contains("result=False")), "Negative item");
                Check(Rows.Exists(x => x.Contains("prefab=BMP_DruidsTome") && x.Contains("result=True") && x.Contains("unrelatedResult=False")), "Both scope results");
                Check(Magic_Supremacy.CustomSlotSystem.Initializations == 0, "Existing registry path only");
            });
            Test("visible result contradiction stops instead of repair", () => { Start(); Reject(() => TomeStageBProbe.CheckVisible("Wood", "True")); Reject(() => TomeStageBProbe.CheckVisible("BMP_DruidsTome", "False")); TomeStageBProbe.CheckVisible("Wood", "False"); });
            Test("other slot preserves original positive overlap rather than calling it conflict", () => {
                Start(); ((Dictionary<string, HashSet<string>>)AppDomain.CurrentDomain.GetData("DO_CustomSlotSystem_SlotPrefabs"))["belt"] = new HashSet<string> { "ArmorMushroomcallerTome_DO" };
                TomeStageBProbe.CaptureInternal(Prefab("ArmorMushroomcallerTome_DO"), "ArmorMushroomcallerTome_DO", Rows.Add);
                Check(Rows.Exists(x => x.Contains("unrelatedResult=True") && x.Contains("unrelatedExpected=True")), "Native overlap preserved");
            });
            Console.WriteLine("All " + _passed + " Stage B diagnostic fixtures passed. Managed doubles only; no real Harmony detours/game assemblies."); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
