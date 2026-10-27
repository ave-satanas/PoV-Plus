#if POVPLUS_TOME_LIFECYCLE_DIAGNOSTIC
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace PoVPlus.Diagnostics
{
    // Pure evidence/policy code. All collections allocated here belong to this observer.
    internal sealed class LifecycleEvidence
    {
        private sealed class Value
        {
            internal string Status, Text, Source, Reason;
        }
        private readonly SortedDictionary<string, Value> _fields = new SortedDictionary<string, Value>(StringComparer.Ordinal);
        internal void Known(string key, string text, string source) { Put(key, "observed", text, source, null); }
        internal void Empty(string key, string source) { Put(key, "empty", null, source, null); }
        internal void Occupied(string key, string text, string source) { Put(key, "occupied", text, source, null); }
        internal void Unknown(string key, string source, string reason) { Put(key, "unknown", null, source, reason); }
        private void Put(string key, string status, string text, string source, string reason)
        {
            if (string.IsNullOrEmpty(source) || (status == "unknown" && string.IsNullOrEmpty(reason)))
                throw new ArgumentException("Evidence requires provenance and an unknown reason.");
            _fields.Add(key, new Value { Status = status, Text = text, Source = source, Reason = reason });
        }
        internal string Serialize(bool equipmentOnly = false)
        {
            var s = new StringBuilder("{"); bool first = true;
            foreach (var pair in _fields)
            {
                if (equipmentOnly && (pair.Key == "eitr.current" || pair.Key == "eitr.regenTimer")) continue;
                if (!first) s.Append(','); first = false;
                s.Append(Json(pair.Key)).Append(":{\"status\":").Append(Json(pair.Value.Status))
                    .Append(",\"value\":").Append(Json(pair.Value.Text)).Append(",\"source\":").Append(Json(pair.Value.Source))
                    .Append(",\"reason\":").Append(Json(pair.Value.Reason)).Append('}');
            }
            return s.Append('}').ToString();
        }
        internal static string Json(string value)
        {
            if (value == null) return "null";
            var s = new StringBuilder("\"");
            foreach (char c in value)
            {
                if (c == '"' || c == '\\') s.Append('\\').Append(c);
                else if (char.IsControl(c) || char.IsSurrogate(c)) s.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                else s.Append(c);
            }
            return s.Append('"').ToString();
        }
        internal static string Number(object value) { return Convert.ToString(value, CultureInfo.InvariantCulture); }
    }

    internal static class LifecycleRead
    {
        internal static object Field(Type type, string name, object instance)
        {
            for (var t = type; t != null; t = t.BaseType)
            {
                var f = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                if (f != null)
                {
                    if (f.IsStatic) throw new InvalidOperationException("Static field requires audited initialization gate: " + name);
                    return f.GetValue(instance);
                }
            }
            throw new MissingFieldException(type.FullName, name);
        }
        internal static object ExistingEntry(object table, object key)
        {
            if (table == null || key == null) throw new InvalidOperationException("Existing runtime table/key unavailable.");
            var t = table.GetType();
            if (!t.IsGenericType || t.GetGenericTypeDefinition().FullName != "System.Runtime.CompilerServices.ConditionalWeakTable`2")
                throw new InvalidOperationException("Unexpected runtime table contract.");
            var args = new[] { key, (object)null };
            var method = t.GetMethod("TryGetValue", new[] { t.GetGenericArguments()[0], t.GetGenericArguments()[1].MakeByRefType() });
            if (method == null) throw new InvalidOperationException("Non-creating TryGetValue unavailable.");
            if (!(bool)method.Invoke(table, args)) throw new InvalidOperationException("No existing runtime entry; not evidence of an empty slot.");
            return args[1];
        }
    }

    internal sealed class LifecycleWindow
    {
        internal const int MaxSamples = 120;
        internal const double MaxSeconds = 600;
        private double? _start;
        private double _last = -1, _next;
        private string _previous, _lastPayload, _lastPhase;
        private double _lastEmission;
        private int _sequence;
        internal bool Finished { get; private set; }
        internal string StopReason { get; private set; }
        internal string Sample(double now, bool ready, string character, string world, Func<LifecycleEvidence> capture, string utc)
        {
            if (Finished) return null;
            if (double.IsNaN(now) || double.IsInfinity(now) || now < 0 || now < _last) { Stop("InvalidMonotonicClock"); return null; }
            _last = now;
            if (!_start.HasValue) _start = now;
            if (now - _start.Value >= MaxSeconds || _sequence >= MaxSamples) { Stop("CollectionBudgetReached"); return null; }
            if (now < _next) return null;
            _next = now + 1;
            if (!ready) return null;
            if (character != DiagnosticSession.ExpectedCharacter || world != DiagnosticSession.ExpectedWorld)
            { Stop("IdentityRejected expected=PoVPlus_Test/PoVPlus_Lab character=" + LifecycleEvidence.Json(character) + " world=" + LifecycleEvidence.Json(world)); return null; }
            // Every callback is preceded by the exact identity check; no cross-world observer reset.
            try
            {
                var evidence = capture(); var structure = evidence.Serialize(true);
                string phase = _previous == null ? "initial" : structure == _previous ? "stable-sample" : "transition";
                _previous = structure;
                string payload = evidence.Serialize();
                if (Encoding.UTF8.GetByteCount(payload) > 65536) { Stop("EvidenceSizeLimit"); return null; }
                // Bound even an idle or continuously changing session. Unchanged evidence has
                // only a ten-second heartbeat; identity is still revalidated before every read.
                if (payload == _lastPayload && phase == _lastPhase && now - _lastEmission < 10) return null;
                _lastPayload = payload; _lastPhase = phase; _lastEmission = now;
                _sequence++;
                return "{\"schema\":1,\"sequence\":" + _sequence.ToString(CultureInfo.InvariantCulture) + ",\"utc\":" + LifecycleEvidence.Json(utc) +
                    ",\"phase\":" + LifecycleEvidence.Json(phase) + ",\"fields\":" + payload + "}";
            }
            catch (Exception error) { Stop("ObservationFailure " + error.GetType().Name + ": " + error.Message); return null; }
        }
        internal void Stop(string reason) { Finished = true; StopReason = reason; }
    }
}
#endif
