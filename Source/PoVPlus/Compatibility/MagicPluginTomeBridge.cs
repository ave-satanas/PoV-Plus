using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;

namespace PoVPlus.Compatibility
{
    // Acceptance only: MS remains responsible for equipment, effects, persistence and visuals.
    internal sealed class MagicPluginTomeBridge
    {
        private const string Owner = "povplus.core.magicplugin-tomes";
        private const BindingFlags Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private static MagicPluginTomeBridge _current;
        private readonly BridgeActivation _activation;
        private readonly ManualLogSource _log;
        private readonly MethodInfo[] _targets = new MethodInfo[2];
        private readonly HashSet<string> _observedExtensions = new HashSet<string>();
        private Harmony _harmony;
        private object _nativeToggle;
        private PropertyInfo _toggleValue;
        private static readonly string[,] Dependencies = {
            { "blacks7ar.MagicPlugin", "6504C2FF9CEAAE7B317820E41EF84D8C28D33FEDBF32B7D0F4FDDFD680BDA795" },
            { "Dreanegade.Magic_Supremacy", "8BAFE7DAEFB9B6AF057FC3B10534764ED9CCEBA430B526A014B2DD12CFABA787" },
            { "seneaL.valheim.api", "2EF9EB8DD3AB7DC72E7F3A908EFF843C4C7FAD19570CB46C3565BC36841E3574" },
            { "seneaL.valheim.ui", "53B0F675BFFD0BAC5FCCA711AF04AA1EF00BB5BECD543D702EBAA146E5C487CD" },
            { "treextr.pathofvalheiman", "83BD50F23D9C301FE469B5FBE2C430FD35C928D7A2CCFD99AF5A1040558C9D72" }
        };

        internal MagicPluginTomeBridge(ManualLogSource log)
        {
            _log = log;
            _activation = new BridgeActivation(message => _log.LogWarning("[PoVPlus.TomeBridge] " + message));
        }

        internal void Start(bool enabled)
        {
            if (!enabled) return; // No dependency inspection, Harmony instance, or patches when disabled.
            if (_current != null) { _log.LogWarning("[PoVPlus.TomeBridge] Duplicate initialization refused."); return; }
            _current = this;
            if (_activation.Start(true, Validate, Install, VerifyInstalled, Cleanup))
                _log.LogInfo("[PoVPlus.TomeBridge] Both acceptance extensions active; effects/gameplay remain unverified. Restart required for configuration changes.");
        }

        private void Validate()
        {
            var assemblies = new Dictionary<string, Assembly>();
            for (int i = 0; i < Dependencies.GetLength(0); i++)
            {
                var guid = Dependencies[i, 0];
                if (!Chainloader.PluginInfos.ContainsKey(guid) || !Chainloader.PluginInfos[guid].Instance)
                    throw new InvalidOperationException("Required initialized dependency missing: " + guid);
                var assembly = Chainloader.PluginInfos[guid].Instance.GetType().Assembly;
                VerifyHash(assembly, Dependencies[i, 1]);
                assemblies.Add(guid, assembly);
            }
            VerifyHash(typeof(Player).Assembly, "94998885B103D6E5F5480123EAEE0A093E82AF2DDEEEE37536C15BF555F24F3A");
            VerifyHash(typeof(Harmony).Assembly, "1A21CC03424FC82C3DD1346905D16494536B9595AE4162228D99FB7C285C1031");
            var magic = assemblies["blacks7ar.MagicPlugin"].GetType("MagicPlugin.Plugin", true);
            var toggle = magic.GetField("_enableTomeSlot", Static);
            if (toggle == null) throw new MissingFieldException(magic.FullName, "_enableTomeSlot");
            _nativeToggle = toggle.GetValue(null);
            if (_nativeToggle == null) throw new InvalidOperationException("MagicPlugin configuration is not initialized.");
            _toggleValue = _nativeToggle.GetType().GetProperty("Value");
            if (_toggleValue == null || _toggleValue.PropertyType.FullName != "MagicPlugin.Functions.Toggle")
                throw new InvalidOperationException("Unsupported native Tome toggle contract.");
            if (!NativeSlotOff()) throw new InvalidOperationException("MagicPlugin native Tome slot must remain Off; no setting changed.");

            var ms = assemblies["Dreanegade.Magic_Supremacy"];
            _targets[0] = ResolveTarget(ms.GetType("Magic_Supremacy.Magic_SupremacyPlugin", true), "IsTomeItem", typeof(ItemDrop.ItemData));
            _targets[1] = ResolveTarget(ms.GetType("Magic_Supremacy.CustomSlotSystem", true), "IsItemValidForSlot", typeof(ItemDrop.ItemData), typeof(string));
            foreach (var target in _targets)
            {
                var info = Harmony.GetPatchInfo(target);
                if (info != null && info.Owners.Count > 0)
                    throw new InvalidOperationException("Acceptance target already patched; manual review required: " + target);
            }
            // Construct only after validating both targets and all dependency pins.
            _harmony = new Harmony(Owner);
        }

