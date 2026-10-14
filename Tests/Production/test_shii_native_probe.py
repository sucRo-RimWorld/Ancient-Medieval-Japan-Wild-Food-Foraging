"""Source-only checks for the disposable Shii diagnostic probe."""
import importlib.util
from pathlib import Path
import tempfile
import unittest
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
SCRIPT = ROOT / "Tests/NativeShiiGatherProbe/build_probe.py"
spec = importlib.util.spec_from_file_location("wff_shii_probe", SCRIPT)
module = importlib.util.module_from_spec(spec)
spec.loader.exec_module(module)

class TestShiiNativeProbe(unittest.TestCase):
    def test_nonshipping_isolated_fixture(self):
        with tempfile.TemporaryDirectory(prefix="amj-wff-shii-") as temp:
            target = Path(temp) / "fixture"
            module.build(target)
            about = ET.parse(target / "About/About.xml").getroot()
            self.assertEqual(about.findtext("packageId"),
                             "sucro.amj.foraging.dev.shiigatherprobe")
            self.assertEqual(about.findtext("./modDependencies/li/packageId"),
                             "sucro.ancientmedievaljapan.environment")
            defs = ET.parse(target / "Defs/AMJWF_ShiiGatherDiagnostic.xml").getroot()
            self.assertEqual(len(defs.findall("JobDef")), 1)
            self.assertEqual(len(defs.findall("DesignationDef")), 1)
            patch = (target / "Patches/AMJWF_ShiiGatherDiagnostic.xml").read_text()
            self.assertIn("<productDef>Steel</productDef>", patch)
            self.assertIn("<cooldownTicks>1800</cooldownTicks>", patch)
            self.assertIn("AMJ_Tree_Shii", patch)
            self.assertNotIn("<harvestedThingDef>", patch)
            self.assertNotIn("<harvestYield>", patch)
            with self.assertRaises(ValueError):
                module.build(target)
        with self.assertRaises(ValueError):
            module.build(ROOT / "About")

    def test_mechanism_direct_only(self):
        code = (ROOT / "Tests/NativeShiiGatherProbe/WffShiiGatherQuickstart.cs").read_text()
        for token in ("WffShiiGatherQuickstart", "comp.TryGather(pawn)",
                      "originalWood", "after - before == 1",
                      "tree.Spawned", "!comp.TryGather(pawn)"):
            self.assertIn(token, code)
        self.assertNotIn("JobDriver_PlantCut", code)
