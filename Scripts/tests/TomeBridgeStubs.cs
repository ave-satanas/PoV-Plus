// Test doubles only. No game/BepInEx/Harmony assemblies are loaded by the harness.
using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine
{
    public class Object
    {
        public string name;
        public static implicit operator bool(Object value) { return !ReferenceEquals(value, null); }
    }
    public class Component : Object { }
    public class GameObject : Object
    {
        public Component Component;
        public T GetComponent<T>() where T : Component { return Component as T; }
    }
}
public class Player { }
public class StatusEffect : UnityEngine.Object { }
public class ItemDrop : UnityEngine.Component
{
    public ItemData m_itemData;
    public class ItemData
    {
        public UnityEngine.GameObject m_dropPrefab;
        public SharedData m_shared;
    }
    public class SharedData { public StatusEffect m_equipStatusEffect; }
}
public class ObjectDB : UnityEngine.Object
{
    public static ObjectDB instance;
    public readonly Dictionary<string, UnityEngine.GameObject> Items = new Dictionary<string, UnityEngine.GameObject>();
    public bool FailLookup;
    public UnityEngine.GameObject GetItemPrefab(string id)
    {
        if (FailLookup) throw new InvalidOperationException("fixture lookup failure");
        UnityEngine.GameObject value; return Items.TryGetValue(id, out value) ? value : null;
    }
    public UnityEngine.GameObject GetItemPrefab(int hash) { return null; }
    public bool TryGetItemPrefab(int hash, out UnityEngine.GameObject value) { value = null; return false; }
}
namespace BepInEx.Logging
{
    public class ManualLogSource
    {
        public readonly List<string> Warnings = new List<string>();
        public readonly List<string> Infos = new List<string>();
        public void LogWarning(object value) { Warnings.Add(value.ToString()); }
        public void LogInfo(object value) { Infos.Add(value.ToString()); }
    }
}
namespace BepInEx.Bootstrap
{
    public class PluginInfo { public UnityEngine.Object Instance; }
    public static class Chainloader
    {
        public static readonly Dictionary<string, PluginInfo> PluginInfos = new Dictionary<string, PluginInfo>();
    }
}
namespace HarmonyLib
{
    public enum HarmonyPatchType { All }
    public class HarmonyMethod { public HarmonyMethod(MethodInfo method) { } }
    public class Patch
    {
        public string owner;
        public MethodInfo PatchMethod;
        public int priority = 400;
        public int index;
        public string[] before = new string[0], after = new string[0];
    }
    public class Patches
    {
        public readonly List<string> Owners = new List<string>();
        public readonly List<Patch> Postfixes = new List<Patch>();
        public readonly List<Patch> Prefixes = new List<Patch>(), Transpilers = new List<Patch>(),
            Finalizers = new List<Patch>(), ILManipulators = new List<Patch>();
    }
    public class Harmony
    {
        public static int Constructed;
        public static readonly Dictionary<MethodBase, Patches> TestPatches = new Dictionary<MethodBase, Patches>();
        public Harmony(string id) { Constructed++; }
        public static Patches GetPatchInfo(MethodBase method)
        {
            Patches value; return TestPatches.TryGetValue(method, out value) ? value : null;
        }
        public void Patch(MethodInfo target, HarmonyMethod postfix) { }
        public void Unpatch(MethodInfo target, HarmonyPatchType type, string owner) { }
    }
}
