# AMJ Wild Food Foraging — Coordination (main only)

This is **handoff/status only**, not the design source. At start of work, read `AGENTS.md`, this file on **main**, and the relevant formal owner documents. Do not create branch-specific competing Coordination records or use the user as a messenger.

## WFF-OWNER-ENTRY-20261010 — Dedicated repository established

**Owner:** `sucRo-RimWorld/Ancient-Medieval-Japan-Wild-Food-Foraging`  
**Status:** OWNER DESIGN ESTABLISHED; ENVIRONMENT REQUIRED; PRIOR-ART ENTRY BLOCKED; NO RUNTIME IMPLEMENTATION

The author supplied this dedicated GitHub repository after Project recorded the confirmed module name, no-new-hunting scope and execution order. Project's previous research/ideas remain historical pointers once owner design is merged. Canonical feature design: `Docs/Design.md`; current source comparison and unresolved VE/non-VE checks: `Docs/Research/ExistingModAudit.md`.

**First source-audit outcome (2026-10-10):** Public RimWorld 1.6.9438.38202 Plant/Job decomp, supplied MO 1.6 XML, and actual Vanilla Expanded Framework DualCrop/Harmony source inspected; details and commit-pinned source links are in `Docs/Research/ExistingModAudit.md`. Native `harvestAfterGrowth>0` retains trees, but the primary harvest item is shared with cutting; a vanilla dual-output/VEF-style extension alone is **not proven to separate nut picking from timber cutting**. **Scope clarification:** Sudajii is the first technical test plant, **not the sole intended tree-nut species**. Existing Environment Buna nuts and the B+-rated (not-yet-added) Kuri remain separate candidates; an existing forest Oak is not an accepted acorn-harvest Def. **Next:** compare a species-reusable independent tree-product collection mechanism, inspect loaded 1.6/MO timber behavior, and complete VE **and non-VE** species/function audit. Environment owns the existing tree/distribution and remains required. No balance/art/Job/new production Def approved. Gate stays HOLD.

**Cross-owner handoff (2026-10-10 author decision):** Wild Food Foraging **requires** AMJ Environment (`sucro.ancientmedievaljapan.environment`) and its Japan-native vegetation, including existing `AMJ_Tree_Shii`. Environment does not require Foraging and continues to own the actual PlantDefs, natural distribution and old-AMJE-save plant additions. Foraging owns edible resource harvesting and consumption. Foraging's future `About/About.xml` must record the required packageId and load order; no playable metadata has been created. No standalone fallback trees or duplicated natural plant Defs. Additional species/food products, harvest values, art, VE/non-VE necessity, native Jobs, runtime and save E2E remain open; Preservation and Coastal Gathering are distinct.
