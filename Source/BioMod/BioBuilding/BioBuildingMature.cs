using Verse;

namespace BioTech
{
    public abstract class BioBuildingMature : Building
    {
        public override void TickLong()
        {
            if (this.HitPoints < this.MaxHitPoints)
            {
                this.HitPoints += (def as BioThingDef).healRate;
            }
        }
    }
}
