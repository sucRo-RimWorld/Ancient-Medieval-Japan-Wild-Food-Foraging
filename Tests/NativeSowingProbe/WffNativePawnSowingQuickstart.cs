// TEST-ONLY RimWorld 1.6 native pawn sowing probe. No food Def, production art or balance.
using System;
using System.Collections.Generic;
using System.Linq;
using RimWorks.Quickstarts;
using RimWorks.Quickstarts.Verification;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace AMJ.WildFoodForaging.NativeTests
{
    public sealed class WffNativePawnSowingQuickstart : AbstractQuickstart
    {
        private Map map;
        private readonly List<Zone_Growing> zones = new List<Zone_Growing>();
        private readonly List<IntVec3> cells = new List<IntVec3>();
        private readonly List<ThingDef> crops = new List<ThingDef>();
        private readonly List<Pawn> planters = new List<Pawn>();
        private readonly List<bool> jobsStarted = new List<bool>();
        private string setupFailure;

        public override TaggedString description =>
            "Real pawn native sow Jobs for diagnostic Fuki and Udo in grow zones";
        public override string seed => "AMJ-WFF-SOW-20261014";
        public override float planetCoverage => 0.30f;
        public override int mapSize => 150;
        public override int ticksBeforeVerify => 3600;

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
            map = Find.CurrentMap;
            if (map == null)
            {
                setupFailure = "Generated map missing";
                return;
            }
            var pawns = map.mapPawns.FreeColonistsSpawned.Take(2).ToList();
            WorkGiverDef workDef = DefDatabase<WorkGiverDef>.AllDefsListForReading
                .FirstOrDefault(d => d.giverClass == typeof(WorkGiver_GrowerSow));
            var work = workDef?.Worker as WorkGiver_GrowerSow;
            ThingDef fuki = DefDatabase<ThingDef>.GetNamedSilentFail("AMJWF_Probe_Fuki");
            ThingDef udo = DefDatabase<ThingDef>.GetNamedSilentFail("AMJWF_Probe_Udo");
            if (pawns.Count != 2 || work == null || fuki?.plant == null || udo?.plant == null)
            {
                setupFailure = "Native growers or diagnostic plant Defs unavailable";
                return;
            }
            ThingDef[] seeds = { fuki, udo };
            for (int i = 0; i < seeds.Length; i++)
            {
                Pawn pawn = pawns[i];
                ThingDef crop = seeds[i];
                Job job = null;
                Zone_Growing zone = null;
                IntVec3 selected = IntVec3.Invalid;
                // Actual native WorkGiver, terrain, reservations and reach decide eligibility.
                // No force-spawning or instant crop placement is permitted.
                foreach (IntVec3 cell in map.AllCells.OrderBy(c => c.DistanceToSquared(pawn.Position)))
                {
                    if (!cell.Standable(map) || cell.Fogged(map) ||
                        map.zoneManager.ZoneAt(cell) != null ||
                        map.terrainGrid.TerrainAt(cell).fertility < crop.plant.fertilityMin ||
                        !pawn.CanReach(cell, PathEndMode.OnCell, Danger.Some) ||
                        cell.GetThingList(map).Any(t => t is Plant ||
                            t.def.category == ThingCategory.Building))
                        continue;
                    Zone_Growing candidate = new Zone_Growing(map.zoneManager);
                    map.zoneManager.RegisterZone(candidate);
                    candidate.AddCell(cell);
                    candidate.SetPlantDefToGrow(crop);
                    Job offered = work.JobOnCell(pawn, cell);
                    if (offered != null && offered.def == JobDefOf.Sow)
                    {
                        job = offered;
                        zone = candidate;
                        selected = cell;
                        break;
                    }
                    candidate.Delete();
                }
                if (job == null)
                {
                    setupFailure = "Native WorkGiver refused every actual sow cell for " + crop.defName;
                    return;
                }
                zones.Add(zone);
                cells.Add(selected);
                crops.Add(crop);
                planters.Add(pawn);
                pawn.jobs.StartJob(job, JobCondition.InterruptForced);
                jobsStarted.Add(pawn.jobs.curJob?.def == JobDefOf.Sow);
            }
        }

        public override QuickstartVerification Verify()
        {
            var report = new QuickstartVerification();
            report.Assert("natural Warm forest biome generated",
                () => map != null && map.Biome?.defName == "AMJ_WarmTemperateForest");
            report.Assert("two independent valid native sow jobs were prepared",
                () => setupFailure == null && zones.Count == 2 && cells.Count == 2 &&
                    crops.Count == 2 && planters.Count == 2 && jobsStarted.Count == 2);
            report.Assert("native pawn JobDefOf.Sow started for Fuki and Udo",
                () => jobsStarted.Count == 2 && jobsStarted.All(x => x));
            report.Assert("Fuki crop planted by actual native pawn job",
                () => CropMatches(0, "AMJWF_Probe_Fuki"));
            report.Assert("Udo crop planted by actual native pawn job",
                () => CropMatches(1, "AMJWF_Probe_Udo"));
            report.Assert("zone grid keeps both crops explicitly selected",
                () => zones.Count == 2 && cells.Count == 2 && crops.Count == 2 &&
                    map != null && Enumerable.Range(0, 2).All(i =>
                        map.zoneManager.ZoneAt(cells[i]) == zones[i] &&
                        zones[i].PlantDefToGrow == crops[i]));
            return report;
        }

        private bool CropMatches(int i, string expected)
        {
            if (map == null || i >= cells.Count || i >= crops.Count ||
                crops[i].defName != expected)
                return false;
            return cells[i].GetThingList(map).OfType<Plant>()
                .Any(p => p.Spawned && !p.Destroyed && p.def == crops[i]);
        }
    }
}
