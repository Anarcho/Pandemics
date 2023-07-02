using RimWorld;
using Verse;
using Verse.AI;

namespace Pandemics
{
    public class Building_VirusLab : Building
    {
        public override void Tick()
        {
            base.Tick();

            if (Find.TickManager.TicksGame % 500 == 0)
            {
                // Check if there is a pawn with the unknown virus hediff
                Pawn pawn = FindPawnWithUnknownVirus();
                if (pawn != null)
                {
                    Pawn analyzer = FindPawnWithResearchJob();
                    // Create a new work order for the "Analyze Virus" job
                    WorkGiver_AnalyzeVirus workGiver = new WorkGiver_AnalyzeVirus();
                    Job job = workGiver.JobOnThing(pawn, this);
                    if (job != null)
                    {
                        // Assign the job to the pawn
                        pawn.jobs.StartJob(job);
                    }
                }
            }
        }

        private Pawn FindPawnWithUnknownVirus()
        {
            // Find a pawn with the unknown virus hediff in the current map
            return Map.mapPawns.FreeColonistsSpawned.FirstOrDefault(pawn =>
                pawn.health.hediffSet.HasHediff(PandemicsDefOf.Pandemics_Virus_UnknownVirus));
        }
        private Pawn FindPawnWithResearchJob()
        {
            // Find a pawn with the "Research" job ticked in the current map
            return Map.mapPawns.FreeColonistsSpawned.FirstOrDefault(pawn =>
                pawn.workSettings?.GetPriority(WorkTypeDefOf.Research) > 0);
        }
    }
}
