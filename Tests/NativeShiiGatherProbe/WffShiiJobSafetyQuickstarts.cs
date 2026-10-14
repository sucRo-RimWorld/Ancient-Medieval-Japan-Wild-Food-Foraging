// Native 1.6 isolated Job safety probes. No production Def or food balance.
using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using RimWorks.Quickstarts;
using RimWorks.Quickstarts.Verification;
using Verse;
using Verse.AI;

namespace AMJ.WildFoodForaging.NativeTests
{
    public abstract class WffShiiJobSafetyBase : AbstractQuickstart
    {
        protected Map map;
        protected Pawn first;
        protected Pawn second;
        protected Plant tree;
        protected ThingDef originalWood;
        protected CompShiiGatherState state;
        protected DesignationDef gather;
        protected JobDef jobDef;
        protected int steelBefore;
        protected bool workGiverOffered;
        protected bool jobStarted;
        protected string setupFailure;

        protected abstract bool RequireSecondPawn { get; }

        public override string seed => "AMJ-WFF-SHII-20261014";
        public override float planetCoverage => 0.30f;
        public override int mapSize => 150;
        protected int SteelCount => map.listerThings.ThingsOfDef(ThingDefOf.Steel)
            .Sum(item => item.stackCount);

        public override void PostApplyConfiguration()
        {
            PlanetTile target = PlanetTile.Invalid;
            for (int i = 0; i < Find.WorldGrid.TilesCount; i++)
            {
                SurfaceTile tile = Find.WorldGrid[i] as SurfaceTile;
                if (tile == null || tile.PrimaryBiome?.defName != "AMJ_WarmTemperateForest" ||
                    tile.WaterCovered || tile.hilliness == Hilliness.Impassable)
                    continue;
                if (TileFinder.IsValidTileForNewSettlement(tile.tile))
                {
                    target = tile.tile;
                    break;
                }
            }
            if (!target.Valid)
                throw new InvalidOperationException("No naturally generated Warm tile");
            Find.GameInitData.startingTile = target;
        }

        public override void PostLoaded()
        {
            map = Find.CurrentMap;
            if (map == null)
            {
                setupFailure = "No game map";
                return;
            }
            ThingDef shii = DefDatabase<ThingDef>.GetNamedSilentFail("AMJ_Tree_Shii");
            gather = DefDatabase<DesignationDef>.GetNamedSilentFail("AMJ_GatherShiiNuts");
            jobDef = DefDatabase<JobDef>.GetNamedSilentFail("AMJ_GatherShiiNuts");
            List<Pawn> pawns = map.mapPawns.FreeColonistsSpawned.ToList();
            first = pawns.FirstOrDefault();
            second = pawns.Skip(1).FirstOrDefault();
            if (shii?.plant == null || gather == null || jobDef == null ||
                first == null || (RequireSecondPawn && second == null))
            {
                setupFailure = "Required tree/test Def/pawn missing";
                return;
            }
            originalWood = shii.plant.harvestedThingDef;
            foreach (Plant candidate in map.listerThings.ThingsOfDef(shii).OfType<Plant>()
                     .Where(x => x.Spawned && !x.Destroyed && !x.Blighted && !x.IsBurning())
                     .OrderBy(x => x.Position.DistanceToSquared(first.Position)))
            {
                candidate.Growth = 1f; // only on disposable test map
                var comp = candidate.TryGetComp<CompShiiGatherState>();
                if (comp != null && comp.CanGather() &&
                    first.CanReserveAndReach(candidate, PathEndMode.Touch, Danger.Some) &&
                    (!RequireSecondPawn ||
                     second.CanReserveAndReach(candidate, PathEndMode.Touch, Danger.Some)))
                {
                    tree = candidate;
                    state = comp;
                    break;
                }
            }
            if (tree == null)
            {
                setupFailure = "No mutually reachable mature Shii";
                return;
            }
            steelBefore = SteelCount;
            map.designationManager.AddDesignation(new Designation(tree, gather));
            var scanner = new WorkGiver_GatherShiiNuts();
            workGiverOffered = scanner.PotentialWorkThingsGlobal(first).Contains(tree);
            Job job = scanner.JobOnThing(first, tree);
            if (!workGiverOffered || job == null)
            {
                setupFailure = "Primary WorkGiver failed before reservation";
                return;
            }
            BeforeFirstJob(scanner);
            first.jobs.StartJob(job, JobCondition.InterruptForced);
            jobStarted = first.jobs.curJob?.def == jobDef;
            AfterFirstJob(scanner);
        }

