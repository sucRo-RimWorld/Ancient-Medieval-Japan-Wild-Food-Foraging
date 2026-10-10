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
