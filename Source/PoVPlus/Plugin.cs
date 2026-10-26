using BepInEx;

namespace PoVPlus
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("blacks7ar.MagicPlugin", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Dreanegade.Magic_Supremacy", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("seneaL.valheim.api", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("seneaL.valheim.ui", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("treextr.pathofvalheiman", BepInDependency.DependencyFlags.SoftDependency)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "povplus.core";
        public const string PluginName = "PoVPlus";
        public const string PluginVersion = "0.1.0";

        private void Awake()
        {
            Logger.LogInfo(PluginName + " " + PluginVersion + " loaded.");
            var bridge = Config.Bind(Compatibility.TomeBridgePolicy.Section, Compatibility.TomeBridgePolicy.Key,
                Compatibility.TomeBridgePolicy.DefaultEnabled,
                "Experimental acceptance-only bridge. Default Off; read at startup. Requires supervised testing. Unequip books before disabling/removing; restart required.");
            new Compatibility.MagicPluginTomeBridge(Logger).Start(bridge.Value);
#if POVPLUS_TOME_DIAGNOSTIC
            _tomeDiagnostic = new Diagnostics.TomeDiagnostic(Logger, bridge.Value);
#endif
        }

#if POVPLUS_TOME_DIAGNOSTIC
        private Diagnostics.TomeDiagnostic _tomeDiagnostic;
        private void Update() { _tomeDiagnostic.Poll(); }
#endif
    }
}
