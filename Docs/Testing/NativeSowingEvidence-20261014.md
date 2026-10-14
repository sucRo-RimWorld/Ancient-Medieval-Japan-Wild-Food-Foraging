# Wild Food Foraging — real Linux RimWorld 1.6 sowing Def evidence (2026-10-14)

**Evidence class:** Actual native RimWorld+Pickle **loaded-Def and resolved-field verification**, **NOT** gameplay planting, sow menu, research unlock in a map, pawn work, regional planting eligibility, edible harvest, visual quality, season, old-save or release evidence.

## Exact sources and native execution

| Source | Verified identity |
| --- | --- |
| Foraging test input | **`339771f769c94ce50dd5739cab1675f206ab19bb`** (11 test-only concrete plants + 14 native abstract sowing parents), **not a shipped Mod**, and `sucro.amj.foraging.dev.sowingprobe` isolated test package |
| Runner | Existing Project `amj-rimworld` self-hosted WSL Linux, **dedicated licensed 1.6 game**; no normal player Mod installation |
| Game detected | `RimWorld 1.6.4871 rev600`, actual Unity/Pickle process exiting 0 |
| First host source | Project `a5f4ac651ca0d90950fa8c44833570663072437e` — source-pinned WFF operation added in [Project PR #104](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/pull/104) |
| Native loaded-Def run | [Actions #38503071680](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38503071680), [artifact #11866810697](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38503071680/artifacts/11866810697). One Pickle scenario, **passed 1/1, failed 0, skipped 0, flaky 0**, exit 0, Unity classified **error_count=0**; `fixture_removed=true`. |
| Second host source | Project `6408ece8ab8d6e8d5f039de3aa715ca81a29962b` — adds resolved loaded-field assertions in [Project PR #107](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/pull/107) |
| Native loaded-field run | [Actions #38503884622, attempt 2](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38503884622), [artifact #11867775514](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38503884622/artifacts/11867775514). One Pickle scenario, **passed 1/1, failed 0, skipped 0, flaky 0**, exit 0, Unity classified **error_count=0**, `fixture_removed=true`. Game elapsed 21 s. |

Second run attempt 1 was a **runner setup/infrastructure failure** before checkout or game start: `actions/checkout@v5` download could not resolve `codeload.github.com` after three attempts. [Run #38503884622 attempt 1](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38503884622/attempts/1) is **not a game assertion failure and not PASS**. A single rerun of the failed job (same run ID, attempt 2) reached real RimWorld and completed successfully. GitHub logs and artifact are the immutable evidence; do not relabel all attempts as successful.

## Exact native Pickle assertions that passed

**Run 1: 13 Def anchors, one scenario.**
- Required package IDs `sucro.ancientmedievaljapan.environment` and `sucro.amj.foraging.dev.sowingprobe` actually loaded.
- `AMJ_WarmTemperateForest` of type `BiomeDef` and **existing** `AMJ_Tree_Shii` of type `ThingDef` exist.
- The eleven Foraging probe ThingDefs exist: `AMJWF_Probe_Kuri`, `Walnut`, `Kaya`, `Warabi`, `Zenmai`, `Fuki`, `Udo`, `Jinenjo`, `Hiratake`, `Matsutake`, `Shiitake`. No claim that an actual living individual spawned.

**Run 2: same Def anchors, plus 16 loaded-field assertions, one scenario.**

| Defs | Loaded property | Expected and observed |
| --- | --- | --- |
| Each of Kuri, Walnut and Kaya (3 trees × 4 tests) | `plant.mustBeWildToSow` | `True` — native region-gated tree-setting flag, **not proof of actual region menu filtering** |
| Each of the same 3 | `plant.sowResearchPrerequisites.Count` | `1` — native researched-tree gate, **not proof the menu unlock was exercised** |
| Each of the same 3 | `plant.sowTags.Count` | `1` — one sowing tag; the parent XML names it `Ground` |
| Each of the same 3 | `plant.harvestedThingDef.defName` | `WoodLog` — timber remains the tree's **primary** harvest output; independent edible nut gathering still not implemented |
| Fuki, Udo (2) | `plant.sowTags.Count` | `1` — native sowing tag inherited; no real pawn work yet |
| Existing Environment Shii (2) | `plant.harvestedThingDef.defName`, `plant.harvestYield` | `WoodLog`, `42` — tree's native cut material/yield preserved |

All sixteen field assertions were evaluated by real RimWorld 1.6 using the v6.6.3 Pickle Vanilla DefSteps `Then def "..." field "..." is "..."` mechanism. Pickle checks fields via reflection after the loaded Defs have been resolved, not by text grepping XML.

## AC contract impact — precise bounded status

| AC | Component actually covered by native evidence | Remaining unexecuted requirements |
| --- | --- | --- |
| **AC18** — Shii | Native preserved `WoodLog` and 42; Shii Def exists | **NOT RUN**: researched Warm-only growing menu, pawn planting, nut gather, saved cooldown, actual cutting/optional MO WoodChain, old saves |
| **AC19** — Kuri/Walnut/Kaya | Native fields: regional restriction flag, 1 research prerequisite, 1 sow tag, primary `WoodLog` | **NOT RUN**: research off/on UI behavior, Warm/Cool effective menu eligibility, actual planted specimens, wood cut, nuts, growth, saves |
| **AC20** — mountain greens | Native `sowTags.Count=1` for Fuki and Udo | **NOT RUN**: grow-zone menu, successful pawn sow and growth, wild-only Warabi/Zenmai menu exclusion, safe shoot harvest/processing |
| **AC21** — wild roots and fungi | Fixture ThingDefs load | **NOT RUN**: negative grow-zone sow tests, actual dig/fungal fruiting, host/substrate constraints |
| **AC22** — regional/compat/save | Source identity/Environment input and isolated Linux loaded package proven | **NOT RUN**: region actual gate, native jobs, saved-game compatibility and MO/CCTO optional profiles |

**All five AC18–AC22 remain NOT FULLY RUN / no case promoted to PASS.** AC01–AC17 and Shii F01–F14 likewise NOT RUN. The two actual native *subtests* above are PASS, not an acceptance of all planting gameplay.

## Artifact and release boundary

- Fixture's `growDays=3/30`, Warm `wildPlants` test weight `0.2`, and Vanilla placeholder artwork are synthetic diagnostics, **not approved species balance, historical appearance or final vegetation distribution**.
- No real nut ThingDefs, food processing, secondary nut gather Job hookup or edible mushroom substrate/host eligibility is implemented by the testing probe. The original Environment Shii remains the sole production source for Sudajii tree.
- Original source and branch/main hashes were verified. The dedicated WSL game was used in isolated session/save dirs; `fixture_removed=true`, with no regular player ModsConfig or Workshop publication.
- Further source development can target the **real grow-zone/research and sow/pawn/save behavior tests**. Report their own native artifacts, never infer from the loaded-property PASS.
