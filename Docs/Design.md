# Ancient & Medieval Japan - Wild Food Foraging — Design

**Author-confirmed name (2026-10-08):** Ancient & Medieval Japan - Wild Food Foraging / 中世日本 - 山野採集  
**Target:** RimWorld 1.6  
**Status:** Project design promoted to a dedicated owner on 2026-10-10; **specification only, no gameplay implementation or approved balance values**.

## 1. Purpose and hard boundary

Represent **edible wild plants and forest foods** accessible to people living near woodland, valleys and mountain foothills in ancient-to-medieval Japan, using meaningful settlement food decisions without creating an abstract inexhaustible resource.

**Within scope:** edible nuts/tree products, wild yams (自然薯), edible mountain greens (山菜), and potentially wild mushrooms **only if** source comparison establishes a gap. Seasonal differences are a candidate for an actual survival decision, not yet a numerical production rule.

**Out of scope:** a new animal hunting/trapping/tracking/butchering subsystem; generic fishing, shellfish, seaweed, coastal gathering or shell middens; dedicated food preservation/drying/pickling; grain farming; arbitrary world-map expeditions in the first implementation; placing inexhaustible gathering buildings just to simulate excursions. Vanilla hunting remains unchanged. Edo-specific culture/processing is outside AMJ's target.

## 2. Environment required dependency and ownership

| Feature | Canonical owner |
| --- | --- |
| New natural species identity, tree/plant Defs as vegetation, regional wildPlants distribution, tree sowing policy, plant art | [Environment](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Environment/blob/main/Docs/Design.md) |
| Wild edible collection and food-use logic, forage-resource treatment, integration with Environment-owned trees | **This repository** |
| Preservation, dried fruit, pickled fruit, other secondary processing | Future Preservation consumer (Project until an owner exists) |
| Wild/coastal shellfish and algae | Separate Coastal Gathering candidate (Project) |
| Dry-field cereals | AMJ Grains |

**Author-approved change (2026-10-10): AMJ Environment is a required runtime dependency.** Japanese natural vegetation, including the existing `AMJ_Tree_Shii` (スダジイ), is indispensable to the intended medieval-Japan forest gathering experience. The Environment `About/About.xml` packageId is **`sucro.ancientmedievaljapan.environment`**. When Foraging's playable `About/About.xml` is introduced, its `modDependencies` must require that exact packageId and its loading must follow Environment. No playable Foraging About metadata exists yet; this is an approved **design contract**, not a claim that RimWorld dependency metadata has been implemented.

**Dependency direction: Wild Food Foraging → Environment, never the reverse.** Environment keeps ownership of native plant and tree Defs, biomes, natural distribution, planting and forestry, and original plant art. Foraging adds edible collection and food use; it does not provide a fallback vegetation/biome set or duplicate Environment trees. MO, CCTO, DLC and other vegetation Mods remain optional. A required Def does **not** imply Sudajii must grow in every biome or on every map; Environment's regional distribution still governs where it is present.

**Required ecological foundation is distinct from approved food content.** `AMJ_Tree_Shii` and `AMJ_Tree_Beech` already exist in Environment, and Sudajii nuts are the first technical research target. Beech nuts, Kuri/chestnut, additional fruit trees, natural yams, mountain greens and mushrooms remain individual candidate decisions; no unapproved species, yields, harvest cycles or recipes become mandatory through this dependency decision.

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
| Kuri / chestnut | Previous Environment vegetation review marked candidate B+; non-VE VV - New Harvest also supplies chestnut trees | Not promoted to a new tree/ThingDef. Compare reuse/optional coexistence and natural Japan-biome suitability **before** introducing a duplicate species; see [fruit-tree prior-art audit](Research/AdditionalFruitTreeModAudit-20261014.md) |
| Natural yams (自然薯) | Forest food instead of farm crop; mountain/foothill soil candidate | Species, harvesting, world placement and food processing unresolved |
| Mountain greens (山菜) | Foraging category from Project design | Individual species, rarity and use unresolved |
| Wild mushrooms | More Mushrooms or MO may already supply enough | Audit first; prefer optional compatibility over duplication |
| Kaki / ume / peach fruit trees | Project DeferredPlantsAndProcessing records preservation overlap / rejection | No plain fruit tree promoted by this work; downstream preservation remains separate |

**The tree-nut foraging scope is not limited to Sudajii.** Sudajii and Buna are explicitly recorded potential nut-food sources; Kuri remains a candidate needing a new Environment tree decision. Existing Vanilla Oak occurs in Environment's cool-temperate biome, but its acorn collection is **not yet an approved food-resource candidate**. Technical investigation starts with Sudajii only as a representative initial case, **not** as an exclusive species list. Reusable collecting mechanics should be evaluated across each subsequently approved species without assuming identical availability, handling or yield.

Existing AMJE `AMJ_Tree_Shii` and `AMJ_Tree_Beech` are currently **wood-producing trees**, not edible nut providers. Their `harvestYield=42/40` are not food values. Refer to [Environment AMJ_WildPlants.xml](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Environment/blob/main/Defs/ThingDefs_Plants/AMJ_WildPlants.xml).

## 5. First technical decision: reusable tree-nut collection without cutting trees

