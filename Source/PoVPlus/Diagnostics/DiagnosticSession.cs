using System;
using System.Text;

namespace PoVPlus.Diagnostics
{
    // Unity-independent policy. Exact ordinal matching is the only authorization rule.
    internal sealed class DiagnosticObservation
    {
        internal bool HasPlayer, PlayerReady, WorldReady, InventoryReady, DescriptorsReady, PrefabsReady;
        internal string Character, World, MissingDetail;
    }

    internal sealed class DiagnosticSession
    {
        internal const string ExpectedCharacter = "PoVPlus_Test";
        internal const string ExpectedWorld = "POVPlus_Lab";
        private readonly Action<string> _log;
        private readonly double _timeout;
        private double? _started;
        private double _lastTime = -1;
        private string _lastState;
        private int _messages;
        internal bool Finished { get; private set; }
        internal bool Captured { get; private set; }

        internal DiagnosticSession(Action<string> log, double timeout = 180)
        {
            if (log == null || timeout <= 0 || double.IsNaN(timeout) || double.IsInfinity(timeout))
                throw new ArgumentException("A logger and finite positive timeout are required.");
            _log = log;
            _timeout = timeout;
        }

        internal static bool Authorized(DiagnosticObservation value)
        {
            return value.Character == ExpectedCharacter && value.World == ExpectedWorld;
        }

        internal void Tick(double now, DiagnosticObservation value, Action capture)
        {
            if (Finished) return;
            if (double.IsNaN(now) || double.IsInfinity(now) || now < 0 || now < _lastTime)
            { Abort("InvalidMonotonicClock"); return; }
            _lastTime = now;
            if (value.HasPlayer && !_started.HasValue) _started = now;
            var unavailable = !Available(value.Character) || !Available(value.World);
            string state;
            if (!value.HasPlayer) state = "WaitingForLocalPlayer";
            else if (!value.PlayerReady || !value.WorldReady) state = "WaitingForInitializedSession";
            else if (unavailable) state = "WaitingForIdentifiers";
            else if (!value.InventoryReady) state = "WaitingForInventory";
            else if (!Authorized(value)) state = "IdentityMismatch";
            else if (!value.DescriptorsReady) state = "WaitingForDescriptors";
            else if (!value.PrefabsReady) state = "WaitingForPrefabs";
            else state = "Ready";

            // Names are the only observed identity data. No player IDs, filenames, or paths.
            var evidence = "state=" + state +
                " characterApi=Player.GetPlayerName expectedCharacter=" + Quote(ExpectedCharacter) +
                " observedCharacter=" + Quote(value.Character) + Comparison("character", value.Character, ExpectedCharacter) +
                " worldApi=ZNet.GetWorldName expectedWorld=" + Quote(ExpectedWorld) +
                " observedWorld=" + Quote(value.World) + Comparison("world", value.World, ExpectedWorld) +
                " detail=" + Quote(value.MissingDetail);
            if (evidence != _lastState)
            {
                _lastState = evidence;
                if (++_messages > 24) { Abort("InitializationFlappingLimit"); return; }
                _log("GUARD " + evidence);
            }
            if (state == "IdentityMismatch")
            {
                Abort("IdentityMismatch characterExact=" + (value.Character == ExpectedCharacter) +
                    " worldExact=" + (value.World == ExpectedWorld) + "; no snapshot authorized");
                return;
            }
            if (_started.HasValue && now - _started.Value >= _timeout)
            { Abort("InitializationTimeout pending=" + state + " detail=" + Quote(value.MissingDetail)); return; }
            if (state != "Ready") return;
            Finished = true; // At most one attempt, including a failed callback.
            try { capture(); Captured = true; _log("COMPLETE snapshot=true observationOnly=true"); }
            catch (Exception error) { _log("ABORT SnapshotException " + error.GetType().Name + ": " + Quote(error.Message)); }
        }

        internal void Abort(string reason)
        {
            if (Finished) return;
            Finished = true;
            _log("ABORT " + reason);
        }

        private static bool Available(string name) { return !string.IsNullOrWhiteSpace(name) && name != "..."; }
        private static string Comparison(string label, string actual, string expected)
        {
            return " " + label + "Available=" + Available(actual) + " " + label + "Exact=" + (actual == expected) +
                " " + label + "CaseOnly=" + (actual != expected && string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase));
        }

        internal static string Quote(string text)
        {
            if (text == null) return "<unavailable>";
            var result = new StringBuilder("\"");
            foreach (char c in text.Length > 96 ? text.Substring(0, 96) : text)
            {
                if (c == '"' || c == '\\') result.Append('\\').Append(c);
                else if (char.IsControl(c) || char.IsSurrogate(c)) result.Append("\\u").Append(((int)c).ToString("x4"));
                else result.Append(c);
            }
            if (text.Length > 96) result.Append("<truncated>");
            return result.Append('"').ToString();
        }
    }
}
