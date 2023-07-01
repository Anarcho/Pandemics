using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Pandemics
{
    public class HediffComp_SocialTransmitter : HediffComp
    {
        public HediffCompProperties_SocialTransmitter Props => (HediffCompProperties_SocialTransmitter)props;

        private List<InteractionRecord> pastInteractions = new List<InteractionRecord>();
        private int lastCheckedTick = -1;
        

        public override void CompPostTick(ref float severityAdjustment)
        {
            int currentTick = Find.TickManager.TicksGame;
            int hashInterval = Props.hashInterval;
            // Check if enough ticks have passed since the last execution
            if (currentTick > lastCheckedTick + hashInterval)
            {
                lastCheckedTick = currentTick;

                base.CompPostTick(ref severityAdjustment);
                Hediff virusHediff = (Hediff)parent;


                if (parent.pawn.Spawned && !parent.pawn.Dead)
                {
                    foreach (Pawn receiver in Find.CurrentMap.mapPawns.AllPawnsSpawned)
                    {
                        if (receiver != parent.pawn && receiver.RaceProps.Humanlike && ShouldTransmitVirus(receiver, virusHediff))
                        {
                            if (!HasRecentInteraction(parent.pawn, receiver, Props.interactionCooldown))
                            {
                                float transmitChance = Props.transmitChance;
                                if (Rand.Chance(transmitChance))
                                {
                                    Hediff newVirusHediff = (Hediff)HediffMaker.MakeHediff(virusHediff.def, receiver);
                                    receiver.health.AddHediff(newVirusHediff, null, null);

                                    string letterText = $"{receiver.NameShortColored} has contracted the virus from {parent.pawn.NameShortColored}!";
                                    Find.LetterStack.ReceiveLetter("Virus Transmission", letterText, LetterDefOf.NegativeEvent, parent.pawn);

                                    // Record the interaction
                                    RecordInteraction(parent.pawn, receiver);
                                }
                            }
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

        private bool HasRecentInteraction(Pawn pawn1, Pawn pawn2, int cooldownTicks)
        {
            foreach (InteractionRecord interaction in pastInteractions)
            {
                if ((interaction.Initiator == pawn1 && interaction.Recipient == pawn2 ||
                     interaction.Initiator == pawn2 && interaction.Recipient == pawn1) &&
                    (Find.TickManager.TicksGame - interaction.Timestamp) < cooldownTicks)
                {
                    // Recent interaction occurred
                    return true;
                }
            }
            // No recent interaction found
            return false;
        }

        private void RecordInteraction(Pawn initiator, Pawn recipient)
        {
            InteractionRecord interaction = new InteractionRecord()
            {
                Initiator = initiator,
                Recipient = recipient,
                Timestamp = Find.TickManager.TicksGame
            };

            pastInteractions.Add(interaction);
        }
    }
    public class InteractionRecord
    {
        public Pawn Initiator { get; set; }
        public Pawn Recipient { get; set; }
        public int Timestamp { get; set; }
    }
}



    
