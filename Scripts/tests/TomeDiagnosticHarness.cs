using System;
using System.Collections.Generic;
using PoVPlus.Diagnostics;

internal static class TomeDiagnosticHarness
{
    private static int _passed;
    private static DiagnosticObservation Ready()
    {
        return new DiagnosticObservation { HasPlayer = true, PlayerReady = true, WorldReady = true,
            InventoryReady = true, DescriptorsReady = true, PrefabsReady = true,
            Character = "PoVPlus_Test", World = "PoVPlus_Lab" };
    }
    private static void Check(bool condition, string reason) { if (!condition) throw new Exception(reason); }
    private static void Test(string name, Action test) { test(); _passed++; Console.WriteLine("PASS " + name); }
    private static int Main()
    {
        try
        {
            Test("valid identity captures exactly once", () => {
                int count = 0; var session = new DiagnosticSession(_ => { });
                session.Tick(0, Ready(), () => count++); session.Tick(1, Ready(), () => count++);
                Check(count == 1 && session.Captured, "Single snapshot invariant"); });
            Test("authorized character and corrected world are exact and accepted", () => {
                Check(DiagnosticSession.ExpectedCharacter == "PoVPlus_Test" &&
                    DiagnosticSession.ExpectedWorld == "PoVPlus_Lab", "Authorized literals");
                Check(DiagnosticSession.Authorized(Ready()), "Authorized identity"); });
            Test("previous uppercase world is rejected with case-only evidence before snapshot", () => {
                var rows = new List<string>(); var session = new DiagnosticSession(rows.Add); var o = Ready();
                o.World = "POVPlus_Lab"; int snapshots = 0;
                session.Tick(0, o, () => snapshots++);
                Check(!DiagnosticSession.Authorized(o) && session.Finished && !session.Captured && snapshots == 0, "No alias/snapshot");
                Check(rows.Exists(s => s.Contains("worldExact=False") && s.Contains("worldCaseOnly=True")) &&
                    rows.Exists(s => s.StartsWith("ABORT IdentityMismatch")), "Case-only rejection logged"); });
            Test("unavailable character or world never authorizes a snapshot", () => {
                foreach (string absent in new[] { null, "", "..." })
                    foreach (bool character in new[] { true, false }) {
                        var session = new DiagnosticSession(_ => { }, 10); var o = Ready(); int snapshots = 0;
                        if (character) o.Character = absent; else o.World = absent;
                        session.Tick(0, o, () => snapshots++); session.Tick(10, o, () => snapshots++);
                        Check(session.Finished && !session.Captured && snapshots == 0, "Unavailable identity fails closed");
                    } });
            foreach (var label in new[] { "character", "world", "case", "whitespace" })
                Test("reject " + label, () => {
                    var rows = new List<string>(); var o = Ready();
                    if (label == "character") o.Character = "other";
                    if (label == "world") o.World = "other";
                    if (label == "case") o.World = "POVPlus_Lab";
                    if (label == "whitespace") o.Character += " ";
                    var session = new DiagnosticSession(rows.Add);
                    session.Tick(0, o, () => { throw new Exception("Unauthorized callback"); });
                    Check(session.Finished && !session.Captured && rows.Exists(s => s.StartsWith("ABORT IdentityMismatch")), "Explicit mismatch");
                    if (label == "case") Check(rows.Exists(s => s.Contains("worldCaseOnly=True")), "Case-only evidence"); });
            Test("unavailable identifiers defer and timeout precisely", () => {
                var rows = new List<string>(); var session = new DiagnosticSession(rows.Add, 10); var o = Ready(); o.World = null;
                session.Tick(0, o, () => { }); Check(!session.Finished, "Unavailable is not mismatch");
                session.Tick(10, o, () => { }); Check(rows.Exists(s => s.Contains("InitializationTimeout pending=WaitingForIdentifiers")), "Precise timeout"); });
            Test("transient identity before initialization does not abort", () => {
                var session = new DiagnosticSession(_ => { }); var o = Ready(); o.Character = "pending"; o.PlayerReady = false;
                session.Tick(0, o, () => { }); Check(!session.Finished, "Premature mismatch rejection");
                session.Tick(1, Ready(), () => { }); Check(session.Captured, "Ready recovery"); });
            Test("inventory readiness precedes mismatch decision", () => {
                var session = new DiagnosticSession(_ => { }); var o = Ready(); o.Character = "pending"; o.InventoryReady = false;
                session.Tick(0, o, () => { }); Check(!session.Finished, "UI readiness"); });
            Test("missing descriptors prevent snapshot", () => {
                var session = new DiagnosticSession(_ => { }); var o = Ready(); o.DescriptorsReady = false; o.MissingDetail = "fixture descriptors pending";
                session.Tick(0, o, () => { throw new Exception("Early snapshot"); }); Check(!session.Finished, "Descriptor wait");
                session.Tick(1, Ready(), () => { }); Check(session.Captured, "Descriptor ready"); });
            Test("missing prefab prevents snapshot", () => {
                var session = new DiagnosticSession(_ => { }); var o = Ready(); o.PrefabsReady = false;
                session.Tick(0, o, () => { throw new Exception("Early snapshot"); }); Check(!session.Finished, "Prefab wait"); });
            Test("menu wait does not pretend a player exists", () => {
                var session = new DiagnosticSession(_ => { }); session.Tick(1000, new DiagnosticObservation(), () => { throw new Exception("No player"); });
                Check(!session.Finished, "Inert menu wait"); });
            Test("failure logs expected observed APIs and comparisons", () => {
                var rows = new List<string>(); var session = new DiagnosticSession(rows.Add); var o = Ready(); o.World = "different\nworld";
                session.Tick(0, o, () => { }); var line = rows[0];
                Check(line.Contains("expectedWorld=") && line.Contains("observedWorld=") && line.Contains("worldExact=False") &&
                    line.Contains("Player.GetPlayerName") && line.Contains("ZNet.GetWorldName") && !line.Contains("\n"), "Actionable bounded logging"); });
            Test("no per-poll duplicate logging", () => {
                var rows = new List<string>(); var session = new DiagnosticSession(rows.Add); var o = new DiagnosticObservation();
                for (int i = 0; i < 50; i++) session.Tick(i, o, () => { }); Check(rows.Count == 1, "Log spam"); });
            Test("snapshot exceptions stop without retry", () => {
                int count = 0; var rows = new List<string>(); var session = new DiagnosticSession(rows.Add);
                session.Tick(0, Ready(), () => { count++; throw new Exception("fixture"); }); session.Tick(1, Ready(), () => count++);
                Check(count == 1 && !session.Captured && rows.Exists(s => s.StartsWith("ABORT SnapshotException")), "No retry"); });
            Test("flapping and invalid clock fail closed", () => {
                var session = new DiagnosticSession(_ => { }); var o = Ready(); o.InventoryReady = false;
                for (int i = 0; i < 30; i++) { o.Character = "pending" + i; session.Tick(i, o, () => { }); }
                Check(session.Finished && !session.Captured, "Bounded transitions");
                var bad = new DiagnosticSession(_ => { }); bad.Tick(double.NaN, Ready(), () => { }); Check(bad.Finished, "Invalid clock"); });
            Console.WriteLine("All " + _passed + " offline diagnostic policy tests passed. No Unity/game code executed.");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
