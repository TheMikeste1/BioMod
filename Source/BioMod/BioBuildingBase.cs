using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace BioMod
{
    public abstract class BioBuildingBase : Building
    {
        protected float growthInt = 0.05f; //Starting growth percent
        protected int unlitTicks;
        protected int ageInt;
        protected string cachedLabelMouseover;
        protected string currentStage = null;
        protected ThingDef nextStage = null;

        protected BioBuildingBase()
        {
            if (def != null)
            {
                def.blueprintDef = null;
            }
        }

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
                float num = 1f / (60000f * (def as BioThingDef).bioProperties.growDays);
                return num * this.GrowthRate;
            }
        }

        protected float GrowthRateFactor_Fertility
        {
            get
            {
                return base.Map.fertilityGrid.FertilityAt(base.Position) * (def as BioThingDef).bioProperties.fertilitySensitivity + (1f - (def as BioThingDef).bioProperties.fertilitySensitivity);
            }
        }

        private float GrowthRateFactor_Light
        {
            get
            {
                float num = base.Map.glowGrid.GameGlowAt(base.Position, false);
                return GenMath.InverseLerp((def as BioThingDef).bioProperties.growMinGlow, (def as BioThingDef).bioProperties.growOptimalGlow, num);
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
                    stringBuilder.Append((def as BioThingDef).LabelCap);
                    stringBuilder.Append(" (" + "PercentGrowth".Translate(growthInt.ToStringPercent()) + ")");
                    this.cachedLabelMouseover = stringBuilder.ToString();
                }
                return this.cachedLabelMouseover;
            }
        }

        /**********************************************************************
         * void TickRare (override)
         *    Gives insructions on what to do every time this building is 
         * ticked. A normal Tick() is 1/60th of a second, a TickRare() is
         * 250 normal ticks, and a TickLong() is 2000 normal ticks.
         *    Basically a copy-paste from RimWorld.Plant.
         *********************************************************************/
        public override void TickLong()
        {
            if (growthInt < 1f) //If the building is mature, skip all this.
            {
                if (base.Destroyed) //Is the colony destroyed?
                {
                    return;
                }

                int ticks = 2000;

                if (PlantUtility.GrowthSeasonNow(base.Position, base.Map)) //Is it currently growing season?
                {
                    this.growthInt += this.GrowthPerTick * ticks;
                    if (LifeStage == PlantLifeStage.Mature)
                    {
                        growthInt = 1f;
                    }
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
                this.HitPoints += (def as BioThingDef).healRate;
            }
            else //When ready to progress to the next stage..
            {
                if (nextStage != null) //If there is a next stage..
                {
                    //Copy current stats
                    IntVec3 currentPosition = this.Position;
                    Map currentMap = this.Map;
                    Faction currentFaction = this.factionInt;
                    ThingDef blueprint = def.blueprintDef;

                    //Destroy current building
                    this.Destroy();

                    //Create new building
                    Thing newBuilding = GenSpawn.Spawn(ThingMaker.MakeThing(nextStage),
                        currentPosition,
                        currentMap); //You have no idea how long it took me to figure out how to do this.
                    newBuilding.SetFactionDirect(currentFaction);
                    newBuilding.def.blueprintDef = blueprint;
                }
                else //Otherwise log an error.
                {
                    Log.ErrorOnce("ThingDef nextStage for " + this.def.defName + " was null.\nBuilding at "
                                  + this.Position + " will not be updated.", thingIDNumber);
                }
            }
                  
            this.cachedLabelMouseover = null; //Don't know if we need to do this, but just in case...
        }

        protected float GrowthRate
        {
            get
            {
                if ((def as BioThingDef).bioProperties.inGround) //Is the building planted in the ground or in a hydroponic?
                {
                    if ((def as BioThingDef).bioProperties.seasonSensitive && !PlantUtility.GrowthSeasonNow(base.Position, base.Map, false)) //Is it growing season?
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

            if (currentStage != null)
            {
                stringBuilder.AppendLine(currentStage);
            }
            stringBuilder.AppendLine("PercentGrowth".Translate(this.GrowthPercentString));
            stringBuilder.AppendLine("GrowthRate".Translate() + ": " + this.GrowthRate.ToStringPercent());
            if (this.Resting)
            {
                stringBuilder.AppendLine("PlantResting".Translate());
            }
            if (!this.HasEnoughLightToGrow)
            {
                stringBuilder.AppendLine("PlantNeedsLightLevel".Translate() + ": " + (def as BioThingDef).bioProperties.growMinGlow.ToStringPercent());
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
