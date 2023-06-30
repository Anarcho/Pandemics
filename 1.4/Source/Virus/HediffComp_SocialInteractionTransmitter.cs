using Mono.Unix.Native;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Pandemics
{
    public class HediffComp_SocialInteractionTransmitter : HediffComp
    {
        public HediffCompProperties_SocialInteractionTransmitter Props => (HediffCompProperties_SocialInteractionTransmitter)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            Hediff virusHediff = (Hediff)parent;

            if (parent.pawn.Spawned && !parent.pawn.Dead && !parent.pawn.InMentalState)
            {
                foreach (Pawn receiver in Find.CurrentMap.mapPawns.AllPawnsSpawned)
                {
                    if (receiver != parent.pawn && receiver.RaceProps.Humanlike && ShouldTransmitVirus(receiver, virusHediff))
                    {
                        float transmitChance = Props.transmitChance;
                        if (Rand.Chance(transmitChance))
                        {
                            Hediff newVirusHediff = (Hediff)HediffMaker.MakeHediff(virusHediff.def, receiver);
                            receiver.health.AddHediff(newVirusHediff, null, null);

                            string letterText = $"{receiver.NameShortColored} has contracted the virus from {parent.pawn.NameShortColored}!";
                            Find.LetterStack.ReceiveLetter("Virus Transmission", letterText, LetterDefOf.NegativeEvent, parent.pawn);
                        }
                    }
                }
            }
        }

        private bool ShouldTransmitVirus(Pawn receiver, Hediff virusHediff)
        {
            if (receiver == null || receiver.Dead || receiver.IsInvisible() || receiver.health.hediffSet.HasHediff(virusHediff.def))
                return false;

            float distance = (receiver.Position - parent.pawn.Position).LengthHorizontalSquared;
            if (distance > Props.maxDistToPawnToReceiveTransmission * Props.maxDistToPawnToReceiveTransmission)
                return false;

            return true;
        }
    }
}
