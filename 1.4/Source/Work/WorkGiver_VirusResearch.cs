using RimWorld;
using Verse;
using Verse.AI;

namespace Pandemics
{
    public class WorkGiver_VirusResearch : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial);

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!t.IsForbidden(pawn) && pawn.CanReserve(t, 1, -1, null, forced) && IsVirusResearchBench(t))
            {
                var unknownVirusHediff = GetUnknownVirusHediff(pawn);
                return unknownVirusHediff != null;
            }

            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            var unknownVirusHediff = GetUnknownVirusHediff(pawn);
            if (unknownVirusHediff != null)
            {
                var job = new Job(DefDatabase<JobDef>.GetNamed("VirusResearchJob"), t);
                job.targetA = t;
                job.targetB = pawn;
                return job;
            }

            return null;
        }

        private bool IsVirusResearchBench(Thing thing)
        {
            return thing is Building_VirusResearchBench;
        }

        private Hediff GetUnknownVirusHediff(Pawn pawn)
        {
            return pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Pandemic_Virus_UnknownVirus);
        }
    }
}
