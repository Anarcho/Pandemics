using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;

namespace Pandemics
{
    public class JobDriver_AnalyzeVirus : JobDriver
    {
        private const float AnalysisDuration = 4000f; // Define the duration of the analysis

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.health.hediffSet.HasHediff(HediffDef.Named("Pandemics_Virus_UnknownVirus"));
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            // Reserve the bench and the pawn
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Reserve.Reserve(TargetIndex.A);

            // Go to the Virus Lab bench
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);

            // Perform the virus analysis task
            Toil analyzeToil = new Toil();
            analyzeToil.defaultDuration = (int)(AnalysisDuration / pawn.GetStatValue(StatDefOf.ResearchSpeed));
            analyzeToil.FailOnDespawnedOrNull(TargetIndex.A);
            analyzeToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            analyzeToil.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            analyzeToil.initAction = () =>
            {
                // Remove the unknown virus hediff
                var unknownVirusHediff = pawn.health.hediffSet.GetFirstHediffOfDef(PandemicsDefOf.Pandemics_Virus_UnknownVirus);
                if (unknownVirusHediff != null)
                    pawn.health.RemoveHediff(unknownVirusHediff);

                // Apply the known virus hediff
                pawn.health.AddHediff(PandemicsDefOf.Pandemics_Virus_KnownVirus); ;
            };
            yield return analyzeToil;
        }
    }
}
