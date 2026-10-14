// DEVELOPMENT-ONLY. Direct component smoke; not a pawn Job/save acceptance.
using System;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using RimWorks.Quickstarts;
using RimWorks.Quickstarts.Verification;
using Verse;

namespace AMJ.WildFoodForaging.NativeTests
{
    public sealed class WffShiiGatherQuickstart : AbstractQuickstart
    {
        public override TaggedString description => "Test-only standing Shii collection";
        public override string seed => "AMJ-WFF-SHII-20261014";
        public override float planetCoverage => 0.30f;
        public override int mapSize => 150;

        public override void PostApplyConfiguration()
        {
            PlanetTile target = PlanetTile.Invalid;
            for (int i = 0; i < Find.WorldGrid.TilesCount; i++)
            {
                SurfaceTile t = Find.WorldGrid[i] as SurfaceTile;
                if (t != null && t.PrimaryBiome?.defName == "AMJ_WarmTemperateForest" &&
                    !t.WaterCovered && t.hilliness != Hilliness.Impassable &&
                    TileFinder.IsValidTileForNewSettlement(t.tile))
                {
                    target = t.tile;
                    break;
                }
            }
            if (!target.Valid) throw new InvalidOperationException("No warm biome");
            Find.GameInitData.startingTile = target;
        }

        public override QuickstartVerification Verify()
        {
            var report = new QuickstartVerification();
            Map map = Find.CurrentMap;
            report.Assert("real Warm map", () => map?.Biome?.defName == "AMJ_WarmTemperateForest");
            if (map == null) return report;
            ThingDef shii = DefDatabase<ThingDef>.GetNamedSilentFail("AMJ_Tree_Shii");
            report.Assert("Environment Shii loaded", () => shii?.plant != null);
            if (shii?.plant == null) return report;
            ThingDef originalWood = shii.plant.harvestedThingDef;
            report.Assert("Shii wood-primary unchanged", () => originalWood != null &&
                originalWood.defName != "Steel" && shii.plant.harvestYield == 42);
            Plant tree = map.listerThings.ThingsOfDef(shii).OfType<Plant>()
                .FirstOrDefault(x => x.Spawned && !x.Destroyed && !x.IsBurning() && !x.Blighted);
            report.Assert("naturally spawned Shii tree exists", () => tree != null);
            if (tree == null) return report;
            tree.Growth = 1f;
            var comp = tree.TryGetComp<CompShiiGatherState>();
            Pawn pawn = map.mapPawns.FreeColonistsSpawned.FirstOrDefault();
            report.Assert("diagnostic tree comp and player pawn", () => comp != null && pawn != null);
            if (comp == null || pawn == null) return report;
            report.Assert("tree eligible before collection", () => comp.CanGather());
            int before = map.listerThings.ThingsOfDef(ThingDefOf.Steel).Sum(x => x.stackCount);
            bool success = comp.TryGather(pawn);
            int after = map.listerThings.ThingsOfDef(ThingDefOf.Steel).Sum(x => x.stackCount);
            report.Assert("one synthetic diagnostic yield", () => success && after - before == 1);
            report.Assert("tree survives with wood unchanged", () => tree.Spawned &&
                !tree.Destroyed && shii.plant.harvestedThingDef == originalWood);
            report.Assert("second direct attempt denied", () =>
                !comp.CanGather() && !comp.TryGather(pawn));
            return report;
        }
    }
}
