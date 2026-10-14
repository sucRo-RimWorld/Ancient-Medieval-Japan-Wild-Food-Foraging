// DEV-ONLY 1.6 Kuri/Walnut/Kaya tree pawn sowing, not production vegetation.
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
    public sealed class WffNativeTreePawnSowingQuickstart : AbstractQuickstart
    {
        private Map map;
        private readonly List<IntVec3> cells = new List<IntVec3>();
        private readonly List<Zone_Growing> zones = new List<Zone_Growing>();
        private readonly List<ThingDef> trees = new List<ThingDef>();
        private readonly List<bool> jobsStarted = new List<bool>();
        private readonly List<Pawn> planters = new List<Pawn>();
        private readonly List<IntVec3> pawnStartCells = new List<IntVec3>();
        private readonly List<bool> nativeSkillSatisfied = new List<bool>();
        private bool treeResearchWasUnfinished;
        private bool treeResearchNowFinished;
        private string setupFailure;

        public override TaggedString description =>
            "Actual native TreeSowing pawn jobs for three synthetic nut trees";
        public override string seed => "AMJ-WFF-SOW-20261014";
        public override float planetCoverage => 0.30f;
        public override int mapSize => 150;
        public override int ticksBeforeVerify => 9000;

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
                setupFailure = "No generated map";
                return;
            }
            var pawns = map.mapPawns.FreeColonistsSpawned
                .OrderByDescending(p => p.skills.GetSkill(SkillDefOf.Plants).Level)
                .Take(3).ToList();
            ResearchProjectDef research = DefDatabase<ResearchProjectDef>.GetNamedSilentFail("TreeSowing");
            WorkGiverDef workDef = DefDatabase<WorkGiverDef>.AllDefsListForReading
                .FirstOrDefault(d => d.giverClass == typeof(WorkGiver_GrowerSow));
            var work = workDef?.Worker as WorkGiver_GrowerSow;
            string[] defs = { "AMJWF_Probe_Kuri", "AMJWF_Probe_Walnut", "AMJWF_Probe_Kaya" };
            var plants = defs.Select(DefDatabase<ThingDef>.GetNamedSilentFail).ToArray();
            if (pawns.Count != 3 || work == null || research == null ||
                plants.Any(p => p?.plant == null))
            {
                setupFailure = "Native research, pawns, WorkGiver or fixture tree missing";
                return;
            }
            treeResearchWasUnfinished = !research.IsFinished;
            if (!treeResearchWasUnfinished)
            {
                setupFailure = "New test game unexpectedly begins with TreeSowing researched";
                return;
            }
            Find.ResearchManager.FinishProject(
                research, doCompletionDialog: false, researcher: null,
                doCompletionLetter: false);
            treeResearchNowFinished = research.IsFinished;
            if (!treeResearchNowFinished)
            {
                setupFailure = "TreeSowing research did not finish";
                return;
            }
            Log.Message("[WFF TreeSow Diagnostic] freeColonists=" + pawns.Count +
                "; researchFinished=" + treeResearchNowFinished +
                "; firstTree=" + plants[0].defName);
            for (int i = 0; i < plants.Length; i++)
            {
                Pawn pawn = pawns[i];
                ThingDef plant = plants[i];
                SkillRecord skill = pawn.skills.GetSkill(SkillDefOf.Plants);
                int naturalRequirement = plant.plant.sowMinSkill;
                int initialSkill = skill.Level;
                // Only in this disposable test colony: create a qualified native planter,
                // rather than removing TreeBase's sowMinSkill or bypassing WorkGiver.
                if (skill.Level < naturalRequirement)
                    skill.Level = naturalRequirement;
                bool meetsNativeRequirement = skill.Level >= naturalRequirement;
                nativeSkillSatisfied.Add(meetsNativeRequirement);
                Log.Message("[WFF TreeSow Diagnostic] species=" + plant.defName +
                    "; initialPlantSkill=" + initialSkill +
                    "; inheritedSowMinSkill=" + naturalRequirement +
                    "; qualifiedPlantSkill=" + skill.Level);
                Job job = null;
                Zone_Growing acceptedZone = null;
                IntVec3 selected = IntVec3.Invalid;
                int precheckedCells = 0;
                int refusedJobs = 0;
                foreach (IntVec3 cell in map.AllCells.OrderBy(c => c.DistanceToSquared(pawn.Position)))
                {
                    if (!cell.Standable(map) || cell.Fogged(map) ||
                        map.zoneManager.ZoneAt(cell) != null ||
                        map.terrainGrid.TerrainAt(cell).fertility < plant.plant.fertilityMin ||
                        !pawn.CanReach(cell, PathEndMode.OnCell, Danger.Some) ||
                        cell.GetThingList(map).Any(t => t is Plant ||
                            t.def.category == ThingCategory.Building))
                        continue;
                    precheckedCells++;
                    Zone_Growing candidate = new Zone_Growing(map.zoneManager);
                    map.zoneManager.RegisterZone(candidate);
                    candidate.AddCell(cell);
                    candidate.SetPlantDefToGrow(plant);
                    Job offered = work.JobOnCell(pawn, cell);
                    if (offered != null && offered.def == JobDefOf.Sow)
                    {
                        job = offered;
                        acceptedZone = candidate;
                        selected = cell;
                        break;
                    }
                    refusedJobs++;
                    candidate.Delete();
                }
                if (job == null)
                {
                    setupFailure = "Native tree sowing rejected all eligible cells for " + plant.defName +
                        "; eligibleTerrainAndReach=" + precheckedCells +
                        "; nativeJobRefusals=" + refusedJobs +
                        "; previouslyStartedJobs=" + jobsStarted.Count;
                    Log.Message("[WFF TreeSow Diagnostic] " + setupFailure);
                    return;
                }
                zones.Add(acceptedZone);
                cells.Add(selected);
                trees.Add(plant);
                planters.Add(pawn);
                pawnStartCells.Add(pawn.Position);
                pawn.jobs.StartJob(job, JobCondition.InterruptForced);
                jobsStarted.Add(pawn.jobs.curJob?.def == JobDefOf.Sow);
            }
        }

        public override QuickstartVerification Verify()
        {
            if (map != null)
            {
                for (int i = 0; i < cells.Count && i < trees.Count &&
                     i < planters.Count && i < pawnStartCells.Count; i++)
                {
                    Pawn planter = planters[i];
                    IntVec3 cell = cells[i];
                    Plant[] observed = cell.GetThingList(map).OfType<Plant>().ToArray();
                    Log.Message("[WFF TreeSow Outcome] species=" + trees[i].defName +
                        "; planter=" + planter.LabelShort +
                        "; originalPosition=" + pawnStartCells[i] +
                        "; finalPosition=" + planter.Position +
                        "; targetCell=" + cell +
                        "; currentJob=" + (planter.jobs.curJob?.def?.defName ?? "none") +
                        "; planterSpawned=" + planter.Spawned +
                        "; planterDowned=" + planter.Downed +
                        "; targetZoneRetained=" + (map.zoneManager.ZoneAt(cell) == zones[i]) +
                        "; targetTerrain=" + map.terrainGrid.TerrainAt(cell).defName +
                        "; targetPlants=" + string.Join(",",
                            observed.Select(p => p.def.defName).ToArray()));
                }
            }
            var report = new QuickstartVerification();
            report.Assert("genuine Environment Warm biome map",
                () => map != null && map.Biome?.defName == "AMJ_WarmTemperateForest");
            report.Assert("TreeSowing unlocked inside fresh isolated game",
                () => treeResearchWasUnfinished && treeResearchNowFinished);
            report.Assert("all three isolated growers meet inherited native sow skill",
                () => nativeSkillSatisfied.Count == 3 && nativeSkillSatisfied.All(x => x));
            report.Assert("three native pawn tree sow jobs started after research; setup=" +
                (setupFailure ?? "OK") + "; started=" + jobsStarted.Count,
                () => setupFailure == null && jobsStarted.Count == 3 &&
                    jobsStarted.All(x => x) && cells.Count == 3);
            report.Assert("Kuri synthetic tree planted by native pawn",
                () => Planted(0, "AMJWF_Probe_Kuri"));
            report.Assert("Walnut synthetic tree planted by native pawn",
                () => Planted(1, "AMJWF_Probe_Walnut"));
            report.Assert("Kaya synthetic tree planted by native pawn",
                () => Planted(2, "AMJWF_Probe_Kaya"));
            report.Assert("three own grow-zone crop selections remain unmodified",
                () => map != null && zones.Count == 3 && trees.Count == 3 &&
                    Enumerable.Range(0, 3).All(i =>
                        map.zoneManager.ZoneAt(cells[i]) == zones[i] &&
                        zones[i].PlantDefToGrow == trees[i]));
            return report;
        }

        private bool Planted(int i, string name)
        {
            if (map == null || i >= cells.Count || i >= trees.Count ||
                trees[i].defName != name)
                return false;
            return cells[i].GetThingList(map).OfType<Plant>()
                .Any(p => p.Spawned && !p.Destroyed && p.def == trees[i]);
        }
    }
}
