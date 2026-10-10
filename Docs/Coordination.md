# AMJ Wild Food Foraging — Coordination (main only)

This is **handoff/status only**, not the design source. At start of work, read `AGENTS.md`, this file on **main**, and the relevant formal owner documents. Do not create branch-specific competing Coordination records or use the user as a messenger.

## WFF-OWNER-ENTRY-20261010 — Dedicated repository established

**Owner:** `sucRo-RimWorld/Ancient-Medieval-Japan-Wild-Food-Foraging`  
**Status:** OWNER DESIGN ESTABLISHED; ENVIRONMENT REQUIRED; PRIOR-ART ENTRY BLOCKED; NO RUNTIME IMPLEMENTATION

The author supplied this dedicated GitHub repository after Project recorded the confirmed module name, no-new-hunting scope and execution order. Project's previous research/ideas remain historical pointers once owner design is merged. Canonical feature design: `Docs/Design.md`; current source comparison and unresolved VE/non-VE checks: `Docs/Research/ExistingModAudit.md`.

**Current first investigative task:** inspect RimWorld 1.6 native harvest/cut mechanics and MO 1.6 apple/lemon/mulberry fruit-tree semantics, then determine whether `AMJ_Tree_Shii` can yield edible nuts repeatedly without changing wood cutting. Environment retains the underlying tree and biome distribution. No fruit/item quantities, artwork or code approved. More Mushrooms/Famine Food/VE overlap remains incomplete and full owner implementation-entry decision is HOLD.

**Cross-owner handoff (2026-10-10 author decision):** Wild Food Foraging **requires** AMJ Environment (`sucro.ancientmedievaljapan.environment`) and its Japan-native vegetation, including existing `AMJ_Tree_Shii`. Environment does not require Foraging and continues to own the actual PlantDefs, natural distribution and old-AMJE-save plant additions. Foraging owns edible resource harvesting and consumption. Foraging's future `About/About.xml` must record the required packageId and load order; no playable metadata has been created. No standalone fallback trees or duplicated natural plant Defs. Additional species/food products, harvest values, art, VE/non-VE necessity, native Jobs, runtime and save E2E remain open; Preservation and Coastal Gathering are distinct.
