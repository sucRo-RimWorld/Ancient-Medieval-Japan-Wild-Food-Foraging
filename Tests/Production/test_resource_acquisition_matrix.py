"""Source/design contract only. No RimWorld runtime, XML loading, or game harvest PASS."""
import json
import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
MATRIX = ROOT / "Docs" / "ResourceAcquisitionMatrix.json"
ADOPTION = ROOT / "Docs" / "ResourceAdoption-v1.md"
MODE_BY_PREFIX = {
    "N": "tree_secondary",
    "G": "shoot_gather",
    "R": "underground_dig",
    "F": "fungal_fruiting_body",
}
NUMERIC_FIELDS = ("harvestQuantity", "workTicks", "regrowthTicks",
                  "seasonWindow", "wildPlantCommonality")


class ResourceAcquisitionContract(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.model = json.loads(MATRIX.read_text(encoding="utf-8"))
        cls.entries = cls.model["entries"]
        cls.by_id = {entry["id"]: entry for entry in cls.entries}
        cls.adopted_text = ADOPTION.read_text(encoding="utf-8")

    def test_all_and_only_adopted_species_are_classified(self):
        adopted = set(re.findall(r"^\| (WF-[NGRF]\d\d) \|", self.adopted_text, re.M))
        self.assertEqual(len(adopted), 12)
        self.assertEqual(len(self.entries), 12)
        self.assertEqual(len(self.by_id), 12)
        self.assertEqual(set(self.by_id), adopted)

    def test_expected_group_partition_and_behavior(self):
        for prefix, expected_count in (("N", 4), ("G", 4), ("R", 1), ("F", 3)):
            group = [entry for entry in self.entries if entry["id"].startswith("WF-" + prefix)]
            self.assertEqual(len(group), expected_count, prefix)
            for item in group:
                self.assertEqual(item["collectionMode"], MODE_BY_PREFIX[prefix])
                self.assertEqual(item["implementationStatus"], "design_candidate_only")
                self.assertTrue(item["collectionBehaviorCandidate"])

    def test_no_unapproved_balance_values(self):
        for item in self.entries:
            for key in NUMERIC_FIELDS:
                self.assertIn(key, item)
                self.assertIsNone(item[key], (item["id"], key))

    def test_environment_plant_not_redefined(self):
        shii = self.by_id["WF-N01"]
        self.assertEqual(shii["existingEnvironmentPlantDef"], "AMJ_Tree_Shii")
        self.assertEqual(shii["candidateBiomes"], ["AMJ_WarmTemperateForest"])
        self.assertTrue(all(
            item["existingEnvironmentPlantDef"] is None
            for id_, item in self.by_id.items() if id_ != "WF-N01"
        ))
        facts = " ".join(self.model["environmentEvidence"]["verifiedFacts"])
        self.assertIn("AMJ_Tree_Shii=2.55", facts)
        self.assertIn("Plant_TreePine=0.35", facts)

    def test_only_actual_biome_def_names_as_candidates(self):
        accepted = {
            "AMJ_WarmTemperateForest", "AMJ_CoolTemperateForest",
            "AMJ_SubalpineForest", "AMJ_AlpineZone",
            "TemperateSwamp", "ColdBog",
        }
        for item in self.entries:
            self.assertTrue(item["candidateBiomes"])
            self.assertTrue(set(item["candidateBiomes"]).issubset(accepted), item["id"])

    def test_matsutake_eligible_host_is_not_assumed(self):
        item = self.by_id["WF-F02"]
        self.assertIn("Plant_TreePine", item["habitatCondition"])
        self.assertIn("未承認", item["habitatCondition"])
        self.assertIn("宿主植物Def", item["notes"])

    def test_fungi_are_not_generic_biome_food(self):
        for id_ in ("WF-F01", "WF-F03"):
            self.assertIn("朽木", self.by_id[id_]["habitatCondition"])
            self.assertIn("子実体", self.by_id[id_]["collectionBehaviorCandidate"])
        for id_ in ("WF-F01", "WF-F02", "WF-F03"):
            self.assertNotEqual(self.by_id[id_]["collectionMode"], "tree_secondary")

    def test_raw_dangerous_food_is_not_assumed_safe(self):
        for id_ in ("WF-G01", "WF-G02", "WF-G03"):
            self.assertIn("アク抜き", self.by_id[id_]["preparationGate"])
        self.assertIn("加熱", self.by_id["WF-F03"]["preparationGate"])
        self.assertIn("生食", self.by_id["WF-N04"]["preparationGate"])

    def test_wild_plant_mod_independence_and_no_farming(self):
        rules = " ".join(self.model["selectionRules"])
        self.assertIn("never limits", rules)
        self.assertIn("naturally occurring", rules)
        self.assertIn("Foraging owns", rules)

    def test_source_pins_and_no_invented_runtime(self):
        evidence = self.model["environmentEvidence"]
        self.assertEqual(len(evidence["biomeGitBlob"]), 40)
        self.assertEqual(len(evidence["plantGitBlob"]), 40)
        self.assertIn("12 adopted", self.model["scope"])
        self.assertNotIn("PASS", self.model["scope"])


if __name__ == "__main__":
    unittest.main()
