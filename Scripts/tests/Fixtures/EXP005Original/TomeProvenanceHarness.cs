using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using PoVPlus.Diagnostics.Provenance;

internal static class TomeProvenanceHarness
{
    internal sealed class Item { internal bool m_equipped; internal Dictionary<string,string> m_customData = new Dictionary<string,string>(); }
    internal sealed class Owner { internal Dictionary<string,string> m_customData = new Dictionary<string,string>(); }
    internal sealed class Data { internal readonly Dictionary<string,Item> EquippedItems = new Dictionary<string,Item>(); }
    internal static int Initializers, Calls;
    internal static int ForeignEnumerationCalls;
    internal sealed class TrapMap : Dictionary<string,Item>, IDictionary
    {
        IDictionaryEnumerator IDictionary.GetEnumerator()
        { ForeignEnumerationCalls++; return (IDictionaryEnumerator)base.GetEnumerator(); }
    }
    internal static class Cold
    {
        internal static readonly ConditionalWeakTable<Owner,Data> Roots = Init();
        private static ConditionalWeakTable<Owner,Data> Init() { Initializers++; return new ConditionalWeakTable<Owner,Data>(); }
        internal static Data GetRuntimeData(Owner owner) { Calls++; return Roots.GetOrCreateValue(owner); }
    }
    private static int _passed;
    private static FieldInfo F(Type type,string name) { return type.GetField(name,BindingFlags.Instance|BindingFlags.NonPublic); }
    private static ExistingStateReader Reader() { return new ExistingStateReader(typeof(Data),typeof(Owner),typeof(Item),
        F(typeof(Data),"EquippedItems"),F(typeof(Item),"m_equipped"),F(typeof(Item),"m_customData"),F(typeof(Owner),"m_customData")); }
    private static void Check(bool ok,string reason) { if(!ok) throw new Exception(reason); }
    private static void Test(string name,Action body) { body(); _passed++; Console.WriteLine("PASS "+name); }
    private sealed class Rig
    {
        internal readonly Owner Owner = new Owner(); internal readonly Data Root = new Data();
        internal readonly Item Book = new Item(); internal readonly Item Other = new Item();
        internal readonly ExistingStateReader Read = Reader();
        internal Rig() { Book.m_customData.Add("DO_CustomSlotGuid","a"); Other.m_customData.Add("DO_CustomSlotGuid","b"); }
        internal Snapshot State(bool init=true,bool quiet=true) { return Read.Read(Root,Owner,Book,init,quiet); }
        internal void Equip(Item item) { Root.EquippedItems["tome"]=item; item.m_equipped=true; Owner.m_customData["DO_Equipped_tome"]=item.m_customData["DO_CustomSlotGuid"]; }
        internal void Remove() { Root.EquippedItems.Remove("tome"); Book.m_equipped=false; Owner.m_customData.Remove("DO_Equipped_tome"); }
        internal Transition Event(long seq,Phase phase,Snapshot state,Kind kind=Kind.Equip,int alias=0,string transaction="t")
        { return new Transition("MS",seq,transaction,kind,phase,alias==0?state.TrackedAlias:alias,state,"independent synthetic driver"); }
    }
    private static ProvenanceLedger Ledger(Rig r,bool initial=true,bool coverage=true)
    { var l=new ProvenanceLedger(); l.Baseline(r.State(),initial,coverage); return l; }
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CaptureTemporary(ReturnedRoot capture,object owner)
    { var root=new Data(); capture.ObserveReturn(owner,root,true,0); return new WeakReference(root); }
    private static void MainCases()
    {
        Test("cold metadata and reader do not initialize foreign fixture holder",()=>{
            var c=typeof(Cold); var r=Reader(); Check(Initializers==0,"observer initialized holder");
            Check(r.Read(null,new Owner(),new Item(),true,true).Complete==false,"missing root"); Check(Initializers==0,"read initialized holder"); });
        Test("game-initiated return capture preserves exact result and call count",()=>{
            var owner=new Owner(); var returned=Cold.GetRuntimeData(owner); int before=Calls;
            var capture=new ReturnedRoot(); capture.ObserveReturn(owner,returned,true,0);
            Check(ReferenceEquals(capture.Borrow(owner,1),returned)&&Calls==before&&Initializers==1,"capture called getter or changed return"); });
        Test("missing or untrusted root fails closed",()=>{
            var c=new ReturnedRoot(); c.ObserveReturn(new Owner(),null,true,0); Check(c.Invalid,"missing root");
            c=new ReturnedRoot(); c.ObserveReturn(new Owner(),new Data(),false,0); Check(c.Invalid,"unauthenticated result"); });
        Test("duplicate root capture does not extend deadline",()=>{
            var c=new ReturnedRoot(); var o=new Owner(); var d=new Data(); c.ObserveReturn(o,d,true,0); c.ObserveReturn(o,d,true,59);
            Check(c.Epoch==1&&c.Borrow(o,60)==null,"duplicate reset deadline"); });
        Test("replacement root and wrong owner invalidate epoch",()=>{
            var c=new ReturnedRoot(); var o=new Owner(); var d=new Data(); c.ObserveReturn(o,d,true,0); c.ObserveReturn(o,new Data(),true,1); Check(c.Invalid,"replacement");
            c=new ReturnedRoot(); c.ObserveReturn(o,d,true,0); Check(c.Borrow(new Owner(),1)==null,"wrong owner"); });
        Test("capture is weak and disposal clears handle",()=>{
            var c=new ReturnedRoot(); var o=new Owner(); var weak=CaptureTemporary(c,o);
            GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect(); Check(!weak.IsAlive,"retained root strongly"); c.Dispose(); Check(c.Borrow(o,1)==null,"dispose"); GC.KeepAlive(o); });
        Test("late populated capture is not an initial-state certificate",()=>{
            var r=new Rig(); r.Equip(r.Book); var s=r.State(); Check(s.Agrees&&s.HasTome,"populated state lost");
            var l=new ProvenanceLedger(); l.Baseline(s,false,true); Check(!l.HistoryComplete&&l.Classification=="UnknownInitial","root became baseline"); });
        Test("null invalid and incomplete objects stay unknown",()=>{
            var r=new Rig(); Check(!r.Read.Read(new object(),r.Owner,r.Book,true,true).Complete,"wrong root type");
            Check(!r.Read.Read(r.Root,null,r.Book,true,true).Complete,"missing owner");
            Check(!r.Read.Read(r.Root,r.Owner,null,true,true).Complete,"missing item");
            r.Owner.m_customData=null; Check(!r.State().Complete,"missing bindings became empty"); });
        Test("uninitialized dictionary is not empty",()=>{
            var r=new Rig(); var d=(Data)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Data));
            Check(!r.Read.Read(d,r.Owner,r.Book,true,true).Complete,"missing map became empty"); });
        Test("unknown initialization and nonquiescent snapshots rejected",()=>{
            var r=new Rig(); Check(!r.State(false).Complete&&!r.State(true,false).Complete,"assumed initialization or concurrency"); });
        Test("null ownership value is invalid not empty",()=>{
            var r=new Rig(); r.Root.EquippedItems["tome"]=null; Check(!r.State().Complete,"null entry"); });
        Test("reader preserves contents references flags bindings and counts",()=>{
            var r=new Rig(); r.Equip(r.Book); var map=r.Root.EquippedItems; var bindings=r.Owner.m_customData; var custom=r.Book.m_customData;
            var before=new Dictionary<string,Item>(map); for(int i=0;i<3;i++) Check(r.State().Agrees,"read failure");
            Check(ReferenceEquals(map,r.Root.EquippedItems)&&ReferenceEquals(bindings,r.Owner.m_customData)&&ReferenceEquals(custom,r.Book.m_customData)&&
                map.Count==before.Count&&ReferenceEquals(map["tome"],before["tome"])&&r.Book.m_equipped&&bindings["DO_Equipped_tome"]=="a"&&custom.Count==1,"foreign mutation"); });
        Test("attempt and successful return without map transfer are not equip",()=>{
            var r=new Rig(); var l=Ledger(r); var s=r.State(); l.Add(r.Event(1,Phase.Begin,s)); Check(l.Classification=="EquipAttempted","attempt");
            l.Add(r.Event(2,Phase.Return,s)); Check(l.Classification=="Incomplete","trusted method return"); });
        Test("completed equip needs independent map flag binding corroboration",()=>{
            var r=new Rig(); var l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State())); r.Equip(r.Book);
            Check(ReferenceEquals(r.Root.EquippedItems["tome"],r.Book),"driver oracle"); l.Add(r.Event(2,Phase.Return,r.State())); Check(l.Classification=="CorroboratedEquip","valid equip rejected"); });
        Test("unequip attempt without removal does not complete",()=>{
            var r=new Rig(); r.Equip(r.Book); var l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State(),Kind.Unequip));
            l.Add(r.Event(2,Phase.Return,r.State(),Kind.Unequip)); Check(l.Classification=="Incomplete","false removal"); });
        Test("corroborated removal is MS scoped not neutral or persistence",()=>{
            var r=new Rig(); r.Equip(r.Book); var l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State(),Kind.Unequip)); r.Remove();
            l.Add(r.Event(2,Phase.Return,r.State(),Kind.Unequip)); Check(l.Classification=="CorroboratedUnequip"&&l.Reason.Contains("no persistence"),"scoped removal"); });
        Test("partial swap with original exception stays partial",()=>{
            var r=new Rig(); r.Equip(r.Book); var l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State(),Kind.Swap));
            var original=new Exception("driver partial swap"); Exception observed=null;
            try { r.Remove(); throw original; } catch(Exception e) { observed=e; l.Add(r.Event(2,Phase.Fault,r.State(),Kind.Swap)); }
            Check(ReferenceEquals(original,observed)&&l.Classification=="Incomplete"&&r.Root.EquippedItems.Count==0,"exception changed or partial swap promoted"); });
        Test("nested swap intermediate invalidates minimal ledger",()=>{
            var r=new Rig(); var l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State(),Kind.Swap)); l.Add(r.Event(2,Phase.Begin,r.State(),Kind.Unequip)); Check(!l.HistoryComplete,"nested unsupported"); });
        Test("explicit restore not ordinary equip can corroborate scoped rebind",()=>{
            var r=new Rig(); var l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State(),Kind.Restore)); r.Equip(r.Book);
            l.Add(r.Event(2,Phase.Return,r.State(),Kind.Restore)); Check(l.Classification=="CorroboratedRestore","restore kind missing"); });
        Test("unrecorded restore delta contradicts previous snapshot",()=>{
            var r=new Rig(); var l=Ledger(r); r.Equip(r.Book); l.Add(r.Event(1,Phase.Begin,r.State())); Check(l.Classification=="Incomplete","missed restore ignored"); });
        foreach(var mode in new[]{"duplicate","out-of-order","missing"}) Test(mode+" sequence rejected",()=>{
            var r=new Rig(); var l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State()));
            l.Add(r.Event(mode=="duplicate"?1:3,Phase.Return,r.State())); Check(l.Classification=="Incomplete","sequence accepted"); });
        Test("conflicting event item identity fails",()=>{
            var r=new Rig(); var l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State())); r.Equip(r.Book);
            l.Add(r.Event(2,Phase.Return,r.State(),Kind.Equip,99)); Check(l.Classification=="Incomplete","identity conflict"); });
        Test("UI flag agreement cannot override ownership disagreement",()=>{
            var r=new Rig(); r.Equip(r.Other); r.Book.m_equipped=true; bool uiShowsBook=true;
            var s=r.State(); Check(uiShowsBook&&r.Book.m_equipped&&!s.Agrees,"UI was accepted as owner"); });
        Test("map flag disagreement fails corroboration",()=>{
            var r=new Rig(); r.Equip(r.Book); r.Book.m_equipped=false; Check(!r.State().Agrees,"flag disagreement"); });
        Test("mutation during collection rejected without reader repair",()=>{
            var r=new Rig(); r.Read.DuringRead=()=>r.Root.EquippedItems["other"]=r.Other;
            Check(!r.State().Complete&&r.Root.EquippedItems.ContainsKey("other"),"mutation hidden/repaired"); });
        Test("interrupted read rejected",()=>{
            var r=new Rig(); r.Read.DuringRead=()=>{throw new InvalidOperationException("fixture interruption");}; Check(!r.State().Complete,"interrupted"); });
        Test("unpaired transaction and exhausted ledger fail closed",()=>{
            var r=new Rig(); var l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State())); l.Seal(); Check(!l.HistoryComplete,"interruption");
            l=Ledger(r); for(int i=1;i<=34;i++) l.Add(r.Event(i,i%2==1?Phase.Begin:Phase.Return,r.State(),Kind.Unequip)); Check(l.Classification=="Incomplete","overflow"); });
        Test("reader alias exhaustion remains unknown",()=>{
            var r=new Rig(); for(int i=0;i<64;i++) Check(r.Read.Read(r.Root,r.Owner,new Item(),true,true).Complete,"premature exhaustion");
            Check(!r.Read.Read(r.Root,r.Owner,new Item(),true,true).Complete,"alias bound"); });
        Test("unknown initial state cannot heal through later success",()=>{
            var r=new Rig(); var l=Ledger(r,false); l.Add(r.Event(1,Phase.Begin,r.State())); r.Equip(r.Book); l.Add(r.Event(2,Phase.Return,r.State()));
            Check(l.Classification=="UnknownHistory"&&!l.HistoryComplete,"success healed unknown initial"); });
        Test("matching sequence numbers do not establish hook coverage",()=>{
            var r=new Rig(); var l=Ledger(r,true,false); l.Add(r.Event(1,Phase.Begin,r.State())); r.Equip(r.Book); l.Add(r.Event(2,Phase.Return,r.State())); Check(l.Classification=="UnknownHistory","coverage invented"); });
        Test("binding absent or mismatched prevents completed equip",()=>{
            var r=new Rig(); r.Equip(r.Book); r.Owner.m_customData.Clear(); Check(!r.State().Agrees,"missing binding");
            r.Owner.m_customData["DO_Equipped_tome"]="wrong"; Check(!r.State().Agrees,"wrong binding"); });
        Test("missing Tome key alone does not establish initial ownership",()=>{
            var r=new Rig(); var s=r.State(); Check(s.Complete&&!s.HasTome&&s.Reason.Contains("NOT proof"),"absence lost provenance");
            var l=new ProvenanceLedger(); l.Baseline(s,false,false); Check(!l.HistoryComplete,"assumed empty baseline"); });
        Test("declared discontinuity and unpaired restore cannot recover history",()=>{
            var r=new Rig(); var l=Ledger(r); l.Add(r.Event(1,Phase.Return,r.State(),Kind.Restore)); Check(!l.HistoryComplete,"unpaired restore");
            l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State(),Kind.Gap)); r.Equip(r.Book); l.Baseline(r.State(),true,true); Check(!l.HistoryComplete,"gap healed in place"); });
        Test("ABA roundtrip is undetectable without external coverage",()=>{
            var r=new Rig(); var l=Ledger(r,true,false); r.Equip(r.Book); r.Remove();
            l.Add(r.Event(1,Phase.Begin,r.State(),Kind.Unequip)); l.Add(r.Event(2,Phase.Return,r.State(),Kind.Unequip));
            Check(l.Classification=="UnknownHistory","invisible roundtrip claimed complete"); });
    }
    private static int Main(string[] args)
    {
        if(args.Length>0 && args[0]=="--falsify")
        {
            var r=new Rig(); int hiddenWrites=0;
            r.Read.DuringRead=()=>{r.Equip(r.Book); hiddenWrites++; r.Remove(); hiddenWrites++;};
            var sampled=r.State();
            if(hiddenWrites==2 && sampled.Complete && sampled.Agrees)
            {
                Console.WriteLine("COUNTEREXAMPLE: supplied quiescence=true + two-pass equality accepts an intervening ownership/flag/binding ABA roundtrip; independent driver saw two writes. Atomic/coherent capture is NOT proved.");
                var other=new Rig(); var l=Ledger(other,true,true); other.Equip(other.Book); other.Remove();
                l.Add(other.Event(1,Phase.Begin,other.State(),Kind.Unequip)); l.Add(other.Event(2,Phase.Return,other.State(),Kind.Unequip));
                Console.WriteLine("COVERAGE COUNTEREXAMPLE: caller-attested coverage=true after missed roundtrip yields "+l.Classification+" history="+l.HistoryComplete+"; sequence continuity cannot authenticate hook completeness.");
                var invalid=new Rig(); var trap=new TrapMap();
                F(typeof(Data),"EquippedItems").SetValue(invalid.Root,trap); // Disposable fixture corruption only.
                var invalidSnapshot=invalid.State();
                Console.WriteLine("READER COUNTEREXAMPLE: declared Dictionary field accepted a derived IDictionary implementation; foreign enumerator executed "+ForeignEnumerationCalls+" times, complete="+invalidSnapshot.Complete+". Field declaration alone does not prove collection read purity.");
                if(ForeignEnumerationCalls==0 || !invalidSnapshot.Complete) return 3;
                return 2; // Deliberate failure of architecture proof, not a normal fixture failure.
            }
            Console.Error.WriteLine("Counterexample assumptions changed; re-review the architecture."); return 3;
        }
        if(args.Length>0)
        {
            var r=new Rig(); var l=Ledger(r,true,false); l.Add(r.Event(1,Phase.Begin,r.State())); r.Equip(r.Book); l.Add(r.Event(2,Phase.Return,r.State()));
            Console.WriteLine("{\"schema\":1,\"scope\":\"synthetic-MS-only\",\"runtimePatchInstalled\":false,\"neutralCertification\":\"unknown\",\"ordering\":\"supplied-local-only\",\"classification\":\""+l.Classification+"\",\"historyComplete\":"+l.HistoryComplete.ToString().ToLowerInvariant()+"}"); return 0;
        }
        try { MainCases(); Console.WriteLine("All "+_passed+" provenance fixtures passed; no foreign assembly executed."); return 0; }
        catch(Exception e) { Console.Error.WriteLine(e.Message); return 1; }
    }
}
