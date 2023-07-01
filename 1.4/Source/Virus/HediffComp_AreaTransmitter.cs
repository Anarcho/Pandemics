using RimWorld;
using Verse;

namespace Pandemics
{
    public class HediffComp_AreaTransmitter : HediffComp
    {
        public HediffCompProperties_AreaTransmitter Props => (HediffCompProperties_AreaTransmitter)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            Hediff virusHediff = (Hediff)parent;
            int hashInterval = Props.hashInterval;
            int lastCheckedTick = -1;

            int currentTick = Find.TickManager.TicksGame;

            // Perform the check only when the hash interval condition is met
            if (currentTick % hashInterval == 0 && currentTick != lastCheckedTick)
            {
                lastCheckedTick = currentTick;

                if (parent.pawn.Spawned && !parent.pawn.Dead)
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
