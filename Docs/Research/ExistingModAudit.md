# Wild Food Foraging — Existing implementation and source audit

**2026-10-10 status:** INCOMPLETE. This is evidence triage, **not** the Project implementation-entry `status=ready` decision. No owner-qualified JSON was created.

**Approved dependency boundary (2026-10-10):** Environment (`sucro.ancientmedievaljapan.environment`) supplies required Japan-native vegetation including `AMJ_Tree_Shii`; no substitute standalone vegetation/biome system is planned for Foraging. This architectural decision does **not** complete VE/non-VE function-overlap research or authorize additional tree/food definitions.

## Proven directly from available sources

- **Environment main** `Defs/ThingDefs_Plants/AMJ_WildPlants.xml` blob `e1985eb77f80f0f0c3c3140962dcb5810e94b4a5`: `AMJ_Tree_Shii` uses `TreeBase`, `harvestYield=42`; `AMJ_Tree_Beech` uses `DeciduousTreeBase`, `harvestYield=40`; no AMJE edible nut harvestedThingDef in these two tree definitions. These values are not assigned to edible nuts.
- **User-provided local MO 1.6 archive**, `3219596926.zip`, SHA-256 `6ed379d7c400db43b3af2a9a7fd6203f7f7e99ca1a670d36b0f73fd1adccc9e3`: file `3219596926/1.6/Defs/ThingDefs_Plants/Plants_Cultivated_Farm.xml` defines `DankPyon_Tree_Apple` fruit `DankPyon_RawApples` (20), `DankPyon_Tree_Lemon` fruit `DankPyon_RawLemons` (12), `DankPyon_Tree_Mulberry` fruit `DankPyon_RawMulberry` (6), all deriving `TreeBase`; `DankPyon_Plant_Mushrooms` harvesting `RawFungus` (6), deriving `CavePlantBase`. These are **source XML facts only**, not evidence of repeated non-destructive harvest or preservation of timber.
- MO also contains fantastic `DankPyon_Tree_GriffonBerry`: existence does not make it historically appropriate or an AMJ fruit candidate.

## Comparison targets requiring current source inspection

