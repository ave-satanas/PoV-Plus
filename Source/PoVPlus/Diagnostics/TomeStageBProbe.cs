#if POVPLUS_TOME_STAGE_B_DIAGNOSTIC
// Temporary observation only. The caller pins all dependency DLLs before CheckStartup.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace PoVPlus.Diagnostics
{
    internal static class TomeStageBProbe
    {
        private const string Owner = "povplus.core.magicplugin-tomes";
        private const string RegistryKey = "DO_CustomSlotSystem_SlotPrefabs";
        private const string Native = "ArmorMushroomcallerTome_DO";
        private const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
        private static MethodInfo _descriptor, _internal, _registryGetter;
        private static MethodInfo[] _prefabReads;

        internal static void CheckStartup(Assembly supremacy, Action<string> log, bool? bridgeEnabledAtStartup)
        {
            // Immutable value supplied from the bound ConfigEntry by Awake. Own PluginInfo.Instance
            // is assigned only after AddComponent/Awake returns; never query it here.
            if (!bridgeEnabledAtStartup.HasValue)
                throw new InvalidOperationException("Missing Stage B configuration snapshot from plugin initialization.");
            if (!bridgeEnabledAtStartup.Value)
                throw new InvalidOperationException("Stage B requires explicitly enabled own setting.");
            _prefabReads = new[] {
                typeof(ObjectDB).GetMethod("GetItemPrefab", new[] { typeof(string) }),
                typeof(ObjectDB).GetMethod("GetItemPrefab", new[] { typeof(int) }),
                typeof(ObjectDB).GetMethod("TryGetItemPrefab", new[] { typeof(int), typeof(UnityEngine.GameObject).MakeByRefType() })
            };
            foreach (var read in _prefabReads) RequirePlainRead(read);
            RequireFeatureActive();
            _descriptor = Compatibility.MagicPluginTomeBridge.ResolveTarget(
                supremacy.GetType("Magic_Supremacy.Magic_SupremacyPlugin", true), "IsTomeItem", typeof(ItemDrop.ItemData));
            var system = supremacy.GetType("Magic_Supremacy.CustomSlotSystem", true);
            _internal = Compatibility.MagicPluginTomeBridge.ResolveTarget(system,
                "IsItemValidForSlot", typeof(ItemDrop.ItemData), typeof(string));
            _registryGetter = system.GetMethod("GetSharedSlotRegistry", All, null, Type.EmptyTypes, null);
            if (_registryGetter == null || !_registryGetter.IsStatic || !_registryGetter.IsPrivate ||
                _registryGetter.ReturnType != typeof(Dictionary<string, HashSet<string>>))
                throw new InvalidOperationException("Unknown registry-helper contract.");
            foreach (var target in new[] { _descriptor, _internal })
            {
                RequireReviewed(target);
                var patch = Harmony.GetPatchInfo(target).Postfixes.Single();
                log("STAGE_B_PATCH gate=" + target.Name + " owner=" + patch.owner + " method=" + patch.PatchMethod.Name +
                    " priority=" + patch.priority + " index=" + patch.index +
                    " before=" + string.Join(",", patch.before ?? new string[0]) +
                    " after=" + string.Join(",", patch.after ?? new string[0]) + " otherInterceptors=0");
            }
            RequireReviewed(_registryGetter);
            log("STAGE_B_MODE configuration=True bridgeActive=True bothPostfixesVerified=True; acceptance-only; no equipment actions");
        }

        private static void RequireFeatureActive()
        {
            var type = typeof(Compatibility.MagicPluginTomeBridge);
            var bridge = type.GetField("_current", All).GetValue(null);
            if (bridge == null || !(bool)ReadProperty(type.GetField("_activation", All).GetValue(bridge), "Active"))
                throw new InvalidOperationException("Bridge initialization did not activate the complete pair.");
        }

        // Stage A remains strictly unpatched. This opt-in mode allows only these two exact postfixes.
        internal static void RequireReviewed(MethodInfo target)
        {
            var patches = Harmony.GetPatchInfo(target);
            bool gate = target == _descriptor || target == _internal;
            if (!gate)
            {
                RequirePlainRead(target);
                return;
            }
            foreach (var read in _prefabReads) RequirePlainRead(read);
            var expected = typeof(Compatibility.MagicPluginTomeBridge).GetMethod(
                target == _descriptor ? "DescriptorPostfix" : "EquipmentPostfix", All);
            if (patches == null || patches.Prefixes.Count != 0 || patches.Transpilers.Count != 0 ||
                patches.Finalizers.Count != 0 || patches.ILManipulators.Count != 0 || patches.Postfixes.Count != 1 ||
                patches.Postfixes[0].owner != Owner || patches.Postfixes[0].PatchMethod != expected)
                throw new InvalidOperationException("Unreviewed patch set/order on " + target.Name);
        }

        private static void RequirePlainRead(MethodInfo method)
        {
            if (method == null) throw new InvalidOperationException("Required read-helper signature missing.");
            var patches = Harmony.GetPatchInfo(method);
            if (patches != null && patches.Owners.Count > 0)
                throw new InvalidOperationException("Unaudited read-helper patches: " + method.DeclaringType.FullName + "." + method.Name);
        }

        internal static Dictionary<string, HashSet<string>> RequireInitializedRegistry()
        {
            RequireFeatureActive();
            if (_internal == null || _registryGetter == null) throw new InvalidOperationException("No audited gate bindings.");
            RequireReviewed(_descriptor); RequireReviewed(_internal); RequireReviewed(_registryGetter);
            var registry = AppDomain.CurrentDomain.GetData(RegistryKey) as Dictionary<string, HashSet<string>>;
            HashSet<string> native;
            if (registry == null || !registry.TryGetValue("tome", out native) || native == null || !native.Contains(Native))
                throw new InvalidOperationException("Registry not initialized; refusing potentially initializing validator.");
            // Pinned helper 0x0600044a returns this existing dictionary at IL_0018-0019.
            return registry;
        }

        internal static void CaptureInternal(ItemDrop.ItemData item, string prefab, Action<string> log)
        {
            var registry = RequireInitializedRegistry();
            string before = Fingerprint(registry);
            bool membership = registry["tome"].Contains(prefab);
            bool result = (bool)_internal.Invoke(null, new object[] { item, "tome" });
            // belt is intentionally a non-Tome identifier, not a claim of a registered MS Belt cell.
            bool beltMember = registry.ContainsKey("belt");
            HashSet<string> belt;
            bool otherExpected = registry.TryGetValue("belt", out belt) && belt != null && belt.Contains(prefab);
            bool other = (bool)_internal.Invoke(null, new object[] { item, "belt" });
            if (!ReferenceEquals(registry, AppDomain.CurrentDomain.GetData(RegistryKey)) || before != Fingerprint(registry))
                throw new InvalidOperationException("Unexpected registry drift during predicate observation; stop.");
            bool expected = Compatibility.TomeBridgePolicy.Supports(prefab) || prefab == Native;
            log("STAGE_B_INTERNAL prefab=" + prefab + " slot=tome result=" + result + " registryMember=" + membership +
                " unrelatedSlot=belt registeredInMS=" + beltMember + " unrelatedResult=" + other +
                " unrelatedExpected=" + otherExpected + " registryUnchanged=True");
            if (result != expected || other != otherExpected)
                throw new InvalidOperationException("Acceptance contradicts reviewed scope for " + prefab + "; no automatic repair.");
        }

        internal static void CheckVisible(string prefab, string result)
        {
            bool expected = Compatibility.TomeBridgePolicy.Supports(prefab) || prefab == Native;
            if (!string.Equals(result, Convert.ToString(expected, CultureInfo.InvariantCulture), StringComparison.Ordinal))
                throw new InvalidOperationException("Unexpected visible acceptance for " + prefab + ": " + result);
        }

        private static string Fingerprint(Dictionary<string, HashSet<string>> registry)
        {
            return string.Join("\n", registry.OrderBy(p => p.Key, StringComparer.Ordinal).Select(p =>
                p.Key + ":" + string.Join(",", p.Value.OrderBy(x => x, StringComparer.Ordinal))));
        }

        private static object ReadProperty(object instance, string name)
        {
            if (instance == null) throw new InvalidOperationException("Missing object for property " + name);
            var property = instance.GetType().GetProperty(name, All);
            if (property == null) throw new MissingMemberException(instance.GetType().FullName, name);
            return property.GetValue(instance, null);
        }
    }
}
#endif
