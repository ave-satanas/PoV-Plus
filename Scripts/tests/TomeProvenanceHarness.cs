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
    internal static int ForeignEnumerationCalls, ForeignCountCalls, ComparerCalls;
    internal sealed class TrapComparer : IEqualityComparer<string>
    {
        public bool Equals(string a,string b) { ComparerCalls++; return StringComparer.Ordinal.Equals(a,b); }
        public int GetHashCode(string value) { ComparerCalls++; return StringComparer.Ordinal.GetHashCode(value); }
    }
    internal sealed class TrapMap : Dictionary<string,Item>, IDictionary, ICollection
    {
        int ICollection.Count { get { ForeignCountCalls++; return base.Count; } }
        IDictionaryEnumerator IDictionary.GetEnumerator()
        { ForeignEnumerationCalls++; return (IDictionaryEnumerator)base.GetEnumerator(); }
    }
    internal static class Cold
    {
        internal static readonly ConditionalWeakTable<Owner,Data> Roots = Init();
        private static ConditionalWeakTable<Owner,Data> Init() { Initializers++; return new ConditionalWeakTable<Owner,Data>(); }
        internal static Data GetRuntimeData(Owner owner) { Calls++; return Roots.GetOrCreateValue(owner); }
    }
    internal sealed class TrapStrings : Dictionary<string,string>, IDictionary, ICollection
    {
        int ICollection.Count { get { ForeignCountCalls++; return base.Count; } }
        IDictionaryEnumerator IDictionary.GetEnumerator()
        { ForeignEnumerationCalls++; return (IDictionaryEnumerator)base.GetEnumerator(); }
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
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Snapshot TemporarySnapshot(out WeakReference[] references)
    {
        var r=new Rig();references=new[]{new WeakReference(r.Root),new WeakReference(r.Owner),new WeakReference(r.Book)};
        return r.State();
    }
    private static void MainCases()
    {
        Test("cold metadata and reader do not initialize foreign fixture holder",()=>{
            var c=typeof(Cold); var r=Reader(); Check(Initializers==0,"observer initialized holder");
            Check(r.Read(null,new Owner(),new Item(),true,true).LocalReadValid==false,"missing root"); Check(Initializers==0,"read initialized holder"); });
        Test("game-initiated return capture preserves exact result and call count",()=>{
            var owner=new Owner(); var returned=Cold.GetRuntimeData(owner); int before=Calls;
            var capture=new ReturnedRoot(); capture.ObserveReturn(owner,returned,true,0);
            Check(ReferenceEquals(capture.Borrow(owner,1),returned)&&Calls==before&&Initializers==1&&capture.OriginValidity=="unknown","capture called getter or changed return"); });
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
            var r=new Rig(); r.Equip(r.Book); var s=r.State(); Check(s.SampledValuesAgree&&s.HasTome,"populated state lost");
            var l=new ProvenanceLedger(); l.Baseline(s,false,true); Check(!l.HistoryComplete&&l.InitialStateValidity=="unknown","root became baseline"); });
        Test("null invalid and incomplete objects stay unknown",()=>{
            var r=new Rig(); Check(!r.Read.Read(new object(),r.Owner,r.Book,true,true).LocalReadValid,"wrong root type");
            Check(!r.Read.Read(r.Root,null,r.Book,true,true).LocalReadValid,"missing owner");
            Check(!r.Read.Read(r.Root,r.Owner,null,true,true).LocalReadValid,"missing item");
            r.Owner.m_customData=null; Check(!r.State().LocalReadValid,"missing bindings became empty"); });
        Test("uninitialized dictionary is not empty",()=>{
            var r=new Rig(); var d=(Data)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Data));
            Check(!r.Read.Read(d,r.Owner,r.Book,true,true).LocalReadValid,"missing map became empty"); });
        Test("unknown initialization and nonquiescent snapshots rejected",()=>{
            var r=new Rig(); Check(!r.State(false).LocalReadValid&&!r.State(true,false).LocalReadValid,"assumed initialization or concurrency"); });
        Test("null ownership value is invalid not empty",()=>{
            var r=new Rig(); r.Root.EquippedItems["tome"]=null; Check(!r.State().LocalReadValid,"null entry"); });
        Test("reader preserves contents references flags bindings and counts",()=>{
            var r=new Rig(); r.Equip(r.Book); var map=r.Root.EquippedItems; var bindings=r.Owner.m_customData; var custom=r.Book.m_customData;
            var before=new Dictionary<string,Item>(map); var mapWitness=map.GetEnumerator();var bindingWitness=bindings.GetEnumerator();var customWitness=custom.GetEnumerator(); for(int i=0;i<3;i++) Check(r.State().SampledValuesAgree,"read failure");
            Check(ReferenceEquals(map,r.Root.EquippedItems)&&ReferenceEquals(bindings,r.Owner.m_customData)&&ReferenceEquals(custom,r.Book.m_customData)&&
                map.Count==before.Count&&ReferenceEquals(map["tome"],before["tome"])&&r.Book.m_equipped&&bindings["DO_Equipped_tome"]=="a"&&custom.Count==1,"foreign mutation");
            Check(mapWitness.MoveNext()&&bindingWitness.MoveNext()&&customWitness.MoveNext(),"reader performed transient dictionary mutation"); });
        Test("attempt and successful return without map transfer are not equip",()=>{
            var r=new Rig(); var l=Ledger(r); var s=r.State(); l.Add(r.Event(1,Phase.Begin,s)); Check(l.Classification=="EquipAttempted","attempt");
            l.Add(r.Event(2,Phase.Return,s)); Check(l.Classification=="Incomplete","trusted method return"); });
        Test("completed equip needs independent map flag binding corroboration",()=>{
            var r=new Rig(); var l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State())); r.Equip(r.Book);
            Check(ReferenceEquals(r.Root.EquippedItems["tome"],r.Book),"driver oracle"); l.Add(r.Event(2,Phase.Return,r.State())); Check(l.Classification=="EquipEndpointAgreement","valid equip rejected"); });
        Test("unequip attempt without removal does not complete",()=>{
            var r=new Rig(); r.Equip(r.Book); var l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State(),Kind.Unequip));
            l.Add(r.Event(2,Phase.Return,r.State(),Kind.Unequip)); Check(l.Classification=="Incomplete","false removal"); });
        Test("corroborated removal is MS scoped not neutral or persistence",()=>{
            var r=new Rig(); r.Equip(r.Book); var l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State(),Kind.Unequip)); r.Remove();
            l.Add(r.Event(2,Phase.Return,r.State(),Kind.Unequip)); Check(l.Classification=="UnequipEndpointAgreement"&&l.Reason.Contains("no persistence"),"scoped removal"); });
        Test("partial swap with original exception stays partial",()=>{
            var r=new Rig(); r.Equip(r.Book); var l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State(),Kind.Swap));
            var original=new Exception("driver partial swap"); Exception observed=null;
            try { r.Remove(); throw original; } catch(Exception e) { observed=e; l.Add(r.Event(2,Phase.Fault,r.State(),Kind.Swap)); }
            Check(ReferenceEquals(original,observed)&&l.Classification=="Incomplete"&&r.Root.EquippedItems.Count==0,"exception changed or partial swap promoted"); });
        Test("nested swap intermediate invalidates minimal ledger",()=>{
            var r=new Rig(); var l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State(),Kind.Swap)); l.Add(r.Event(2,Phase.Begin,r.State(),Kind.Unequip)); Check(l.Classification=="Incomplete","nested unsupported"); });
        Test("explicit restore not ordinary equip can corroborate scoped rebind",()=>{
            var r=new Rig(); var l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State(),Kind.Restore)); r.Equip(r.Book);
            l.Add(r.Event(2,Phase.Return,r.State(),Kind.Restore)); Check(l.Classification=="RestoreEndpointAgreement","restore kind missing"); });
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
            var s=r.State(); Check(uiShowsBook&&r.Book.m_equipped&&!s.SampledValuesAgree,"UI was accepted as owner"); });
        Test("map flag disagreement fails corroboration",()=>{
            var r=new Rig(); r.Equip(r.Book); r.Book.m_equipped=false; Check(!r.State().SampledValuesAgree,"flag disagreement"); });
        Test("mutation during collection rejected without reader repair",()=>{
            var r=new Rig(); r.Read.DuringRead=()=>r.Root.EquippedItems["other"]=r.Other;
            Check(!r.State().LocalReadValid&&r.Root.EquippedItems.ContainsKey("other"),"mutation hidden/repaired"); });
        Test("interrupted read rejected",()=>{
            var r=new Rig(); r.Read.DuringRead=()=>{throw new InvalidOperationException("fixture interruption");}; Check(!r.State().LocalReadValid,"interrupted"); });
        Test("unpaired transaction and exhausted ledger fail closed",()=>{
            var r=new Rig(); var l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State())); l.Seal(); Check(l.Classification=="Incomplete","interruption");
            l=Ledger(r); for(int i=1;i<=34;i++) l.Add(r.Event(i,i%2==1?Phase.Begin:Phase.Return,r.State(),Kind.Unequip)); Check(l.Classification=="Incomplete","overflow"); });
        Test("reader alias exhaustion remains unknown",()=>{
            var r=new Rig(); for(int i=0;i<64;i++) Check(r.Read.Read(r.Root,r.Owner,new Item(),true,true).LocalReadValid,"premature exhaustion");
            Check(!r.Read.Read(r.Root,r.Owner,new Item(),true,true).LocalReadValid,"alias bound"); });
        Test("unknown initial state cannot heal through later success",()=>{
            var r=new Rig(); var l=Ledger(r,false); l.Add(r.Event(1,Phase.Begin,r.State())); r.Equip(r.Book); l.Add(r.Event(2,Phase.Return,r.State()));
            Check(l.TransitionCoverage=="unknown" && l.InitialStateValidity=="unknown"&&!l.HistoryComplete,"success healed unknown initial"); });
        Test("matching sequence numbers do not establish hook coverage",()=>{
            var r=new Rig(); var l=Ledger(r,true,false); l.Add(r.Event(1,Phase.Begin,r.State())); r.Equip(r.Book); l.Add(r.Event(2,Phase.Return,r.State())); Check(l.TransitionCoverage=="unknown" && l.InitialStateValidity=="unknown","coverage invented"); });
        Test("binding absent or mismatched prevents completed equip",()=>{
            var r=new Rig(); r.Equip(r.Book); r.Owner.m_customData.Clear(); Check(!r.State().SampledValuesAgree,"missing binding");
            r.Owner.m_customData["DO_Equipped_tome"]="wrong"; Check(!r.State().SampledValuesAgree,"wrong binding"); });
        Test("missing Tome key alone does not establish initial ownership",()=>{
            var r=new Rig(); var s=r.State(); Check(s.LocalReadValid&&!s.HasTome&&s.Reason.Contains("NOT proof"),"absence lost provenance");
            var l=new ProvenanceLedger(); l.Baseline(s,false,false); Check(!l.HistoryComplete,"assumed empty baseline"); });
        Test("declared discontinuity and unpaired restore cannot recover history",()=>{
            var r=new Rig(); var l=Ledger(r); l.Add(r.Event(1,Phase.Return,r.State(),Kind.Restore)); Check(l.Classification=="Incomplete","unpaired restore");
            l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State(),Kind.Gap)); r.Equip(r.Book); l.Baseline(r.State(),true,true); Check(l.Classification=="Incomplete","gap healed in place"); });
        Test("ABA roundtrip is undetectable without external coverage",()=>{
            var r=new Rig(); var l=Ledger(r,true,false); r.Equip(r.Book); r.Remove();
            l.Add(r.Event(1,Phase.Begin,r.State(),Kind.Unequip)); l.Add(r.Event(2,Phase.Return,r.State(),Kind.Unequip));
            Check(l.TransitionCoverage=="unknown" && l.InitialStateValidity=="unknown","invisible roundtrip claimed complete"); });
        Test("derived ownership dictionary rejected before Count or enumeration",()=>{
            var r=new Rig(); ForeignEnumerationCalls=0; ForeignCountCalls=0;
            F(typeof(Data),"EquippedItems").SetValue(r.Root,new TrapMap());
            var s=r.State(); Check(!s.LocalReadValid&&ForeignEnumerationCalls==0&&ForeignCountCalls==0,"derived ownership enumerator executed"); });
        Test("derived binding and item metadata dictionaries also rejected",()=>{
            var r=new Rig(); ForeignEnumerationCalls=0; ForeignCountCalls=0; r.Owner.m_customData=new TrapStrings();
            Check(!r.State().LocalReadValid&&ForeignEnumerationCalls==0&&ForeignCountCalls==0,"derived owner bindings executed");
            r=new Rig(); r.Book.m_customData=new TrapStrings();
            Check(!r.State().LocalReadValid&&ForeignEnumerationCalls==0&&ForeignCountCalls==0,"derived item metadata executed"); });
        Test("mutation inside ownership enumeration rejected by BCL enumerator",()=>{
            var r=new Rig(); r.Equip(r.Book); bool once=false;
            ExistingStateReader.EnumerationStep=()=>{if(!once){once=true;r.Root.EquippedItems["other"]=r.Other;}};
            try { Check(!r.State().LocalReadValid,"mutation during enumeration accepted"); }
            finally { ExistingStateReader.EnumerationStep=null; }
            Check(once&&r.Root.EquippedItems.ContainsKey("other"),"driver mutation lost or repaired"); });
        Test("equal endpoints after intervening mutation give no continuity",()=>{
            var r=new Rig(); int writes=0;
            r.Read.DuringRead=()=>{r.Equip(r.Book);writes++;r.Remove();writes++;};
            var s=r.State(); Check(writes==2&&s.LocalReadValid&&s.EndpointsAgree,"roundtrip fixture not exercised");
            Check(s.Continuity=="unknown"&&s.Atomicity=="unknown"&&s.AuthoritativeState=="unknown","endpoint equality promoted to continuous state"); });
        Test("false initial and coverage assertions never upgrade evidence",()=>{
            foreach(bool initial in new[]{false,true}) foreach(bool coverage in new[]{false,true}) {
                var r=new Rig();var l=Ledger(r,initial,coverage);r.Equip(r.Book);r.Remove();
                l.Add(r.Event(1,Phase.Begin,r.State(),Kind.Unequip));l.Add(r.Event(2,Phase.Return,r.State(),Kind.Unequip));
                Check(l.Classification=="NoChangeInRecordedProjection"&&!l.HistoryComplete&&l.TransitionCoverage=="unknown"&&
                    l.InitialStateValidity=="unknown"&&l.Continuity=="unknown","caller claims promoted into proof");
            } });
        Test("apparently empty map does not grant rollback prerequisites",()=>{
            var r=new Rig();var s=r.State();var l=Ledger(r,true,true);
            Check(s.LocalReadValid&&!s.HasTome&&s.SampledValuesAgree,"local absence missing");
            Check(l.InitialStateValidity=="unknown"&&l.NeutralState=="unknown"&&l.RollbackSafety=="unknown","missing obligations treated as neutral"); });
        Test("missing and conflicting item identity rejected",()=>{
            var r=new Rig(); var l=Ledger(r);
            l.Add(new Transition("MS",1,"t",Kind.Equip,Phase.Begin,0,r.State(),"fixture"));
            Check(l.Classification=="Incomplete","missing identity accepted");
            l=Ledger(r); l.Add(r.Event(1,Phase.Begin,r.State(),Kind.Equip,99));
            Check(l.Classification=="Incomplete","conflicting attempt identity accepted"); });
        Test("local snapshot and event comparisons do not certify ownership",()=>{
            var r=new Rig();var l=Ledger(r,true,true);l.Add(r.Event(1,Phase.Begin,r.State()));r.Equip(r.Book);
            var s=r.State();l.Add(r.Event(2,Phase.Return,s));
            Check(s.LocalReadValid&&s.EndpointsAgree&&s.SampledValuesAgree&&l.Classification=="EquipEndpointAgreement","useful local observation lost");
            Check(s.AuthoritativeState=="unknown"&&!l.HistoryComplete&&l.TransitionCoverage=="unknown"&&l.NeutralState=="unknown","observation certified authority"); });
        Test("known conflicting baseline is not silently repaired by endpoint scoring",()=>{
            var r=new Rig();r.Equip(r.Book);r.Book.m_equipped=false;var l=Ledger(r);
            r.Book.m_equipped=true;l.Add(r.Event(1,Phase.Begin,r.State()));
            Check(l.Classification=="Incomplete","previous flag conflict silently disappeared"); });
        Test("unrecorded count delta is retained as inconsistent evidence",()=>{
            var r=new Rig();var l=Ledger(r);r.Root.EquippedItems.Add("other",r.Other);
            l.Add(r.Event(1,Phase.Begin,r.State()));Check(l.Classification=="Incomplete","known count delta ignored"); });
        Test("immutable observations retained within budget without exposing internal list",()=>{
            var r=new Rig();var l=Ledger(r);var before=r.State();var entry=r.Event(1,Phase.Begin,before);
            l.Add(entry);r.Equip(r.Book);l.Add(r.Event(2,Phase.Return,r.State()));
            var records=l.Observations;Check(records.Length==2&&ReferenceEquals(records[0],entry)&&!records[0].State.HasTome&&records[1].State.HasTome,"original scalar evidence lost");
            records[0]=null;Check(l.Observations[0]!=null,"mutable internal record list exposed");
            l=Ledger(r);for(int i=1;i<=34;i++)l.Add(r.Event(i,i%2==1?Phase.Begin:Phase.Return,r.State(),Kind.Equip));
            Check(l.Observations.Length==32&&l.Classification=="Incomplete","record budget exceeded"); });
        Test("equal numeric aliases from different readers cannot be correlated",()=>{
            var first=new Rig();var second=new Rig();var l=Ledger(first);
            l.Add(first.Event(1,Phase.Begin,first.State()));second.Equip(second.Book);
            Check(!ReferenceEquals(first.Book,second.Book)&&first.State().TrackedAlias==second.State().TrackedAlias,"collision fixture did not use distinct equal-numbered instances");
            l.Add(second.Event(2,Phase.Return,second.State()));
            Check(l.Classification=="Incomplete","unrelated readers with alias 1 were correlated"); });
        Test("reader refuses root or owner replacement within an observation scope",()=>{
            var first=new Rig();var second=new Rig();Check(first.State().LocalReadValid,"first scope");
            Check(!first.Read.Read(second.Root,second.Owner,second.Book,true,true).LocalReadValid,"root/owner scope reused"); });
        Test("retained snapshot scope holds no root owner or item strongly",()=>{
            WeakReference[] references;var snapshot=TemporarySnapshot(out references);
            GC.Collect();GC.WaitForPendingFinalizers();GC.Collect();
            foreach(var reference in references)Check(!reference.IsAlive,"snapshot scope retained a sampled object");
            GC.KeepAlive(snapshot); });
        Test("sparse dictionary traversal and string inputs are bounded",()=>{
            var r=new Rig();for(int i=0;i<96;i++)r.Root.EquippedItems.Add("s"+i,r.Book);
            for(int i=0;i<95;i++)r.Root.EquippedItems.Remove("s"+i);
            Check(r.Root.EquippedItems.Count==1&&!r.State().LocalReadValid,"sparse storage bypassed traversal bound");
            r=new Rig();r.Root.EquippedItems.Add(new string('k',129),r.Book);Check(!r.State().LocalReadValid,"long source key bypassed bound");
            r=new Rig();r.Owner.m_customData.Add("other",new string('v',4097));Check(!r.State().LocalReadValid,"long binding value bypassed bound"); });
        Test("exact dictionary enumeration never invokes its supplied comparer",()=>{
            var r=new Rig();r.Equip(r.Book);var map=new Dictionary<string,Item>(new TrapComparer());map.Add("tome",r.Book);
            F(typeof(Data),"EquippedItems").SetValue(r.Root,map);ComparerCalls=0;
            var s=r.State();Check(s.LocalReadValid&&s.SampledValuesAgree&&ComparerCalls==0,"observer invoked source comparer"); });
        Test("oversized collections and record labels remain bounded",()=>{
            var r=new Rig();for(int i=0;i<65;i++) r.Root.EquippedItems.Add("s"+i,r.Book);
            Check(!r.State().LocalReadValid,"collection bound"); r=new Rig();var l=Ledger(r);
            l.Add(new Transition("MS",1,new string('x',97),Kind.Equip,Phase.Begin,r.State().TrackedAlias,r.State(),"fixture"));
            Check(l.Classification=="Incomplete","label bound"); });
    }
    private static int Main(string[] args)
    {
        if(args.Length>0)
        {
            var r=new Rig(); var l=Ledger(r,true,false); l.Add(r.Event(1,Phase.Begin,r.State())); r.Equip(r.Book); var s=r.State();l.Add(r.Event(2,Phase.Return,s));
            Console.WriteLine("{\"schema\":2,\"scope\":\"synthetic-MS-only\",\"runtimePatchInstalled\":false,\"neutralCertification\":\"unknown\",\"ordering\":\"supplied-local-only\",\"hostStorageField\":\""+(typeof(Dictionary<string,Item>).GetField("_count",BindingFlags.Instance|BindingFlags.NonPublic)??typeof(Dictionary<string,Item>).GetField("count",BindingFlags.Instance|BindingFlags.NonPublic)).Name+"\",\"recordCount\":"+l.Observations.Length+",\"firstSequence\":"+l.Observations[0].Sequence+",\"lastSequence\":"+l.Observations[1].Sequence+",\"classification\":\""+l.Classification+"\",\"localReadValid\":"+s.LocalReadValid.ToString().ToLowerInvariant()+",\"endpointsAgree\":"+s.EndpointsAgree.ToString().ToLowerInvariant()+",\"sampledValuesAgree\":"+s.SampledValuesAgree.ToString().ToLowerInvariant()+",\"originValidity\":\""+s.OriginValidity+"\",\"atomicity\":\""+s.Atomicity+"\",\"authoritativeState\":\""+s.AuthoritativeState+"\",\"coverage\":\""+l.TransitionCoverage+"\",\"initialState\":\""+l.InitialStateValidity+"\",\"continuity\":\""+l.Continuity+"\",\"rollback\":\""+l.RollbackSafety+"\",\"historyComplete\":"+l.HistoryComplete.ToString().ToLowerInvariant()+"}"); return 0;
        }
        try { MainCases(); Console.WriteLine("All "+_passed+" provenance fixtures passed; no foreign assembly executed."); return 0; }
        catch(Exception e) { Console.Error.WriteLine(e.Message); return 1; }
    }
}
