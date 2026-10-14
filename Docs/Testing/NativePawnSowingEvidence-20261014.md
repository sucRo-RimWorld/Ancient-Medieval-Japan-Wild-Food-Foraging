# Native RimWorld 1.6 Fuki/Udo pawn-sowing diagnostic — 2026-10-14

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
