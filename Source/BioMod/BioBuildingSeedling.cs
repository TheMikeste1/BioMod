namespace BioTech
{
    public abstract class BioBuildingSeedling : BioBuildingBase
    {
        protected BioBuildingSeedling()
        {
            currentStage = "Seedling";
        }
    }

    public class TestWallSeedling : BioBuildingSeedling
    {
        public TestWallSeedling()
        {
            nextStage = BioThingDefOf.BIO_TestWallSapling;
        }
    }
}
