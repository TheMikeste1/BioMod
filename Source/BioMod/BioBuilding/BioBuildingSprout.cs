using System.Text;
using Verse;

namespace BioTech
{
    public abstract class BioBuildingSprout : BioBuildingBase
    {
        protected BioBuildingSprout()
        {
            currentStage = "Sprout";
        }
    }
}