        protected virtual void BeforeFirstJob(WorkGiver_GatherShiiNuts scanner) { }
        protected virtual void AfterFirstJob(WorkGiver_GatherShiiNuts scanner) { }

        protected void CommonAssertions(QuickstartVerification report)
        {
            report.Assert("genuine Warm map generated",
                () => map != null && map.Biome?.defName == "AMJ_WarmTemperateForest");
            report.Assert("diagnostic setup completed",
                () => setupFailure == null && tree != null && first != null);
            report.Assert("real pawn Job was started through WorkGiver",
                () => workGiverOffered && jobStarted);
            report.Assert("Environment timber Definition and tree identity unchanged",
                () => tree != null && tree.Spawned && !tree.Destroyed &&
                    tree.def.defName == "AMJ_Tree_Shii" &&
                    tree.def.plant.harvestedThingDef == originalWood &&
                    tree.def.plant.harvestYield == 42);
        }
    }

    public sealed class WffShiiRaceQuickstart : WffShiiJobSafetyBase
    {
        private bool secondEligibleBefore;
        private bool secondRejectedAfter;

        protected override bool RequireSecondPawn => true;
        public override TaggedString description =>
            "Two real colonists contend for a single Shii gather designation";
        public override int ticksBeforeVerify => 1200;

        protected override void BeforeFirstJob(WorkGiver_GatherShiiNuts scanner)
        {
            secondEligibleBefore = scanner.JobOnThing(second, tree) != null;
        }

        protected override void AfterFirstJob(WorkGiver_GatherShiiNuts scanner)
        {
            secondRejectedAfter = scanner.JobOnThing(second, tree) == null;
        }

        public override QuickstartVerification Verify()
        {
            var report = new QuickstartVerification();
            CommonAssertions(report);
            report.Assert("second pawn had a valid independent route before reservation",
                () => second != null && secondEligibleBefore);
            report.Assert("second pawn WorkGiver rejects tree reserved by first Job",
                () => secondRejectedAfter);
            report.Assert("concurrent competition yields exactly one diagnostic product",
                () => map != null && SteelCount - steelBefore == 1);
            report.Assert("completed job clears designation on shared tree",
                () => tree != null && gather != null &&
                    map.designationManager.DesignationOn(tree, gather) == null);
            report.Assert("per-tree cooldown prevents second same-tree work",
                () => tree != null && state != null && !state.CanGather() &&
                    new WorkGiver_GatherShiiNuts().JobOnThing(second, tree) == null);
            return report;
        }
    }

    public sealed class WffShiiCancelQuickstart : WffShiiJobSafetyBase
    {
        protected override bool RequireSecondPawn => false;
        public override TaggedString description =>
            "Cancel active Shii gather Job before minimum work duration";
        public override int ticksBeforeVerify => 10; // strictly less than 60 work ticks

        public override QuickstartVerification Verify()
        {
            var report = new QuickstartVerification();
            CommonAssertions(report);
            bool active = first != null && first.jobs.curJob?.def == jobDef;
            report.Assert("real pawn is still working before premature cancellation",
                () => active);
            if (active)
                first.jobs.StopAll();
            report.Assert("cancellation cannot produce diagnostic product",
                () => map != null && SteelCount == steelBefore);
            report.Assert("cancelled gathering order remains for future retry",
                () => tree != null && gather != null &&
                    map.designationManager.DesignationOn(tree, gather) != null);
            report.Assert("cancelled Job does not start a collection cooldown",
                () => state != null && state.CanGather());
            return report;
        }
    }
}
