using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace BioMod
{
    public  class BioBuilding : Building //Remove abstract when Def works
    {
        //I'd like to put most of these inside a new Def, but I can't figure it out.
        //Everytime I edit the XML to use my custom Def, the buildings disappear from
        //the build menu. 🤷🏻‍
        //I'll keep trying, but for now the values will have to be hard-coded.
        
        //Growth
        protected float growthInt = 0.47f; //Starting growth percent
        protected float growDays = 3.5f; //How many days it takes to grow.
        protected float passableThreshold = 0.5f; //The point at which pawns can no longer walk through the building.
        protected int unlitTicks;
        protected int ageInt;
        //Light
        protected float growMinGlow = 0.3f;
        protected float growOptimalGlow = 1.0f;
        //Fertility
        protected float fertilitySensitivity = 0.5f;
        protected float fertilityMin = 0.9f;
        protected bool seasonSensitive = true;
        //Misc.
        protected bool inGround = true;
        protected string cachedLabelMouseover;
        public float fillPercent;
        private bool freePassageWhenClearedReachabilityCache;

        public bool FreePassage => growthInt < passableThreshold;

        protected virtual PlantLifeStage LifeStage
        {
            get
            {
                if (this.growthInt < 0.001f)
                {
                    return PlantLifeStage.Sowing;
                }
                if (this.growthInt > 0.999f)
                {
                    growthInt = 1f; //Round numbers are easier to work with.
                    return PlantLifeStage.Mature;
                }
                return PlantLifeStage.Growing;
            }
        }

        protected virtual bool Resting => GenLocalDate.DayPercent(this) < 0.25f || GenLocalDate.DayPercent(this) > 0.8f;

        protected float GrowthPerTick
        {
            get
            {
                if (this.LifeStage != PlantLifeStage.Growing || this.Resting)
                {
                    return 0f;
                }
                float num = 1f / (60000f * growDays);
                return num * this.GrowthRate;
            }
        }

        protected float GrowthRateFactor_Fertility
        {
            get
            {
                return base.Map.fertilityGrid.FertilityAt(base.Position) * this.fertilitySensitivity + (1f - this.fertilitySensitivity);
            }
        }

        private float GrowthRateFactor_Light
        {
            get
            {
                float num = base.Map.glowGrid.GameGlowAt(base.Position, false);
                return GenMath.InverseLerp(growMinGlow, growOptimalGlow, num);
            }
        }

        protected virtual bool HasEnoughLightToGrow => this.GrowthRateFactor_Light > 0.001f;

        protected string GrowthPercentString => (this.growthInt + 0.0001f).ToStringPercent();

        /**********************************************************************
         * string LabelMouseover (override)
         *    Creates a a string containing the percentage of growth which
         * appears in the bottom left corner of the UI. This override is nearly
         * identical to the one in RimWorld.Plant.
         * *******************************************************************/
        public override string LabelMouseover
        {
            get
            {
                if (this.cachedLabelMouseover == null)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(this.def.LabelCap);
                    stringBuilder.Append(" (" + "PercentGrowth".Translate(growthInt.ToStringPercent()) + ")");
                    this.cachedLabelMouseover = stringBuilder.ToString();
                }
                return this.cachedLabelMouseover;
            }
        }

        private void ClearReachabilityCache(Map map)
        {
            map.reachability.ClearCache();
            this.freePassageWhenClearedReachabilityCache = this.FreePassage;
        }

        /**********************************************************************
         * void TickRare (override)
         *    Gives insructions on what to do every time this building is 
         * ticked. A normal Tick() is 1/60th of a second, a TickRare() is
         * 250 normal ticks, and a TickLong() is 2000 normal ticks.
         *    Basically a copy-paste from RimWorld.Plant.
         * *******************************************************************/
        public override void TickLong()
        {
            if (growthInt < 1.0f) //If the building is mature, skip all this.
            {
                if (base.Destroyed) //Is the colony destroyed?
                {
                    return;
                }

                int ticks = 2000;

                if (PlantUtility.GrowthSeasonNow(base.Position, base.Map)) //Is it currently growing season?
                {
                    this.growthInt += this.GrowthPerTick * ticks;
                }

                if (!this.HasEnoughLightToGrow) //Is it bright enough to grow?
                {
                    this.unlitTicks += ticks;
                }
                else
                {
                    this.unlitTicks = 0;
                }

                this.ageInt += ticks;
                this.fillPercent = growthInt;
            }

            if (this.FreePassage != this.freePassageWhenClearedReachabilityCache)
            {
                this.ClearReachabilityCache(base.Map);
            }

            this.cachedLabelMouseover = null; //Don't know if we need to do this, but just in case...
        }

        public bool CanPhysicallyPass(Pawn p)
        {
            return this.FreePassage;
        }

        private float GrowthRate
        {
            get
            {
                if (inGround) //Is the building planted in the ground or in a hydroponic?
                {
                    if (seasonSensitive && !PlantUtility.GrowthSeasonNow(base.Position, base.Map, false)) //Is it growing season?
                    {
                        return 0f;
                    }

                    return this.GrowthRateFactor_Fertility * this.GrowthRateFactor_Temperature *
                           this.GrowthRateFactor_Light;
                }

                return this.GrowthRateFactor_Temperature * this.GrowthRateFactor_Light;
            }
        }

        private float GrowthRateFactor_Temperature
        {
            get
            {
                float num;
                if (!GenTemperature.TryGetTemperatureForCell(base.Position, base.Map, out num))
                {
                    return 1f;
                }
                if (num < 10f)
                {
                    return Mathf.InverseLerp(0f, 10f, num);
                }
                if (num > 42f)
                {
                    return Mathf.InverseLerp(58f, 42f, num);
                }
                return 1f;
            }
        }

       /**********************************************************************
        * string GetInspectString (override)
        *    Creates a various strings of information displayed in the box
        * that opens upon click the thing. This override is nearly
        * identical to the one in RimWorld.Plant.
        * *******************************************************************/
        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (this.LifeStage == PlantLifeStage.Growing)
            {
                stringBuilder.AppendLine("PercentGrowth".Translate(new object[]
                {
                    this.GrowthPercentString
                }));
                stringBuilder.AppendLine("GrowthRate".Translate() + ": " + this.GrowthRate.ToStringPercent());
                if (this.Resting)
                {
                    stringBuilder.AppendLine("PlantResting".Translate());
                }
                if (!this.HasEnoughLightToGrow)
                {
                    stringBuilder.AppendLine("PlantNeedsLightLevel".Translate() + ": " + growMinGlow.ToStringPercent());
                }
            }
            else if (this.LifeStage == PlantLifeStage.Mature)
            {
                stringBuilder.AppendLine("Mature".Translate());
            }
            return stringBuilder.ToString().TrimEndNewlines();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this.growthInt, "growth", 0f, false);
            Scribe_Values.Look<int>(ref this.ageInt, "age", 0, false);
            Scribe_Values.Look<int>(ref this.unlitTicks, "unlitTicks", 0, false);
        }
    }
}
