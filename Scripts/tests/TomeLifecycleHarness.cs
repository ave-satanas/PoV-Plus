using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using PoVPlus.Diagnostics;

internal static class TomeLifecycleHarness
{
    private static int _passed, _trapCount;
    private sealed class StaticTrap { internal static int Value; static StaticTrap() { _trapCount++; Value=1; } }
    private sealed class State
    {
        internal static int Created;
        internal bool Equipped;
        internal readonly Dictionary<string, object> Owners = new Dictionary<string, object>();
        internal State() { Created++; }
        internal int DangerousProperty { get { throw new Exception("Getter must never execute"); } }
    }
    private static void Check(bool value, string label) { if (!value) throw new Exception(label); }
    private static void Reject(Action action) { bool failed = false; try { action(); } catch (Exception) { failed = true; } Check(failed, "Expected rejection"); }
    private static LifecycleEvidence Empty()
    {
        var e = new LifecycleEvidence(); e.Empty("ms.tome", "existing map.TryGetValue"); return e;
    }
    private static string Sample(LifecycleWindow w, double t, Func<LifecycleEvidence> read, string character = "PoVPlus_Test", string world = "PoVPlus_Lab", bool ready = true)
    { return w.Sample(t, ready, character, world, read, "2026-10-26T00:00:00Z"); }
    private static void Test(string label, Action test) { test(); _passed++; Console.WriteLine("PASS " + label); }
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length == 1 && args[0] == "--json")
            {
                var e = new LifecycleEvidence(); e.Known("quoted", "quote\" newline\n tab\t slash\\", "fixture field"); e.Empty("empty", "dictionary"); e.Unknown("missing", "table", "No existing entry");
                Console.WriteLine(e.Serialize()); return 0;
            }
            Test("shares established identity constants and policy", () => {
                var o = new DiagnosticObservation { HasPlayer=true, PlayerReady=true, WorldReady=true, InventoryReady=true, DescriptorsReady=true, PrefabsReady=true, Character="PoVPlus_Test", World="PoVPlus_Lab", MissingDetail="fixture" };
                Check(DiagnosticSession.Authorized(o), "Established authorization");
            });
            Test("exact identities allow capture", () => { int calls = 0; var w = new LifecycleWindow(); Check(Sample(w, 0, () => { calls++; return Empty(); }) != null && calls == 1, "Capture"); });
            foreach (var pair in new[] { new[] {"povplus_Test","PoVPlus_Lab"}, new[] {"PoVPlus_Test","POVPlus_Lab"}, new[] {"other","PoVPlus_Lab"}, new[] {"PoVPlus_Test","other"}, new[] {(string)null,"PoVPlus_Lab"} })
            {
                var identity = pair;
                Test("reject identity " + string.Join("/", identity), () => { int calls=0;var w=new LifecycleWindow();Check(Sample(w,0,()=>{calls++;return Empty();},identity[0],identity[1])==null && w.Finished && calls==0,"No read"); });
            }
            Test("revalidates every sample and never resumes after mismatch", () => { int calls=0;var w=new LifecycleWindow();Func<LifecycleEvidence> f=()=>{calls++;return Empty();};Sample(w,0,f);Sample(w,1,f,"other");Sample(w,2,f);Check(calls==1 && w.Finished,"Permanent reject"); });
            Test("unready state is not empty and does not invoke capture", () => { var w=new LifecycleWindow();Check(Sample(w,0,()=>{throw new Exception();},ready:false)==null && !w.Finished,"Deferred"); });
            Test("menu waiting expires rather than polling forever", () => {var w=new LifecycleWindow();Sample(w,0,Empty,ready:false);Sample(w,600,Empty,ready:false);Check(w.Finished,"Budget");});
            Test("one sample per second",()=>{var w=new LifecycleWindow();Check(Sample(w,0,Empty)!=null && Sample(w,0.1,Empty)==null && Sample(w,1,Empty)!=null,"Interval");});
            Test("sample count bounded at120",()=>{var w=new LifecycleWindow();int calls=0;for(int i=0;i<125;i++)Sample(w,i,()=>{calls++;var e=new LifecycleEvidence();e.Known("changing",calls.ToString(),"fixture");return e;});Check(calls==120 && w.Finished,"Sample bound");});
            Test("backward clock fails closed",()=>{var w=new LifecycleWindow();Sample(w,2,Empty);Sample(w,1,Empty);Check(w.Finished,"Clock");});
            Test("nonfinite clock fails closed",()=>{foreach(double n in new[]{double.NaN,double.PositiveInfinity,-1}){var w=new LifecycleWindow();Sample(w,n,Empty);Check(w.Finished,"Bad clock");}});
            Test("exception aborts without retry",()=>{var w=new LifecycleWindow();Sample(w,0,()=>{throw new Exception("fixture");});Check(Sample(w,1,Empty)==null && w.StopReason.Contains("ObservationFailure"),"Stop");});
            Test("deterministic ordinal serialization and JSON escaping",()=>{var a=new LifecycleEvidence();var b=new LifecycleEvidence();a.Known("z","\"\n\\","field");a.Empty("a","map");b.Empty("a","map");b.Known("z","\"\n\\","field");Check(a.Serialize()==b.Serialize() && a.Serialize().Contains("\\u000a") && a.Serialize().StartsWith("{\"a\":"),"Canonical serialization");});
            Test("invariant numeric serialization",()=>{var old=CultureInfo.CurrentCulture;try{CultureInfo.CurrentCulture=new CultureInfo("fr-FR");Check(LifecycleEvidence.Number(1.25f)=="1.25","Invariant");}finally{CultureInfo.CurrentCulture=old;}});
            Test("empty occupied and unknown remain distinct with provenance",()=>{var e=new LifecycleEvidence();e.Empty("empty","observed dictionary");e.Occupied("worn","native-control","mapping");e.Unknown("missing","weak table","No existing entry");var s=e.Serialize();Check(s.Contains("\"empty\"")&&s.Contains("\"occupied\"")&&s.Contains("\"unknown\"")&&s.Contains("No existing entry"),"Evidence states");});
            Test("unknown requires reason and all fields require source",()=>{Reject(()=>new LifecycleEvidence().Unknown("x","table",null));Reject(()=>new LifecycleEvidence().Known("x","y",null));});
            Test("weak-table miss never creates runtime entry",()=>{var table=new ConditionalWeakTable<object,State>();var owner=new object();int before=State.Created;Reject(()=>LifecycleRead.ExistingEntry(table,owner));State value;Check(!table.TryGetValue(owner,out value)&&State.Created==before,"No lazy creation");});
            Test("existing native mapping and flags remain unchanged",()=>{var table=new ConditionalWeakTable<object,State>();var owner=new object();var state=new State{Equipped=true};var native=new object();state.Owners.Add("tome",native);table.Add(owner,state);Check(ReferenceEquals(LifecycleRead.ExistingEntry(table,owner),state),"Same entry");Check(ReferenceEquals(LifecycleRead.Field(state.GetType(),"Owners",state),state.Owners)&&state.Owners.Count==1&&state.Equipped&&ReferenceEquals(state.Owners["tome"],native),"No mutation");});
            Test("field reader cannot execute a property getter",()=>{var state=new State();Reject(()=>LifecycleRead.Field(state.GetType(),"DangerousProperty",state));});
            Test("generic field reader refuses static initialization",()=>{Reject(()=>LifecycleRead.Field(typeof(StaticTrap),"Value",null));Check(_trapCount==0,"No static initializer");});
            Test("foreign weak-table contract rejected",()=>{Reject(()=>LifecycleRead.ExistingEntry(new Dictionary<object,object>(),new object()));});
            Test("stable and transition phases reflect observed state",()=>{var w=new LifecycleWindow();Check(Sample(w,0,Empty).Contains("\"initial\""),"Initial");Check(Sample(w,1,Empty).Contains("\"stable-sample\""),"Stable");Check(Sample(w,2,()=>{var e=new LifecycleEvidence();e.Occupied("ms.tome","native","map");return e;}).Contains("\"transition\""),"Transition");});
            Test("unchanged evidence is deduplicated with bounded heartbeat",()=>{var w=new LifecycleWindow();Sample(w,0,Empty);Sample(w,1,Empty);Check(Sample(w,2,Empty)==null && Sample(w,11,Empty)!=null,"Heartbeat");});
            Test("UTF8 evidence byte budget includes non-ASCII data",()=>{var w=new LifecycleWindow();Sample(w,0,()=>{var e=new LifecycleEvidence();e.Known("unicode",new string('\u6f22',22000),"fixture");return e;});Check(w.Finished&&w.StopReason=="EvidenceSizeLimit","Byte bound");});
            Test("evidence size bounded",()=>{var w=new LifecycleWindow();Sample(w,0,()=>{var e=new LifecycleEvidence();e.Known("huge",new string('x',66000),"fixture");return e;});Check(w.Finished&&w.StopReason=="EvidenceSizeLimit","Size");});
            Console.WriteLine("All "+_passed+" lifecycle policy/read fixtures passed. No Unity/mod execution; runtime purity is not proved.");return 0;
        }
        catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
}