| Ecosystem | Candidate | Current evidence / unresolved issue |
| --- | --- | --- |
| Vanilla/AMJ | Vanilla TreeBase harvest/cut; AMJE Shii/Beech | Need native 1.6 loaded behavior, job and wood results, optional AMJE mechanics |
| Medieval Overhaul | MO fruit trees, mushrooms and wood resource patches | Defs confirmed above, runtime cut/harvest sequence **not** checked |
| VE | [Vanilla Plants Expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=2134308522), [More Plants](https://steamcommunity.com/sharedfiles/filedetails/?id=2748889667) | Features overlap; current XML/code, load order, actual 1.6 compatibility and Japan-forest need unresolved |
| VE | [Vanilla Fungi Expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=3006389281) | Potential fungi overlap; suitability / actual source unresolved |
| Non-VE | [More Mushrooms](https://steamcommunity.com/sharedfiles/filedetails/?id=3813323629) | Project previously identified conditional patch/reuse candidate, actual current Defs/MO compatibility unresolved |
| Non-VE | Famine Food (prior Project research) | Prior-art food scarcity candidate; exact current version/Defs and necessity unresolved |

Project's 2026-10-10 screening in [historical research](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/blob/main/Docs/Research/HuntingGatheringCoastalCandidate.md) listed these alternatives; public descriptions are not proof of actual XML/load behavior. Do not promote partial source evidence into a completed VE **and non-VE** audit.

## Pending decision

Evaluate function overlap, historical fit, dependency footprint, retention, progression/balance, reusable framework, maintenance and licensing for each material alternative. Decide reuse / optional compatibility / narrow patch / independent implementation with concrete unmet need. Save validated evidence notes and a Project-format decision only once the unresolved material cases are closed. Then evaluate a minimal **non-destructive Sudajii nut harvest** using the required Environment tree without changing its identity or vanilla/MO wood output. No new runtime content is authorized by this record.

## 2026-10-10 — 1.6 native harvest/woodcut source audit (code-level, not runtime PASS)

**Snapshot and evidence level.** Public [Chillu1/RimWorldDecompiled](https://github.com/Chillu1/RimWorldDecompiled/tree/2d508035082e7cb0c8e29e230d26bda6e546928f), commit `2d508035082e7cb0c8e29e230d26bda6e546928f` (committer 2026-05-20, message `v1.6.9438.38202 update, block namespaces`). Inspected the actual decompiled **1.6 snapshot source**, **not** the author's installed game's Assembly-CSharp.dll and not a loaded game. These code-path observations refine but do not replace the unrun runtime checks.

| Source | Verified mechanism and constraint |
| --- | --- |
| [PlantProperties.cs](https://github.com/Chillu1/RimWorldDecompiled/blob/2d508035082e7cb0c8e29e230d26bda6e546928f/RimWorld/PlantProperties.cs#L201-L216) | `Harvestable` is derived from `harvestYield`; `HarvestDestroys` is `harvestAfterGrowth <= 0`; `IsTree` depends on `harvestTag=Wood` or `forceIsTree`. |
| [Plant.cs](https://github.com/Chillu1/RimWorldDecompiled/blob/2d508035082e7cb0c8e29e230d26bda6e546928f/RimWorld/Plant.cs#L621-L663) | `PlantCollected` destroys the plant and may spawn a stump only if `HarvestDestroys`; otherwise it retains the plant and resets `growthInt` to `harvestAfterGrowth`. |
| [JobDriver_PlantWork.cs](https://github.com/Chillu1/RimWorldDecompiled/blob/2d508035082e7cb0c8e29e230d26bda6e546928f/RimWorld/JobDriver_PlantWork.cs#L64-L108) | **Both harvest and cut jobs** use the plant's sole `harvestedThingDef` as primary yield and invoke `PlantCollected`; `ThingComp.GetAdditionalHarvestYield` is called through the same job, not through a distinct fruit-picking command. |
| [JobDriver_PlantCut.cs](https://github.com/Chillu1/RimWorldDecompiled/blob/2d508035082e7cb0c8e29e230d26bda6e546928f/RimWorld/JobDriver_PlantCut.cs#L5-L24) and [Toils_Interact.cs](https://github.com/Chillu1/RimWorldDecompiled/blob/2d508035082e7cb0c8e29e230d26bda6e546928f/Verse.AI/Toils_Interact.cs#L5-L25) | Cut uses `PlantCollected` then **explicitly destroys its target**; the positive `harvestAfterGrowth` retention rule therefore does not make the **cut** job retain a tree. |
| [WorkGiver_PlantsCut.cs](https://github.com/Chillu1/RimWorldDecompiled/blob/2d508035082e7cb0c8e29e230d26bda6e546928f/RimWorld/WorkGiver_PlantsCut.cs#L59-L72) | Existing `HarvestPlant` and `CutPlant` designations map to different jobs, but **primary harvested item definition does not switch between those jobs**. |

**Vanilla/MO comparison:** The supplied MO **1.6** `Plants_Cultivated_Farm.xml` sets `TreeBase` apple/lemon/mulberry to `harvestTag=Standard`, a **fruit** `harvestedThingDef`, and `harvestAfterGrowth=0.30`. This is an existing **non-destructive primary fruit-harvest** pattern at XML/code level, but its old tree timber value/woodcut output and actual loaded MO changes are **not** proven. Environment's existing `AMJ_Tree_Shii` and `AMJ_Tree_Beech` provide the wood-yield base and do not set a separate nut output. Their `harvestYield=42/40` must not be reused as fruit balances.

**Actual VE framework mechanism inspected:** [VanillaExpandedFramework DualCropExtension.cs](https://github.com/Vanilla-Expanded/VanillaExpandedFramework/blob/656a58858c833a3c281d8aa04788e078de168977/Source/VEF/Plants/DefExtensions/DualCropExtension.cs) and its [PlantCollected Harmony prefix](https://github.com/Vanilla-Expanded/VanillaExpandedFramework/blob/656a58858c833a3c281d8aa04788e078de168977/Source/VEF/Plants/Harmony/Plant_PlantCollected.cs). This extension can give a **second product on PlantCollected**, including the same callback used by cutting; it does **not** define a separate independent nut-picking action. If a wood-producing tree were made non-destroying and given this extension, a native harvest would still also generate the tree's primary wood output. VEF is therefore **prior art for dual yields, not yet a proven solution** to "pick only nuts while keeping timber for cutting"; it remains an **optional** ecosystem rather than a new mandatory dependency. Actual VPE fruit-tree 1.6 Defs, VE load interaction and runtime are still uninspected.

### Evidence-based conclusion — what NOT to change yet

1. **Do not** patch `AMJ_Tree_Shii.plant.harvestedThingDef` from wood to nuts and claim timber compatibility: the cut job uses the same primary item.
2. **Do not** simply add `harvestAfterGrowth > 0` to the existing wood-producing tree: native harvest would still primarily yield wood repeatedly, an unintended timber source.
3. **Do not** add a dual-crop extension and claim separate nut harvest: normal harvest and cut both invoke `PlantCollected`.
4. **Research next:** inspect native ThingComp/gizmo/Job interfaces and any credible existing *independent tree-product collection* implementation, to see if a narrowly scoped optional, tree-preserving fruit-collection action can avoid modifying Environment's wood TreeDef. If it requires a new Job/C# component, justify it against actual VE and non-VE alternatives and explicitly author the smallest tested contract **before production**. MO cutting/wood chain and previous-AMJE-save are separate native/runtime gates.
5. This source audit **does not** settle the 1.6 loaded `TreeBase` inheritance, any MO Harmony wood overrides, VE fruit-tree Defs, current More Mushrooms/Famine Food, harvest quantities, regrowth timing, seasonal control, visual coverage, historical species acceptance or release compatibility.

**Decision remains HOLD**: no `status=ready` independent-implementation record, no gameplay Def, no Job, no packaged Mod, no RimWorld/Pickle/RimTest PASS. The dependency on Environment remains a separate already-approved author decision.
