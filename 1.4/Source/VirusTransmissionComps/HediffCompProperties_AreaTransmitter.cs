using Verse;
using RimWorld;

namespace Pandemics
{
    public class HediffCompProperties_AreaTransmitter : HediffCompProperties
    {
        public float transmitChance = 0.5f;
        public float transmitSeverityFactor = 0.8f;
        public float maxDistToPawnToReceiveTransmission = 5f;
        public int hashInterval = 100;

        public HediffCompProperties_AreaTransmitter()
        {
            compClass = typeof(HediffComp_AreaTransmitter);
        }
    }
}
