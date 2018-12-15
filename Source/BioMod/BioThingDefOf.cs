using System.IO;
using RimWorld;
using Verse;

namespace BioTech
{
    [DefOf]
    public static class BioThingDefOf
    {
        static BioThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BioThingDefOf));
        }

        public static BioThingDef BIO_TestWallPlanter;
        public static BioThingDef BIO_TestWallSprout;
        public static BioThingDef BIO_TestWallSeedling;
        public static BioThingDef BIO_TestWallSapling;
        public static BioThingDef BIO_TestWallMature;
    }
}
