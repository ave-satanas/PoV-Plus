using BepInEx;

namespace PoVPlus
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "povplus.core";
        public const string PluginName = "PoVPlus";
        public const string PluginVersion = "0.1.0";

        private void Awake()
        {
            Logger.LogInfo(PluginName + " " + PluginVersion + " loaded.");
#if POVPLUS_TOME_DIAGNOSTIC
            _tomeDiagnostic = new Diagnostics.TomeDiagnostic(Logger);
#endif
        }

#if POVPLUS_TOME_DIAGNOSTIC
        private Diagnostics.TomeDiagnostic _tomeDiagnostic;
        private void Update() { _tomeDiagnostic.Poll(); }
#endif
    }
}
