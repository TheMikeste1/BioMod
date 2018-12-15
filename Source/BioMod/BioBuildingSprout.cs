using System.Text;
using Verse;

namespace BioMod
{
    public abstract class BioBuildingSprout : BioBuildingBase
    {
        protected BioBuildingSprout()
        {
            currentStage = "Sprout";
        }
    }

    public class TestWallSprout : BioBuildingSprout
    {
        public TestWallSprout()
        {
            nextStage = BioThingDefOf.BIO_TestWallSeedling;
        }
    }
}