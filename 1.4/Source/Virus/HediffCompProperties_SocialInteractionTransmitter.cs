using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Pandemics
{
    using RimWorld;
    using Verse;

    public class HediffCompProperties_SocialInteractionTransmitter : HediffCompProperties
    {
        public float transmitChance = 0.5f;
        public float transmitSeverityFactor = 0.8f;
        public float maxDistToPawnToReceiveTransmission = 5f;

        public HediffCompProperties_SocialInteractionTransmitter()
        {
            compClass = typeof(HediffComp_SocialInteractionTransmitter);
        }
    }
}
