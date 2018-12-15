using Verse;

namespace BioMod
{
    public abstract class BioBuilding : Building
    {
        public override void TickLong()
        {
            if (this.HitPoints < this.MaxHitPoints)
            {
                this.HitPoints += (def as BioThingDef).healRate;
            }
        }
    }

    public class TestWall : BioBuilding
    {
        public TestWall()
        {

        }
    }
}
