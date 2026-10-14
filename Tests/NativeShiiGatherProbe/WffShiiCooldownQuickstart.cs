// Native 1.6 disposable Shii cooldown-boundary diagnostic. No save/reload or food acceptance.
using System;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using RimWorks.Quickstarts;
using RimWorks.Quickstarts.Verification;
using Verse;

namespace AMJ.WildFoodForaging.NativeTests
{
    public sealed class WffShiiCooldownQuickstart : AbstractQuickstart
    {
        private Map map;
        private Pawn pawn;
        private Plant tree;
        private CompShiiGatherState state;
        private ThingDef wood;
        private int steelBefore;
        private int gatheredAtTick;
        private bool firstGathered;
        private bool immediatelyBlocked;
        private string setupFailure;

        public override TaggedString description =>
            "Synthetic Shii product is available only after per-tree cooldown";
        public override string seed => "AMJ-WFF-SHII-20261014";
        public override float planetCoverage => 0.30f;
        public override int mapSize => 150;
        public override int ticksBeforeVerify => 1800; // synthetic fixture only, not balance

        private int SteelCount => map.listerThings.ThingsOfDef(ThingDefOf.Steel)
            .Sum(item => item.stackCount);

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
                throw new InvalidOperationException("No native Warm settlement tile");
            Find.GameInitData.startingTile = selected;
        }

        public override void PostLoaded()
        {
            map = Find.CurrentMap;
            if (map == null)
            {
                setupFailure = "Generated map absent";
                return;
            }
            ThingDef shii = DefDatabase<ThingDef>.GetNamedSilentFail("AMJ_Tree_Shii");
            pawn = map.mapPawns.FreeColonistsSpawned.FirstOrDefault();
            if (shii?.plant == null || pawn == null)
            {
                setupFailure = "Native tree or colonist absent";
                return;
            }
            wood = shii.plant.harvestedThingDef;
            foreach (Plant candidate in map.listerThings.ThingsOfDef(shii)
                .OfType<Plant>().Where(p => p.Spawned && !p.Destroyed &&
                    !p.Blighted && !p.IsBurning()))
            {
                candidate.Growth = 1f; // disposable map; does not prove natural maturity
                CompShiiGatherState comp = candidate.TryGetComp<CompShiiGatherState>();
                if (comp == null || !comp.CanGather())
                    continue;
                tree = candidate;
                state = comp;
                break;
            }
            if (tree == null)
            {
                setupFailure = "No eligible naturally spawned Shii";
                return;
            }
            steelBefore = SteelCount;
            gatheredAtTick = Find.TickManager.TicksGame;
            firstGathered = state.TryGather(pawn);
            immediatelyBlocked = firstGathered && !state.CanGather() &&
                                 !state.TryGather(pawn);
        }

        public override QuickstartVerification Verify()
        {
            var report = new QuickstartVerification();
            report.Assert("native Warm map with existing tree",
                () => map != null && map.Biome?.defName == "AMJ_WarmTemperateForest" &&
                    setupFailure == null && tree != null && state != null);
            report.Assert("natural Environment Shii still standing",
                () => tree != null && tree.Spawned && !tree.Destroyed &&
                    tree.def.defName == "AMJ_Tree_Shii");
            report.Assert("Environment timber primary and yield are unchanged",
                () => tree != null && tree.def.plant.harvestedThingDef == wood &&
                    tree.def.plant.harvestYield == 42);
            report.Assert("first direct gathering generated one synthetic marker",
                () => firstGathered && map != null && SteelCount - steelBefore == 1);
            report.Assert("instant repeat was forbidden before ticking",
                () => immediatelyBlocked);
            int elapsed = Find.TickManager.TicksGame - gatheredAtTick;
            report.Assert("native game advanced through diagnostic cooldown threshold",
                () => firstGathered && elapsed >= 1800);
            bool available = state != null && state.CanGather();
            report.Assert("same living tree becomes eligible after threshold",
                () => available);
            bool secondGathered = available && state.TryGather(pawn);
            report.Assert("second gather yields exactly one further marker",
                () => secondGathered && SteelCount - steelBefore == 2);
            report.Assert("new gather advances cooldown again",
                () => secondGathered && !state.CanGather());
            return report;
        }
    }
}
