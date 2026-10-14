"""Static source guard for the Shii mechanism scaffold; does NOT test loaded RimWorld."""
from pathlib import Path
import json
import unittest

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / "Source" / "AMJWildFoodForaging"


def read(name):
    return (SOURCE / name).read_text(encoding="utf-8")


class TestShiiSourceContract(unittest.TestCase):
    def test_entry_is_scoped(self):
        record = json.loads((ROOT / "Docs/Research/ShiiMechanismDecision.json").read_text(encoding="utf-8"))
        self.assertEqual(record["status"], "ready")
        self.assertIn("mechanism", record["feature"])
        self.assertEqual(record["decision"]["specification"], "Docs/Design.md")
        self.assertIn("No approved resource ThingDef", record["scope"])

    def test_tree_primary_wood_never_retargeted(self):
        for filename in ["CompShiiGatherState.cs", "JobDriver_GatherShiiNuts.cs", "WorkGiver_GatherShiiNuts.cs"]:
            content = read(filename)
            self.assertNotIn("harvestedThingDef =", content)
            self.assertNotIn("PlantCollected(", content)
            self.assertNotIn("PlantCut(", content)

    def test_state_is_per_tree_saved_and_disabled_without_balance(self):
        content = read("CompShiiGatherState.cs")
        for field in ('Scribe_Values.Look(ref lastGatherTick', 'Scribe_Values.Look(ref hasGathered',
                      'plant.def.defName != "AMJ_Tree_Shii"', 'Props.productDef == null',
                      'Props.yieldCount <= 0', 'Props.cooldownTicks <= 0', 'Props.workTicks <= 0',
                      'plant.LifeStage != PlantLifeStage.Mature', 'Props.yieldCount > Props.productDef.stackLimit',
                      'lastGatherTick = Find.TickManager.TicksGame'):
            self.assertIn(field, content)
        self.assertLess(content.index('GenPlace.TryPlaceThing'), content.index('lastGatherTick = Find.TickManager.TicksGame;'))

    def test_job_reserves_checks_then_gathers_without_destroying_tree(self):
        content = read("JobDriver_GatherShiiNuts.cs")
        for token in ['pawn.Reserve(', 'FailOnDespawnedNullOrForbidden', 'FailOnBurningImmobile',
                      'FailOnCannotTouch', 'state.CanGather()', 'state.TryGather(pawn)']:
            self.assertIn(token, content)
        self.assertNotIn('plant.Destroy(', content)

    def test_gather_order_is_one_shot_only_after_success(self):
        job = read("JobDriver_GatherShiiNuts.cs")
        self.assertIn("!state.TryGather(pawn)", job)
        self.assertIn('GetNamedSilentFail("AMJ_GatherShiiNuts")', job)
        self.assertIn("designationManager.RemoveDesignation(designation)", job)
        self.assertLess(job.index("!state.TryGather(pawn)"), job.index("RemoveDesignation(designation)"))
        self.assertIn("if (designation != null)", job)
        self.assertNotIn("RemoveAllDesignations", job)

    def test_scanner_uses_public_spawned_designation_api(self):
        content = read("WorkGiver_GatherShiiNuts.cs")
        self.assertIn("designationManager.SpawnedDesignationsOfDef(gather)", content)
        self.assertNotIn("designationManager.allDesignations", content)
        self.assertIn("designation.target.HasThing", content)

    def test_workgiver_is_limited_to_designated_shii(self):
        content = read("WorkGiver_GatherShiiNuts.cs")
        for token in ['DesignationOn(thing, gather)', 'thing.def.defName != "AMJ_Tree_Shii"',
                      'DesignationDefOf.CutPlant', 'DesignationDefOf.HarvestPlant',
                      'pawn.CanReserveAndReach', 'state.CanGather()', 'JobMaker.MakeJob']:
            self.assertIn(token, content)

    def test_no_playable_defs_or_unapproved_payload(self):
        for path in ['About/About.xml', 'Defs/ThingDefs/AMJ_ShiiNuts.xml',
                     'Defs/JobDefs/AMJ_ShiiGather.xml', 'Assemblies/AMJWildFoodForaging.dll']:
            self.assertFalse((ROOT / path).exists(), path)


if __name__ == "__main__":
    unittest.main()
