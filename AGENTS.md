# AMJ Wild Food Foraging — Agent Instructions

## Start here
1. Read this file and **main:Docs/Coordination.md** at the start of this repository's work.
2. Read Project [AGENTS.md](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/blob/main/AGENTS.md) and [SharedRules](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/blob/main/Docs/SharedRules.md), including the latest Project main Coordination only when cross-repository status matters.
3. Read the local [Design](Docs/Design.md), then the relevant [Existing-Mod Audit](Docs/Research/ExistingModAudit.md) and actual source before implementation.

## Ownership
This repository owns **edible wild nature as content and gameplay** for Ancient & Medieval Japan - Wild Food Foraging / 中世日本 - 山野採集: newly introduced edible wild tree, herb/green, fungus and tuber/root PlantDefs (where needed), their edible products, plant graphics, biome wildPlants placement patches, gathering and food-use behavior. Its runtime prerequisite is AMJ Environment. **Environment retains its existing Shii/Beech and other baseline landscape/forestry plants and owns biome/climate identities**, so Foraging adds edible gathering to already owned environmental trees via patches without duplicating their PlantDefs. New food-purpose species are **not** implementation requests to Environment. Foraging does not own general agriculture, food preservation, hunting, fishing or coastal gathering.

The confirmed design is in Docs/Design.md; Project retains only shared architecture and ownership pointers. Cross-repository handoff/status belongs in main:Docs/Coordination.md of the actual owner, never via the user.

## Resource selection is independent of other Mods
- **Author-confirmed 2026-10-14:** Foraging's objective is the historically appropriate collection of wild foods in ancient-to-medieval Japan. Individual edible species are selected by Japan-period use, natural habitat, edible part, safety and fit with the forest/foothill gathering theme. **Another Mod containing the same tree, nut, green or mushroom never excludes, defers or downgrades its adoption in Foraging.** There is no project-design requirement to delegate its food list to generic fruit/mushroom Mods.
- VE/non-VE/MO source comparisons remain relevant to **technical implementation reuse** under Project's implementation-entry rules. Optional co-install compatibility (duplicate resources/spawns, XML conflicts) is a **later optional compatibility test**, not a precondition for accepting species or implementing the independently-owned core Foraging content. Never silently convert a missing third-party XML checkout into an adoption HOLD.

## Entry and evidence
- **Production implementation is HOLD** until VE **and non-VE**, Vanilla, MO and AMJ-owner actual-source comparison establishes an unresolved need and a validated Project-format implementation decision. Prior-art research and disposable tests may continue.
- Existing AMJE Sudajii/Beech harvest values are wood-related. Do not reinterpret them as edible nuts or replace wood-cut yields without checking the actual loaded semantics.
- **Approved exception to default minimal dependencies:** Foraging requires AMJ Environment (`sucro.ancientmedievaljapan.environment`) as its one-way upstream native-vegetation provider. No additional Mod dependency or species, yield, harvest cycle, season, recipe, JobDef, texture, or Foraging packageId is approved by repository creation.
- Prefer native RimWorld harvest designations/jobs. New C# behavior must be justified by actual engine limitations, and preservation of cut/wood flow must be tested.
- Follow [Project GitHub main protection](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/blob/main/Docs/GitHubMainProtection.md): topic branch → PR → exact head and triggered CI checks → merge → main SHA verification. Never write directly to main. Do not enable Codex PR review.
- Release claims require actual code, assets, loaded-game and old-save/profile evidence. Steam publication is author-managed. This repository contains no playable Mod yet.
