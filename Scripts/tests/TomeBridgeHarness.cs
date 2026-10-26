using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using PoVPlus.Compatibility;

internal static class TomeBridgeHarness
{
    private const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    private static int _passed;
    private sealed class Toggle { public int Value { get; set; } }
    private static bool Valid(ItemDrop.ItemData item) { return false; }
    private static bool ValidSlot(ItemDrop.ItemData item, string slot) { return false; }
    private static string WrongReturn(ItemDrop.ItemData item) { return "no"; }
    public static bool WrongAccess(ItemDrop.ItemData item) { return false; }
    private static void Check(bool value, string label) { if (!value) throw new Exception(label); }
    private static void Test(string label, Action run) { Reset(); run(); _passed++; Console.WriteLine("PASS " + label); }
    private static void Reset()
    {
        Field("_current").SetValue(null, null);
        ObjectDB.instance = new ObjectDB();
        BepInEx.Bootstrap.Chainloader.PluginInfos.Clear();
        HarmonyLib.Harmony.Constructed = 0;
        AppDomain.CurrentDomain.SetData("DO_CustomSlotSystem_SlotPrefabs",
            new Dictionary<string, HashSet<string>> { { "tome", new HashSet<string> { "ArmorMushroomcallerTome_DO" } } });
    }
    private static FieldInfo Field(string name) { return typeof(MagicPluginTomeBridge).GetField(name, All); }
    private static BridgeActivation Activate(MagicPluginTomeBridge bridge)
    {
        var activation = (BridgeActivation)Field("_activation").GetValue(bridge);
        Check(activation.Start(true, () => { }, _ => { }, () => { }, () => { }), "mock activation");
        Field("_current").SetValue(null, bridge);
        var toggle = new Toggle();
        Field("_nativeToggle").SetValue(bridge, toggle);
        Field("_toggleValue").SetValue(bridge, typeof(Toggle).GetProperty("Value"));
        return activation;
    }
    private static ItemDrop.ItemData Book(string id)
    {
        var item = new ItemDrop.ItemData { m_dropPrefab = new UnityEngine.GameObject { name = id },
            m_shared = new ItemDrop.SharedData { m_equipStatusEffect = new StatusEffect { name = TomeBridgePolicy.EffectFor(id) } } };
        item.m_dropPrefab.Component = new ItemDrop { m_itemData = item };
        ObjectDB.instance.Items[id] = item.m_dropPrefab;
        return item;
    }
    private static bool Gate(int gate, ItemDrop.ItemData item, bool original = false, string slot = "tome")
    {
        var method = typeof(MagicPluginTomeBridge).GetMethod(gate == 0 ? "DescriptorPostfix" : "EquipmentPostfix", All);
        var args = gate == 0 ? new object[] { item, original } : new object[] { item, slot, original };
        method.Invoke(null, args);
        return (bool)args[args.Length - 1];
    }
    private static int Main()
    {
        try
        {
            Test("default off and actual disabled startup does not create Harmony or inspect missing deps", () => {
                Check(!TomeBridgePolicy.DefaultEnabled, "default"); var log = new ManualLogSource();
                new MagicPluginTomeBridge(log).Start(false);
                Check(HarmonyLib.Harmony.Constructed == 0 && log.Warnings.Count == 0 && Field("_current").GetValue(null) == null, "disabled startup");
                Check(!Gate(0, Book("BMP_BeginnersMagicBook")) && Gate(1, null, true), "disabled results"); });
            Test("three exact prefabs pass both production postfixes with canonical data", () => {
                Activate(new MagicPluginTomeBridge(new ManualLogSource()));
                foreach (var id in new[] { "BMP_BeginnersMagicBook", "BMP_AdvanceMagicBook", "BMP_DruidsTome" })
                    for (int gate = 0; gate < 2; gate++) Check(Gate(gate, Book(id)), id); });
            Test("reject unrelated names, case changes, clones and whitespace", () => {
                Activate(new MagicPluginTomeBridge(new ManualLogSource()));
                foreach (var id in new[] { "BMP_OtherTome", "bmp_BeginnersMagicBook", "BMP_DruidsTome(Clone)", "BMP_DruidsTome ", "ArmorMushroomcallerTome_DO", "" })
                    for (int gate = 0; gate < 2; gate++) Check(!Gate(gate, Book(id)), id); });
            Test("preserve every original native or other acceptance result", () => {
                Activate(new MagicPluginTomeBridge(new ManualLogSource()));
                Check(Gate(0, Book("ArmorMushroomcallerTome_DO"), true), "native");
                Check(Gate(1, null, true, "belt"), "foreign native true"); });
            Test("internal gate strictly scoped to tome", () => {
                Activate(new MagicPluginTomeBridge(new ManualLogSource())); var item = Book("BMP_DruidsTome");
                foreach (var slot in new[] { null, "", "Tome", "$tome_DO", "belt", "tome " }) Check(!Gate(1, item, false, slot), "slot"); });
            Test("null and incomplete items remain rejected", () => {
                Activate(new MagicPluginTomeBridge(new ManualLogSource()));
                Check(!Gate(0, null) && !Gate(1, new ItemDrop.ItemData()), "nulls");
                var item = Book("BMP_DruidsTome"); item.m_shared.m_equipStatusEffect = null; Check(!Gate(0, item), "missing SE"); });
            Test("foreign shared metadata, fake prefab and wrong SE fail closed", () => {
                Activate(new MagicPluginTomeBridge(new ManualLogSource())); var item = Book("BMP_DruidsTome");
                var copy = new ItemDrop.ItemData { m_dropPrefab = item.m_dropPrefab, m_shared = new ItemDrop.SharedData { m_equipStatusEffect = item.m_shared.m_equipStatusEffect } };
                Check(!Gate(0, copy), "foreign shared data");
                copy.m_shared = item.m_shared; copy.m_dropPrefab = new UnityEngine.GameObject { name = "BMP_DruidsTome" }; Check(!Gate(1, copy), "spoof");
                item.m_shared.m_equipStatusEffect.name = "other"; Check(!Gate(0, item), "wrong effect"); });
            Test("absent upstream slot registry or native control prevents either extension", () => {
                Activate(new MagicPluginTomeBridge(new ManualLogSource())); var item = Book("BMP_DruidsTome");
                foreach (object registry in new object[] { null, new Dictionary<string, HashSet<string>>(),
                    new Dictionary<string, HashSet<string>> { { "tome", new HashSet<string>() } } }) {
                    AppDomain.CurrentDomain.SetData("DO_CustomSlotSystem_SlotPrefabs", registry);
                    Check(!Gate(0, item) && !Gate(1, item), "missing upstream authority");
                } });
            Test("missing dependency disables before either patch", () => {
                var log = new ManualLogSource(); new MagicPluginTomeBridge(log).Start(true);
                Check(HarmonyLib.Harmony.Constructed == 0 && log.Warnings.Count == 1 && log.Warnings[0].Contains("missing"), "missing dependency");
                Check(!Gate(0, Book("BMP_DruidsTome")) && !Gate(1, Book("BMP_DruidsTome")), "no partial acceptance"); });
            Test("exact private static bool signatures required", () => {
                MagicPluginTomeBridge.ResolveTarget(typeof(TomeBridgeHarness), "Valid", typeof(ItemDrop.ItemData));
                MagicPluginTomeBridge.ResolveTarget(typeof(TomeBridgeHarness), "ValidSlot", typeof(ItemDrop.ItemData), typeof(string));
                foreach (string name in new[] { "WrongReturn", "WrongAccess", "Missing" }) {
                    bool rejected = false;
                    try { MagicPluginTomeBridge.ResolveTarget(typeof(TomeBridgeHarness), name, typeof(ItemDrop.ItemData)); }
                    catch (InvalidOperationException) { rejected = true; }
                    Check(rejected, "signature " + name);
                } });
            Test("unsupported assembly fingerprint is rejected without loading plugins", () => {
                bool rejected = false;
                try { typeof(MagicPluginTomeBridge).GetMethod("VerifyHash", All).Invoke(null,
                    new object[] { typeof(TomeBridgeHarness).Assembly, "unsupported" }); }
                catch (TargetInvocationException error) { rejected = error.InnerException is InvalidOperationException; }
                Check(rejected, "fingerprint drift"); });
            Test("transaction stays inactive until both gates verified", () => {
                var state = new BridgeActivation(_ => { }); int count = 0;
                Check(state.Start(true, () => Check(!state.Active, "preflight"), _ => { Check(!state.Active, "install"); count++; },
                    () => Check(!state.Active && count == 2, "verify"), () => { throw new Exception("unexpected cleanup"); }), "success");
                Check(state.Active, "active"); });
            foreach (int phase in new[] { 0, 1, 2, 3 }) {
                int failure = phase;
                Test("failure-safe transaction phase " + phase, () => {
                    var log = new List<string>(); var state = new BridgeActivation(log.Add); bool cleaned = false;
                    state.Start(true, () => { if (failure == 0) throw new Exception("dependency/signature failure"); },
                        gate => { if (failure == gate + 1) throw new Exception("patch failure"); },
                        () => { if (failure == 3) throw new Exception("verification failure"); }, () => cleaned = true);
                    Check(!state.Active && cleaned && log.Count == 1, "failed closed"); });
            }
            Test("cleanup failure is explicit and never activates residual patches", () => {
                var rows = new List<string>(); var state = new BridgeActivation(rows.Add);
                state.Start(true, () => { }, _ => { throw new Exception("partial patch"); }, () => { }, () => { throw new Exception("unpatch failure"); });
                Check(!state.Active && rows.Count == 2, "inert cleanup failure"); });
            Test("disabled transaction never calls any install or validation delegate", () => {
                var state = new BridgeActivation(_ => { throw new Exception("unexpected log"); });
                Action forbidden = () => { throw new Exception("unexpected action"); };
                Check(!state.Start(false, forbidden, _ => forbidden(), forbidden, forbidden), "disabled"); });
            Test("runtime API read failure disables both postfixes with one warning", () => {
                var log = new ManualLogSource(); var bridge = new MagicPluginTomeBridge(log); var state = Activate(bridge);
                var item = Book("BMP_DruidsTome"); ObjectDB.instance.FailLookup = true;
                Check(!Gate(0, item) && !state.Active && !Gate(1, item) && log.Warnings.Count == 1, "runtime failure"); });
            Test("native slot activation disables both extensions without editing toggle", () => {
                var log = new ManualLogSource(); var bridge = new MagicPluginTomeBridge(log); var state = Activate(bridge);
                ((Toggle)Field("_nativeToggle").GetValue(bridge)).Value = 1;
                var item = Book("BMP_DruidsTome"); Check(!Gate(0, item) && !state.Active && !Gate(1, item), "competing owner");
                Check(((Toggle)Field("_nativeToggle").GetValue(bridge)).Value == 1, "no config write"); });
            Test("acceptance leaves item identity metadata and effect references unchanged", () => {
                Activate(new MagicPluginTomeBridge(new ManualLogSource())); var item = Book("BMP_DruidsTome");
                var prefab = item.m_dropPrefab; var shared = item.m_shared; var effect = shared.m_equipStatusEffect;
                Check(Gate(0, item) && Gate(1, item), "accepted");
                Check(ReferenceEquals(prefab, item.m_dropPrefab) && ReferenceEquals(shared, item.m_shared) && ReferenceEquals(effect, item.m_shared.m_equipStatusEffect), "read-only"); });
            Test("extension evidence is logged once per prefab and gate", () => {
                var log = new ManualLogSource(); Activate(new MagicPluginTomeBridge(log)); var item = Book("BMP_DruidsTome");
                for (int i = 0; i < 50; i++) { Gate(0, item); Gate(1, item); }
                Check(log.Infos.Count == 2 && log.Infos[0].Contains("gate=descriptor") && log.Infos[1].Contains("gate=equipment"), "bounded evidence"); });
            Console.WriteLine("All " + _passed + " Tome bridge fixture tests passed. No game assemblies executed.");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
