using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AMJ.WildFoodForaging
{
    // Activation requires separately approved DesignationDef, JobDef and content.
    public sealed class WorkGiver_GatherShiiNuts : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            var gather = DefDatabase<DesignationDef>.GetNamedSilentFail("AMJ_GatherShiiNuts");
            if (gather == null)
                yield break;

            foreach (var designation in pawn.Map.designationManager.allDesignations)
            {
                if (designation.def == gather && designation.target.HasThing)
                    yield return designation.target.Thing;
            }
        }

        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
        {
            var gather = DefDatabase<DesignationDef>.GetNamedSilentFail("AMJ_GatherShiiNuts");
            var jobDef = DefDatabase<JobDef>.GetNamedSilentFail("AMJ_GatherShiiNuts");
            if (gather == null || jobDef == null || thing == null ||
                thing.def.defName != "AMJ_Tree_Shii")
                return null;

            if (pawn.Map.designationManager.DesignationOn(thing, gather) == null ||
                pawn.Map.designationManager.DesignationOn(thing, DesignationDefOf.CutPlant) != null ||
                pawn.Map.designationManager.DesignationOn(thing, DesignationDefOf.HarvestPlant) != null ||
                thing.IsForbidden(pawn) || thing.IsBurning() ||
                !pawn.CanReserveAndReach(thing, PathEndMode.Touch, Danger.Some))
                return null;

            var state = thing.TryGetComp<CompShiiGatherState>();
            return state != null && state.CanGather()
                ? JobMaker.MakeJob(jobDef, thing)
                : null;
        }
    }
}
