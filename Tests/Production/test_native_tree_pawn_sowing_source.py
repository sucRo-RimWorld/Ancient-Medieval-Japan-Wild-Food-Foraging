"""Source-only guard; does NOT prove that any tree was actually sown in RimWorld."""
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / "Tests/NativeSowingProbe/WffNativeTreePawnSowingQuickstart.cs"

class TestTreePawnSowingSource(unittest.TestCase):
    def test_research_native_job_and_three_original_tree_def_names(self):
        src = SOURCE.read_text(encoding="utf-8")
        for token in (
            "class WffNativeTreePawnSowingQuickstart : AbstractQuickstart",
            'GetNamedSilentFail("TreeSowing")',
            "treeResearchWasUnfinished = !research.IsFinished",
            "Find.ResearchManager.FinishProject(",
            '"AMJWF_Probe_Kuri"',
            '"AMJWF_Probe_Walnut"',
            '"AMJWF_Probe_Kaya"',
            "new Zone_Growing(map.zoneManager)",
            "candidate.SetPlantDefToGrow(plant)",
            "map.mapPawns.AllPawnsSpawned.Any(other => other.Position == cell)",
            "plant.plant.sowMinSkill",
            "skill.Level = naturalRequirement",
            "nativeSkillSatisfied.Count == 3",
            "inheritedSowMinSkill=",
            "work.JobOnCell(pawn, cell)",
            "offered.def == JobDefOf.Sow",
            "pawn.jobs.StartJob(job, JobCondition.InterruptForced)",
            "Planted(0,", "Planted(1,", "Planted(2,",
            "map.zoneManager.ZoneAt(cells[i]) == zones[i]",
            "nativeJobRefusals=",
            "eligibleTerrainAndReach=",
            "Log.Message(\"[WFF TreeSow Diagnostic]",
            "[WFF TreeSow Outcome]",
            "currentJob=",
            "targetZoneRetained=",
            "targetPlants=",
            "[WFF TreeSow Assignment]",
            "job.plantDefToSow",
            "nativeJobSowPlants[i] == trees[i]",
            'p.def.defName + "/sown=" + p.sown',
            "p.def == trees[i] && p.sown",
        ):
            self.assertIn(token, src)
        for forbidden in ("GenSpawn.Spawn(", "PlantCollected(", "ThingMaker.MakeThing(plant)",
                          "SpawnPlant(", "harvestedThingDef =", "harvestYield ="):
            self.assertNotIn(forbidden, src)

if __name__ == "__main__":
    unittest.main()
