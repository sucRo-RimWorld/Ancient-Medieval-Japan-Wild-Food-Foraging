# Shii native 1.6 diagnostic (nonshipping)

Build `build_probe.py` under a **new system/runner temporary directory**, never a player's installed Mods. The producer emits a temporary Steel marker with 60-tick work and 1800-tick cooldown purely to test code behavior. These are not approved edible items or balance.

The Project private host compiles the three source mechanism classes in `Source/AMJWildFoodForaging/` together with a specific Quickstart against its **actual installed 1.6 Managed assemblies**, then activates only the temporary fixture with Environment. The host runs headlessly with Xvfb, records provenance/complete logs and a Quickstarts report, rejects any Unity ERROR/prelaunch ERROR, and removes the temporary fixture.

- `WffShiiGatherQuickstart`: **direct component** test of the existing naturally spawned tree; no Job; validates synthetic marker, unchanged wood primary, tree survival, cooldown.
- `WffShiiPawnJobQuickstart`: actual **WorkGiver → JobDef → JobDriver** on a designated natural Shii. Starts an actual colonist Job at `PostLoaded`, runs 1800 native game ticks before `Verify`, and requires one marker, tree survival, removal of the one-shot designation and post-harvest cooldown. This is not a mocked call to `CompShiiGatherState.TryGather`.

- `WffShiiRaceQuickstart`: two real spawned colonists must be able to reach the same designated tree before either Job starts. Once the first reserves/starts gathering, the second WorkGiver must refuse that target; after 1200 native ticks there must be only one marker and the first tree must survive. This validates reservation at WorkGiver level, not an actual second forced Job run.
- `WffShiiCancelQuickstart`: an actual pawn starts the gathering Job, then after only 10 native ticks (less than the 60-tick diagnostic work interval) the test cancels it. The tree/designation must remain, and no item/cooldown may be produced. It does **not** prove all mid-toil cancellation timings.

- `WffShiiCooldownQuickstart`: direct-component diagnostic on a naturally generated Shii. Executes one synthetic collection, denies an immediate repeat, advances 1800 native game ticks and checks that exactly one further collection is then possible and starts a new cooldown. **Does not save or reload**, prove an exact frame edge, validate natural season or approve a gameplay cooldown value. A native run is required; source availability is not PASS.

No diagnostic scenario establishes a realistic nuts product, food safety, old AMJE save addition/removal, persistence after save/reload, MO WoodChain, CCTO, cancellation races, natural regeneration, or all F01–F14. The formal [tree acceptance contract](../../Docs/Testing/TreeProductCollectionContract.md) remains open except for individually supported portions. Never silently relabel a sub-check as full F04/F05/F06 acceptance.
