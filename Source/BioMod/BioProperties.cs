namespace BioMod
{
    public class BioProperties
    {
        //Growth
        //public float growthThreshold = 0.5f; //The point at which the building evolves.
        public float growDays = 0.2f; //How many days it takes to grow.
        //Light
        public float growMinGlow = 0.3f; //Minimum light level for growth.
        public float growOptimalGlow = 1.0f; //Optimal light level for growth.
        //Fertility
        public float fertilitySensitivity = 0.5f; //How much the fertility of the ground affects the plant.
        public float fertilityMin = 0.9f; //The minimum fertility level for the plant to grow.
        public bool seasonSensitive = true; //Is the plant affected by seasons?
        public bool inGround = true; //Is the plant affected by the ground (fertility, support level, ect.)
    }
}
