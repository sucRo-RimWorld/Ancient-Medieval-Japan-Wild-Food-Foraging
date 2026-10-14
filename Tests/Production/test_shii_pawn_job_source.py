"""Guard the separate real-pawn Job probe; source only, not a game PASS."""
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / "Tests/NativeShiiGatherProbe/WffShiiPawnJobQuickstart.cs"

class TestShiiPawnJobProbe(unittest.TestCase):
    def test_native_pawn_work_and_ticks_are_observed(self):
        source = SOURCE.read_text(encoding="utf-8")
        for part in (
            "class WffShiiPawnJobQuickstart : AbstractQuickstart",
            "public override void PostLoaded()",
            "public override int ticksBeforeVerify => 1800",
            "new WorkGiver_GatherShiiNuts()",
            "PotentialWorkThingsGlobal(selectedPawn).Contains(selectedTree)",
            "scanner.JobOnThing(selectedPawn, selectedTree)",
            "selectedPawn.jobs.StartJob(job, JobCondition.InterruptForced)",
            "steelAfter - steelBefore == 1",
            "selectedTree.Spawned && !selectedTree.Destroyed",
            "DesignationOn(selectedTree, gather) == null",
            "state != null && !state.CanGather()",
        ):
            self.assertIn(part, source)
        self.assertNotIn("state.TryGather(", source)
        self.assertNotIn("PlantCollected(", source)
        self.assertNotIn("old_save_pass", source)

if __name__ == "__main__":
    unittest.main()
