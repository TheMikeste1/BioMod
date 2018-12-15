namespace BioMod
{
    public abstract class BioBuildingSapling : BioBuildingBase
    {
        protected BioBuildingSapling()
        {
            currentStage = "Sapling";
        }
    }

    public class TestWallSapling : BioBuildingSapling
    {
        public TestWallSapling()
        {
            nextStage = BioThingDefOf.BIO_TestWall;
        }
    }
}
