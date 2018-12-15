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

        public static BioThingDef BIO_TestPlanter;
        public static BioThingDef BIO_TestWallSprout;
        public static BioThingDef BIO_TestWallSeedling;
        public static BioThingDef BIO_TestWallSapling;
        public static BioThingDef BIO_TestWall;
    }
}
