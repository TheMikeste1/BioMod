using System.Text;
using Verse;

namespace BioTech
{
    public class TestWallSprout : BioBuildingSprout
    {
        public TestWallSprout()
        {
            nextStage = BioThingDefOf.BIO_TestWallSeedling;
        }
    }
}