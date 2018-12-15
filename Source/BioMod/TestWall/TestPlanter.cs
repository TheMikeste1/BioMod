using RimWorld;
using Verse;

namespace BioTech
{
    public class TestWallPlanter : BuildingPlanterBase
    {
        public TestWallPlanter()
        {
            nextStage = BioThingDefOf.BIO_TestWallSprout;
            growthCountdown = 0;
        }
    }
}