        internal static MethodInfo ResolveTarget(Type type, string name, params Type[] parameters)
        {
            var method = type.GetMethod(name, Static | BindingFlags.DeclaredOnly, null, parameters, null);
            if (method == null || !method.IsPrivate || !method.IsStatic || method.ReturnType != typeof(bool) ||
                method.ContainsGenericParameters || method.GetMethodBody() == null)
                throw new InvalidOperationException("Unsupported acceptance signature: " + type.FullName + "." + name);
            return method;
        }

        private void Install(int gate)
        {
            var postfix = typeof(MagicPluginTomeBridge).GetMethod(gate == 0 ? "DescriptorPostfix" : "EquipmentPostfix", Static);
            _harmony.Patch(_targets[gate], postfix: new HarmonyMethod(postfix));
        }

        private void VerifyInstalled()
        {
            for (int i = 0; i < _targets.Length; i++)
            {
                var info = Harmony.GetPatchInfo(_targets[i]);
                string name = i == 0 ? "DescriptorPostfix" : "EquipmentPostfix";
                if (info == null || info.Postfixes.Count(p => p.owner == Owner && p.PatchMethod.Name == name) != 1 ||
                    info.Owners.Any(owner => owner != Owner))
                    throw new InvalidOperationException("Incomplete or competing patch installation: " + _targets[i]);
            }
        }

        private void Cleanup()
        {
            if (_harmony == null) return;
            var failures = new List<Exception>();
            foreach (var target in _targets)
                if (target != null)
                    try { _harmony.Unpatch(target, HarmonyPatchType.All, Owner); }
                    catch (Exception error) { failures.Add(error); }
            if (failures.Count > 0) throw new AggregateException("Owner-only cleanup incomplete.", failures);
        }

        private bool NativeSlotOff() { return Convert.ToInt32(_toggleValue.GetValue(_nativeToggle, null)) == 0; }

        private bool Eligible(ItemDrop.ItemData item)
        {
            if (item == null || item.m_shared == null || !item.m_dropPrefab || !ObjectDB.instance) return false;
            string id = item.m_dropPrefab.name;
            if (!TomeBridgePolicy.Supports(id)) return false;
            // Do not accept a renamed/spoofed prefab, a foreign shared data object, or a missing SE.
            var canonical = ObjectDB.instance.GetItemPrefab(id);
            if (!canonical || canonical != item.m_dropPrefab) return false;
            var drop = canonical.GetComponent<ItemDrop>();
            if (!drop || drop.m_itemData == null || !ReferenceEquals(drop.m_itemData.m_shared, item.m_shared)) return false;
            if (!item.m_shared.m_equipStatusEffect || item.m_shared.m_equipStatusEffect.name != TomeBridgePolicy.EffectFor(id)) return false;
            // Preserve the existence of the upstream slot/registry; never initialize a second registry.
            var registry = AppDomain.CurrentDomain.GetData("DO_CustomSlotSystem_SlotPrefabs")
                as Dictionary<string, HashSet<string>>;
            HashSet<string> native;
            if (registry == null || !registry.TryGetValue("tome", out native) || native == null ||
                !native.Contains("ArmorMushroomcallerTome_DO")) return false;
            if (!NativeSlotOff())
            {
                _activation.Stop("MagicPlugin native Tome setting changed; no setting was modified by PoVPlus.");
                return false;
            }
            return true;
        }

        private static void DescriptorPostfix(ItemDrop.ItemData __0, ref bool __result)
        {
            Extend(__0, "tome", "descriptor", ref __result);
        }

        private static void EquipmentPostfix(ItemDrop.ItemData __0, string __1, ref bool __result)
        {
            Extend(__0, __1, "equipment", ref __result);
        }

        private static void Extend(ItemDrop.ItemData item, string slot, string gate, ref bool result)
        {
            var bridge = _current;
            if (result || bridge == null || !bridge._activation.Active || slot != "tome") return;
            try
            {
                bool valid = bridge.Eligible(item);
                result = TomeBridgePolicy.Extend(result, bridge._activation.Active,
                    item != null && item.m_dropPrefab ? item.m_dropPrefab.name : null, slot, valid);
                if (result && bridge._observedExtensions.Add(gate + ":" + item.m_dropPrefab.name))
                    bridge._log.LogInfo("[PoVPlus.TomeBridge] Extended gate=" + gate + " prefab=" + item.m_dropPrefab.name +
                        "; first observed eligibility extension, not proof of equip/effects.");
            }
            catch (Exception error)
            {
                // A runtime binding/read failure disables both extensions; preserve the original result.
                result = false; // This path is entered only for an originally false result.
                bridge._activation.Stop(error.GetType().Name + ": " + error.Message);
            }
        }

        private static void VerifyHash(Assembly assembly, string expected)
        {
            using (var stream = File.OpenRead(assembly.Location))
            using (var sha = SHA256.Create())
                if (BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "") != expected)
                    throw new InvalidOperationException("Unsupported assembly fingerprint: " + assembly.GetName().Name);
        }
    }
}
