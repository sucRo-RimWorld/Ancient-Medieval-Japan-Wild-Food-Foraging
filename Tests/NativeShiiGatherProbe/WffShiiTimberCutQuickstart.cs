// 1.6 native diagnostic only: gather then fell original Environment Shii.
using System;
using System.Linq;
using RimWorks.Quickstarts;
using RimWorks.Quickstarts.Verification;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace AMJ.WildFoodForaging.NativeTests
{
    public sealed class WffShiiTimberCutQuickstart : AbstractQuickstart
    {
        private Map map;
        private Pawn pawn;
        private Plant tree;
        private ThingDef wood;
        private bool directGathered;
        private bool cutJobStarted;
        private int initialTreeHitPoints;
        private IntVec3 pawnStartPosition;
        private IntVec3 treeStartPosition;
        private int steelBefore;
        private int woodBefore;
        private string setupFailure;

        public override TaggedString description =>
            "After separate test gathering, native Shii felling yields original timber only";
        public override string seed => "AMJ-WFF-SHII-20261014";
        public override float planetCoverage => 0.30f;
        public override int mapSize => 150;
        public override int ticksBeforeVerify => 6000;

        public override void PostApplyConfiguration()
        {
            PlanetTile selected = PlanetTile.Invalid;
            for (int i = 0; i < Find.WorldGrid.TilesCount; ++i)
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
                throw new InvalidOperationException("No natural Warm Shii settlement tile");
            Find.GameInitData.startingTile = selected;
        }

        private int Count(ThingDef def)
        {
            return map.listerThings.ThingsOfDef(def).Sum(item => item.stackCount);
        }

        public override void PostLoaded()
        {
            map = Find.CurrentMap;
            if (map == null)
            {
                setupFailure = "No map";
                return;
            }
            pawn = map.mapPawns.FreeColonistsSpawned.FirstOrDefault();
            ThingDef shii = DefDatabase<ThingDef>.GetNamedSilentFail("AMJ_Tree_Shii");
            WorkGiverDef cutDef = DefDatabase<WorkGiverDef>.AllDefsListForReading
                .FirstOrDefault(d => d.giverClass == typeof(WorkGiver_PlantsCut));
            var work = cutDef?.Worker as WorkGiver_PlantsCut;
            if (pawn == null || shii?.plant == null || work == null)
            {
                setupFailure = "Shii, pawn or native cutting WorkGiver missing";
                return;
            }
            wood = shii.plant.harvestedThingDef;
            if (wood == null || wood.defName != "WoodLog" || shii.plant.harvestYield != 42)
            {
                setupFailure = "Environment Shii wood primary differs from Vanilla baseline";
                return;
            }
            foreach (Plant candidate in map.listerThings.ThingsOfDef(shii)
                .OfType<Plant>().Where(p => p.Spawned && !p.Destroyed &&
                    !p.IsBurning() && !p.Blighted)
                .OrderBy(p => p.Position.DistanceToSquared(pawn.Position)))
            {
                candidate.Growth = 1f; // disposable map only; not natural maturity
                var comp = candidate.TryGetComp<CompShiiGatherState>();
                if (comp != null && comp.CanGather() &&
                    pawn.CanReserveAndReach(candidate, PathEndMode.Touch, Danger.Some))
                {
                    tree = candidate;
                    break;
                }
            }
            if (tree == null)
            {
                setupFailure = "No reachable eligible natural Shii";
                return;
            }
            initialTreeHitPoints = tree.HitPoints;
            pawnStartPosition = pawn.Position;
            treeStartPosition = tree.Position;
            steelBefore = Count(ThingDefOf.Steel);
            woodBefore = Count(wood);
            var gathering = tree.TryGetComp<CompShiiGatherState>();
            directGathered = gathering != null && gathering.TryGather(pawn);
            if (!directGathered || Count(ThingDefOf.Steel) - steelBefore != 1)
            {
                setupFailure = "Synthetic separate gather failed before timber cutting";
                return;
            }
            map.designationManager.AddDesignation(new Designation(tree, DesignationDefOf.CutPlant));
            Job job = work.JobOnThing(pawn, tree);
            if (job == null || job.def != JobDefOf.CutPlantDesignated)
            {
                setupFailure = "Native WorkGiver rejected designated Shii felling";
                return;
            }
            pawn.jobs.StartJob(job, JobCondition.InterruptForced);
            cutJobStarted = pawn.jobs.curJob?.def == JobDefOf.CutPlantDesignated;
        }

        public override QuickstartVerification Verify()
        {
            if (tree != null && map != null && pawn != null && wood != null)
            {
                var designation = map.designationManager.DesignationOn(
                    tree, DesignationDefOf.CutPlant);
                Log.Message("[WFF Shii Timber Diagnostic] setup=" +
                    (setupFailure ?? "OK") + "; treeSpawned=" + tree.Spawned +
                    "; treeDestroyed=" + tree.Destroyed +
                    "; startHP=" + initialTreeHitPoints + "; endHP=" + tree.HitPoints +
                    "; initialPawn=" + pawnStartPosition + "; finalPawn=" + pawn.Position +
                    "; treeCell=" + treeStartPosition +
                    "; currentJob=" + (pawn.jobs.curJob?.def?.defName ?? "none") +
                    "; cutDesignationPresent=" + (designation != null) +
                    "; gainedWood=" + (Count(wood) - woodBefore) +
                    "; extraMarkers=" + (Count(ThingDefOf.Steel) - steelBefore - 1));
            }
            var report = new QuickstartVerification();
            report.Assert("natural Warm biome generated",
                () => map != null && map.Biome?.defName == "AMJ_WarmTemperateForest");
            report.Assert("standing Environment tree had unchanged wood-primary identity",
                () => setupFailure == null && tree != null && wood != null &&
                    tree.def.defName == "AMJ_Tree_Shii" &&
                    tree.def.plant.harvestedThingDef == wood &&
                    tree.def.plant.harvestYield == 42);
            report.Assert("separate direct non-destructive gather produced one marker",
                () => directGathered);
            report.Assert("actual native designated cutting job started",
                () => cutJobStarted);
            report.Assert("native cut removes original Shii tree",
                () => tree != null && (!tree.Spawned || tree.Destroyed));
            report.Assert("normal cutting produced original timber item",
                () => map != null && wood != null && Count(wood) > woodBefore);
            report.Assert("cutting did not create any additional synthetic edible marker",
                () => map != null && Count(ThingDefOf.Steel) - steelBefore == 1);
            return report;
        }
    }
}
