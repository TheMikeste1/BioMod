using Verse;

namespace BioMod
{
    public class BioThingDef : ThingDef
    {
        public BioProperties bioProperties;
        public int healRate = 0;

        public BioThingDef()
        {
            bioProperties = new BioProperties();
        }
    }
}
