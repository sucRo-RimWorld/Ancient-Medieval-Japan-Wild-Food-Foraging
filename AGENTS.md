# AMJ Wild Food Foraging — Agent Instructions

## Start here
1. Read this file and **main:Docs/Coordination.md** at the start of this repository's work.
2. Read Project [AGENTS.md](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/blob/main/AGENTS.md) and [SharedRules](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/blob/main/Docs/SharedRules.md), including the latest Project main Coordination only when cross-repository status matters.
3. Read the local [Design](Docs/Design.md), then the relevant [Existing-Mod Audit](Docs/Research/ExistingModAudit.md) and actual source before implementation.

## Ownership
This repository owns **wild edible plant gathering and food-use behavior** for Ancient & Medieval Japan - Wild Food Foraging / 中世日本 - 山野採集. It does not own Environment's natural tree/vegetation identity, biome/world distribution, general agriculture, food preservation, hunting, fishing or coastal gathering.

The confirmed design is in Docs/Design.md; Project retains only shared architecture and ownership pointers. Cross-repository handoff/status belongs in main:Docs/Coordination.md of the actual owner, never via the user.

## Entry and evidence
- **Production implementation is HOLD** until VE **and non-VE**, Vanilla, MO and AMJ-owner actual-source comparison establishes an unresolved need and a validated Project-format implementation decision. Prior-art research and disposable tests may continue.
- Existing AMJE Sudajii/Beech harvest values are wood-related. Do not reinterpret them as edible nuts or replace wood-cut yields without checking the actual loaded semantics.
- No species, yields, harvest cycles, season numbers, research unlocks, ingredients, recipes, JobDefs, textures, packageId or dependencies are accepted solely by repository creation.
- Prefer native RimWorld harvest designations/jobs. New C# behavior must be justified by actual engine limitations, and preservation of cut/wood flow must be tested.
- Follow [Project GitHub main protection](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/blob/main/Docs/GitHubMainProtection.md): topic branch → PR → exact head and triggered CI checks → merge → main SHA verification. Never write directly to main. Do not enable Codex PR review.
- Release claims require actual code, assets, loaded-game and old-save/profile evidence. Steam publication is author-managed. This repository contains no playable Mod yet.
