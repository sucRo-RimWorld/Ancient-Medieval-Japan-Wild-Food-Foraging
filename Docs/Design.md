# Ancient & Medieval Japan - Wild Food Foraging — Design

**Author-confirmed name (2026-10-08):** Ancient & Medieval Japan - Wild Food Foraging / 中世日本 - 山野採集  
**Target:** RimWorld 1.6  
**Status:** Project design promoted to a dedicated owner on 2026-10-10; **specification only, no gameplay implementation or approved balance values**.

## 1. Purpose and hard boundary

Represent **edible wild plants and forest foods** accessible to people living near woodland, valleys and mountain foothills in ancient-to-medieval Japan, using meaningful settlement food decisions without creating an abstract inexhaustible resource.

**Within scope:** edible nuts/tree products, wild yams (自然薯), edible mountain greens (山菜), and potentially wild mushrooms **only if** source comparison establishes a gap. Seasonal differences are a candidate for an actual survival decision, not yet a numerical production rule.

**Out of scope:** a new animal hunting/trapping/tracking/butchering subsystem; generic fishing, shellfish, seaweed, coastal gathering or shell middens; dedicated food preservation/drying/pickling; grain farming; arbitrary world-map expeditions in the first implementation; placing inexhaustible gathering buildings just to simulate excursions. Vanilla hunting remains unchanged. Edo-specific culture/processing is outside AMJ's target.

## 2. Ownership and optional dependencies

| Feature | Canonical owner |
| --- | --- |
| New natural species identity, tree/plant Defs as vegetation, regional wildPlants distribution, tree sowing policy, plant art | [Environment](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Environment/blob/main/Docs/Design.md) |
| Wild edible collection and food-use logic, forage-resource treatment, integration with Environment-owned trees | **This repository** |
| Preservation, dried fruit, pickled fruit, other secondary processing | Future Preservation consumer (Project until an owner exists) |
| Wild/coastal shellfish and algae | Separate Coastal Gathering candidate (Project) |
| Dry-field cereals | AMJ Grains |

This Mod must not require Environment, MO, DLC or unrelated vegetation Mods merely to provide the bounded wild-food loop. **How a standalone natural supply is provided is still an open design/implementation question**. With Environment installed, prefer its owned vegetation and avoid duplicate species/plant generation, conflicting harvest overrides or a second source-of-truth for regional distribution. Do not add an Environment tree Def in this repository as an expedient.

## 3. Geography and action interface — confirmed direction

- Work on reachable **cells in the current settlement map**: forest, valley, foothill, natural soil, open cells near rock. A pawn does not need a new mountain-climbing system to gather.
- Mountainous / Large Hills Hilliness is a **candidate factor** for food-resource availability and variety; do not encode a guaranteed numeric hilliness multiplier or fake biome spawn precision before checking the actual RimWorld generator and Environment coexistence.
- Prefer **normal harvest designation and existing Plant harvest jobs** when feasible. New interactions, additional harvest JobDefs or C# are not pre-authorized; prove the missing native behavior first.
- No initial world-map gathering expedition site, abstract production building or infinite spawn mechanic.

## 4. Species / food candidate ledger (not approved Defs)

| Candidate | Evidence from prior Project design | Current decision |
| --- | --- | --- |
| Sudajii / Castanopsis nuts | Existing Environment `AMJ_Tree_Shii`; its tree description notes edible seeds | First **technical research** target. Separate edible product collection must not break cutting/wood flow |
| Buna / beech nuts | Existing Environment `AMJ_Tree_Beech`; an optional forest-food distinction was proposed | Investigate edibility, processing and real added gameplay; **not adopted** |
| Kuri / chestnut | Previous Environment vegetation review marked candidate B+ | Not promoted to a new tree/ThingDef |
| Natural yams (自然薯) | Forest food instead of farm crop; mountain/foothill soil candidate | Species, harvesting, world placement and food processing unresolved |
| Mountain greens (山菜) | Foraging category from Project design | Individual species, rarity and use unresolved |
| Wild mushrooms | More Mushrooms or MO may already supply enough | Audit first; prefer optional compatibility over duplication |
| Kaki / ume / peach fruit trees | Project DeferredPlantsAndProcessing records preservation overlap / rejection | No plain fruit tree promoted by this work; downstream preservation remains separate |

Existing AMJE `AMJ_Tree_Shii` and `AMJ_Tree_Beech` are currently **wood-producing trees**, not edible nut providers. Their `harvestYield=42/40` are not food values. Refer to [Environment AMJ_WildPlants.xml](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Environment/blob/main/Defs/ThingDefs_Plants/AMJ_WildPlants.xml).

## 5. First technical decision: collecting nuts without cutting the tree

The necessary first check is whether RimWorld 1.6's **actual loaded native Plant/Tree harvest behavior** supports separately collecting an edible product repeatedly while preserving tree cutting and its existing wood outputs, including MO wood-chain substitution.

Do not conclude that simply setting `harvestedThingDef` on a `TreeBase` preserves both products: inspect Vanilla source, MO 1.6 fruit-tree definitions, actual cut-vs-harvest behavior and existing VE/non-VE implementation first. Compare narrow XML/patch-only routes to a justified minimal Harmony/C# implementation if the native route fails. A disposable probe may establish semantics but is **not** production implementation.

Determine in order: existing behavior → source ownership of product and patch → no-Environment fallback → real harvest and cut coexistence → seasonal/renewal mechanics → balance and art. No default numeric fruit yield, regrowth period or season is fixed.

## 6. Entry tests and release gates

1. [Existing-Mod Audit](Research/ExistingModAudit.md): inspect actual XML/code for Vanilla/MO, VE and non-VE, including More Mushrooms/Famine Food; record versions, licensing, dependency cost, retained content, and exact mechanical overlap. A Project-format `status=ready` JSON is **not present** because this audit is open. [Entry format](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/blob/main/Docs/Research/ExistingModAudit.md).
2. The actual native harvest/cut workflow must prove that gathering does **not** consume the tree unintentionally, replace timber yields, or double-reward wood and food. MO, stand-alone and Environment/coexistence cases are separate checks.
3. Species additions and existing-save compatibility follow the Environment [old-save fruit-tree test specification](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Environment/blob/main/Docs/GoldenPaths/PlantSowingTests.md): pre-update AMJE save → upgrade → **natural** existing-map appearance → mature real harvest and (where sowable) research-gated sow Job → save/reload, complete logs, ERROR zero. Forced-spawn-only tests do not clear natural-spawning claims.
4. Distinguish Vanilla, MO, CCTO and MO+CCTO runtime profiles; do not infer an unrun profile PASS from XML, mocks or historical logs.
5. Approved Japanese historical descriptions before translation, actual accepted artwork and the exact final Steam subscriber payload are separate future gates. No public version, package ID, Workshop ID, runtime PASS or release approval has been assigned.

## Sources / provenance

Migrated from Project main `Docs/Research/HuntingGatheringCoastalCandidate.md` (scope/geography/harvest directions), `Docs/Ideas.md` (species priorities), `Docs/Research/DeferredPlantsAndProcessing.md` (food-processing boundaries), `Docs/ImplementationPriorities.md` and `Docs/Roadmap.md` (execution after Ironmaking, before further Waterworks/Rice feature expansion). Project's research filename remains as a historical index to this document. No newly approved feature is implied by migration.
