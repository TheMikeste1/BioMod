using RimWorld;
using Verse;

namespace BioTech
{
    public abstract class BuildingPlanterBase : Building
    {
        protected int growthCountdown = 3;
        protected ThingDef nextStage = null;
        protected bool minifiable = true;

        /**********************************************************************
         * void TickRare (override)
         *    Gives insructions on what to do every time this building is 
         * ticked. A normal Tick() is 1/60th of a second, a TickRare() is
         * 250 normal ticks, and a TickLong() is 2000 normal ticks.
         *********************************************************************/
        public override void TickLong()
        {
            if (growthCountdown == 0)
            {
                if (nextStage != null)
                {
                    IntVec3 currentPosition = this.Position;
                    Map currentMap = this.Map;
                    Faction currentFaction = this.factionInt;

                    this.Destroy();

                    Thing newBuilding = GenSpawn.Spawn(ThingMaker.MakeThing(nextStage),
                        currentPosition,
                        currentMap); //You have no idea how long it took me to figure out how to do this.
                    newBuilding.SetFactionDirect(currentFaction);
                }
                else
                {
                    Log.ErrorOnce("ThingDef nextStage for " + this.def.defName + " was null.\nBuilding at "
                                  + this.Position + " will not be updated.", thingIDNumber);
                }
            }
            else
            {
                growthCountdown--;
            }
        }
    }
}
