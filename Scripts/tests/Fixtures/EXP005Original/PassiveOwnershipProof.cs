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
        internal void ObserveReturn(object owner, object result, bool authenticatedOriginal, double now)
        {
            try
            {
                if (Invalid) return;
                if (!authenticatedOriginal || owner == null || result == null || !Finite(now))
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
        internal readonly bool Complete, HasTome, Flag, BindingAgrees;
        internal readonly int OwnerAlias, TrackedAlias, Count;
        internal readonly string Reason;
        internal Snapshot(bool complete, bool hasTome, bool flag, bool binding, int ownerAlias,
            int trackedAlias, int count, string reason)
        { Complete = complete; HasTome = hasTome; Flag = flag; BindingAgrees = binding;
          OwnerAlias = ownerAlias; TrackedAlias = trackedAlias; Count = count; Reason = reason; }
        internal static Snapshot Unknown(string reason) { return new Snapshot(false, false, false, false, 0, 0, 0, reason); }
        internal bool Agrees { get { return Complete && BindingAgrees &&
            (HasTome ? OwnerAlias == TrackedAlias && Flag : !Flag); } }
    }

    // FieldInfos must come from the pinned metadata contract, never inferred from a getter name.
    // No static fields/properties, arbitrary IEnumerable implementations, or method Invoke allowed.
    internal sealed class ExistingStateReader
    {
        private readonly Type _rootType, _ownerType, _itemType;
        private readonly FieldInfo _map, _flag, _itemData, _bindings;
        private readonly List<WeakReference> _aliases = new List<WeakReference>();
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
        private static Dictionary<string, object> Copy(IDictionary map)
        {
            if (map == null || map.Count > 64) throw new InvalidOperationException("Missing/oversized collection");
            var copy = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (DictionaryEntry pair in map) copy.Add((string)pair.Key, pair.Value);
            return copy;
        }
        private static bool Same(Dictionary<string, object> a, Dictionary<string, object> b, bool references)
        {
            if (a.Count != b.Count) return false;
            foreach (var p in a)
            { object v; if (!b.TryGetValue(p.Key, out v) || (references ? !ReferenceEquals(v, p.Value) : !Equals(v, p.Value))) return false; }
            return true;
        }
        internal Snapshot Read(object root, object owner, object tracked, bool initialized,
            bool externallyQuiescent)
        {
            if (_exhausted) return Snapshot.Unknown("Alias budget exhausted");
            if (root == null || owner == null || tracked == null || root.GetType() != _rootType ||
                owner.GetType() != _ownerType || tracked.GetType() != _itemType)
                return Snapshot.Unknown("Missing/invalid root, owner or tracked instance");
            if (!initialized || !externallyQuiescent)
                return Snapshot.Unknown("Initialization/quiescence not independently established");
            try
            {
                var map = (IDictionary)_map.GetValue(root);
                var itemData = (IDictionary)_itemData.GetValue(tracked);
                var bindings = (IDictionary)_bindings.GetValue(owner);
                var a = Copy(map); var b = Copy(itemData); var c = Copy(bindings);
                bool flag = (bool)_flag.GetValue(tracked);
#if POVPLUS_PROVENANCE_FIXTURE
                if (DuringRead != null) DuringRead();
#endif
                if (!ReferenceEquals(map, _map.GetValue(root)) || !ReferenceEquals(itemData, _itemData.GetValue(tracked)) ||
                    !ReferenceEquals(bindings, _bindings.GetValue(owner)) || flag != (bool)_flag.GetValue(tracked) ||
                    !Same(a, Copy(map), true) || !Same(b, Copy(itemData), false) || !Same(c, Copy(bindings), false))
                    return Snapshot.Unknown("Collection/flag changed or interrupted");
                foreach (var p in a) if (p.Value == null || p.Value.GetType() != _itemType)
                    return Snapshot.Unknown("Incomplete ownership entry");
                object tome, guid, saved;
                bool has = a.TryGetValue("tome", out tome);
                bool hasGuid = b.TryGetValue("DO_CustomSlotGuid", out guid);
                bool hasSaved = c.TryGetValue("DO_Equipped_tome", out saved);
                bool match = has ? hasGuid && hasSaved && guid is string &&
                    !string.IsNullOrEmpty((string)guid) && Equals(guid, saved) : !hasSaved;
                return new Snapshot(true, has, flag, match, Alias(tome), Alias(tracked), a.Count,
                    has ? "Observed MS entry; coherence conditional on supplied quiescence" :
                    "Complete map has no Tome key; NOT proof of empty ownership elsewhere");
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
        private Transition _pending;
        private Snapshot _current;
        private bool _initial, _coverage, _invalid;
        internal string Classification { get; private set; } = "UnknownInitial";
        internal string Reason { get; private set; } = "No independent baseline";
        internal bool HistoryComplete { get { return _initial && _coverage && !_invalid; } }
        internal void Baseline(Snapshot snapshot, bool independentInitial, bool mutationCoverageEstablished)
        {
            if (_count != 0 || _current != null) { Fail("Baseline replacement requires a new ledger/epoch"); return; }
            _current = snapshot; _initial = independentInitial && snapshot != null && snapshot.Agrees;
            _coverage = mutationCoverageEstablished;
            Classification = _initial ? "ScopedSnapshotOnly" : "UnknownInitial";
            Reason = _coverage ? "Caller-attested fixture coverage; not global ordering" : "Mutation coverage unproved";
        }
        private void Fail(string reason) { _invalid = true; Classification = "Incomplete"; Reason = reason; _pending = null; }
        internal void Add(Transition e)
        {
            if (_invalid) return;
            if (++_count > 32) { Fail("Evidence budget exhausted"); return; }
            if (e == null || e.Source != "MS" || e.Sequence != _sequence + 1 ||
                string.IsNullOrEmpty(e.Transaction) || string.IsNullOrEmpty(e.Provenance))
            { Fail("Missing/duplicate/reordered/untrusted evidence"); return; }
            _sequence = e.Sequence;
            if (e.Kind == Kind.Gap) { Fail("Observation discontinuity"); return; }
            if (e.Phase == Phase.Fault) { Fail("Partial transaction after exception"); return; }
            if (e.Phase == Phase.Begin)
            {
                if (_pending != null) { Fail("Nested/swap intermediate unsupported"); return; }
                if (e.State == null || !e.State.Agrees || (_current != null &&
                    (_current.OwnerAlias != e.State.OwnerAlias || _current.HasTome != e.State.HasTome)))
                { Fail("Unrecorded state delta or contradictory pre-state"); return; }
                _pending = e; Classification = e.Kind + "Attempted"; Reason = "Entry is not completion"; return;
            }
            if (_pending == null || _pending.Transaction != e.Transaction || _pending.Kind != e.Kind ||
                _pending.ItemAlias != e.ItemAlias || e.State == null || !e.State.Agrees)
            { Fail("Unpaired/contradictory completion"); return; }
            if ((e.Kind == Kind.Equip || e.Kind == Kind.Restore || e.Kind == Kind.Swap) &&
                (!e.State.HasTome || e.State.OwnerAlias != e.ItemAlias))
            { Fail("Return without ownership transfer"); return; }
            if (e.Kind == Kind.Unequip && (e.State.HasTome || e.State.Flag))
            { Fail("Return without removal"); return; }
            bool changed = _current != null && (_current.HasTome != e.State.HasTome || _current.OwnerAlias != e.State.OwnerAlias);
            _current = e.State; _pending = null;
            Classification = !HistoryComplete ? "UnknownHistory" : !changed ? "CorroboratedNoOp" : "Corroborated" + e.Kind;
            Reason = "MS-only fixture scope; no persistence, all-owner, neutral or rollback claim";
        }
        internal void Seal()
        { if (_pending != null) Fail("Interrupted operation"); else if (!_initial || !_coverage) Classification = "UnknownHistory"; }
    }
}
#endif
