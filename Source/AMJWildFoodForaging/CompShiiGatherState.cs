using RimWorld;
using Verse;

namespace AMJ.WildFoodForaging
{
    // Mechanism only: until food, yield, cooldown and work are specified, disabled.
    public sealed class CompProperties_ShiiGatherState : CompProperties
    {
        public ThingDef productDef;
        public int yieldCount;
        public int cooldownTicks;
        public int workTicks;

        public CompProperties_ShiiGatherState()
        {
            compClass = typeof(CompShiiGatherState);
        }
    }

    public sealed class CompShiiGatherState : ThingComp
    {
        private int lastGatherTick = -1;
        private bool hasGathered;

        private CompProperties_ShiiGatherState Props => (CompProperties_ShiiGatherState)props;

        public int WorkTicks => Props.workTicks;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref lastGatherTick, "amj_shiiLastGatherTick", -1);
            Scribe_Values.Look(ref hasGathered, "amj_shiiHasGathered", false);
        }

        public bool CanGather()
        {
            if (!(parent is Plant plant) || !plant.Spawned || plant.Destroyed ||
                plant.IsBurning() || plant.Blighted || plant.LifeStage != PlantLifeStage.Mature)
                return false;

            if (plant.def.defName != "AMJ_Tree_Shii" ||
                plant.Map.designationManager.DesignationOn(plant, DesignationDefOf.CutPlant) != null ||
                plant.Map.designationManager.DesignationOn(plant, DesignationDefOf.HarvestPlant) != null)
                return false;

            // No reinterpretation of existing harvestYield=42 timber as food.
            if (Props.productDef == null || Props.yieldCount <= 0 ||
                Props.cooldownTicks <= 0 || Props.workTicks <= 0 ||
                Props.yieldCount > Props.productDef.stackLimit)
                return false;

            return !hasGathered ||
                   Find.TickManager.TicksGame - lastGatherTick >= Props.cooldownTicks;
        }

        // One completed job owns both spawning its product and advancing cooldown.
        public bool TryGather(Pawn gatherer)
        {
            if (!CanGather() || gatherer == null || gatherer.Map != parent.Map)
                return false;

            Thing product = ThingMaker.MakeThing(Props.productDef);
            product.stackCount = Props.yieldCount;
            if (!GenPlace.TryPlaceThing(product, parent.Position, parent.Map,
                    ThingPlaceMode.Near))
            {
                product.Destroy();
                return false;
            }

            lastGatherTick = Find.TickManager.TicksGame;
            hasGathered = true;
            return true;
        }
    }
}
