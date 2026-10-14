"""Source-only regression checks: cooldown probe needs real RimWorld 1.6 execution."""
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
QUICKSTART = ROOT / "Tests/NativeShiiGatherProbe/WffShiiCooldownQuickstart.cs"

class TestNativeShiiCooldownSource(unittest.TestCase):
    def test_real_tick_boundary_and_one_marker_per_cycle(self):
        code = QUICKSTART.read_text(encoding="utf-8")
        for token in (
            "class WffShiiCooldownQuickstart : AbstractQuickstart",
            "public override int ticksBeforeVerify => 1800",
            "Find.TickManager.TicksGame",
            "candidate.Growth = 1f",
            "firstGathered = state.TryGather(pawn);",
            "!state.CanGather()",
            "!state.TryGather(pawn)",
            "elapsed >= 1800",
            "bool available = state != null && state.CanGather();",
            "bool secondGathered = available && state.TryGather(pawn);",
            "SteelCount - steelBefore == 2",
            "tree.def.plant.harvestedThingDef == wood",
        ):
            self.assertIn(token, code)
        self.assertNotIn("PlantCollected(", code)
        self.assertNotIn("tree.Destroy(", code)
        self.assertNotIn("SaveGame(", code)  # persistence is a separate, still-open gate

if __name__ == "__main__":
    unittest.main()
