namespace BioTech
{
    public class HedgePlanter : BuildingPlanterBase
    {
        public HedgePlanter()
        {
            nextStage = BioThingDefOf.BIO_HedgeSprout;
            growthCountdown = 1;
        }
    }
}
