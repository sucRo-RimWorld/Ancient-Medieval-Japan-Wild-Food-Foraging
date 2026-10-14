# Shii direct gathering and genuine pawn Job — native 1.6 evidence (2026-10-14)

**Scope:** isolated, disposable developer-only test fixture attached to an original Environment-owned `AMJ_Tree_Shii`. None of these results promote a playable food Def, approved nut quantity, season, work/cooldown balance, image, UI or Workshop release.

## Source and actual runtime

| Run | Scenario, source | Native result | What it proves |
| --- | --- | --- | --- |
| [38513500827](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38513500827) | first direct test, WFF `40fab5780a55417f6d606f4b704098815bb72287` | **FAIL before game**: C# CS1061 on private `DesignationManager.allDesignations` | The source WorkGiver did **not compile** in actual 1.6; no native game test |
| [38514414357](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38514414357) | direct component, WFF `d47692357ddd58e31a1fd01ab74808309775fdb3` | **FAIL overall**: 9/9 behavioral assertions but **one Unity prelaunch ERROR**, `DesignationDef.ResolveReferences` when temporary texture path null | Real Comp behavior occurred; strict ERROR-zero was **not** satisfied |
| [38514810836](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38514810836) | `WffShiiGatherQuickstart`, WFF `0764d632304f629464c1c36d3e6da21492b7ee84` | **PASS**: 9/9 assertions, 0 prelaunch/Unity/log errors, game exit 0 | Actual generated Warm map, natural standing Shii, component-attached tree, direct single diagnostic collection, retained identity and original wood-primary `harvestedThingDef` (`harvestYield=42`), immediate cooldown |
| [38515385161](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38515385161) | `WffShiiPawnJobQuickstart`, WFF `20c1abd408535b63df54e2ce545b6db32cefcfa9` | **PASS**: 8/8 assertions, 0 prelaunch/Unity/log errors, game exit 0, 1800 real native ticks | `WorkGiver` enumerates a designated naturally generated Shii, real colonist starts `JobDriver`, job creates exactly one diagnostic product, original tree/timber survive, successful job removes one-shot designation, immediate second job blocked |

Both successful runs launched real isolated **RimWorldLinux 1.6.4871 rev600** with mandatory Environment and Xvfb using the Project private host. The source SHA is pinned at runtime; the fixture was removed afterwards in each result (`fixture_removed=true`). The actual job run compiled the three original WFF mechanic classes plus a separate test Quickstart using **real 1.6 Managed references**, zero compiler errors and warnings; native source artifact fingerprints are in the linked run results.

| [38516174487](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38516174487) | `WffShiiCancelQuickstart`, WFF `b2e9cbb29058d92acd350506c93cb933f4307903` | **PASS**: 8/8 assertions, 0 prelaunch/Unity/log errors, game exit 0, 10 real ticks | Real pawn Job remains active before cancellation; `StopAll` generates no synthetic output/cooldown, retains original gathering designation and timber |
| [38516178483](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38516178483) | `WffShiiRaceQuickstart`, same WFF source | **PASS**: 9/9 assertions, 0 prelaunch/Unity/log errors, game exit 0, 1200 real ticks | Two genuine colonists can independently reach the target before reservation; after first pawn starts and reserves, second WorkGiver rejects it; one diagnostic output, no duplicate; tree, timber and cooldown intact |

## Actual test premises — diagnostic, not adopted balance

- **Target:** Existing naturally spawned Environment `AMJ_Tree_Shii` in an actual `AMJ_WarmTemperateForest` world tile. The test may set `Growth=1` on its selected tree **only on the disposable map**, so it does **not** prove natural harvest season or naturally reached maturity.
- **Product:** `Steel` x1 synthetic countable marker, **not edible nuts**; `workTicks=60`, `cooldownTicks=1800` are disposable test values **only**. No production ThingDef or balance was approved.
- **Wood/forestry:** The Environment tree identity, original `harvestedThingDef` and `harvestYield=42` must remain unchanged. The actual job must keep the tree alive and not substitute the original main wood harvest.
- **Job:** A real spawned player pawn receives the test `JobDef` through the original `WorkGiver`, then the game advances 1800 native ticks. Exactly one marker, no outstanding gather designation and no immediate second valid Job are required.
- **Verification:** Each run requires all assertions and `preLaunchErrors=0`, `logErrors=0`, genuine Unity `error_count=0`, exit 0 and no timeout. Merely satisfying the per-assertion total is insufficient.

## Cooldown-boundary next native run — blocked before game (2026-10-14)

WFF [PR #52](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Wild-Food-Foraging/pull/52), main `b473bf151944464d49c308026950823dcca86324`, adds test-only `WffShiiCooldownQuickstart`: direct 1.6 gathering with synthetic Steel x1, immediate repeat rejection, native advance through synthetic 1,800 ticks and a single new gather followed by renewed cooldown. Project [PR #143](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/pull/143), main `6601e00f106ab79fed8a76fdc145f2bfc9e50612`, added the **source-pinned** `/amj wff-shii-cooldown-16` isolated Xvfb route; both relevant Project PR checks and WFF source CI passed. This new case is **not a save/reload or exact 1,799/1,800 frame-edge test** and does not approve balance.

[Project Actions run 38517643035](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38517643035) **attempt 1 and attempt 2 FAILED at `Set up job`, before checkout/C# build/game**. Both logged DNS resolution failures for `codeload.github.com:443` when downloading `actions/checkout@v5` (3 attempted downloads each). No Quickstart report, RimWorld runtime or loaded map was produced. **Native cooldown-boundary result: NOT RUN; do not claim PASS or a gameplay regression.** Host/network prerequisites belong to Project's persistent-runner workstream; WFF resumes the already-pinned scenario when the host can download actions. Preserve earlier successful Shii direct/Job/race/cancel evidence independently.

## Explicitly remaining acceptance gaps

- **F01–F04:** Production install metadata, real edible nut output, cut-tree timber count and `PlantCollected` interception instrumentation, MO WoodChain and cut-after-gather paths remain untested. The test-only native Job route provides **partial F04 mechanism** evidence, not full F04.
- **F05:** Only immediate cooldown suppression; persistence, boundary crossing, save/reload and re-add still **NOT RUN**.
- **F06:** One-pawn WorkGiver/Job and designation completion observed. Two-pawn WorkGiver reservation contention and early Job cancellation **PASS in separate isolated diagnostic scenarios**; simultaneous forced second Job, late cancellation, drafted/forbidden/unreachable transitions and failed placement remain **NOT RUN**.
- **F07–F14:** Target loss during work, species eligibility beyond this fixture, genuine pre-Foraging older save, natural full lifecycle, season, 4 profiles, 1.7, UI/approved artwork and distribution evidence remain **NOT RUN**.
- **AC01–AC22:** no complete resource acceptance has been promoted. The earlier Warm/Cool 40/40 sowing-command checks are separate diagnostic evidence.

**Next tests:** save/reload and boundary retention of per-tree cooldown, old-AMJE pre-Foraging save upgrade, post-work-mid-tick cancellation and target-loss/drafted/unreachable states; then MO/CCTO profiles. Do not use the previously incompatible unrelated DLC `test-colony` save, and do not run tests in the player's active game.
