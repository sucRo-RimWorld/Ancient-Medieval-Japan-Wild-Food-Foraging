# Wild Food Foraging — native 1.6 Warm/Cool sowing tests (2026-10-14)

**Evidence class: native map and actual RimWorld growing-command eligibility PASS, not pawn sow/growth/save.** The two successful runs used genuinely generated maps with the specified AMJ Environment biomes. The 11 test plants and their synthetic placement conditions are not Foraging production content.

## Immutable native evidence

| Case | Run and artifact | Result |
| --- | --- | --- |
| WarmTemperateForest | [Actions #38509307452](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38509307452), [artifact 11869227407](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38509307452/artifacts/11869227407) | **20/20 PASS**, game exit 0, Unity ERROR 0, zero preLaunchErrors/logErrors, test Mod removed |
| CoolTemperateForest | [Actions #38509808851](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38509808851), [artifact 11868898921](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38509808851/artifacts/11868898921) | **20/20 PASS**, game exit 0, Unity ERROR 0, zero preLaunchErrors/logErrors, test Mod removed |

- Native game: **RimWorld 1.6.4871 rev600**, dedicated isolated Linux self-hosted game, not the author’s personal Steam ModsConfig/saves.
- Exact owner source: **sucRo-RimWorld/Ancient-Medieval-Japan-Wild-Food-Foraging at 45d69e6eadfa8cfab59193c6cc546421ddeb8268**. Actual [Quickstart C#](../Tests/NativeSowingProbe/WffSowingQuickstart.cs) uses reflection to invoke the real RimWorld Command_SetPlantToGrow.IsPlantAvailable API, rejecting unknown signatures. Host [runner](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/blob/main/Tools/PersistentHost/wff_quickstart16.py) compiled the test-only assembly against native game dependencies and required original Environment binary. Both runs removed the diagnostic Mod.
- Tree fixture: actual original Environment Shii plus **test-only** Kuri/walnut/Kaya; plants inherit native sowing rules. **Only Warm** has synthetic wildPlants placement for these test trees, to test mustBeWildToSow. The fixture says nothing about which forests the eventual production Kuri/walnut/Kaya may inhabit.
- Synthetic stand-ins: Vanilla graphic references, fixture growDays 3/30, Warm placement weights 0.2. **Never copy these into production**, and do not assert playable food or authentic plant visuals.

## What the 20 assertions in each real map mean

| Premise/test item | Warm result | Cool result | Scope |
| --- | --- | --- | --- |
| Naturally generated map biome equals target | PASS | PASS | No forced reclassification of a different natural biome |
| TreeSowing research exists; initially unfinished | PASS / PASS | PASS / PASS | Test opens with original research restriction |
| Before research, Shii and all 3 fixture trees are ineligible | 4/4 PASS | 4/4 PASS | Actual native growing command rejects before prerequisite |
| Actual ResearchManager finishes TreeSowing | PASS | PASS | Real game research manager, not a fake flag |
| After research, original Shii and 3 fixture trees have expected native grow-command availability | **4/4 allowed** | **4/4 denied** | Warm-only **synthetic** tree wildPlants definitions; region restriction actually evaluated |
| Fuki and Udo eligible to grow by ordinary command | 2/2 PASS | 2/2 PASS | Does not prove a pawn completed a sow job |
| Warabi, Zenmai, Jinenjo, Hiratake, Matsutake, Shiitake lack sowTags | 6/6 PASS | 6/6 PASS | Native loaded field checked; grow-zone menu was **not visually inspected** |

**Per map: 1 biome + 1 research existence + 1 unfinished + 4 pre-research + 1 finished + 4 region decisions + 2 ordinary greens + 6 wild-only flags = 20. Total 40/40 PASS across both independent game maps.**

## Previous incomplete runs and still-open coverage

- Prior Warm [run #38507535254](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38507535254) reported 20 individual predicates true but the run **FAILED** its overall gate due **16 preLaunchErrors**. It is not counted as a successful run; the corrected later Warm run has preLaunchErrors 0.
- Earlier native map/save Pickle [run #38506268559](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/38506268559) **TIMED OUT after 240 seconds**. Do not claim save or map-plant persistence was tested successfully.
- These results extend the previous [loaded-Def and 16-field evidence](NativeSowingEvidence-20261014.md) but **do not fully PASS AC18–AC22**: real colonist sow jobs, actual planted plants, growth, save/reload, old saves, actual gathering/nut products, forestry output after harvest, food safety, MO/CCTO profiles and suitable fungal substrate remain OPEN. Source-specific native menu eligibility is more advanced than simply checking sowTags, but is **not a screenshot/on-screen manual selection test**.
- Owner design remains [Design](../Design.md), [AcquisitionDesign](../AcquisitionDesign.md), [12 resources](../ResourceAdoption-v1.md). No production tree, edible item, biome distribution, art, or gameplay C# was modified by the native test.
