using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Pandemics
{
    public class HediffCompProperties_LovinTransmitted : HediffCompProperties
    {
        public int hashInterval = 250;
        public int interactionCooldown = 60000;
        public float transmitChance = 0.5f;

        public HediffCompProperties_LovinTransmitted()
        {
            compClass = typeof(HediffComp_LovinTransmitted);
        }
    }
    public class HediffComp_LovinTransmitted : HediffComp
    {
        public HediffCompProperties_LovinTransmitted Props => (HediffCompProperties_LovinTransmitted)props;

        private List<InteractionRecord> pastInteractions = new List<InteractionRecord>();
        private int lastCheckedTick = -1;

        public override void CompPostTick(ref float severityAdjustment)
        {
            int currentTick = Find.TickManager.TicksGame;
            int hashInterval = Props.hashInterval;

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
                            if (receiver.health.hediffSet.GetFirstHediffOfDef(virusHediff.def) == null)
                            {
                                if (HasRecentLovinJob(receiver))
                                {
                                    float transmitChance = Props.transmitChance;
                                    if (Rand.Chance(transmitChance))
                                    {
                                        Hediff newVirusHediff = (Hediff)HediffMaker.MakeHediff(virusHediff.def, receiver);
                                        receiver.health.AddHediff(newVirusHediff, null, null);
                                        VirusManager.AddPawnToVirus(receiver, virusHediff.def.defName);
                                        string letterText = $"{receiver.NameShortColored} has contracted the virus from {parent.pawn.NameShortColored} during some lovin!";
                                        Find.LetterStack.ReceiveLetter("Virus Transmission", letterText, LetterDefOf.NegativeEvent, parent.pawn);

                                        RecordInteraction(parent.pawn, receiver);
                                    }
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
            return true;
        }

        private bool HasRecentLovinJob(Pawn pawn)
        {
            Job curJob = pawn.CurJob;
            if (curJob != null && curJob.def == JobDefOf.Lovin)
            {
                if (curJob.GetTarget(TargetIndex.A).HasThing && curJob.GetTarget(TargetIndex.A).Thing == parent.pawn)
                {
                    return true;
                }
            }
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
}
