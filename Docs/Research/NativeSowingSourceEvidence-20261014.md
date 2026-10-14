# Native sowing source evidence — 2026-10-14

Scope: Foraging-owned plantability inheritance only; not edible ThingDefs, balance, artwork, real planting jobs or publication.

Vanilla: RimWorld-zh/RimWorld-Core master `Core/Defs/ThingDefs_Plants/Plants_Bases.xml` Git blob `d43954f49934369407ff3df34964cc08c5387a69` contains abstract `PlantBase` with no sowTags, and abstract `TreeBase` with sowTags Ground, sowResearchPrerequisites TreeSowing, mustBeWildToSow=true and native wood harvestedThingDef. Actual 1.6-compatible loaded inheritance still needs a real game test. `Core/Defs/ThingDefs_Plants/Plants_Cultivated_Farm.xml` includes vanilla sowable crops using PlantBase plus sowTags Ground; source: https://github.com/RimWorld-zh/RimWorld-Core/tree/master/Core/Defs/ThingDefs_Plants

AMJ: Environment main `Docs/Design.md` Regional tree sowing contract retains researched, region-limited native Shii tree planting. `Defs/ThingDefs_Plants/AMJ_WildPlants.xml` Shii uses TreeBase, actual git blob in ResourceAcquisitionMatrix.json. Do not redefine Shii.

MO: author-provided `3219596926.zip` MO 1.6 actual fruit trees derive TreeBase, but edible primary-fruit harvesting is separate from tree planting. No MO dependency introduced.

VE: VPE/More Plants earlier direct 1.6 XML fruit tree comparison in `Docs/Research/TreeNutMechanismAudit-20261014.md` shows fruit-primary harvesting, not an alternative mandatory planting system. No VE framework necessary to obtain native sow tags.

Non-VE: VV fruit/nut trees and Vivi Race separately demonstrate custom tree content/harvesting, but cannot replace Vanilla tree sow gating for our baseline. Non-VE comparison and limitations are recorded in `Docs/Research/AdditionalFruitTreeModAudit-20261014.md` and `Docs/Research/ShiiMechanismEntryEvidence-20261014.md`. Do not copy third-party code, XML or art.

Decision: use Vanilla inherited XML for a small set of **abstract definition bases**: 3 future nut-tree classes under TreeBase (the inherited research/region guard must not be overridden); 2 future sowable wild greens under PlantBase with native Ground sowTags; wild-only plants/fungi have no sowTags and cannot be added to growing-zone menus. All child templates stay abstract with no harvestedThingDef, yields, graphics or biomes, so none is a playable species until separately accepted. This is a native XML reuse of the already approved 12-species planting policy, not a new sowing framework.

Limits: vanilla source repository master is a documentation mirror, not certified as exactly the installed RimWorld 1.6 assemblies. Native 1.6 sow menu, research, pawn job, forest region, save migration, MO/CCTO still NOT RUN. Parent XML templates cannot demonstrate these behaviors without concrete source Defs.
