using System;
using System.Collections.Generic;
using System.Reflection;
using PoVPlus.Diagnostics;

// Proposed decision rule, intentionally fixture-only. NOT wired to the runtime observer.
// Inputs are independently supported evidence obligations, not default bools or UI guesses.
internal static class TomeNeutralHarness
{
    private enum Evidence { Unknown, Neutral, NonNeutral, Conflict }
    private static readonly string[] Required = { "identity", "freshComplete", "initializedOwner", "msTome", "savedBinding", "flags", "customOwners", "manifest", "effects", "renderedConsistency", "settledReload" };
    private static int passed, initialized;
    private sealed class ForeignHolder
    {
        internal static object Table;
        static ForeignHolder() { initialized++; Table = new object(); }
    }
    private static string Assess(Dictionary<string,Evidence> data)
    {
        if (data == null) return "unknown";
        Evidence value;
        if (!data.TryGetValue("identity", out value) || value != Evidence.Neutral ||
            !data.TryGetValue("freshComplete", out value) || value != Evidence.Neutral) return "unknown";
        bool missing=false, occupied=false;
        foreach(var key in Required)
        {
            if (!data.TryGetValue(key,out value)) value=Evidence.Unknown;
            if (value==Evidence.Conflict) return "unknown";
            if (value==Evidence.Unknown) missing=true;
            if (value==Evidence.NonNeutral) occupied=true;
        }
#if MUTATE_UNKNOWN_TO_EMPTY
        missing=false; // Deliberate disposable mutant; tests MUST kill this assumption.
#endif
        if (occupied) return "confirmed-non-neutral";
        return missing ? "unknown" : "confirmed-neutral";
    }
    private static Dictionary<string,Evidence> Complete()
    { var d=new Dictionary<string,Evidence>();foreach(var key in Required)d[key]=Evidence.Neutral;return d; }
    private static void Check(bool ok,string label){if(!ok)throw new Exception(label);passed++;Console.WriteLine("PASS "+label);}
    private static void Case(string label, Action<Dictionary<string,Evidence>> arrange, string expected)
    {var d=Complete();arrange(d);Check(Assess(d)==expected,label);}
    private static int Main()
    {
        try {
            Case("consistent occupied",d=>{d["msTome"]=Evidence.NonNeutral;d["flags"]=Evidence.NonNeutral;d["savedBinding"]=Evidence.NonNeutral;},"confirmed-non-neutral");
            Case("fully evidenced empty fixture",d=>{},"confirmed-neutral");
            Case("missing ownership dictionary is not empty",d=>d.Remove("msTome"),"unknown");
            Case("missing initialization evidence",d=>d.Remove("initializedOwner"),"unknown");
            Case("slot/flag contradiction",d=>{d["msTome"]=Evidence.NonNeutral;d["flags"]=Evidence.Conflict;},"unknown");
            Case("stale rendered binding",d=>d["renderedConsistency"]=Evidence.Unknown,"unknown");
            Case("missing manifest item",d=>d["manifest"]=Evidence.Unknown,"unknown");
            Case("duplicate GUID/conflicting identities",d=>d["manifest"]=Evidence.Conflict,"unknown");
            Case("apparently empty but custom inventory unknown",d=>d["customOwners"]=Evidence.Unknown,"unknown");
            Case("partial capture invalidates assessment",d=>d["freshComplete"]=Evidence.Unknown,"unknown");
            var unchanged=Complete();Check(Assess(unchanged)==Assess(unchanged)&&unchanged.Count==Required.Length,"repeat evaluation deterministic and non-writing");
            Case("interrupted/exhausted collection cannot use old sample",d=>d["freshComplete"]=Evidence.Unknown,"unknown");
            Case("unavailable residual effects",d=>d["effects"]=Evidence.Unknown,"unknown");
            Case("saved binding occupied despite empty visible slot",d=>d["savedBinding"]=Evidence.NonNeutral,"confirmed-non-neutral");
            Case("two agreeing empty sources insufficient",d=>{d.Remove("manifest");d.Remove("settledReload");},"unknown");
            Case("wrong identity overrides occupied evidence",d=>{d["identity"]=Evidence.Conflict;d["msTome"]=Evidence.NonNeutral;},"unknown");
            foreach(var key in Required) Case("mandatory obligation "+key,d=>d.Remove(key),"unknown");
            // A matching public marker can exist without the holder's cctor having run.
            AppDomain.CurrentDomain.SetData("PoVPlus.Fixture.Owner","Magic_Supremacy");
            Check((string)AppDomain.CurrentDomain.GetData("PoVPlus.Fixture.Owner")=="Magic_Supremacy"&&initialized==0,"matching marker is not initialization proof");
            bool refused=false;
            try { LifecycleRead.Field(typeof(ForeignHolder),"Table",null); } catch(InvalidOperationException){refused=true;}
            Check(refused&&initialized==0,"corrected static-read policy refuses without initialization");
            // Positive fault demonstration in fixture only: old reflection pattern creates state.
            typeof(ForeignHolder).GetField("Table",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null);
            Check(initialized==1,"old static reflection path demonstrably initializes fixture state");
            AppDomain.CurrentDomain.SetData("PoVPlus.Fixture.Owner",null);
            Console.WriteLine("All "+passed+" neutral/adversarial fixture checks passed. No live equipment inference.");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
