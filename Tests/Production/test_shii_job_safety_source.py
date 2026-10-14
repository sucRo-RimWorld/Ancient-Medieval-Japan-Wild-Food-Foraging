"""Source guard only; native race/cancellation still require real logs."""
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
PATH = ROOT / "Tests/NativeShiiGatherProbe/WffShiiJobSafetyQuickstarts.cs"

class TestShiiJobSafetySource(unittest.TestCase):
    def test_real_tick_race_and_cancellation(self):
        source = PATH.read_text(encoding="utf-8")
        for text in (
            "WffShiiRaceQuickstart : WffShiiJobSafetyBase",
            "WffShiiCancelQuickstart : WffShiiJobSafetyBase",
            "second.CanReserveAndReach(candidate",
            "secondEligibleBefore = scanner.JobOnThing(second, tree) != null;",
            "secondRejectedAfter = scanner.JobOnThing(second, tree) == null;",
            "first.jobs.StartJob(job, JobCondition.InterruptForced);",
            "public override int ticksBeforeVerify => 1200",
            "public override int ticksBeforeVerify => 10",
            "first.jobs.StopAll()",
            "SteelCount - steelBefore == 1",
            "SteelCount == steelBefore",
            "map.designationManager.DesignationOn(tree, gather) != null",
            "state != null && state.CanGather()",
        ):
            self.assertIn(text, source)
        self.assertNotIn("state.TryGather(", source)
        self.assertNotIn("PlantCollected(", source)

if __name__ == "__main__":
    unittest.main()
