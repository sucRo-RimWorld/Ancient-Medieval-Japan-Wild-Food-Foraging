"""Native XML fixture contract: actual concrete test ThingDefs, not gameplay PASS."""
import importlib.util
from pathlib import Path
import tempfile
import unittest
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
FIXTURE = ROOT / "Tests/Fixtures/NativeSowingProbe"
CONCRETE = FIXTURE / "Defs/ThingDefs_Plants/AMJWF_ProbePlants.xml"
PATCH = FIXTURE / "Patches/AMJWF_ProbeWarmWildPlants.xml"
BASES = ROOT / "Defs/ThingDefs_Plants/AMJWF_SowingBases.xml"
BUILDER = ROOT / "Tests/NativeSowingProbe/build_probe.py"

spec = importlib.util.spec_from_file_location("native_sowing_probe_builder", BUILDER)
builder = importlib.util.module_from_spec(spec)
spec.loader.exec_module(builder)

EXPECTED = {
    "Kuri": "AMJWF_KuriSowingTemplate",
    "Walnut": "AMJWF_WalnutSowingTemplate",
    "Kaya": "AMJWF_KayaSowingTemplate",
    "Warabi": "AMJWF_WarabiWildTemplate",
    "Zenmai": "AMJWF_ZenmaiWildTemplate",
    "Fuki": "AMJWF_FukiSowingTemplate",
    "Udo": "AMJWF_UdoSowingTemplate",
    "Jinenjo": "AMJWF_JinenjoWildTemplate",
    "Hiratake": "AMJWF_HiratakeWildTemplate",
    "Matsutake": "AMJWF_MatsutakeWildTemplate",
    "Shiitake": "AMJWF_ShiitakeWildTemplate",
}

class NativeSowingProbeContract(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.xml = ET.parse(CONCRETE).getroot()
        cls.defs = cls.xml.findall("ThingDef")
        cls.patches = ET.parse(PATCH).getroot()
        cls.bases = {d.get("Name"): d for d in ET.parse(BASES).getroot().findall("ThingDef")}

    def test_all_eleven_probe_species_reuse_exact_staged_parent(self):
        self.assertEqual(len(self.defs), len(EXPECTED))
        got = {}
        for def_ in self.defs:
            name = def_.findtext("defName")
            self.assertTrue(name.startswith("AMJWF_Probe_"), name)
            self.assertNotIn(name, got)
            got[name] = def_.get("ParentName")
            self.assertNotEqual(def_.get("Abstract"), "True")
            self.assertIsNotNone(def_.find("graphicData/texPath"))
            self.assertIsNotNone(def_.find("plant/growDays"))
            self.assertNotIn("AMJ_Tree_Shii", name)
        self.assertEqual(got, {"AMJWF_Probe_" + k: v for k, v in EXPECTED.items()})
        self.assertTrue(set(got.values()).issubset(self.bases))

    def test_research_and_region_gates_are_inherited_not_overridden(self):
        for def_ in self.defs:
            if def_.findtext("defName") in {
                "AMJWF_Probe_Kuri", "AMJWF_Probe_Walnut", "AMJWF_Probe_Kaya"
            }:
                self.assertEqual(def_.get("ParentName").endswith("SowingTemplate"), True)
                for tag in ("sowTags", "sowResearchPrerequisites", "mustBeWildToSow"):
                    self.assertIsNone(def_.find("plant/" + tag))
        tree = self.bases["AMJWF_ForageNutTreeBase"]
        self.assertEqual(tree.get("ParentName"), "TreeBase")

    def test_wild_plant_patch_is_warm_only_and_excludes_fungi(self):
        self.assertEqual(self.patches.tag, "Patch")
        op = self.patches.find("Operation")
        self.assertEqual(op.get("Class"), "PatchOperationAdd")
        self.assertEqual(op.findtext("xpath"), 'Defs/BiomeDef[defName="AMJ_WarmTemperateForest"]/wildPlants')
        tags = {el.tag for el in op.find("value")}
        self.assertEqual(tags, {
            "AMJWF_Probe_Kuri", "AMJWF_Probe_Walnut", "AMJWF_Probe_Kaya",
            "AMJWF_Probe_Warabi", "AMJWF_Probe_Zenmai", "AMJWF_Probe_Fuki",
            "AMJWF_Probe_Udo", "AMJWF_Probe_Jinenjo",
        })
        self.assertTrue(all(el.text == "0.2" for el in op.find("value")))
        self.assertNotIn("AMJ_Tree_Shii", tags)

    def test_probe_builder_stages_only_in_system_temp(self):
        with tempfile.TemporaryDirectory(prefix="amjwf-probe-check-") as temp:
            output = Path(temp) / "staged"
            dest = builder.build(output)
            self.assertEqual(dest, output.resolve())
            self.assertTrue((dest / "About/About.xml").is_file())
            self.assertTrue((dest / "Defs/ThingDefs_Plants/AMJWF_SowingBases.xml").is_file())
            self.assertTrue((dest / "Defs/ThingDefs_Plants/AMJWF_ProbePlants.xml").is_file())
            self.assertTrue((dest / "Patches/AMJWF_ProbeWarmWildPlants.xml").is_file())
            self.assertIn("TEST ONLY", (dest / "README-TEST-ONLY.md").read_text())
            about = ET.parse(dest / "About/About.xml").getroot()
            self.assertEqual(about.findtext("packageId"), "sucro.amj.foraging.dev.sowingprobe")
            self.assertFalse((ROOT / "About/About.xml").exists())
            with self.assertRaises(ValueError):
                builder.build(output)

    def test_fixture_is_not_public_distribution_source(self):
        self.assertNotIn("/Defs/", str(CONCRETE.relative_to(ROOT)))
        self.assertNotIn("/Patches/", str(PATCH.relative_to(ROOT)))
        self.assertNotIn("AMJWF_Probe_Kuri", BASES.read_text(encoding="utf-8"))
        self.assertIn("TEST ONLY", (FIXTURE / "Defs/ThingDefs_Plants/AMJWF_ProbePlants.xml").read_text())

if __name__ == "__main__":
    unittest.main()
