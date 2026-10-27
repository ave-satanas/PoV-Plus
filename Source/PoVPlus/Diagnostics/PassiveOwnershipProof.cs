#if POVPLUS_TOME_PROVENANCE_POC
// ARCH-003 offline experiment. No installer, Harmony attributes, polling, or game API calls.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace PoVPlus.Diagnostics.Provenance
{
    // Prospective object-valued postfix handoff, simulated by the fixture driver only.
    // The caller must independently authenticate original execution, owner and patch chain.
    internal sealed class ReturnedRoot : IDisposable
    {
        private WeakReference _owner, _root;
        private double _expires;
        internal bool Invalid { get; private set; }
        internal int Epoch { get; private set; }
        internal string OriginValidity { get { return "unknown"; } }
        internal void ObserveReturn(object owner, object result, bool callerReportsOriginal, double now)
        {
            try
            {
                if (Invalid) return;
                if (!callerReportsOriginal || owner == null || result == null || !Finite(now))
                { Invalidate(); return; }
                if (_root != null)
                {
                    if (now >= _expires || !ReferenceEquals(_owner.Target, owner) ||
                        !ReferenceEquals(_root.Target, result)) Invalidate();
                    return; // Duplicate captures cannot extend the lifetime/window.
                }
                _owner = new WeakReference(owner); _root = new WeakReference(result);
                _expires = now + 60; Epoch++;
            }
            catch { Invalidate(); } // Recorder failure must not escape into the observed operation.
        }
        internal object Borrow(object owner, double now)
        {
            if (Invalid || _root == null || !Finite(now) || now < _expires - 60 || now >= _expires ||
                !ReferenceEquals(_owner.Target, owner)) { Invalidate(); return null; }
            var root = _root.Target;
            if (root == null) Invalidate();
            return root; // A caller holds a temporary strong reference only during bounded reading.
        }
        private static bool Finite(double n) { return n >= 0 && !double.IsNaN(n) && !double.IsInfinity(n); }
        private void Invalidate() { Invalid = true; _owner = null; _root = null; }
        public void Dispose() { Invalidate(); }
    }

    internal sealed class Snapshot
    {
        internal readonly bool LocalReadValid, EndpointsAgree, HasTome, Flag, BindingAgrees;
        internal readonly int OwnerAlias, TrackedAlias, Count;
        internal readonly string Reason;
        internal readonly object ReaderIdentity; // Observer-owned scope marker, never a game reference or coverage proof.
        internal string Continuity { get { return "unknown"; } }
        internal string AuthoritativeState { get { return "unknown"; } }
        internal string Atomicity { get { return "unknown"; } }
        internal string OriginValidity { get { return "unknown"; } }
        internal Snapshot(bool valid, bool hasTome, bool flag, bool binding, int ownerAlias,
            int trackedAlias, int count, object readerIdentity, string reason)
        { LocalReadValid = valid; EndpointsAgree = valid; HasTome = hasTome; Flag = flag;
          BindingAgrees = binding; OwnerAlias = ownerAlias; TrackedAlias = trackedAlias;
          Count = count; ReaderIdentity = readerIdentity; Reason = reason; }
        internal static Snapshot Unknown(string reason) { return new Snapshot(false, false, false, false, 0, 0, 0, null, reason); }
        internal bool SampledValuesAgree { get { return LocalReadValid && EndpointsAgree && BindingAgrees &&
            (HasTome ? OwnerAlias == TrackedAlias && Flag : !Flag); } }
    }

    // FieldInfos must come from the pinned metadata contract, never inferred from a getter name.
    // No static fields/properties, arbitrary IEnumerable implementations, or method Invoke allowed.
    internal sealed class ExistingStateReader
    {
        private readonly Type _rootType, _ownerType, _itemType;
        private readonly FieldInfo _map, _flag, _itemData, _bindings;
        private readonly List<WeakReference> _aliases = new List<WeakReference>();
        private readonly object _identity = new object();
        private WeakReference _rootSeen, _ownerSeen;
        private bool _exhausted;
#if POVPLUS_PROVENANCE_FIXTURE
        internal Action DuringRead; // Deterministic adversary; absent from every plugin artifact.
#endif
        internal ExistingStateReader(Type root, Type owner, Type item, FieldInfo map, FieldInfo flag,
            FieldInfo itemData, FieldInfo bindings)
        {
            _rootType = root; _ownerType = owner; _itemType = item;
            _map = Check(map, root); _flag = Check(flag, item);
            _itemData = Check(itemData, item); _bindings = Check(bindings, owner);
            if (_flag.FieldType != typeof(bool) || _itemData.FieldType != typeof(Dictionary<string, string>) ||
                _bindings.FieldType != typeof(Dictionary<string, string>) ||
                _map.FieldType != typeof(Dictionary<,>).MakeGenericType(typeof(string), item))
                throw new ArgumentException("Unaudited field layout");
        }
        private static FieldInfo Check(FieldInfo field, Type owner)
        {
            if (field == null || field.IsStatic || field.DeclaringType != owner)
                throw new ArgumentException("Only exact declaring-type instance fields permitted");
            return field;
        }
        private int Alias(object item)
        {
            if (item == null) return 0;
            for (int i = 0; i < _aliases.Count; i++) if (ReferenceEquals(_aliases[i].Target, item)) return i + 1;
            if (_aliases.Count == 64) { _exhausted = true; throw new InvalidOperationException("Alias budget exhausted"); }
            _aliases.Add(new WeakReference(item)); return _aliases.Count;
        }
        // Validate before Count/GetEnumerator: a declared Dictionary field can hold a subclass
        // that explicitly reimplements IDictionary. Exact type restricts dispatch to audited BCL.
        // This proves neither atomicity nor absence of runtime detours/concurrent mutation.
        private static int StorageCount(object value, Type exactType)
        {
            // Enumeration scans occupied-plus-free slots, not public Count. Reject unsupported
            // BCL layouts rather than assuming a sparse Dictionary is cheap to enumerate.
            var field = exactType.GetField("_count", BindingFlags.Instance | BindingFlags.NonPublic) ??
                exactType.GetField("count", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null || field.IsStatic || field.DeclaringType != exactType || field.FieldType != typeof(int))
                throw new InvalidOperationException("Unsupported BCL dictionary storage layout");
            int used = (int)field.GetValue(value);
            if (used < 0 || used > 64) throw new InvalidOperationException("Dictionary traversal budget exhausted");
            return used;
        }
        private static Dictionary<string, object> Copy(object value, Type exactType)
        {
            if (value == null || value.GetType() != exactType)
                throw new InvalidOperationException("Unsupported concrete collection type");
            var map = (IDictionary)value;
            int storage = StorageCount(value, exactType);
            int count = map.Count;
            if (count < 0 || count > storage) throw new InvalidOperationException("Oversized collection");
            var copy = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (DictionaryEntry pair in map)
            {
                if (copy.Count == 64) throw new InvalidOperationException("Collection changed or budget exhausted");
#if POVPLUS_PROVENANCE_FIXTURE
                if (EnumerationStep != null) EnumerationStep();
#endif
                var key = pair.Key as string;
                if (key == null || key.Length > 128 || (pair.Value is string && ((string)pair.Value).Length > 4096))
                    throw new InvalidOperationException("Collection string budget exhausted");
                copy.Add(key, pair.Value);
            }
            if (StorageCount(value, exactType) != storage || copy.Count != count || map.Count != count)
                throw new InvalidOperationException("Collection count changed");
            return copy;
        }
#if POVPLUS_PROVENANCE_FIXTURE
        internal static Action EnumerationStep; // Fixture-only deterministic concurrent writer.
#endif
        private static bool Same(Dictionary<string, object> a, Dictionary<string, object> b, bool references)
        {
            if (a.Count != b.Count) return false;
            foreach (var p in a)
            { object v; if (!b.TryGetValue(p.Key, out v) || (references ? !ReferenceEquals(v, p.Value) : !Equals(v, p.Value))) return false; }
            return true;
        }
        internal Snapshot Read(object root, object owner, object tracked, bool callerReportsReady,
            bool callerReportsQuiescent)
        {
            if (_exhausted) return Snapshot.Unknown("Alias budget exhausted");
            if (root == null || owner == null || tracked == null || root.GetType() != _rootType ||
                owner.GetType() != _ownerType || tracked.GetType() != _itemType)
                return Snapshot.Unknown("Missing/invalid root, owner or tracked instance");
            // Negative readiness gates only: true values never authenticate initialization,
            // atomicity, origin, or continuity. Those claims remain unknown.
            if (!callerReportsReady || !callerReportsQuiescent)
                return Snapshot.Unknown("Caller readiness preconditions unavailable; no local read attempted");
            try
            {
                if (_rootSeen == null)
                { _rootSeen = new WeakReference(root); _ownerSeen = new WeakReference(owner); }
                else if (!ReferenceEquals(_rootSeen.Target, root) || !ReferenceEquals(_ownerSeen.Target, owner))
                    return Snapshot.Unknown("Reader root/owner changed or lost; new observation scope required");
                var map = _map.GetValue(root);
                var itemData = _itemData.GetValue(tracked);
                var bindings = _bindings.GetValue(owner);
                var a = Copy(map, _map.FieldType); var b = Copy(itemData, _itemData.FieldType); var c = Copy(bindings, _bindings.FieldType);
                bool flag = (bool)_flag.GetValue(tracked);
#if POVPLUS_PROVENANCE_FIXTURE
                if (DuringRead != null) DuringRead();
#endif
                if (!ReferenceEquals(map, _map.GetValue(root)) || !ReferenceEquals(itemData, _itemData.GetValue(tracked)) ||
                    !ReferenceEquals(bindings, _bindings.GetValue(owner)) || flag != (bool)_flag.GetValue(tracked) ||
                    !Same(a, Copy(map, _map.FieldType), true) || !Same(b, Copy(itemData, _itemData.FieldType), false) || !Same(c, Copy(bindings, _bindings.FieldType), false))
                    return Snapshot.Unknown("Collection/flag changed or interrupted");
                foreach (var p in a) if (p.Value == null || p.Value.GetType() != _itemType)
                    return Snapshot.Unknown("Incomplete ownership entry");
                object tome, guid, saved;
                bool has = a.TryGetValue("tome", out tome);
                bool hasGuid = b.TryGetValue("DO_CustomSlotGuid", out guid);
                bool hasSaved = c.TryGetValue("DO_Equipped_tome", out saved);
                bool match = has ? hasGuid && hasSaved && guid is string &&
                    !string.IsNullOrEmpty((string)guid) && Equals(guid, saved) : !hasSaved;
                return new Snapshot(true, has, flag, match, Alias(tome), Alias(tracked), a.Count, _identity,
                    has ? "Observed MS entry; sampled endpoints agree; continuity and authority unknown" :
                    "Sampled map has no Tome key; NOT proof of initial emptiness, continuity or rollback safety");
            }
            catch (Exception error) { return Snapshot.Unknown("Read rejected: " + error.GetType().Name); }
        }
    }

    internal enum Kind { Equip, Unequip, Swap, Restore, Gap }
    internal enum Phase { Begin, Return, Fault }
    internal sealed class Transition
    {
        internal readonly string Source, Transaction, Provenance;
        internal readonly long Sequence;
        internal readonly Kind Kind;
        internal readonly Phase Phase;
        internal readonly int ItemAlias;
        internal readonly Snapshot State;
        internal Transition(string source, long sequence, string transaction, Kind kind, Phase phase,
            int itemAlias, Snapshot state, string provenance)
        { Source = source; Sequence = sequence; Transaction = transaction; Kind = kind; Phase = phase;
          ItemAlias = itemAlias; State = state; Provenance = provenance; }
    }

    // One bounded outer operation at a time. Nested/swap intermediates intentionally invalidate
    // this minimal ledger rather than pretending to reconstruct a full transaction stack.
    internal sealed class ProvenanceLedger
    {
        private long _sequence;
        private int _count;
        private readonly List<Transition> _records = new List<Transition>(32);
        internal Transition[] Observations { get { return _records.ToArray(); } }
        private Transition _pending;
        private Snapshot _current;
        private bool _invalid;
        internal string Classification { get; private set; } = "UnknownObservation";
        internal string Reason { get; private set; } = "No local samples";
        // This component has no independent source for any of these claims. Caller assertions
        // and record numbering cannot create one. Keep the uncertainty visible on every path.
        internal bool HistoryComplete { get { return false; } }
        internal string TransitionCoverage { get { return "unknown"; } }
        internal string Continuity { get { return "unknown"; } }
        internal string InitialStateValidity { get { return "unknown"; } }
        internal string NeutralState { get { return "unknown"; } }
        internal string RollbackSafety { get { return "unknown"; } }
        internal void Baseline(Snapshot snapshot, bool claimedInitial, bool claimedCoverage)
        {
            if (_count != 0 || _current != null) { Fail("Baseline replacement requires a new ledger/epoch"); return; }
            _current = snapshot;
            Classification = snapshot != null && snapshot.LocalReadValid ? "LocalSnapshotOnly" : "UnknownObservation";
            Reason = "Unverified caller initial=" + claimedInitial + " coverage=" + claimedCoverage +
                "; local samples cannot establish initial state or mutation coverage";
        }
        private static bool SameObservation(Snapshot a, Snapshot b)
        {
            return a.LocalReadValid == b.LocalReadValid && a.EndpointsAgree == b.EndpointsAgree &&
                a.HasTome == b.HasTome && a.OwnerAlias == b.OwnerAlias && a.TrackedAlias == b.TrackedAlias &&
                a.Flag == b.Flag && a.BindingAgrees == b.BindingAgrees && a.Count == b.Count;
        }
        private void Fail(string reason) { _invalid = true; Classification = "Incomplete"; Reason = reason; _pending = null; }
        internal void Add(Transition e)
        {
            if (_invalid) return;
            if (++_count > 32) { Fail("Evidence budget exhausted"); return; }
            if (e == null || e.Source != "MS" || e.Sequence != _sequence + 1 ||
                string.IsNullOrEmpty(e.Transaction) || e.Transaction.Length > 96 ||
                string.IsNullOrEmpty(e.Provenance) || e.Provenance.Length > 160 ||
                e.ItemAlias <= 0 || e.State == null || e.ItemAlias != e.State.TrackedAlias ||
                e.Kind < Kind.Equip || e.Kind > Kind.Gap || e.Phase < Phase.Begin || e.Phase > Phase.Fault)
            { Fail("Missing/duplicate/reordered/untrusted evidence"); return; }
            if (_current == null || _current.ReaderIdentity == null ||
                !ReferenceEquals(_current.ReaderIdentity, e.State.ReaderIdentity))
            { Fail("Different or missing observation context"); return; }
            _sequence = e.Sequence;
            _records.Add(e); // Bounded immutable local records, never evidence of source completeness.
            if (e.Kind == Kind.Gap) { Fail("Observation discontinuity"); return; }
            if (e.Phase == Phase.Fault) { Fail("Partial transaction after exception"); return; }
            if (e.Phase == Phase.Begin)
            {
                if (_pending != null) { Fail("Nested/swap intermediate unsupported"); return; }
                if (e.State == null || !e.State.SampledValuesAgree || (_current != null &&
                    !SameObservation(_current, e.State)))
                { Fail("Unrecorded state delta or contradictory pre-state"); return; }
                _pending = e; Classification = e.Kind + "Attempted"; Reason = "Entry is not completion"; return;
            }
            if (_pending == null || _pending.Transaction != e.Transaction || _pending.Kind != e.Kind ||
                _pending.ItemAlias != e.ItemAlias || e.State == null || !e.State.SampledValuesAgree)
            { Fail("Unpaired/contradictory completion"); return; }
            if ((e.Kind == Kind.Equip || e.Kind == Kind.Restore || e.Kind == Kind.Swap) &&
                (!e.State.HasTome || e.State.OwnerAlias != e.ItemAlias))
            { Fail("Return lacks expected sampled ownership postcondition"); return; }
            if (e.Kind == Kind.Unequip && (e.State.HasTome || e.State.Flag))
            { Fail("Return lacks expected sampled removal postcondition"); return; }
            bool changed = _current != null && !SameObservation(_current, e.State);
            _current = e.State; _pending = null;
            Classification = !changed ? "NoChangeInRecordedProjection" : e.Kind + "EndpointAgreement";
            Reason = "Sampled values only; continuity/coverage/initial state unknown; no persistence, neutral or rollback claim";
        }
        internal void Seal()
        { if (_pending != null) Fail("Interrupted operation"); }
    }
}
#endif
