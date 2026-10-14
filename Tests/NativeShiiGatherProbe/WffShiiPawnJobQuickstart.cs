// PRIVATE 1.6 DIAGNOSTIC: real pawn JobDriver with native game ticking.
// Not production, not food balance, not an old-save migration test.
using System;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using RimWorks.Quickstarts;
using RimWorks.Quickstarts.Verification;
using Verse;
using Verse.AI;

namespace AMJ.WildFoodForaging.NativeTests
{
    public sealed class WffShiiPawnJobQuickstart : AbstractQuickstart
    {
        private Pawn selectedPawn;
        private Plant selectedTree;
        private ThingDef originalWoodDef;
        private int steelBefore;
        private bool workGiverFound;
        private bool jobStarted;
        private string setupFailure;

        public override TaggedString description =>
            "Real pawn non-destructive Shii gathering Job on a generated Warm map";
        public override string seed => "AMJ-WFF-SHII-20261014";
        public override float planetCoverage => 0.30f;
        public override int mapSize => 150;
        public override int ticksBeforeVerify => 1800;

        public override void PostApplyConfiguration()
        {
            PlanetTile selected = PlanetTile.Invalid;
            for (int i = 0; i < Find.WorldGrid.TilesCount; i++)
            {
                SurfaceTile tile = Find.WorldGrid[i] as SurfaceTile;
                if (tile == null || tile.PrimaryBiome?.defName != "AMJ_WarmTemperateForest" ||
                    tile.WaterCovered || tile.hilliness == Hilliness.Impassable)
                    continue;
                if (TileFinder.IsValidTileForNewSettlement(tile.tile))
                {
                    selected = tile.tile;
                    break;
                }
            }
            if (!selected.Valid)
                throw new InvalidOperationException("No natural Warm settlement tile");
            Find.GameInitData.startingTile = selected;
        }

        public override void PostLoaded()
        {
            Map map = Find.CurrentMap;
            if (map == null)
            {
                setupFailure = "Generated map is missing";
                return;
            }
            ThingDef shii = DefDatabase<ThingDef>.GetNamedSilentFail("AMJ_Tree_Shii");
            DesignationDef gather = DefDatabase<DesignationDef>.GetNamedSilentFail("AMJ_GatherShiiNuts");
            JobDef gatherJob = DefDatabase<JobDef>.GetNamedSilentFail("AMJ_GatherShiiNuts");
            selectedPawn = map.mapPawns.FreeColonistsSpawned.FirstOrDefault();
            if (shii?.plant == null || gather == null || gatherJob == null || selectedPawn == null)
            {
                setupFailure = "Required Environment tree, test Def or colonist missing";
                return;
            }
            originalWoodDef = shii.plant.harvestedThingDef;
            foreach (Plant candidate in map.listerThings.ThingsOfDef(shii)
                     .OfType<Plant>()
                     .Where(p => p.Spawned && !p.Destroyed && !p.Blighted && !p.IsBurning())
                     .OrderBy(p => p.Position.DistanceToSquared(selectedPawn.Position)))
            {
                candidate.Growth = 1f; // only in the isolated disposable game map
                CompShiiGatherState comp = candidate.TryGetComp<CompShiiGatherState>();
                if (comp != null && comp.CanGather() &&
                    selectedPawn.CanReserveAndReach(candidate, PathEndMode.Touch, Danger.Some))
                {
                    selectedTree = candidate;
                    break;
                }
            }
            if (selectedTree == null)
            {
                setupFailure = "No reachable mature native Shii exists";
                return;
            }
            steelBefore = map.listerThings.ThingsOfDef(ThingDefOf.Steel)
                .Sum(thing => thing.stackCount);
            map.designationManager.AddDesignation(new Designation(selectedTree, gather));
            var scanner = new WorkGiver_GatherShiiNuts();
            workGiverFound = scanner.PotentialWorkThingsGlobal(selectedPawn).Contains(selectedTree);
            Job job = scanner.JobOnThing(selectedPawn, selectedTree);
            if (job == null)
            {
                setupFailure = "WorkGiver rejected valid designated native tree";
                return;
            }
            selectedPawn.jobs.StartJob(job, JobCondition.InterruptForced);
            jobStarted = selectedPawn.jobs.curJob?.def == gatherJob;
        }

        public override QuickstartVerification Verify()
        {
            var result = new QuickstartVerification();
            Map map = Find.CurrentMap;
            result.Assert("actual Warm biome map was generated",
                () => map != null && map.Biome?.defName == "AMJ_WarmTemperateForest");
            result.Assert("pawn job fixture configured successfully",
                () => setupFailure == null && selectedPawn != null && selectedTree != null);
            if (map == null || selectedTree == null || selectedPawn == null)
                return result;
            result.Assert("WorkGiver enumerated the explicit Shii gathering designation",
                () => workGiverFound);
            result.Assert("real colonist started custom JobDriver",
                () => jobStarted);
            int steelAfter = map.listerThings.ThingsOfDef(ThingDefOf.Steel)
                .Sum(thing => thing.stackCount);
            result.Assert("actual pawn work produced exactly one synthetic marker",
                () => steelAfter - steelBefore == 1);
            result.Assert("standing tree identity and wood product survived the Job",
                () => selectedTree.Spawned && !selectedTree.Destroyed &&
                    selectedTree.def.defName == "AMJ_Tree_Shii" &&
                    selectedTree.def.plant.harvestedThingDef == originalWoodDef &&
                    selectedTree.def.plant.harvestYield == 42);
            DesignationDef gather = DefDatabase<DesignationDef>.GetNamedSilentFail("AMJ_GatherShiiNuts");
            result.Assert("successful pawn Job removed one-shot gathering designation",
                () => gather != null &&
                    map.designationManager.DesignationOn(selectedTree, gather) == null);
            CompShiiGatherState state = selectedTree.TryGetComp<CompShiiGatherState>();
            result.Assert("per-tree cooldown blocks immediate second Job",
                () => state != null && !state.CanGather() &&
                    new WorkGiver_GatherShiiNuts().JobOnThing(selectedPawn, selectedTree) == null);
            // Different test boundaries: no save/reload, MO WoodChain, food or season.
            return result;
        }
    }
}
