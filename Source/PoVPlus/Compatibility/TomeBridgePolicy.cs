using System;

namespace PoVPlus.Compatibility
{
    // Pure policy shared with the offline harness. Never normalize prefab or slot identifiers.
    internal static class TomeBridgePolicy
    {
        internal const bool DefaultEnabled = false;
        internal const string Section = "Compatibility.MagicSupremacy";
        internal const string Key = "EnableMagicPluginTomeBridge";

        internal static bool Supports(string prefab)
        {
            return prefab == "BMP_BeginnersMagicBook" || prefab == "BMP_AdvanceMagicBook" ||
                prefab == "BMP_DruidsTome";
        }

        internal static string EffectFor(string prefab)
        {
            switch (prefab)
            {
                case "BMP_BeginnersMagicBook": return "bmp_beginners_magicbook";
                case "BMP_AdvanceMagicBook": return "bmp_advance_magicbook";
                case "BMP_DruidsTome": return "bmp_druidstome";
                default: return null;
            }
        }

        internal static bool Extend(bool original, bool active, string prefab, string slot, bool valid)
        {
            return original || (active && valid && slot == "tome" && Supports(prefab));
        }
    }

    // Installation is staged while inactive. Even unsuccessful cleanup leaves passive postfixes.
    internal sealed class BridgeActivation
    {
        private readonly Action<string> _warn;
        private volatile bool _active;
        private bool _attempted;
        internal bool Active { get { return _active; } }

        internal BridgeActivation(Action<string> warn) { _warn = warn; }

        internal bool Start(bool requested, Action validate, Action<int> install, Action verify, Action cleanup)
        {
            if (_attempted) throw new InvalidOperationException("Bridge initialization is startup-only.");
            _attempted = true;
            if (!requested) return false;
            try
            {
                validate();
                install(0);
                install(1);
                verify();
                _active = true;
                return true;
            }
            catch (Exception error)
            {
                _active = false;
                _warn("Bridge inactive: " + error.GetType().Name + ": " + error.Message);
                try { cleanup(); }
                catch (Exception cleanupError)
                {
                    _warn("Owner-only patch cleanup failed; remaining postfixes are inactive: " +
                        cleanupError.GetType().Name + ": " + cleanupError.Message);
                }
                return false;
            }
        }

        internal void Stop(string reason)
        {
            if (!_active) return;
            _active = false;
            _warn("Both bridge extensions disabled until restart: " + reason);
        }
    }
}
