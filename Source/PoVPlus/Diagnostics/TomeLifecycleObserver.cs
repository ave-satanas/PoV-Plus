#if POVPLUS_TOME_LIFECYCLE_DIAGNOSTIC
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace PoVPlus.Diagnostics
{
    // No equipment API calls, callbacks, patch installation, save access or field/property setters.
    internal sealed class TomeLifecycleObserver
    {
        private readonly ManualLogSource _log;
        private readonly bool _bridgeSetting;
        private readonly LifecycleWindow _window = new LifecycleWindow();
        private readonly Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();
        private readonly List<ItemDrop.ItemData> _aliases = new List<ItemDrop.ItemData>();
        private readonly string _salt = Guid.NewGuid().ToString("N"); // Observer-only; never an item GUID.
        private double _nextPoll, _lastClock = -1;
        private bool _reportedStop;
        private static readonly string[,] Pins = {
            {"blacks7ar.MagicPlugin", "6504C2FF9CEAAE7B317820E41EF84D8C28D33FEDBF32B7D0F4FDDFD680BDA795"},
            {"Dreanegade.Magic_Supremacy", "8BAFE7DAEFB9B6AF057FC3B10534764ED9CCEBA430B526A014B2DD12CFABA787"},
            {"seneaL.valheim.api", "2EF9EB8DD3AB7DC72E7F3A908EFF843C4C7FAD19570CB46C3565BC36841E3574"},
            {"seneaL.valheim.ui", "53B0F675BFFD0BAC5FCCA711AF04AA1EF00BB5BECD543D702EBAA146E5C487CD"},
            {"treextr.pathofvalheiman", "83BD50F23D9C301FE469B5FBE2C430FD35C928D7A2CCFD99AF5A1040558C9D72"}
        };
        internal TomeLifecycleObserver(ManualLogSource log, bool bridgeSetting)
        {
            _log = log; _bridgeSetting = bridgeSetting;
            try
            {
                Pin(typeof(Player).Assembly, "94998885B103D6E5F5480123EAEE0A093E82AF2DDEEEE37536C15BF555F24F3A");
                Pin(typeof(UnityEngine.Object).Assembly, "FBA3821AFB6867FE4D471DC50BF04BA21E6982B33AB6B5336B08A5C55119D990");
                Pin(typeof(string).Assembly, "5ED1180FC8CB409D57952296C7F573F2B1B3D3D2A2CD37258EF407498A17DD4D");
                Pin(typeof(Harmony).Assembly, "1A21CC03424FC82C3DD1346905D16494536B9595AE4162228D99FB7C285C1031");
                for (int i = 0; i < Pins.GetLength(0); i++)
                {
                    var guid = Pins[i, 0];
                    if (!Chainloader.PluginInfos.ContainsKey(guid) || !Chainloader.PluginInfos[guid].Instance)
                        throw new InvalidOperationException("Missing initialized dependency " + guid);
                    var assembly = Chainloader.PluginInfos[guid].Instance.GetType().Assembly;
                    Pin(assembly, Pins[i, 1]); _assemblies.Add(guid, assembly);
                }
                RequirePlain(typeof(Time).GetProperty("realtimeSinceStartup").GetGetMethod());
                RequirePlain(typeof(UnityEngine.Object).GetMethod("op_Implicit", new[] { typeof(UnityEngine.Object) }));
                RequirePlain(typeof(UnityEngine.Object).GetMethod("op_Equality", new[] { typeof(UnityEngine.Object), typeof(UnityEngine.Object) }));
                RequirePlain(typeof(UnityEngine.Object).GetProperty("name").GetGetMethod());
                RequirePlain(typeof(Component).GetProperty("transform").GetGetMethod());
                RequirePlain(typeof(Component).GetProperty("gameObject").GetGetMethod());
                RequirePlain(typeof(GameObject).GetProperty("activeInHierarchy").GetGetMethod());
                RequirePlain(typeof(Transform).GetMethod("Find", new[] { typeof(string) }));
                RequirePlain(typeof(Player).GetMethod("GetPlayerName", Type.EmptyTypes));
                RequirePlain(typeof(ZNet).GetMethod("GetWorldName", Type.EmptyTypes));
                RequirePlain(typeof(ZNetView).GetMethod("IsValid", Type.EmptyTypes));
                RequirePlain(typeof(ZNetView).GetMethod("GetZDO", Type.EmptyTypes));
                RequirePlain(typeof(ZDO).GetMethod("GetString", new[] { typeof(int), typeof(string) }));
                Say("ARMED bridgeSetting=" + bridgeSetting + " exact=PoVPlus_Test/PoVPlus_Lab maxSamples=120 maxSeconds=600 intervalSeconds=1 noGameplayWrites=true");
            }
            catch (Exception error) { _window.Stop("InitializationFailure " + error.GetType().Name + ": " + error.Message); ReportStop(); }
        }
        internal void Poll()
        {
            if (_window.Finished) { ReportStop(); return; }
            double now;
            try
            {
                RequirePlain(typeof(Time).GetProperty("realtimeSinceStartup").GetGetMethod());
                RequirePlain(typeof(UnityEngine.Object).GetMethod("op_Implicit", new[] { typeof(UnityEngine.Object) }));
                RequirePlain(typeof(UnityEngine.Object).GetMethod("op_Equality", new[] { typeof(UnityEngine.Object), typeof(UnityEngine.Object) }));
                now = Time.realtimeSinceStartup;
            }
            catch (Exception error) { _window.Stop("UnauditedClockOrObjectRead " + error.Message); ReportStop(); return; }
            if (double.IsNaN(now) || double.IsInfinity(now) || now < 0 || now < _lastClock)
            { _window.Stop("InvalidMonotonicClock"); ReportStop(); return; }
            _lastClock = now;
            if (now < _nextPoll) return;
            _nextPoll = now + 1;
            try
            {
                RequirePlain(typeof(ZNet).GetProperty("instance").GetGetMethod());
                RequirePlain(typeof(Game).GetProperty("instance").GetGetMethod());
                var player = Player.m_localPlayer;
                bool ready = player && ZNet.instance && Game.instance;
                string character = null, world = null;
                if (ready)
                {
                    // Audited getters may be patched later; recheck at every sample.
                    RequirePlain(typeof(Player).GetMethod("GetPlayerName", Type.EmptyTypes));
                    RequirePlain(typeof(ZNet).GetMethod("GetWorldName", Type.EmptyTypes));
                    RequirePlain(typeof(ZNetView).GetMethod("IsValid", Type.EmptyTypes));
                    RequirePlain(typeof(ZNetView).GetMethod("GetZDO", Type.EmptyTypes));
                    RequirePlain(typeof(ZDO).GetMethod("IsValid", Type.EmptyTypes));
                    RequirePlain(typeof(ZDO).GetMethod("GetString", new[] { typeof(int), typeof(string) }));
                    var view = Read(player, "m_nview") as ZNetView;
                    ready = view && view.IsValid() && !(bool)Read(player, "m_isLoading");
                    if (ready)
                    {
                        character = player.GetPlayerName(); world = ZNet.instance.GetWorldName();
                        ready = !string.IsNullOrWhiteSpace(character) && character != "..." && !string.IsNullOrWhiteSpace(world);
                    }
                }
                var row = _window.Sample(now, ready, character, world, () => Capture(player, character, world), DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));
                if (row != null) Say("OBSERVATION " + row);
                ReportStop();
            }
            catch (Exception error) { _window.Stop("ObservationFailure " + error.GetType().Name + ": " + error.Message); ReportStop(); }
        }
        private LifecycleEvidence Capture(Player player, string character, string world)
        {
            RequirePlain(typeof(UnityEngine.Object).GetProperty("name").GetGetMethod());
            RequirePlain(typeof(Component).GetProperty("transform").GetGetMethod());
            RequirePlain(typeof(Component).GetProperty("gameObject").GetGetMethod());
            RequirePlain(typeof(GameObject).GetProperty("activeInHierarchy").GetGetMethod());
            RequirePlain(typeof(Transform).GetMethod("Find", new[] { typeof(string) }));
            var e = new LifecycleEvidence();
            e.Known("identity.character", character, "Player.GetPlayerName (audited unpatched call)");
            e.Known("identity.world", world, "ZNet.GetWorldName (audited unpatched call)");
            e.Known("bridge.settingAtStartup", _bridgeSetting.ToString(), "ConfigEntry<bool>.Value passed by Awake; not a hot toggle");
            Observe(e, "bridge.patches", () => BridgePatches(e));
            Observe(e, "inventory.main", () => Inventory(e, "inventory.main", Read(player, "m_inventory") as Inventory));
            Observe(e, "equipment.vanillaUtility", () => Item(e, "equipment.vanillaUtility", Read(player, "m_utilityItem") as ItemDrop.ItemData, "Humanoid.m_utilityItem"));
            Observe(e, "ms", () => Supremacy(e, player));
            Observe(e, "ui", () => Rendered(e));
            Observe(e, "effects", () => Effects(e, player));
            Scalar(e, "eitr.maximum", player, "m_maxEitr");
            Scalar(e, "eitr.current", player, "m_eitr");
            Scalar(e, "eitr.regenDelay", player, "m_eitrRegenDelay");
            Scalar(e, "eitr.regenTimer", player, "m_eitrRegenTimer");
            e.Unknown("eitr.actualRegenRate", "not calculated", "Requires separately approved controlled expenditure and timed comparison; modifiers are not Eitr/sec.");
            e.Unknown("ui.tomeItem", "SlotDef.Source", "Source delegate not invoked; rendered descriptor is distinct from authoritative item ownership.");
            e.Unknown("inventory.seneaLCustom", "CustomStore.Inv", "Existing-store initialization provenance incomplete; static fields/lazy getters not touched.");
            e.Unknown("equipment.povUtilityOwners", "PassiveTree.Gear.State", "No fully audited non-initializing owner-store path; static fields/getters deliberately not touched.");
            e.Unknown("equipment.exclusivity", "combined owner graph", "PoV utility authority and all linked-source item bindings are not completely observable; do not infer exclusivity from flags.");
            e.Unknown("neutral.certification", "lifecycle observer", "Partial evidence only: manual manifest, all authorities, settled samples and enabled-bridge neutral reload still required.");
            return e;
        }
        private void Supremacy(LifecycleEvidence e, Player player)
        {
            var owner = AppDomain.CurrentDomain.GetData("DO_CustomSlotSystem_Owner") as string;
            if (owner == null) e.Unknown("ms.authorityMarker", "AppDomain.GetData DO_CustomSlotSystem_Owner", "Owner marker absent.");
            else e.Known("ms.authorityMarker", owner, "AppDomain.GetData DO_CustomSlotSystem_Owner");
            if (owner != "Magic_Supremacy") throw new InvalidOperationException("MS owner marker missing/foreign; refuse static field access (cctor gate).");
            var type = TypeFor("Dreanegade.Magic_Supremacy", "Magic_Supremacy.CustomSlotSystem");
            RequirePlain(type.GetMethod("TryClaimSystemOwner", Flags)); // Marker alone does not prove CLR initialization.
            object table = InitializedField(type, "RuntimeData");
            if (table == null) throw new InvalidOperationException("Runtime table absent; no initialization attempted.");
            RequirePlain(table.GetType().GetMethod("TryGetValue"));
            object data = LifecycleRead.ExistingEntry(table, player); // Never a creating upstream getter.
            var map = Read(data, "EquippedItems") as Dictionary<string, ItemDrop.ItemData>;
            if (map == null || map.Count > 64) throw new InvalidOperationException("Unknown/bounded ownership dictionary contract.");
            e.Known("ms.map", "existing entries=" + map.Count, "RuntimeData.TryGetValue(player) -> EquippedItems");
            ItemDrop.ItemData tome;
            map.TryGetValue("tome", out tome);
            Item(e, "ms.tome", tome, "existing EquippedItems.TryGetValue(tome); dictionary observed");
            foreach (var pair in new SortedDictionary<string, ItemDrop.ItemData>(map, StringComparer.Ordinal))
                Item(e, "ms.owner." + pair.Key, pair.Value, "existing EquippedItems entry");
            bool multipleClaim = false, unflaggedOwner = false;
            var owned = new List<ItemDrop.ItemData>();
            var utility = Read(player, "m_utilityItem") as ItemDrop.ItemData;
            foreach (var pair in map)
            {
                if (pair.Value == null) continue;
                foreach (var previous in owned) if (ReferenceEquals(previous, pair.Value)) multipleClaim = true;
                if (ReferenceEquals(utility, pair.Value)) multipleClaim = true;
                if (!pair.Value.m_equipped) unflaggedOwner = true;
                owned.Add(pair.Value); // Observer-local list only; never changes the upstream map.
            }
            e.Known("ms.observedMultiClaim", multipleClaim.ToString(), "reference equality across existing MS entries and Humanoid.m_utilityItem; partial scope, not conflict diagnosis");
            e.Known("ms.observedOwnerWithoutFlag", unflaggedOwner.ToString(), "existing MS owner references compared with ItemData.m_equipped; observation only");
            var custom = Read(player, "m_customData") as Dictionary<string, string>;
            if (custom == null) e.Unknown("ms.savedBinding", "Player.m_customData", "Dictionary unavailable; not empty.");
            else
            {
                string guid;
                if (!custom.TryGetValue("DO_Equipped_tome", out guid) || string.IsNullOrEmpty(guid)) e.Empty("ms.savedBinding", "Player.m_customData.TryGetValue(DO_Equipped_tome); in-memory only");
                else e.Occupied("ms.savedBinding", Digest(guid), "existing in-memory binding; salted session fingerprint, not disk verification");
            }
        }
        private void Inventory(LifecycleEvidence e, string key, Inventory inventory)
        {
            if (inventory == null) throw new InvalidOperationException("Inventory unavailable; not proof of zero items.");
            var list = Read(inventory, "m_inventory") as List<ItemDrop.ItemData>;
            if (list == null || list.Count > 256) throw new InvalidOperationException("Inventory list missing/over256 bound.");
            e.Known(key + ".count", list.Count.ToString(CultureInfo.InvariantCulture), "Inventory.m_inventory.Count; includes equipped references");
            for (int i = 0; i < list.Count; i++) Item(e, key + ".item." + i.ToString("D3", CultureInfo.InvariantCulture), list[i], "Inventory.m_inventory[index]; no returned list is modified");
        }
        private void Item(LifecycleEvidence e, string key, ItemDrop.ItemData item, string source)
        {
            if (item == null) { e.Empty(key, source); return; }
            string alias = Alias(item), prefab = item.m_dropPrefab ? item.m_dropPrefab.name : "<unavailable>";
            string kind = prefab == "ArmorMushroomcallerTome_DO" ? "native-control" :
                prefab == "BMP_BeginnersMagicBook" || prefab == "BMP_AdvanceMagicBook" || prefab == "BMP_DruidsTome" ? "MP-book" : "other";
            string guid = null;
            if (item.m_customData != null) item.m_customData.TryGetValue("DO_CustomSlotGuid", out guid);
            if (item.m_customData == null) e.Unknown(key + ".guid", "ItemData.m_customData", "Custom dictionary unavailable; no GUID created.");
            else if (string.IsNullOrEmpty(guid)) e.Empty(key + ".guid", "existing custom dictionary.TryGetValue(DO_CustomSlotGuid)");
            else e.Known(key + ".guid", Digest(guid), "existing GUID salted fingerprint; comparable only within this observer process");
            var shared = item.m_shared;
            if (shared == null || !item.m_dropPrefab) e.Unknown(key + ".metadata", "ItemData.m_shared/m_dropPrefab", "Shared data or prefab unavailable; object still observed occupied.");
            var effect = shared == null ? null : shared.m_equipStatusEffect;
            e.Occupied(key, "alias=" + alias + " prefab=" + prefab + " family=" + kind + " stack=" + item.m_stack +
                " quality=" + item.m_quality + " equipped=" + item.m_equipped +
                " existingGuid=" + (string.IsNullOrEmpty(guid) ? "<absent>" : Digest(guid)) +
                " type=" + (shared == null ? "<unknown>" : ((int)shared.m_itemType).ToString(CultureInfo.InvariantCulture)) +
                " equipSE=" + (effect ? effect.name : "<none>") + " sharedRegenModifier=" +
                (shared == null ? "<unknown>" : shared.m_eitrRegenModifier.ToString("R", CultureInfo.InvariantCulture)),
                source + "; actual ItemData fields; Unity Object.name; no GetOrCreateItemGuid");
        }
        private void Rendered(LifecycleEvidence e)
        {
            // A named live root is presentation evidence, not proof of the static holder's
            // initialization or of this View's identity. The static read below remains refused.
            RequirePlain(typeof(InventoryGui).GetProperty("instance").GetGetMethod());
            var gui = InventoryGui.instance;
            var root = gui ? gui.transform.Find("root/SeneaLUI_Inventory") : null;
            if (!root || !root.gameObject.activeInHierarchy) throw new InvalidOperationException("Existing active SeneaL inventory root unavailable; no static UI cache read attempted.");
            var patchType = TypeFor("seneaL.valheim.ui", "SeneaLUI.Patches.InventoryGuiPatches");
            var view = InitializedField(patchType, "View");
            if (view == null) throw new InvalidOperationException("Existing View unavailable.");
            var links = Read(view, "_links") as IEnumerable;
            if (links == null) throw new InvalidOperationException("Existing bound-cell list unavailable.");
            int n = 0, tomes = 0;
            foreach (var cell in links)
            {
                if (++n > 64) throw new InvalidOperationException("UI link bound exceeded.");
                var def = Read(cell, "Def");
                if (def == null) continue;
                var id = Read(def, "<Id>k__BackingField") as string;
                if (id != "$tome_DO") continue;
                var owner = Read(def, "<Owner>k__BackingField") as string;
                var slot = Read(cell, "Slot");
                var boundRoot = slot == null ? null : Read(slot, "Root") as RectTransform;
                if (!boundRoot || !boundRoot.gameObject.activeInHierarchy) continue;
                tomes++;
                e.Occupied("ui.tomeBinding." + tomes, "owner=" + owner + " id=" + id + " origin=" +
                    (Read(def, "<Origin>k__BackingField") as string) + " boundRoot=" + boundRoot.name,
                    "existing View._links -> LinkCell.Def auto-property fields and Slot.Root.activeInHierarchy; no delegates invoked");
            }
            if (tomes == 0) e.Unknown("ui.tomeBinding", "existing active link cells", "No active Tome descriptor found; no slot-creation attempt.");

        }
        private void Effects(LifecycleEvidence e, Player player)
        {
            var seman = Read(player, "m_seman");
            if (seman == null) throw new InvalidOperationException("SEMan unavailable.");
            var effects = Read(seman, "m_statusEffects") as List<StatusEffect>;
            if (effects == null || effects.Count > 128) throw new InvalidOperationException("Status list missing/over128 bound.");
            e.Known("effects.count", effects.Count.ToString(CultureInfo.InvariantCulture), "SEMan.m_statusEffects.Count");
            for (int i = 0; i < effects.Count; i++)
            {
                var effect = effects[i];
                if (!effect) e.Unknown("effects." + i, "SEMan.m_statusEffects[index]", "Null/destroyed effect reference.");
                else e.Known("effects." + i, effect.name + " type=" + effect.GetType().FullName, "existing active SE object; Unity Object.name; no effect update calls");
            }
        }
        private void BridgePatches(LifecycleEvidence e)
        {
            var type = TypeFor("Dreanegade.Magic_Supremacy", "Magic_Supremacy.Magic_SupremacyPlugin");
            PatchEvidence(e, "descriptor", type.GetMethod("IsTomeItem", Flags));
            type = TypeFor("Dreanegade.Magic_Supremacy", "Magic_Supremacy.CustomSlotSystem");
            PatchEvidence(e, "internal", type.GetMethod("IsItemValidForSlot", Flags));
        }
        private static void PatchEvidence(LifecycleEvidence e, string gate, MethodInfo method)
        {
            if (method == null) throw new InvalidOperationException("Gate metadata unavailable.");
            var info = Harmony.GetPatchInfo(method);
            if (info == null || info.Owners.Count == 0) { e.Empty("bridge.patch." + gate, "Harmony.GetPatchInfo (metadata only; not bridge activation proof)"); return; }
            var owners = new List<string>(info.Owners); owners.Sort(StringComparer.Ordinal);
            e.Known("bridge.patch." + gate, "owners=" + string.Join(",", owners) + " postfixes=" + info.Postfixes.Count +
                " prefixes=" + info.Prefixes.Count + " transpilers=" + info.Transpilers.Count + " finalizers=" + info.Finalizers.Count + " ilManipulators=" + info.ILManipulators.Count,
                "Harmony.GetPatchInfo; does not invoke gate or assert complete activation");
        }
        private static void Scalar(LifecycleEvidence e, string key, object obj, string field)
        { Observe(e, key, () => e.Known(key, LifecycleEvidence.Number(Read(obj, field)), obj.GetType().Name + "." + field + " existing field")); }
        private static void Observe(LifecycleEvidence e, string key, Action read)
        { try { read(); } catch (Exception error) { e.Unknown(key, "audited field read", error.GetType().Name + ": " + error.Message); } }
        private string Alias(ItemDrop.ItemData item)
        {
            for (int i = 0; i < _aliases.Count; i++) if (ReferenceEquals(_aliases[i], item)) return "i" + i.ToString(CultureInfo.InvariantCulture);
            if (_aliases.Count >= 1024) throw new InvalidOperationException("Live-alias bound exceeded.");
            _aliases.Add(item); return "i" + (_aliases.Count - 1).ToString(CultureInfo.InvariantCulture);
        }
        private string Digest(string value)
        { using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(_salt + value))).Replace("-", ""); }
        private static readonly BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
        private Type TypeFor(string guid, string name) { return _assemblies[guid].GetType(name, true); }
        // Audit correction: external markers do not prove CLR type initialization. In particular,
        // MS is beforefieldinit and TryClaimSystemOwner/Log need not read any static field.
        // Retain candidate paths for review, but refuse their static reads rather than initialize
        // a foreign registry/cache. The surrounding Observe reports unknown, never empty.
        private static object InitializedField(Type type, string name)
        {
            if (!((type.FullName == "Magic_Supremacy.CustomSlotSystem" && name == "RuntimeData") ||
                  (type.FullName == "SeneaLUI.Patches.InventoryGuiPatches" && name == "View")))
                throw new InvalidOperationException("Unreviewed static field request.");
            return LifecycleRead.Field(type, name, null); // Always refuses these static fields.
        }
        private static object Read(object value, string field)
        { if (value == null) throw new InvalidOperationException("Unavailable instance for " + field); return LifecycleRead.Field(value.GetType(), field, value); }
        private static void RequirePlain(MethodInfo method)
        {
            if (method == null) throw new InvalidOperationException("Required audited read signature unavailable.");
            var patches = Harmony.GetPatchInfo(method);
            if (patches != null && patches.Owners.Count > 0) throw new InvalidOperationException("Unaudited read interceptors: " + method.Name);
        }
        private static void Pin(Assembly assembly, string expected)
        { using (var sha = SHA256.Create()) using (var stream = File.OpenRead(assembly.Location)) if (BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "") != expected) throw new InvalidOperationException("Unaudited binary " + assembly.GetName().Name); }
        private void Say(string message) { _log.LogInfo("[PoVPlus.TomeLifecycle] " + message); }
        private void ReportStop()
        { if (_window.Finished && !_reportedStop) { _reportedStop = true; Say("STOP " + LifecycleEvidence.Json(_window.StopReason)); } }
    }
}
#endif
