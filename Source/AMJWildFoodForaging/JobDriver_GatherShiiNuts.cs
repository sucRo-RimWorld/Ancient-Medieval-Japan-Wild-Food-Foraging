using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AMJ.WildFoodForaging
{
    // Distinct from native PlantCollected and plant cutting.
    public sealed class JobDriver_GatherShiiNuts : JobDriver
    {
        private CompShiiGatherState State =>
            job.targetA.Thing?.TryGetComp<CompShiiGatherState>();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, errorOnFailed: errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch)
                .FailOnBurningImmobile(TargetIndex.A);

            Toil wait = Toils_General.Wait(State?.WorkTicks ?? 0, TargetIndex.A);
            wait.FailOn(() => State == null || !State.CanGather());
            wait.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            wait.FailOnBurningImmobile(TargetIndex.A);
            wait.WithProgressBarToilDelay(TargetIndex.A);
            yield return wait;

            yield return Toils_General.Do(() =>
            {
                var state = State;
                if (state == null || !state.CanGather() || !state.TryGather(pawn))
                    return;

                // A completed order is one-shot. Remove only after the product
                // was successfully placed and the per-tree cooldown saved.
                Thing tree = job.targetA.Thing;
                if (tree == null || !tree.Spawned)
                    return;
                DesignationDef gather = DefDatabase<DesignationDef>.GetNamedSilentFail("AMJ_GatherShiiNuts");
                if (gather == null)
                    return;
                Designation designation = tree.Map.designationManager.DesignationOn(tree, gather);
                if (designation != null)
                    tree.Map.designationManager.RemoveDesignation(designation);
            });
        }
    }
}
