"""Guard actual XML abstract sowing bases before concrete gameplay is approved.

This does not run RimWorld, load Vanilla Def inheritance, or test pawn sow jobs.
"""
import json
import unittest
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
SRC = ROOT / "Defs/ThingDefs_Plants/AMJWF_SowingBases.xml"
MATRIX = ROOT / "Docs/ResourceAcquisitionMatrix.json"
EXPECTED = {
    "WF-N02": ("AMJWF_KuriSowingTemplate", "AMJWF_ForageNutTreeBase"),
    "WF-N03": ("AMJWF_WalnutSowingTemplate", "AMJWF_ForageNutTreeBase"),
    "WF-N04": ("AMJWF_KayaSowingTemplate", "AMJWF_ForageNutTreeBase"),
    "WF-G01": ("AMJWF_WarabiWildTemplate", "AMJWF_WildOnlyPlantBase"),
    "WF-G02": ("AMJWF_ZenmaiWildTemplate", "AMJWF_WildOnlyPlantBase"),
    "WF-G03": ("AMJWF_FukiSowingTemplate", "AMJWF_SowableWildGreenBase"),
    "WF-G04": ("AMJWF_UdoSowingTemplate", "AMJWF_SowableWildGreenBase"),
    "WF-R01": ("AMJWF_JinenjoWildTemplate", "AMJWF_WildOnlyPlantBase"),
    "WF-F01": ("AMJWF_HiratakeWildTemplate", "AMJWF_WildOnlyPlantBase"),
    "WF-F02": ("AMJWF_MatsutakeWildTemplate", "AMJWF_WildOnlyPlantBase"),
    "WF-F03": ("AMJWF_ShiitakeWildTemplate", "AMJWF_WildOnlyPlantBase"),
}


class NativeSowingBaseContract(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.root = ET.parse(SRC).getroot()
        cls.defs = cls.root.findall("ThingDef")
        cls.by_name = {d.get("Name"): d for d in cls.defs}
        cls.matrix = {
            x["id"]: x for x in json.loads(MATRIX.read_text(encoding="utf-8"))["entries"]
        }

    def test_exactly_eleven_new_species_and_three_base_templates(self):
        self.assertEqual(self.root.tag, "Defs")
        self.assertEqual(len(self.defs), 14)
        self.assertEqual(len(self.by_name), 14)
        self.assertEqual(set(EXPECTED), set(self.matrix) - {"WF-N01"})
        self.assertNotIn("AMJ_Tree_Shii", self.by_name)
        self.assertTrue(all(d.find("defName") is None for d in self.defs))

    def test_tree_inheritance_uses_native_planting_research_without_override(self):
        tree = self.by_name["AMJWF_ForageNutTreeBase"]
        self.assertEqual(tree.get("ParentName"), "TreeBase")
        for field in ("sowTags", "sowResearchPrerequisites", "mustBeWildToSow",
                      "harvestYield", "harvestedThingDef"):
            self.assertIsNone(tree.find("plant/" + field))
        for id_ in ("WF-N02", "WF-N03", "WF-N04"):
            self.assertEqual(self.matrix[id_]["sowingResearchGate"], "TreeSowing")

    def test_only_fuki_udo_base_explicitly_sowable(self):
        plant = self.by_name["AMJWF_SowableWildGreenBase"]
        self.assertEqual(plant.get("ParentName"), "PlantBase")
        self.assertEqual(
            [x.text for x in plant.findall("plant/sowTags/li")], ["Ground"]
        )
        for base in ("AMJWF_WildOnlyPlantBase", "AMJWF_ForageNutTreeBase"):
            self.assertIsNone(self.by_name[base].find("plant/sowTags"))

    def test_species_parent_matches_author_approved_plantability(self):
        for id_, (name, parent) in EXPECTED.items():
            self.assertEqual(self.by_name[name].get("ParentName"), parent, id_)
            self.assertEqual(self.by_name[name].get("Abstract"), "True", id_)
            self.assertEqual(self.matrix[id_]["plantingImplementationStatus"],
                             "specification_only_not_in_game", id_)

    def test_no_concrete_def_or_new_balance_art_in_abstract_templates(self):
        forbidden = ("defName", "texPath", "harvestedThingDef", "harvestYield",
                     "growDays", "sowWork", "harvestWork", "ingestible")
        for d in self.defs:
            self.assertEqual(d.get("Abstract"), "True")
            for tag in forbidden:
                self.assertIsNone(d.find(".//" + tag), (d.get("Name"), tag))
        self.assertEqual(
            [x.text for x in self.root.findall(".//sowTags/li")], ["Ground"]
        )

    def test_wild_ferns_yam_and_fungi_have_no_grow_zone_sowing(self):
        for id_ in ("WF-G01", "WF-G02", "WF-R01",
                    "WF-F01", "WF-F02", "WF-F03"):
            name, parent = EXPECTED[id_]
            self.assertEqual(parent, "AMJWF_WildOnlyPlantBase", name)
            self.assertIn(self.matrix[id_]["plantingPolicy"],
                          ("wild_only_initial", "wild_dig_only_initial", "fungus_not_sowable"))

    def test_no_mandatory_second_mod_or_unapproved_numbers(self):
        for id_, item in self.matrix.items():
            for key in ("harvestQuantity", "workTicks", "regrowthTicks",
                        "seasonWindow", "wildPlantCommonality"):
                self.assertIsNone(item[key], (id_, key))


if __name__ == "__main__":
    unittest.main()
