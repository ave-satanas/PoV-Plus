// Independent falsification: synthetic objects only. No game/third-party assemblies executed.
using System;
using System.Collections.Generic;
using System.Reflection;
using PoVPlus.Diagnostics.Provenance;
internal static class Independent
{
    internal class Item { internal bool m_equipped; internal Dictionary<string,string> m_customData=new Dictionary<string,string>(); }
    internal sealed class DerivedItem:Item {}
    internal sealed class Evil:Item
    {
        internal static int Calls;
        public override bool Equals(object value){Calls++;return true;}
        public override int GetHashCode(){return 1;}
    }
    internal class Data { internal Dictionary<string,Item> EquippedItems=new Dictionary<string,Item>(); }
    internal sealed class DerivedData:Data {}
    internal class Owner { internal Dictionary<string,string> m_customData=new Dictionary<string,string>(); }
    static FieldInfo F(Type type,string name){return type.GetField(name,BindingFlags.NonPublic|BindingFlags.Instance);}
    sealed class Rig
    {
        internal Data Root=new Data(); internal Owner Owner=new Owner(); internal Item Book=new Item(),Other=new Item();
        internal ExistingStateReader Reader=new ExistingStateReader(typeof(Data),typeof(Owner),typeof(Item),F(typeof(Data),"EquippedItems"),F(typeof(Item),"m_equipped"),F(typeof(Item),"m_customData"),F(typeof(Owner),"m_customData"));
        internal Rig(){Book.m_customData["DO_CustomSlotGuid"]="a";Other.m_customData["DO_CustomSlotGuid"]="b";}
        internal Snapshot Read(){return Reader.Read(Root,Owner,Book,true,true);}
        internal void Equip(){Root.EquippedItems["tome"]=Book;Book.m_equipped=true;Owner.m_customData["DO_Equipped_tome"]="a";}
        internal void Remove(){Root.EquippedItems.Remove("tome");Book.m_equipped=false;Owner.m_customData.Clear();}
        internal Transition Ev(int sequence,Phase phase,Snapshot snapshot,Kind kind=Kind.Equip){return new Transition("MS",sequence,"tx",kind,phase,snapshot.TrackedAlias,snapshot,"independent driver");}
    }
    static int Main(string[] args)
    {
        try{
            string test=args[0];var r=new Rig();
            if(test=="subclass-entry"){
                r.Root.EquippedItems["utility"]=new DerivedItem();
                Require(!r.Read().LocalReadValid,"subclass entry accepted");
            }else if(test=="subclass-tracked"){
                r.Book=new DerivedItem();Require(!r.Read().LocalReadValid,"subclass tracked accepted");
            }else if(test=="subclass-root"){
                r.Root=new DerivedData();Require(!r.Read().LocalReadValid,"subclass root accepted");
            }else if(test=="virtual-equals"){
                r.Root.EquippedItems["utility"]=new Evil();
                r.Reader.DuringRead=()=>r.Root.EquippedItems["utility"]=new Evil();
                var snapshot=r.Read();Require(Evil.Calls==0,"foreign Equals executed before invalid entry rejection");
                Require(!snapshot.LocalReadValid,"invalid delta accepted");
            }else if(test=="dead-root-latch"){
                var capture=new ReturnedRoot();var owner=new Owner();var data=new Data();capture.ObserveReturn(owner,data,true,0);
                var weak=(WeakReference)F(typeof(ReturnedRoot),"_root").GetValue(capture);weak.Target=null;
                Require(capture.Borrow(owner,1)==null&&capture.Invalid,"missing root failed to invalidate latch");
                GC.KeepAlive(data);
            }else if(test=="distinct-ordered-stale"){
                var baseline=r.Read();var begin=r.Read();r.Equip();var returned=r.Read();r.Remove();
                Require(!ReferenceEquals(baseline,begin)&&!ReferenceEquals(begin,returned),"samples not distinct");
                var ledger=new ProvenanceLedger();ledger.Baseline(baseline,false,false);
                ledger.Add(r.Ev(1,Phase.Begin,begin));ledger.Add(r.Ev(2,Phase.Return,returned));
                Require(ledger.Classification=="EquipEndpointAgreement"&&!r.Root.EquippedItems.ContainsKey("tome"),"stale ordered counterexample not reproduced");
                Console.WriteLine("three actual reader invocations in strict order; final driver ownership empty; "+ledger.Classification);
            }else if(test=="occupied-same-item-swap"){
                r.Equip();var ledger=new ProvenanceLedger();ledger.Baseline(r.Read(),false,false);
                ledger.Add(r.Ev(1,Phase.Begin,r.Read(),Kind.Swap));
                Require(ledger.Classification=="SwapAttempted","occupied same-item Swap Begin rejected");
                ledger.Add(r.Ev(2,Phase.Return,r.Read(),Kind.Swap));
                Require(ledger.Classification=="NoChangeInRecordedProjection","no-op Swap rejected");
                Console.WriteLine("Swap Begin is reachable from occupied tracked-item state; no replacement proved");
            }else if(test=="same-count-foreign-change"){
                r.Root.EquippedItems["utility"]=r.Other;var ledger=new ProvenanceLedger();ledger.Baseline(r.Read(),false,false);
                r.Root.EquippedItems["utility"]=new Item();ledger.Add(r.Ev(1,Phase.Begin,r.Read()));
                Require(ledger.Classification=="EquipAttempted","same-count foreign change not absorbed");
                Console.WriteLine("foreign value changed BETWEEN records but projection count stayed equal; "+ledger.Classification);
            }else if(test=="reachable-add"){
                for(int i=0;i<47;i++)r.Root.EquippedItems["slot-"+i]=r.Other;
                r.Read();Require(r.Root.EquippedItems.Count==47&&!r.Root.EquippedItems.ContainsKey("__crossreview"),"observer mutated source map at reachable count47");
            }else if(test=="reachable-clear"){
                for(int i=0;i<47;i++)r.Root.EquippedItems["slot-"+i]=r.Other;
                r.Read();var type=r.Root.EquippedItems.GetType();
                var entries=(Array)(type.GetField("_entries",BindingFlags.NonPublic|BindingFlags.Instance)??type.GetField("entries",BindingFlags.NonPublic|BindingFlags.Instance)).GetValue(r.Root.EquippedItems);
                var entry=entries.GetValue(0);var value=entry.GetType().GetField("value",BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance).GetValue(entry);
                Require(ReferenceEquals(value,r.Other),"observer cleared source backing array at reachable count47");
            }else if(test=="original-clear-trigger"){
                r.Root.EquippedItems["slot-0"]=r.Other;
                var type=r.Root.EquippedItems.GetType();
                var count=type.GetField("_count",BindingFlags.NonPublic|BindingFlags.Instance)??type.GetField("count",BindingFlags.NonPublic|BindingFlags.Instance);
                count.SetValue(r.Root.EquippedItems,int.MaxValue);
                var snap=r.Read();
                Require(r.Root.EquippedItems["slot-0"]==r.Other,"original clear unexpectedly executed");
                Console.WriteLine("malformed-count trigger: "+snap.Reason+"; _entries="+(type.GetField("_entries",BindingFlags.NonPublic|BindingFlags.Instance)!=null));
            }else if(test=="original-add-trigger"){
                r.Root.EquippedItems["slot-0"]=r.Other;
                var type=r.Root.EquippedItems.GetType();
                var count=type.GetField("_count",BindingFlags.NonPublic|BindingFlags.Instance)??type.GetField("count",BindingFlags.NonPublic|BindingFlags.Instance);
                var free=type.GetField("_freeCount",BindingFlags.NonPublic|BindingFlags.Instance)??type.GetField("freeCount",BindingFlags.NonPublic|BindingFlags.Instance);
                free.SetValue(r.Root.EquippedItems,1-int.MaxValue);
                Require(r.Root.EquippedItems.Count==int.MaxValue,"malformed Count trigger not established");
                var snap=r.Read();
                Require((int)count.GetValue(r.Root.EquippedItems)==2,"original Add payload did not mutate malformed dictionary");
                Console.WriteLine("malformed-freeCount trigger reached Add; storage count now2; "+snap.Reason);
            }else throw new Exception("unknown independent case");
            Console.WriteLine("PASS "+test);return 0;
        }catch(Exception error){Console.Error.WriteLine("INDEPENDENT FAILURE: "+error.Message);return 1;}
    }
    static void Require(bool condition,string message){if(!condition)throw new Exception(message);}
}
