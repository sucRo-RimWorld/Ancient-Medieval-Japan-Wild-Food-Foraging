"""Nonshipping source contract only; real 1.6 native planting must run separately."""
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / "Tests/NativeSowingProbe/WffNativePawnSowingQuickstart.cs"

class TestNativePawnSowingSource(unittest.TestCase):
    def test_actual_planting_jobs_are_required_not_forced_plant_spawns(self):
        src = SOURCE.read_text(encoding="utf-8")
        for token in (
            "class WffNativePawnSowingQuickstart : AbstractQuickstart",
            '"AMJ_WarmTemperateForest"',
            '"AMJWF_Probe_Fuki"',
            '"AMJWF_Probe_Udo"',
            "new Zone_Growing(map.zoneManager)",
            "map.zoneManager.RegisterZone(candidate)",
            "candidate.AddCell(cell)",
            "candidate.SetPlantDefToGrow(crop)",
            "work.JobOnCell(pawn, cell)",
            "offered.def == JobDefOf.Sow",
            "pawn.jobs.StartJob(job, JobCondition.InterruptForced)",
            "pawn.jobs.curJob?.def == JobDefOf.Sow",
            "CropMatches(0,",
            "CropMatches(1,",
            "p.Spawned && !p.Destroyed && p.def == crops[i]",
            "map.zoneManager.ZoneAt(cells[i]) == zones[i]",
        ):
            self.assertIn(token, src)
        for forbidden in ("Plant.SpawnSetup(", "GenSpawn.Spawn(", "PlantCollected(",
                          "ThingMaker.MakeThing(crop)", "SpawnPlant(", "research.FinishProject("):
            self.assertNotIn(forbidden, src)

if __name__ == "__main__":
    unittest.main()
