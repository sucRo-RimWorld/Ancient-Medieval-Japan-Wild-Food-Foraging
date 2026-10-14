# Shii gathering mechanism source (not yet activated in-game)

## Scope

Original AMJ C# technical scaffold for independent collection from an **existing**
Environment-owned `AMJ_Tree_Shii`. Unlike standard RimWorld plant harvesting,
the separate `JobDriver_GatherShiiNuts` never invokes `PlantCollected`,
never destroys a tree, and never changes its wood-primary `harvestedThingDef`.

- `CompShiiGatherState`: serialized `lastGatherTick` + `hasGathered`, per-tree
  eligibility and atomic product-spawn/cooldown completion.
- `WorkGiver_GatherShiiNuts`: designated trees only; tests reach/reservation,
  forbid/burn and blocks simultaneous Vanilla cut/harvest designations.
- `JobDriver_GatherShiiNuts`: reserves the actual tree; waits, revalidates
  eligibility, requests a single gather completion, and clears the one-shot\n  designation only after a successful product placement.

## Explicitly not yet executable

The previous Project-format [feature-scoped decision](../../Docs/Research/ShiiMechanismDecision.json)
covers technical source development **only**, not food or gameplay balance.
No playable `About/About.xml`, ThingDef/item art, configurable product count,
cooldown duration, work amount, JobDef/WorkGiverDef/DesignationDef, UI, DLL or
steam package has been approved/introduced. With the four numeric/product
settings missing, `CanGather` always returns false. **No actual gathering
can occur with this checkout.**

The source is not proven to compile against the installed RimWorld 1.6
`Assembly-CSharp.dll`, and has not been run with native RimWorld, MO or existing
saves. It must not be published as a functioning Mod.

## Activation prerequisites

Before shipping, finalize the Shii-nut ThingDef and nutrition, finite count,
cooldown/season, work duration and designation UX in `Docs/Design.md`. Then
add the exact Environment dependency to metadata; attach this component to
Shii through an opt-in patch **without replacing the Environment PlantDef or
primary wood yield**; add linked JobDef, WorkGiverDef and designation, compile
with actual RimWorld version references, and execute F01–F14 in real profiles.
Do not copy Vivi or any upstream source/graphics; the current classes were
written specifically for AMJ.

Run static checks with:

```sh
python -m unittest discover -s Tests/Production -p 'test_*.py'
```

A static PASS does not imply C# compilation, game load, working UI, old-save
safety or complete tree-gather behavior.
