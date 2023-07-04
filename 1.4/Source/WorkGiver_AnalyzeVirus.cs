using RimWorld;
using Verse;
using Verse.AI;

namespace Pandemics
{
    public class WorkGiver_AnalyzeVirus : WorkGiver_Scanner
    {
        private const int JobCheckIntervalTicks = 100; // Interval for checking available jobs
        private int lastJobCheckTick = -JobCheckIntervalTicks - 1;

        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(PandemicsDefOf.Pandemics_Building_VirusLab);

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            // Check job check interval
            if (Find.TickManager.TicksGame - lastJobCheckTick < JobCheckIntervalTicks)
                return true;

            // Update last job check tick
            lastJobCheckTick = Find.TickManager.TicksGame;

            // Check if any pawn in the colony has the unknown virus hediff
            bool anyPawnHasVirus = PawnsFinder.AllMaps_FreeColonistsSpawned.Any(p => p.health.hediffSet.HasHediff(PandemicsDefOf.Pandemics_Virus_UnknownVirus));
            if (!anyPawnHasVirus)
                return true;

            // Check if any pawn is already performing the virus analysis job
            bool anyPawnAnalyzingVirus = PawnsFinder.AllMaps_FreeColonistsSpawned.Any(p => p.jobs?.curDriver?.GetReport() == PandemicsDefOf.Pandemics_AnalyzeVirus.defName);
            if (anyPawnAnalyzingVirus)
                return true;

            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
        {
            Building_VirusLab virusLab = thing as Building_VirusLab;
            if (virusLab == null || !virusLab.Spawned || virusLab.IsBurning())
                return null;

            // Create a new job to analyze the virus
            Job job = JobMaker.MakeJob(PandemicsDefOf.Pandemics_AnalyzeVirus, thing);
            return job;
        }
    }
}
