#!/usr/bin/env python3
"""Build a disposable 1.6 Shii mechanism probe, never a playable WFF Mod."""
from __future__ import annotations

import argparse
from pathlib import Path
import os
import tempfile
import uuid
import xml.etree.ElementTree as ET

ABOUT = """<?xml version="1.0" encoding="utf-8"?>
<ModMetaData>
  <name>AMJ Shii Gather Diagnostic - NOT FOR RELEASE</name>
  <author>AMJ test fixture</author>
  <packageId>sucro.amj.foraging.dev.shiigatherprobe</packageId>
  <supportedVersions><li>1.6</li></supportedVersions>
  <modDependencies><li><packageId>sucro.ancientmedievaljapan.environment</packageId><displayName>AMJ Environment</displayName></li></modDependencies>
  <description>Temporary, isolated native-mechanism diagnostic. Produces one unit of Steel as a marker; never food or release balance.</description>
</ModMetaData>
"""

DEFS = """<?xml version="1.0" encoding="utf-8"?>
<Defs>
  <JobDef>
    <defName>AMJ_GatherShiiNuts</defName>
    <driverClass>AMJ.WildFoodForaging.JobDriver_GatherShiiNuts</driverClass>
    <reportString>diagnosing Shii gathering.</reportString>
  </JobDef>
  <DesignationDef>
    <defName>AMJ_GatherShiiNuts</defName>
    <label>Shii gather diagnostic order</label>
    <texturePath>UI/Designators/Harvest</texturePath>
  </DesignationDef>
</Defs>
"""

# This is a test-only patch on an existing Environment tree. It does not edit
# the wood-primary harvest fields, original tree graphics or biome occurrence.
PATCH = """<?xml version="1.0" encoding="utf-8"?>
<Patch>
  <Operation Class="PatchOperationConditional">
    <xpath>Defs/ThingDef[defName="AMJ_Tree_Shii"]/comps</xpath>
    <match Class="PatchOperationAdd">
      <xpath>Defs/ThingDef[defName="AMJ_Tree_Shii"]/comps</xpath>
      <value>
        <li Class="AMJ.WildFoodForaging.CompProperties_ShiiGatherState">
          <productDef>Steel</productDef>
          <yieldCount>1</yieldCount>
          <cooldownTicks>1800</cooldownTicks>
          <workTicks>60</workTicks>
        </li>
      </value>
    </match>
    <nomatch Class="PatchOperationAdd">
      <xpath>Defs/ThingDef[defName="AMJ_Tree_Shii"]</xpath>
      <value>
        <comps>
          <li Class="AMJ.WildFoodForaging.CompProperties_ShiiGatherState">
            <productDef>Steel</productDef>
            <yieldCount>1</yieldCount>
            <cooldownTicks>1800</cooldownTicks>
            <workTicks>60</workTicks>
          </li>
        </comps>
      </value>
    </nomatch>
  </Operation>
</Patch>
"""

FILES = {
    "About/About.xml": ABOUT,
    "Defs/AMJWF_ShiiGatherDiagnostic.xml": DEFS,
    "Patches/AMJWF_ShiiGatherDiagnostic.xml": PATCH,
}
NOTICE = """# DIAGNOSTIC ONLY — NEVER DISTRIBUTE
Uses synthetic Steel marker x1, 1800-tick cooldown, 60-tick work.
All numbers are temporary test input, NOT Foraging balance.
Only stage under system temporary directory on isolated native 1.6 game.
Compile the three owner C# scaffold sources and a separate diagnostic
Quickstart into a temporary assembly before invoking the native runner.
No author's player saves, Steam Workshop, or normal installed Mods touched.
A direct component action does not demonstrate pawn Job execution or old-save
migration. F04–F14 cannot be claimed PASS from this fixture alone.
"""

def build(output: Path) -> Path:
    roots = [Path(tempfile.gettempdir()).resolve()]
    if os.environ.get("RUNNER_TEMP"):
        roots.append(Path(os.environ["RUNNER_TEMP"]).resolve())
    dest = output.resolve()
    if dest.exists() or dest.is_symlink():
        raise ValueError("Refusing to overwrite a probe path")
    if not any(dest != root and dest.is_relative_to(root) for root in roots):
        raise ValueError("Only new system/runner temporary directories allowed")
    for name, body in FILES.items():
        parsed = ET.fromstring(body)
        if parsed.tag != {"About/About.xml": "ModMetaData",
                          "Defs/AMJWF_ShiiGatherDiagnostic.xml": "Defs",
                          "Patches/AMJWF_ShiiGatherDiagnostic.xml": "Patch"}[name]:
            raise ValueError("Unexpected test fixture root: " + name)
        file = dest / name
        file.parent.mkdir(parents=True, exist_ok=True)
        file.write_text(body, encoding="utf-8")
    (dest / "README-TEST-ONLY.md").write_text(NOTICE, encoding="utf-8")
    return dest

def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output", type=Path)
    args = parser.parse_args()
    target = args.output or Path(tempfile.gettempdir()) / (
        "amjwf-shii-diagnostic-" + uuid.uuid4().hex[:12])
    print(build(target))

if __name__ == "__main__":
    main()
