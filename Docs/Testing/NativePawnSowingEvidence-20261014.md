# Native RimWorld 1.6 Fuki/Udo and nut-tree pawn-sowing diagnostics — 2026-10-14

**Scope:** disposable developer-only planting fixture, **not the playable Wild Food Foraging Mod**. The two forage plants, artwork, growth (three fixture days) and any artificial biome placement remain **synthetic test inputs**. No historical processing/food, natural maturation, harvest, old-save or production resource is approved by this run.

## Exact native evidence

- **Owner source:** WFF main `0526abf83ea37fb44839ac5c2485bc04bdc4a5c3` ([PR #55](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Wild-Food-Foraging/pull/55)). The exact test is `Tests/NativeSowingProbe/WffNativePawnSowingQuickstart.cs` with temporary fixture built from `Tests/NativeSowingProbe/build_probe.py`.
- **Private host source:** Project main `09090f2e2987ce9d9687a6bcfa340402146816ae` ([PR #152](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/pull/152)); pinned command `/amj wff-pawn-sow-16`. The host compiles the test against actual installed RimWorld 1.6 Managed references and uses a dedicated Linux game, isolated ModsConfig and Xvfb.
- **Actual execution:** [Project Actions run #38520507440](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38520507440), artifact [#11874270521](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38520507440/artifacts/11874270521). `WffNativePawnSowingQuickstart` **6/6 assertions PASS**; RimWorld **1.6.4871 rev600**, exit 0, timeout false, `preLaunchErrors=0`, `logErrors=0`, native Unity `error_count=0`. `fixture_removed=true` after the isolated run. Host result explicitly records `actual_pawn_sow=PASS_TEST_FIXTURE`, `crop_growth=NOT_RUN`, `saved_game_compat=NOT_RUN`.

## Verified premises and test results

| Assertion | Test premise / required observation | Actual |
| --- | --- | --- |
| Native warm biome | Existing Environment `AMJ_WarmTemperateForest` must be the genuinely selected map tile, not forced onto another region | PASS |
| Two native sow jobs prepared | Two player colonists have independent reachable/fertile grow-zone cells, one assigned each to `AMJWF_Probe_Fuki` and `AMJWF_Probe_Udo` | PASS |
| Native sow Job started | Each pawn receives the real game's `WorkGiver_GrowerSow.JobOnCell` → `JobDefOf.Sow`, not an artificial plant spawn | PASS |
| Fuki actually planted | A spawned, surviving `Plant` of the exact fixture Fuki `ThingDef` occupies its assigned cell after native ticks | PASS |
| Udo actually planted | A spawned, surviving `Plant` of the exact fixture Udo `ThingDef` occupies its assigned cell after native ticks | PASS |
| Zone ownership and crop selection | Both cells remain owned by their correct independent `Zone_Growing` with the intended `PlantDefToGrow` | PASS |

This **closes only the diagnostic actual-pawn-sowing subcheck of AC20 for synthetic Fuki/Udo**. AC20 as a whole is **not accepted**: no naturally generated approved Foraging plants, natural harvest/recovery, safe processing, complete growth cycle, mature crop or real old save. All other AC18–AC22 full acceptance, four-profile compatibility and release gates remain OPEN. Other earlier Warm/Cool **40/40** menu results remain valid for their distinct test source and scope.


## Three nut-tree actual Pawn Sow jobs — AC19 diagnostic PARTIAL FAIL (2026-10-14)

This is a **different native scenario** from the Fuki/Udo 6/6 PASS above. All runs use a disposable Warm-biome fixture with Vanilla placeholder graphics, artificially staged **0.2 Warm wild-plant commonality** for test trees, and **30-day synthetic growth durations**. A fresh game's Vanilla `TreeSowing` is initially unfinished then unlocked; each of three genuine colonists is *only in the disposable test colony* raised to the native inherited Plants skill threshold **6** if below it. Real `WorkGiver_GrowerSow.JobOnCell` → `JobDefOf.Sow` pawn Jobs are started. The game advances **9,000 native ticks** and asserts actual correct Plant instances in three distinct `Zone_Growing` cells; an unrelated planted species at the test cell **does not count as success**. No direct `GenSpawn.Spawn`, Def mutation or production content is used.

| Native run | Exact WFF source SHA | Result and observed limit |
| --- | --- | --- |
| [38521798715](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38521798715) | `7952b296a083954bfbc0019bd115201016aace2e` | **2/7 PASS**, 5 failures: Warm biome and TreeSowing unlock PASS; tree sow jobs and planting not proved. Pre-qualified three native growers were not guaranteed. |
| [38531471236](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38531471236), [artifact #11878681297](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38531471236/artifacts/11878681297) | `72e66148741ba43c88e5b8ae98034ee19daf80ad` | **7/8 PASS**, only Walnut missing. All 3 growers qualified, Jobs started, Kuri and Kaya actually planted, zone choices retained. |
| [38532344993](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38532344993), [artifact #11878920237](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38532344993/artifacts/11878920237) | `96ecadff6c2ae5190b2977a9757e9e1fcfc89625` | **7/8 PASS**, only Walnut missing. Diagnostic log: Walnut Pawn originally at **(77,0,70)**; Walnut test cell also **(77,0,70)**; at verification that cell contained **`AMJWF_Probe_Kuri`**, while its zone still selected Walnut. |
| [38533291055](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38533291055), [artifact #11879280915](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38533291055/artifacts/11879280915) | `6bb561d6451e2e186b1b044d22270774e3b4c4c8` | **7/8 PASS**, only Walnut missing **despite excluding every pawn-occupied test cell**. The diagnostic identifies `AMJWF_Probe_Walnut` assigned to the grow zone at **(77,0,69)**, but **`AMJWF_Probe_Kuri` occupies that exact cell** at the end. Pawn has left native sow Job and is wandering. |

The last three runs compiled successfully, were **RimWorldLinux 1.6.4871 rev600**, `preLaunchErrors=0`, `logErrors=0`, Unity `error_count=0`, `fixture_removed=true`; they are **overall FAIL**, not PASS (Quickstart one assertion failed, `game_exit=1`). The 0 Unity ERROR count does not override failed assertions.

### Precisely established versus still uncertain

- **Observed PASS within this fixture:** one genuine Warm Environment map; initial research unfinished→finished; all three native sow-skill qualifications; all three assigned native Sow Jobs start; Kuri and Kaya plants exist in their correct growing zones; all three zone crop choices persist.
- **Observed FAIL:** after 9,000 ticks the Walnut zone contains a *different species* (synthetic Kuri), not a Walnut plant; the corresponding Pawn's current Job at verification is idle/wander. Changing the target to an unoccupied cell **did not fix it**.
- **Not established:** whether test-fixture wild seed spawning, another grower, or Job's species resolution caused Kuri to occupy Walnut's target. The **synthetic 0.2 wildPlants population weight** is a plausible confounder but **not yet a proven cause**. Before modifying further fixtures or treating native sow behavior as defective, examine actual Job plant-def selection and/or isolate natural spawn from pawn planting with an independent observable mechanism. **Do not simply lower assertions, choose a lucky seed or dilute the test to infer success.**
- **Production still OPEN:** AC19 as a whole, natural approved habitat/spawn, exact wood/food content, growth to maturity, native harvest, long saves, MO/CCTO compatibility and real gameplay balancing. The adopted trees remain design-approved; the failed synthetic diagnostic does not rescind adoption.

