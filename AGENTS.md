# AMJ Wild Food Foraging — Agent Instructions

## Start here
1. Read this file and **main:Docs/Coordination.md** at the start of this repository's work.
2. Read Project [AGENTS.md](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/blob/main/AGENTS.md) and [SharedRules](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/blob/main/Docs/SharedRules.md), including the latest Project main Coordination only when cross-repository status matters.
3. Read the local [Design](Docs/Design.md), then the relevant [Existing-Mod Audit](Docs/Research/ExistingModAudit.md) and actual source before implementation.

## Ownership — superseded by author's 2026-10-17 consolidation

**This repository is a historical R&D and diagnostic-test archive, not an independent active gameplay Mod owner.** The author decided to integrate all twelve adopted wild food resources, related food/PlantDefs, original art, biome placement, sowing and gathering rules into **[AMJ Environment](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Environment/blob/main/Docs/ForagingIntegrationDesign-ja.md)** as a future Environment update. Environment `Docs/Design.md` and `Docs/ForagingIntegrationDesign-ja.md` are the sole current implementation specifications. Previously approved content/safety/ecology/evidence is retained, not undone.

Do **not** start new standalone Foraging production, create an independent packageId, release it, or ship old synthetic test fixtures or inert Shii C# as gameplay. Do **not** create a reverse or required Environment→WFF dependency. Existing historical files below document the previous split and their ownership assertions are **superseded**; preserve source links, experiments and evidence. Inter-owner updates go via main `Docs/Coordination.md` and canonical Environment docs, not the user.

## Resource selection is independent of other Mods
- **Author-confirmed 2026-10-14:** Foraging's objective is the historically appropriate collection of wild foods in ancient-to-medieval Japan. Individual edible species are selected by Japan-period use, natural habitat, edible part, safety and fit with the forest/foothill gathering theme. **Another Mod containing the same tree, nut, green or mushroom never excludes, defers or downgrades its adoption in Foraging.** There is no project-design requirement to delegate its food list to generic fruit/mushroom Mods.
- VE/non-VE/MO source comparisons remain relevant to **technical implementation reuse** under Project's implementation-entry rules. Optional co-install compatibility (duplicate resources/spawns, XML conflicts) is a **later optional compatibility test**, not a precondition for accepting species or implementing the independently-owned core Foraging content. Never silently convert a missing third-party XML checkout into an adoption HOLD.

## Entry and evidence
- **Separate WFF production is RETIRED.** VE/non-VE/MO/Vanilla comparisons and actual loaded source gates apply under Environment's future integrated implementation. This repo's research records may be consulted, not used as a new independent implementation route.
- Existing AMJE Sudajii/Beech harvest values are wood-related. Do not reinterpret them as edible nuts or replace wood-cut yields without checking the actual loaded semantics.
- **Dependency decision superseded:** Environment includes approved wild-food scope itself when implemented; neither WFF packageId nor Environment→WFF dependencies are authorized. No species/season/yield/recipe/art numerical content gains production approval from this ownership transfer.
- Prefer native RimWorld harvest designations/jobs. New C# behavior must be justified by actual engine limitations, and preservation of cut/wood flow must be tested.
- Follow [Project GitHub main protection](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/blob/main/Docs/GitHubMainProtection.md): topic branch → PR → exact head and triggered CI checks → merge → main SHA verification. Never write directly to main. Do not enable Codex PR review.
- Release claims require actual code, assets, loaded-game and old-save/profile evidence. Steam publication is author-managed. This repository contains no playable Mod yet.
