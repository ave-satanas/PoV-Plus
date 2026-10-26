// Test-only MS semantics and explicit managed postfix simulation, not real Harmony detours.
using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace Magic_Supremacy
{
    public static class FakeDispatch
    {
        internal static bool Run(MethodInfo method, object[] args)
        {
            var patches = Harmony.GetPatchInfo(method);
            if (patches != null)
                foreach (var patch in patches.Postfixes) patch.PatchMethod.Invoke(null, args);
            return (bool)args[args.Length - 1];
        }
    }
    public class Magic_SupremacyPlugin
    {
        private static bool IsTomeItem(ItemDrop.ItemData item)
        {
            bool result = item != null && item.m_dropPrefab && item.m_dropPrefab.name == "ArmorMushroomcallerTome_DO";
            return FakeDispatch.Run((MethodInfo)MethodBase.GetCurrentMethod(), new object[] { item, result });
        }
    }
    public class CustomSlotSystem
    {
        public static int Initializations;
        private static Dictionary<string, HashSet<string>> GetSharedSlotRegistry()
        {
            var existing = AppDomain.CurrentDomain.GetData("DO_CustomSlotSystem_SlotPrefabs") as Dictionary<string, HashSet<string>>;
            if (existing != null) return existing;
            Initializations++;
            var created = new Dictionary<string, HashSet<string>>();
            AppDomain.CurrentDomain.SetData("DO_CustomSlotSystem_SlotPrefabs", created);
            return created;
        }
        private static bool IsItemValidForSlot(ItemDrop.ItemData item, string slot)
        {
            bool result = false;
            if (item != null && item.m_dropPrefab && !string.IsNullOrWhiteSpace(slot))
            {
                HashSet<string> allowed;
                result = GetSharedSlotRegistry().TryGetValue(slot, out allowed) && allowed.Contains(item.m_dropPrefab.name);
            }
            return FakeDispatch.Run((MethodInfo)MethodBase.GetCurrentMethod(), new object[] { item, slot, result });
        }
    }
}
