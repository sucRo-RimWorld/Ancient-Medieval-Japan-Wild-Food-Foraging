#!/usr/bin/env python3
"""Build an isolated disposable RimWorld 1.6 sowing probe (NOT a release Mod).

Copies test fixture concrete plants, Foraging abstract parents and a synthetic
Warm-biome spawn patch to a separate temporary directory. No RimWorld files,
player saves, Workshop files or installed Mod directories are modified.

A subsequent native 1.6 test runner may stage this folder in its OWN
isolated test profile. Running this builder is only a file/manifest smoke test.
"""
from __future__ import annotations

import argparse
from pathlib import Path
import shutil
import tempfile
import uuid
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
FIXTURE = ROOT / "Tests" / "Fixtures" / "NativeSowingProbe"
BASES = ROOT / "Defs" / "ThingDefs_Plants" / "AMJWF_SowingBases.xml"
PLANTS = FIXTURE / "Defs" / "ThingDefs_Plants" / "AMJWF_ProbePlants.xml"
PATCH = FIXTURE / "Patches" / "AMJWF_ProbeWarmWildPlants.xml"

ABOUT = """<?xml version="1.0" encoding="utf-8"?>
<ModMetaData>
  <name>AMJ Wild Foraging Sowing Probe - DEV TEST ONLY</name>
  <author>AMJ development fixture - do not publish</author>
  <packageId>sucro.amj.foraging.dev.sowingprobe</packageId>
  <description>Disposable XML planting-inheritance smoke fixture.
Not playable Foraging content. Placeholder Vanilla graphics and synthetic
biome weights. No edible resources or natural fungus spawner.</description>
  <supportedVersions>
    <li>1.6</li>
  </supportedVersions>
  <modDependencies>
    <li>
      <packageId>sucro.ancientmedievaljapan.environment</packageId>
      <displayName>AMJ Environment</displayName>
    </li>
  </modDependencies>
</ModMetaData>
"""

NOTICE = """# DEVELOPER-ONLY RIMWORLD TEST PROBE

Never publish, install into the author's normal RimWorld Mods directory,
or include in Steam Workshop output.

Eleven **concrete** test ThingDefs reference the Foraging abstract sowing
parents. Vanilla art and fixture-only 3/30 day growth are deliberate test
stand-ins, **not approved Foraging art or balancing**. Artificial 0.2 Warm
biome weights exist solely to test mustBeWildToSow.

This probe does **not** contain real nuts, edible greens, or fungal spawning.
Under an **isolated native RimWorld 1.6 test instance**, verify:
1. After/before TreeSowing research, tree grow-zone menu differs for the
   three fixture nut trees in AMJ_WarmTemperateForest.
2. In AMJ_CoolTemperateForest, the same tree choices are absent even after
   TreeSowing because the fixture defines no wild population there.
3. Fuki/Udo are available as Ground sow options with normal pawns, but
   Warabi/Zenmai/Jinenjo and three fungus probes are not sowable.
4. Actually plant, grow, save/reload; inspect logs and wood chopping.
5. Shii in Environment is unchanged and still independently sowable under
   its original native research/region conditions.

The build script merely stages fixtures: it does NOT run these native checks.
"""

def build(output: Path) -> Path:
    temp_root = Path(tempfile.gettempdir()).resolve()
    dest = output.resolve()
    # Deliberately accept only temporary folders to avoid touching live Mods.
    try:
        dest.relative_to(temp_root)
    except ValueError as exc:
        raise ValueError("Probe output must remain under system temporary directory") from exc
    if dest == temp_root or dest.exists():
        raise ValueError("Output must be a NEW, empty probe folder under temp")
    for source in (BASES, PLANTS, PATCH):
        tree = ET.parse(source)
        if tree.getroot().tag not in {"Defs", "Patch"}:
            raise ValueError("Unexpected XML root: " + str(source))
    if len(ET.parse(PLANTS).getroot().findall("ThingDef")) != 11:
        raise ValueError("Expected exactly eleven concrete test plants")
    dest.mkdir(parents=True)
    targets = (
        (BASES, dest / "Defs/ThingDefs_Plants/AMJWF_SowingBases.xml"),
        (PLANTS, dest / "Defs/ThingDefs_Plants/AMJWF_ProbePlants.xml"),
        (PATCH, dest / "Patches/AMJWF_ProbeWarmWildPlants.xml"),
    )
    for src, dst in targets:
        dst.parent.mkdir(parents=True, exist_ok=True)
        shutil.copyfile(src, dst)
    about = dest / "About/About.xml"
    about.parent.mkdir(parents=True)
    about.write_text(ABOUT, encoding="utf-8")
    (dest / "README-TEST-ONLY.md").write_text(NOTICE, encoding="utf-8")
    ET.parse(about)
    return dest

def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output", type=Path, help="New folder within system temp")
    args = parser.parse_args()
    folder = args.output
    if folder is None:
        folder = Path(tempfile.gettempdir()) / ("amjwf-native-sowing-probe-" + uuid.uuid4().hex[:12])
    print("Staged TEST-ONLY, non-publishable, non-player Mod at:", build(folder))

if __name__ == "__main__":
    main()
