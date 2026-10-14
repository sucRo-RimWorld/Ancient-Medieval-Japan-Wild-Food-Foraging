"""Source-only native Shii cut-after-gather guard, not a RimWorld PASS."""
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / "Tests/NativeShiiGatherProbe/WffShiiTimberCutQuickstart.cs"

class TestShiiNativeTimberSource(unittest.TestCase):
    def test_real_cut_job_preserves_wood_and_does_not_duplicate_product(self):
        source = SOURCE.read_text(encoding="utf-8")
        for token in (
            "class WffShiiTimberCutQuickstart : AbstractQuickstart",
            '"AMJ_Tree_Shii"', 'wood.defName != "WoodLog"',
            "shii.plant.harvestYield != 42",
            "gathering.TryGather(pawn)",
            "new Designation(tree, DesignationDefOf.CutPlant)",
            "typeof(WorkGiver_PlantsCut)",
            "work.JobOnThing(pawn, tree)",
            "job.def != JobDefOf.CutPlantDesignated",
            "pawn.jobs.StartJob(job, JobCondition.InterruptForced)",
            "Count(wood) > woodBefore",
            "Count(ThingDefOf.Steel) - steelBefore == 1",
        ):
            self.assertIn(token, source)
        for forbidden in ("harvestedThingDef =", "harvestYield =",
                          "GenSpawn.Spawn(", "PlantCollected(", "ThingMaker.MakeThing("):
            self.assertNotIn(forbidden, source)

if __name__ == "__main__":
    unittest.main()