**1.6 code inspection result (2026-10-10):** The 1.6 public decompiled `PlantCollected` path already supports a **non-destroying ordinary harvest** when `harvestAfterGrowth > 0`, but the plant's **primary harvestedThingDef is shared by harvest and cutting**. `JobDriver_PlantCut` also explicitly destroys its plant target. Thus merely switching the existing Environment `AMJ_Tree_Shii` to a fruit output, making its existing wood yield repeatable, or attaching an unqualified dual-output extension does **not** establish **separate edible-nut picking while preserving woodcut yields**. The actual MO 1.6 fruit-tree XML demonstrates regrowing primary fruit harvesting, **not** a separate timber output: apple/lemon/mulberry have only primary fruit (20/12/6), `harvestAfterGrowth=0.30`, and no timber-yield extension; mulberry's additional product is silkworm. MO's WoodChain setting targets `WoodLog` primary yields rather than these fruit Defs. The source therefore **predicts fruit rather than timber from mature fruit-tree cutting**, although the actual loaded MO cut/drop result has not been executed. This does not provide the required Shii dual-use timber contract. [MO 1.6 source audit](Research/ExistingModAudit.md#2026-10-14--author-supplied-mo-16-fruit-tree-felling-path-source-only). Read [actual source evidence and restrictions](Research/ExistingModAudit.md#2026-10-10--16-native-harvestwoodcut-source-audit-code-level-not-runtime-pass).

The remaining technical question is whether a **separate tree-preserving nut collection action, reusable for more than one eligible species**, can use native ThingComp/Job capabilities or established small existing mechanics while leaving Environment's original tree planting, growth, timber harvestedThingDef, wood cutting and MO substitution untouched.

**Independent 2026-10-14 fruit-tree corroboration:** VV - New Harvest: Trees' author confirms ordinary fruit picking leaves the tree alive for renewed harvest, and describes its implementation as basic XML only; a third-party 1.6 VGP loaded-error log quotes apple/peach/cherry fruit-primary XML with `harvestAfterGrowth=0.30`. These are orchard/fruit-primary precedents **not an inspected independent second nut output on wood-primary Shii**. See [direct-source and author statements audit](Research/AdditionalFruitTreeModAudit-20261014.md#2026-10-14--primary-sourceloaded-log-follow-up-for-vv-and-vgp). The VV Trees author declares **CC BY-NC-ND 4.0**; no copying/modification of upstream artwork/content assets into AMJ is approved. The first-stage technical necessity assessment stays on *preserving the exact existing Environment wood behavior*, not adding an equivalent fruit orchard. Test the common mechanics first against Sudajii, then evaluate applicability to existing Buna and any independently approved future tree (including Kuri); do not treat the Sudajii proof as clearance for other species or their food yields. Research alternate native/VE/non-VE actual implementations and then compare a narrow XML/patch route against minimal justified custom behavior. Do not add an independent Job/C# or gameplay Def before the full Project-format prior-art implementation decision.

Determine in order: original native behavior (source checked, loaded game untested) → independent-harvest prior art and loaded Environment+MO coexistence → food-item/patch ownership → real tree survival and **separate** normal timber cutting → seasonal and renewal design → balance/art. No fruit yield, regrowth period or season is fixed.

## 6. Entry tests and release gates

1. [Existing-Mod Audit](Research/ExistingModAudit.md): inspect actual XML/code for Vanilla/MO, VE and non-VE, including More Mushrooms/Famine Food; record versions, licensing, dependency cost, retained content, and exact mechanical overlap. A Project-format `status=ready` JSON is **not present** because this audit is open. [Entry format](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/blob/main/Docs/Research/ExistingModAudit.md).
2. The actual native harvest/cut workflow must prove that gathering does **not** consume the tree unintentionally, replace timber yields, or double-reward wood and food. Environment+Vanilla and Environment+MO cases (with optional CCTO profiles) are separate checks; an Environment-free Foraging profile is no longer in scope.
3. Species additions and existing-save compatibility follow the Environment [old-save fruit-tree test specification](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Environment/blob/main/Docs/GoldenPaths/PlantSowingTests.md): existing-AMJE save → add Foraging and/or upgrade AMJE as applicable → **natural** existing-map appearance for added wild plants → mature real harvest and (where sowable) research-gated sow Job → save/reload, complete logs, ERROR zero. Foraging food extraction from trees already on the map and newly added AMJE wild plant spawning must be tested as distinct scenarios. Forced-spawn-only tests do not clear natural-spawning claims.
4. Include Environment in **every** runtime profile; distinguish Environment+Vanilla, Environment+MO, Environment+CCTO and Environment+MO+CCTO; do not infer an unrun profile PASS from XML, mocks or historical logs.
5. Approved Japanese historical descriptions before translation, actual accepted artwork and the exact final Steam subscriber payload are separate future gates. No public version, package ID, Workshop ID, runtime PASS or release approval has been assigned.

## Sources / provenance

Migrated from Project main `Docs/Research/HuntingGatheringCoastalCandidate.md` (scope/geography/harvest directions), `Docs/Ideas.md` (species priorities), `Docs/Research/DeferredPlantsAndProcessing.md` (food-processing boundaries), `Docs/ImplementationPriorities.md` and `Docs/Roadmap.md` (execution after Ironmaking, before further Waterworks/Rice feature expansion). Project's research filename remains as a historical index to this document. No newly approved feature is implied by migration.
