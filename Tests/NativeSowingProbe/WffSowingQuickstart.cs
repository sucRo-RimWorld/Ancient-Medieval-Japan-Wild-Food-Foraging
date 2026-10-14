// DEVELOPER-ONLY 1.6 Quickstart acceptance probe. Never ship in WFF.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorks.Quickstarts;
using RimWorks.Quickstarts.Verification;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace AMJ.WildFoodForaging.NativeTests
{
    public abstract class WffNativeSowingQuickstart : AbstractQuickstart
    {
        protected abstract string TargetBiome { get; }

        public override TaggedString description
        {
            get { return "Isolated native growing-command eligibility test for " + TargetBiome; }
        }

        public override string seed
        {
            get { return "AMJ-WFF-SOW-20261014"; }
        }

        public override float planetCoverage
        {
            get { return 0.30f; }
        }

        public override int mapSize
        {
            get { return 150; }
        }

        public override void PostApplyConfiguration()
        {
            PlanetTile selected = PlanetTile.Invalid;
            // Do NOT force a different biome onto an unrelated tile.
            // A genuine biome/settlement candidate is a precondition.
            for (int i = 0; i < Find.WorldGrid.TilesCount; ++i)
            {
                SurfaceTile candidate = Find.WorldGrid[i] as SurfaceTile;
                if (candidate == null || candidate.PrimaryBiome == null ||
                    candidate.PrimaryBiome.defName != TargetBiome ||
                    candidate.WaterCovered || candidate.hilliness == Hilliness.Impassable)
                    continue;
                if (TileFinder.IsValidTileForNewSettlement(candidate.tile))
                {
                    selected = candidate.tile;
                    break;
                }
            }
            if (!selected.Valid)
                throw new InvalidOperationException("No natural settlement tile with biome " + TargetBiome);
            Find.GameInitData.startingTile = selected;
            Log.Message("[WFF Sowing Quickstart] selected " + TargetBiome + " tile=" + selected);
        }

        // Invoke the actual game's growing-zone availability method rather than
        // duplicating its native research/biome algorithm in a test helper.
        private static bool NativeGrowCommandOffers(ThingDef def, Map map)
        {
            Type type = typeof(Command_SetPlantToGrow);
            MethodInfo method = type.GetMethods(
                BindingFlags.Instance | BindingFlags.Static |
                BindingFlags.NonPublic | BindingFlags.Public)
                .FirstOrDefault(x => x.Name == "IsPlantAvailable");
            if (method == null)
                throw new MissingMethodException(type.FullName, "IsPlantAvailable");
            ParameterInfo[] parameters = method.GetParameters();
            object[] args;
            if (parameters.Length == 2 && parameters[0].ParameterType == typeof(ThingDef)
                && parameters[1].ParameterType == typeof(Map))
                args = new object[] { def, map };
            else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(ThingDef))
                args = new object[] { def };
            else
                throw new InvalidOperationException("Unknown native IsPlantAvailable signature");
            object instance = method.IsStatic ? null : Activator.CreateInstance(type, true);
            return (bool)method.Invoke(instance, args);
        }

        public override QuickstartVerification Verify()
        {
            QuickstartVerification result = new QuickstartVerification();
            Map map = Find.CurrentMap;
            result.Assert("actual generated map has requested biome", delegate
            {
                return map != null && map.Biome != null &&
                       map.Biome.defName == TargetBiome;
            });
            if (map == null || map.Biome == null)
                return result;
            ResearchProjectDef research = DefDatabase<ResearchProjectDef>.GetNamedSilentFail("TreeSowing");
            result.Assert("TreeSowing research project exists", delegate { return research != null; });
            if (research == null)
                return result;
            bool wasFinished = research.IsFinished;
            result.Assert("TreeSowing initially unfinished in new scenario",
                delegate { return !wasFinished; });
            if (wasFinished)
                return result;

            string[] treeNames =
            {
                "AMJWF_Probe_Kuri", "AMJWF_Probe_Walnut",
                "AMJWF_Probe_Kaya", "AMJ_Tree_Shii",
            };
            Dictionary<string, bool> before = new Dictionary<string, bool>();
            foreach (string name in treeNames)
            {
                ThingDef def = DefDatabase<ThingDef>.GetNamedSilentFail(name);
                bool available = def != null && NativeGrowCommandOffers(def, map);
                before[name] = available;
                string id = name;
                result.Assert(id + " is blocked by unfinished TreeSowing",
                    delegate { return !before[id]; });
            }
            // The project is finished in the *isolated test map only*.
            Find.ResearchManager.FinishProject(
                research, doCompletionDialog: false, researcher: null,
                doCompletionLetter: false);
            result.Assert("TreeSowing becomes finished in game",
                delegate { return research.IsFinished; });

            bool expectedTree = TargetBiome == "AMJ_WarmTemperateForest";
            foreach (string name in treeNames)
            {
                ThingDef def = DefDatabase<ThingDef>.GetNamedSilentFail(name);
                bool native = def != null && NativeGrowCommandOffers(def, map);
                string id = name;
                bool value = native;
                result.Assert(id + " native growing-command eligibility in " + TargetBiome,
                    delegate { return value == expectedTree; });
            }
            foreach (string name in new string[] { "Fuki", "Udo" })
            {
                ThingDef def = DefDatabase<ThingDef>.GetNamedSilentFail("AMJWF_Probe_" + name);
                bool available = def != null && NativeGrowCommandOffers(def, map);
                result.Assert(name + " native growing command accepts ordinary crop",
                    delegate { return available; });
            }
            foreach (string name in new string[] { "Warabi", "Zenmai",
                "Jinenjo", "Hiratake", "Matsutake", "Shiitake" })
            {
                ThingDef def = DefDatabase<ThingDef>.GetNamedSilentFail("AMJWF_Probe_" + name);
                bool notSowable = def != null &&
                    (def.plant.sowTags == null || def.plant.sowTags.Count == 0);
                result.Assert(name + " has no grow-zone sow tag",
                    delegate { return notSowable; });
            }
            return result;
        }
    }

    public sealed class WffWarmSowingQuickstart : WffNativeSowingQuickstart
    {
        protected override string TargetBiome
        {
            get { return "AMJ_WarmTemperateForest"; }
        }
    }

    public sealed class WffCoolSowingQuickstart : WffNativeSowingQuickstart
    {
        protected override string TargetBiome
        {
            get { return "AMJ_CoolTemperateForest"; }
        }
    }
}
