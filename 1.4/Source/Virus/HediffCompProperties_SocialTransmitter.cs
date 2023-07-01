using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Pandemics
{ 
    public class HediffCompProperties_SocialTransmitter : HediffCompProperties
    {
        public float transmitChance = 0.5f;
        public float transmitSeverityFactor = 0.8f;
        public float maxDistToPawnToReceiveTransmission = 5f;
        public int hashInterval = 100;
        public int interactionCooldown;

        public HediffCompProperties_SocialTransmitter()
        {
            compClass = typeof(HediffComp_SocialTransmitter);
        }
    }
}
