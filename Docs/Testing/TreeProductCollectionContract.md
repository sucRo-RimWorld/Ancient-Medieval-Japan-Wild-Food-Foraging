# Tree-product collection — proposed acceptance contract

**State:** MECHANISM SOURCE STAGED BUT INACTIVE; no compiled assembly, gameplay resource, linked Defs, accepted numeric specification or completed runtime checks. The narrowly scoped [mechanism entry decision](../Research/ShiiMechanismDecision.json) and [original C# sources](../../Source/AMJWildFoodForaging/README.md) exist. This describes **what would be tested** before making a claim; none of these cases has been executed in RimWorld. Tree collection, species, yields, seasons and Defs remain unapproved until the entry decision and owning Design specify them.

**Dependencies:** every game profile must have Environment loaded first, not just an optional fallback. Foraging must not duplicate Environment PlantDefs. MO and CCTO are optional and independent profile axes. Use the actual source/provider fingerprints, full Player.log and ERROR-zero gates.

| ID | Test item | Intended specification premise and required observation |
| --- | --- | --- |
| F01 | Fresh 1.6 Def load, Environment+Vanilla | Only existing Environment Shii/Beech identities are present; no patched primary wood `harvestedThingDef`, `harvestYield` or map distribution. Missing required Environment is a dependency failure, not a supported gameplay profile. |
| F02 | Ordinary tree felling (no gather order) | Original tree destruction, wood yield and stump/wood state match Environment-only baseline. No foraged food is created by cutting or by a building-clearance cut. |
| F03 | Optional MO wood-chain tree felling and fruit-tree baseline | On unchanged **Environment Shii/Beech**, assert their original native timber resource/count (WoodLog/RawWood as appropriate) with WoodChain **on and off**; a test run must record the actual loaded harvestedThingDef rather than assume inherited Defs were patched. Separately, cut mature **MO apple/lemon/mulberry** trees in isolated MO baseline: source predicts fruit-primary output (plus possible mulberry silkworm) **without** a dedicated wood drop. Verify actual drops and destruction, do not infer a timber-preserving solution from fruit harvest. |
| F04 | Independent gather on mature tree | When eligible, a pawn targets the **existing Environment tree instance** via a separate gathering order/Job, obtains only the approved food resource, and leaves that tree alive with unchanged identity/growth/primary wood harvest. Instrument or otherwise observe **zero `PlantCollected`, `Destroy`, wood and duplicate food** events on gather. No bill-only abstract-gather-building substitute. |
| F05 | Repeat gather / finite yield | Repeat immediately after completing gathering must yield **zero** until a formally approved renewal condition. Save/reload immediately before/after gather and at the cooldown boundary; the saved per-tree state must prevent duplicate food and survive the boundary. No provisional cooldown/quantity/season number is assumed. |
| F06 | Job validity and races | Validate target-instance eligibility, WorkGiver reservation/path access, drafted/unreachable or forbidden trees, pending cut-vs-gather orders, two pawns competing for one tree, interruption and incapacitation. Exactly one successful gather may advance the tree's per-instance cooldown; cancellation must not duplicate food, leave orphaned designations or interfere with timber. |
| F07 | Tree lost mid-job | Fire, frost, tree death, removal, or concurrent cutting during gathering produces no ghost yield; tree disappearance clears its pending work safely. |
| F08 | Noneligible species | The new action is absent for Shirasbiso, Haimatsu, Vanilla Oak and non-approved species. Buna and Kuri must not become gathering producers merely by implementing Shii's mechanism. |
| F09 | Existing-AMJE save add/reload/remove/re-add | Add Foraging to a **real** pre-Foraging AMJE colony with existing naturally generated Shii; check newly attached gather-state component, first gather and save/reload; remove and re-add only under a safe verified scenario. Tree identity, wood, loaded cooldown and valid jobs must remain intact; no missing-comp/def reference or ERROR. Forced-spawn/new-game-only checks do not meet this case. |
| F10 | Natural species addition (separate Environment scope) | If Environment later approves a new wild fruit tree, prove natural appearance in a preexisting map after relevant upgrade, real mature harvest and research-gated sow where applicable. A fixed artificial spawn is not this test. |
| F11 | Seasonal behavior | Only when the owning Design approves a season/temperature rule: prove eligible and ineligible periods, late season, cold/dormancy and transition/save reload; no numerical value is presumed by the test list. |
| F12 | Four-profile integration | Environment+Vanilla, Environment+CCTO, Environment+MO, Environment+MO+CCTO: each required functional case runs through the actual loaded game and completes with **zero runtime ERROR**. Do not infer a pass from static audits or a single profile. |
| F13 | 1.7 future test-only coverage | When 1.7 is supported/testable, rerun version-appropriate Def/job/save checks without changing the shared 1.6 production behavior; do not claim 1.7 production support before its release gate. |
| F14 | UI/art and release boundaries | Separately confirm visible button/designation, Japanese/English strings, accepted food art, and exact private Workshop payload; these do not follow from a code/static PASS. |

## Deliberately excluded from the initial tree mechanism

No new hunting, fungal species, chestnuts, wild yams, yam recipes, preservation, coastal gathering or world-map excursions. No fake gathering structure or global wood-yield rebalance. No speculative quantity, regrowth, season or duration assertions until approved.

## Evidence and status

- [Source comparison](../Research/TreeNutMechanismAudit-20261014.md) identifies the concrete shared-harvest obstacle.
- [Owner Design](../Design.md) remains the only content specification authority.
- Current result: **NOT RUN** for all F01–F14; static comparison is source observation, **not** a PASS for any F-case.
- This plan is promoted to actual Pickle/RimTest/native tests **only** when production implementation receives a validated Project-format existing-Mod entry decision. Remove or narrow obsolete scenarios if the accepted mechanism uses existing native behavior instead of custom jobs.

## Mechanism source coverage (not RimWorld)

`Tests/Production/test_shii_source_contract.py` checks exact source invariants: original wood-primary harvest unchanged; no native `PlantCollected`; saved per-tree state; disabled output without food/cooldown/work configuration; target reservation and native cut-order exclusion. These are static assertions only. A passing result does **not** clear any F01–F14 native scenario, build, save migration, CCTO/MO compatibility, UI or release approval.
