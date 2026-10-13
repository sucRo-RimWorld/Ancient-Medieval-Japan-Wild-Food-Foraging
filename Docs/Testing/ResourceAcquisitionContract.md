# 12-resource gathering acceptance contract — NOT RUN (2026-10-14)

The natural-product identities/edible parts are the twelve **adopted** objects in [owner adoption list](../ResourceAdoption-v1.md); the proposed collection mode and habitat/preparation constraints are in [AcquisitionDesign](../AcquisitionDesign.md) and machine-readable [matrix](../ResourceAcquisitionMatrix.json). **All AC01–AC12 are proposed real-game tests, currently NOT RUN**, not proven implementations. Each item will be given exact numerical expectations **only after** those values are specified and source/game action exists. The currently available Shii-only static C# scaffold does not cover these cases.

| Test | Premise — concrete adopted item | Required observable behavior | Native result |
| --- | --- | --- | --- |
| AC01 / WF-N01 | Existing naturally grown Environment Shii in Warm; raw wood cut still available | Gather only Shii nuts and leave same tree intact; chopping later preserves Environment wood-only and optional MO WoodChain conversion; prevent duplicate post-save gather | NOT RUN |
| AC02 / WF-N02 | A new Foraging-owned wild Kuri appears naturally in approved habitat | Gather nuts without killing the tree; retain separately tested timber-cut result and saved repeat state; no forced-spawn-only completion | NOT RUN |
| AC03 / WF-N03 | Foraging-owned wild walnut with approved damp-valley/riverside condition | Nuts are gathered without felling; shells/inedible outer parts are not silently treated as edible meat; no unchecked all-biome spawning | NOT RUN |
| AC04 / WF-N04 | Foraging-owned wild Kaya tree where climatic habitat is accepted | Tree and wood survive nut collection; seed outer parts/processing are distinguished from safely edible product; 1259 text is not a spawn-mechanics specification | NOT RUN |
| AC05 / WF-G01 | Naturally spawned Warabi in supported open/edge habitat | Collect young shoots without unconditionally killing their rhizome; harvested raw shoots cannot be consumed as a safe untreated meal; repeat/season boundaries enforced | NOT RUN |
| AC06 / WF-G02 | Naturally spawned Zenmai in supported moist slope/valley | Pick young shoots with specified plant-recovery rule and needed treatment; drying/preservation is not silently implemented in this Mod | NOT RUN |
| AC07 / WF-G03 | Naturally spawned Fuki in humid edge/stream/approved wetland | Gather **leaf stalk** not flower buds; no unconditional raw ingestion. Preprocessing actually represents leaching/de-bittering, not just a false safety label | NOT RUN |
| AC08 / WF-G04 | Naturally spawned wild Udo on a forest edge | Young shoots/young stems only; no later-period blanched agricultural Udo system; parent plant fate/recovery tested | NOT RUN |
| AC09 / WF-R01 | Naturally grown wild Jinenjo with soil conditions accepting underground growth | Dig **underground portion** with finite resource removal/recovery and no unlimited repeated digging; not converted into a sowable cultivated-yam farming loop | NOT RUN |
| AC10 / WF-F01 | Wild Hiratake naturally arises in **actual valid hardwood-decay** surroundings | Gather only fruiting body without cutting host wood; finite natural emergence/harvest; no arbitrary forest-floor fungus from biome name alone | NOT RUN |
| AC11 / WF-F02 | Wild Matsutake with **genuine eligible living pine-host** evidence; current Environment Cool lists generic Plant_TreePine only | Fruit body harvest does not harm host pine; **no Matsutake spawn if no accepted host/nearby eligibility**. Generic Vanilla Pine is not automatically equated to Akamatsu | NOT RUN |
| AC12 / WF-F03 | Wild Shiitake only where accepted hardwood decay substrates exist | Gather fruiting body while leaving substrate under agreed lifecycle; reject raw/undercooked meal; no mushroom bed/log inoculation gameplay | NOT RUN |

## Cross-cutting acceptance cases (also NOT RUN)

- **AC13 — Environment ownership:** Existing Shii and Beech source Defs, normal timber yields, biome climate/ecology remain unmodified. New food-purpose trees/greens/fungi/roots and conditional biome-pool patches belong entirely to Foraging. Foraging requires Environment in game metadata.
- **AC14 — Two actions and saves:** Tree cuts and food gathering are independent; reservation races, cancellation and existing-save migration yield no lost trees, duplicate resource, Def errors or disappearing timber.
- **AC15 — Correctly bounded processing:** Raw ingredient, correctly processed ingredient and full meal are distinct stages when safety requires it. Inedible dangerous young shoots, Fuki alkaloid products, uncooked Shiitake are never silently equivalent to a safe meal.
- **AC16 — Distribution versus habitat:** Approved zone/host/substrate conditions, normal world generation and preexisting-map behavior are tested; `wildPlantRegrowDays` is not assumed equal to individual gathering refresh; no invented commonality or season values.
- **AC17 — Optional profiles:** Baseline Environment with Vanilla first, then separate CCTO, MO and MO+CCTO profiles and any specifically advertised extra-Mod coexistence. **Presence/absence of VV or More Mushrooms cannot veto the twelve adopted species or basic Foraging content.**

**Unverified dependencies that must not be fictionalized:** exact Pine proxy acceptance for Matsutake; hardwood-deadwood substrate availability for Hiratake/Shiitake; foraging-safe handling of Fuki/Warabi/Zenmai; native RimWorld 1.6 XML/C# for whether shrub/rhizome regrowth can be represented by standard harvesting; real data for quantity/regrowth/season. Cross-feature source and loaded-game evidence remain separate.

**Static guard only:** `python -m unittest discover -s Tests/Production -p 'test_*.py'` checks declared IDs, prospective modes, no numeric content, and explicit source controls. Even a static PASS cannot promote a single AC item to PASS.
