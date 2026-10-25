#if POVPLUS_TOME_DIAGNOSTIC
// INV-003 observation only. No Harmony patches, configuration bindings, or game writes.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace PoVPlus.Diagnostics
{
    internal sealed class TomeDiagnostic
    {
        private const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.Static;
        private readonly ManualLogSource _log;
        private readonly Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();
        private readonly DiagnosticSession _session;
        private float _nextPoll;
        private static readonly string[] Items = { "BMP_BeginnersMagicBook", "BMP_AdvanceMagicBook",
            "BMP_DruidsTome", "ArmorMushroomcallerTome_DO" };
        private static readonly string[,] Dependencies = {
            { "blacks7ar.MagicPlugin", "6504C2FF9CEAAE7B317820E41EF84D8C28D33FEDBF32B7D0F4FDDFD680BDA795" },
            { "Dreanegade.Magic_Supremacy", "8BAFE7DAEFB9B6AF057FC3B10534764ED9CCEBA430B526A014B2DD12CFABA787" },
            { "seneaL.valheim.api", "2EF9EB8DD3AB7DC72E7F3A908EFF843C4C7FAD19570CB46C3565BC36841E3574" },
            { "seneaL.valheim.ui", "53B0F675BFFD0BAC5FCCA711AF04AA1EF00BB5BECD543D702EBAA146E5C487CD" },
            { "treextr.pathofvalheiman", "83BD50F23D9C301FE469B5FBE2C430FD35C928D7A2CCFD99AF5A1040558C9D72" }
        };

        internal TomeDiagnostic(ManualLogSource log)
        {
            _log = log;
            _session = new DiagnosticSession(Say);
            Say("ARMED repairedOfflineProbe; exact identities required; manual session only; no gameplay writes");
            try
            {
                VerifyHash(typeof(Player).Assembly, "94998885B103D6E5F5480123EAEE0A093E82AF2DDEEEE37536C15BF555F24F3A");
                VerifyHash(typeof(Harmony).Assembly, "1A21CC03424FC82C3DD1346905D16494536B9595AE4162228D99FB7C285C1031");
            }
            catch (Exception error) { _session.Abort("UnauditedRuntime " + DiagnosticSession.Quote(error.Message)); }
        }

        internal void Poll()
        {
            if (_session.Finished || Time.realtimeSinceStartup < _nextPoll) return;
            _nextPoll = Time.realtimeSinceStartup + 1f;
            try
            {
                var player = Player.m_localPlayer;
                var observation = new DiagnosticObservation { HasPlayer = player };
                observation.WorldReady = ZNet.instance && Game.instance;
                if (ZNet.instance) observation.World = ZNet.instance.GetWorldName();
                if (player)
                {
                    var view = player.GetComponent<ZNetView>();
                    observation.PlayerReady = view && view.IsValid() && !(bool)Field(typeof(Player), "m_isLoading", player);
                    if (view && view.IsValid()) observation.Character = player.GetPlayerName();
                }
                observation.InventoryReady = observation.PlayerReady && InventoryGui.IsVisible();
                var visible = new List<object>();
                // Do not inspect item/UI/mod state until exact names and session readiness agree.
                if (observation.PlayerReady && observation.WorldReady && observation.InventoryReady &&
                    DiagnosticSession.Authorized(observation))
                {
                    if (_assemblies.Count == 0) VerifyDependencies();
                    var view = Field(TypeFor("seneaL.valheim.ui", "SeneaLUI.Patches.InventoryGuiPatches"), "View", null);
                    var links = view == null ? null : Field(view.GetType(), "_links", view) as IEnumerable;
                    if (links != null)
                        foreach (var cell in links)
                        {
                            var def = Field(cell.GetType(), "Def", cell);
                            if (def == null || !IsTome(def)) continue;
                            var slot = Field(cell.GetType(), "Slot", cell);
                            var root = slot == null ? null : Field(slot.GetType(), "Root", slot) as RectTransform;
                            if (root && root.gameObject.activeInHierarchy) visible.Add(cell);
                        }
                    observation.DescriptorsReady = visible.Count > 0;
                    observation.PrefabsReady = ObjectDB.instance && Items.All(id => ObjectDB.instance.GetItemPrefab(id));
                    observation.MissingDetail = !observation.DescriptorsReady ? "No active bound Tome LinkCell" :
                        !observation.PrefabsReady ? "ObjectDB/prefab unavailable" : null;
                }
                _session.Tick(Time.realtimeSinceStartup, observation, () => Capture(player, visible));
            }
            catch (Exception error)
            {
                _session.Abort("BindingOrReadFailure " + DiagnosticSession.Quote(error.Message));
            }
        }

        private void VerifyDependencies()
        {
            for (int i = 0; i < Dependencies.GetLength(0); i++)
            {
                var guid = Dependencies[i, 0];
                if (!Chainloader.PluginInfos.ContainsKey(guid) || !Chainloader.PluginInfos[guid].Instance)
                    throw new InvalidOperationException("Missing dependency: " + guid);
                var asm = Chainloader.PluginInfos[guid].Instance.GetType().Assembly;
                VerifyHash(asm, Dependencies[i, 1]);
                _assemblies.Add(guid, asm);
            }
        }

        private void Capture(Player player, List<object> cells)
        {
            Say("BEGIN utc=" + DateTime.UtcNow.ToString("o") +
                " character=PoVPlus_Test world=POVPlus_Lab localPlayer=true inventoryVisible=true");
            foreach (var pair in _assemblies)
                Say("DEPENDENCY guid=" + pair.Key + " version=" + Chainloader.PluginInfos[pair.Key].Metadata.Version);
            var magic = TypeFor("blacks7ar.MagicPlugin", "MagicPlugin.Functions.MagicSlot");
            var ms = TypeFor("Dreanegade.Magic_Supremacy", "Magic_Supremacy.Magic_SupremacyPlugin");
            var equipment = TypeFor("treextr.pathofvalheiman", "Valheiman.Api.Equipment");
            var mpPredicate = Method(magic, "IsTomeItem", typeof(ItemDrop.ItemData));
            var msPredicate = Method(ms, "IsTomeItem", typeof(ItemDrop.ItemData));
            var povPredicate = Method(equipment, "Accepts", typeof(string), typeof(ItemDrop.ItemData));
            var config = Field(TypeFor("blacks7ar.MagicPlugin", "MagicPlugin.Plugin"), "_enableTomeSlot", null);
            Say("POLICY MagicPlugin.EnableTomeSlot=" + Property(config, "Value") +
                " nativePoVEquipmentOn=" + StaticProperty(equipment, "On"));
            var registry = AppDomain.CurrentDomain.GetData("DO_CustomSlotSystem_SlotPrefabs")
                as Dictionary<string, HashSet<string>>;
            Say("AUTHORITY MS.owner=" + AppDomain.CurrentDomain.GetData("DO_CustomSlotSystem_Owner") +
                " registryAvailable=" + (registry != null));
            var customSystem = TypeFor("Dreanegade.Magic_Supremacy", "Magic_Supremacy.CustomSlotSystem");
            var definitions = Field(customSystem, "SlotDefinitions", null) as IEnumerable;
            if (definitions != null)
                foreach (var def in definitions)
                    Say("MS_DEFINITION slotId=" + Field(def.GetType(), "SlotId", def) +
                        " predicate=" + Describe(Field(def.GetType(), "IsValid", def) as Delegate));
            var api = TypeFor("seneaL.valheim.api", "SeneaL.API.CustomSlots");
            var descriptors = (IEnumerable)Method(api, "Snapshot").Invoke(null, null);
            foreach (var def in descriptors)
                if (IsTome(def)) Say("REGISTERED " + Descriptor(def));
            var auditedCells = new HashSet<object>();
            foreach (var cell in cells)
            {
                var def = Field(cell.GetType(), "Def", cell);
                var slot = Field(cell.GetType(), "Slot", cell);
                var root = (RectTransform)Field(slot.GetType(), "Root", slot);
                Say("RENDERED " + Descriptor(def) + " activeInHierarchy=true hierarchy=" + Hierarchy(root));
                try { ValidateVisiblePredicate(def); auditedCells.Add(cell); }
                catch (InvalidOperationException error) { Say("SKIP visiblePredicate " + DiagnosticSession.Quote(error.Message)); }
            }
            foreach (var id in Items)
            {
                var prefab = ObjectDB.instance.GetItemPrefab(id);
                var drop = prefab.GetComponent<ItemDrop>();
                if (!drop || drop.m_itemData == null || drop.m_itemData.m_shared == null)
                    throw new InvalidOperationException("Incomplete item prefab: " + id);
                var item = drop.m_itemData;
                var shared = item.m_shared;
                Say("ITEM prefab=" + id + " dropPrefab=" + (item.m_dropPrefab ? item.m_dropPrefab.name : "null") +
                    " sharedName=" + shared.m_name + " type=" + shared.m_itemType +
                    " numericType=" + (int)shared.m_itemType + " equipped=" + item.m_equipped +
                    " equipSE=" + (shared.m_equipStatusEffect ? shared.m_equipStatusEffect.name : "null") +
                    " regen=" + shared.m_eitrRegenModifier.ToString(CultureInfo.InvariantCulture) +
                    " customKeys=" + (item.m_customData == null ? "null" : string.Join(",", item.m_customData.Keys)) +
                    " components=" + string.Join(",", prefab.GetComponents<Component>()
                        .Where(c => c).Select(c => c.GetType().FullName)));
                HashSet<string> names;
                Say("ACCEPT prefab=" + id + " MagicPlugin=" + Query(mpPredicate, null, item) +
                    " MagicSupremacyDirect=" + Query(msPredicate, null, item) +
                    " MSRegistryTome=" + (registry != null && registry.TryGetValue("tome", out names)
                        ? names.Contains(id).ToString() : "unavailable") +
                    " nativePoV=" + Query(povPredicate, null, "utility.tome", item));
                foreach (var cell in auditedCells)
                {
                    var def = Field(cell.GetType(), "Def", cell);
                    var result = Query(Method(def.GetType(), "Allows", typeof(ItemDrop.ItemData)), def, item);
                    Say("VISIBLE_ACCEPT prefab=" + id + " key=" + Property(def, "Key") + " result=" + result);
                }
            }
            Say("END snapshotComplete=true observationOnly=true; no equipment/stat/persistence test performed");
        }

        private void ValidateVisiblePredicate(object def)
        {
            if ((string)Property(def, "Owner") != "AzuEPI.emulated" || (string)Property(def, "Id") != "$tome_DO")
                throw new InvalidOperationException("Unexpected visible Tome descriptor; no predicates invoked.");
            var accepts = Property(def, "Accepts") as Delegate;
            if (accepts == null || accepts.Method.DeclaringType.FullName != "SeneaL.API.StubHost+<>c__DisplayClass8_3" ||
                accepts.Method.Name != "<Emulate>b__5")
                throw new InvalidOperationException("Unaudited visible acceptance wrapper.");
            var host = TypeFor("seneaL.valheim.api", "SeneaL.API.StubHost");
            var emus = Field(host, "Emus", null) as IDictionary;
            var emu = emus == null ? null : emus["$tome_DO"];
            if (emu == null || !ReferenceEquals(Field(emu.GetType(), "Def", emu), def))
                throw new InvalidOperationException("Descriptor does not match audited emulation entry.");
            if (!ReferenceEquals(Field(accepts.Target.GetType(), "emu", accepts.Target), emu))
                throw new InvalidOperationException("Acceptance closure targets a different emulation entry.");
            RequireUnpatched(accepts.Method);
            RequireUnpatched(Method(accepts.Method.DeclaringType.Assembly.GetType(
                "SeneaL.API.StubHost+<>c__DisplayClass8_4", true), "<Emulate>b__6",
                typeof(Func<ItemDrop.ItemData, bool>)));
            var valids = Field(emu.GetType(), "Valids", emu) as IEnumerable;
            int count = 0;
            foreach (Delegate predicate in valids)
            {
                if (predicate.Method.DeclaringType != TypeFor("Dreanegade.Magic_Supremacy",
                    "Magic_Supremacy.Magic_SupremacyPlugin") || predicate.Method.Name != "IsTomeItem" ||
                    predicate.GetInvocationList().Length != 1)
                    throw new InvalidOperationException("Unaudited underlying predicate: " + Describe(predicate));
                Say("VISIBLE_PREDICATE key=" + Property(def, "Key") + " target=" + Describe(predicate));
                RequireUnpatched(predicate.Method);
                count++;
            }
            if (count == 0 || accepts.GetInvocationList().Length != 1)
                throw new InvalidOperationException("Empty/multicast acceptance wrapper.");
        }

        private static bool IsTome(object def)
        {
            return ((string)Property(def, "Id") + " " + (string)Property(def, "Title"))
                .IndexOf("tome", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string Descriptor(object def)
        {
            return "owner=" + Property(def, "Owner") + " id=" + Property(def, "Id") +
                " key=" + Property(def, "Key") + " title=" + Property(def, "Title") +
                " origin=" + Property(def, "Origin") + " mode=" + Property(def, "Mode") +
                " accepts=" + Describe(Property(def, "Accepts") as Delegate) +
                " source=" + Describe(Property(def, "Source") as Delegate) +
                " put=" + Describe(Property(def, "Put") as Delegate) +
                " take=" + Describe(Property(def, "Take") as Delegate);
        }

        private Type TypeFor(string guid, string name) { return _assemblies[guid].GetType(name, true); }
        private static void VerifyHash(Assembly assembly, string expected)
        {
            using (var stream = File.OpenRead(assembly.Location))
            using (var sha = SHA256.Create())
                if (BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "") != expected)
                    throw new InvalidOperationException("Unaudited DLL: " + assembly.GetName().Name);
        }
        private static void RequireUnpatched(MethodInfo method)
        {
            var patches = Harmony.GetPatchInfo(method);
            if (patches != null && patches.Owners.Count > 0)
                throw new InvalidOperationException("Unaudited runtime patches on " + method.DeclaringType.FullName + "." + method.Name);
        }
        private static string Query(MethodInfo method, object instance, params object[] arguments)
        {
            try { RequireUnpatched(method); return Convert.ToString(method.Invoke(instance, arguments), CultureInfo.InvariantCulture); }
            catch (Exception error) { return "unverified:" + DiagnosticSession.Quote(error.GetType().Name + ": " + error.Message); }
        }
        private static object Field(Type type, string name, object instance)
        {
            var field = type.GetField(name, All);
            if (field == null) throw new MissingFieldException(type.FullName, name);
            return field.GetValue(instance);
        }
        private static object Property(object instance, string name)
        {
            var property = instance.GetType().GetProperty(name, All);
            if (property == null) throw new MissingMemberException(instance.GetType().FullName, name);
            return property.GetValue(instance, null);
        }
        private static object StaticProperty(Type type, string name)
        {
            var property = type.GetProperty(name, All);
            if (property == null) throw new MissingMemberException(type.FullName, name);
            return property.GetValue(null, null);
        }
        private static MethodInfo Method(Type type, string name, params Type[] parameters)
        {
            var method = type.GetMethod(name, All, null, parameters, null);
            if (method == null) throw new MissingMethodException(type.FullName, name);
            return method;
        }
        private static string Describe(Delegate method)
        {
            return method == null ? "null" : method.Method.DeclaringType.Assembly.GetName().Name + ":" +
                method.Method.DeclaringType.FullName + "." + method.Method.Name;
        }
        private static string Hierarchy(Transform transform)
        {
            var names = new List<string>();
            for (int i = 0; transform && i < 10; i++, transform = transform.parent) names.Add(transform.name);
            names.Reverse();
            return string.Join("/", names);
        }
        private void Say(string value) { _log.LogInfo("[PoVPlus.TomeDiagnostic] " + value); }
    }
}
#endif
