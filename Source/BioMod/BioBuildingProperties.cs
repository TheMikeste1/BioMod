namespace BioMod
{
    public class BioBuildingProperties
    {
        public float growDays = 3.5f; //How many days it takes to grow.
        //Light
        public float growMinGlow = 0.3f;
        public float growOptimalGlow = 1.0f;
        //Fertility
        public float fertilitySensitivity = 0.5f;
        public float fertilityMin = 0.9f;
        public bool seasonSensitive = true;
        public bool inGround = true;
    }
}
