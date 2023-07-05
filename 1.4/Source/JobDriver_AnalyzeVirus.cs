using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Pandemics
{
    public class JobDriver_AnalyzeVirus : JobDriver
    {
        private float virusAnalysisProgress = 0f; // Current progress of virus analysis
        private float AnalysisDuration => PandemicsMod.settings.AnalysisDuration; // Duration of the analysis

        private float PercentageDone = 0f;

        private Building_VirusLab VirusLab => (Building_VirusLab)base.TargetThingA;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            bool pawnReserved = this.pawn.Reserve(this.VirusLab, this.job, 1, -1, null, errorOnFailed);
            bool benchReserved = this.pawn.Reserve(TargetA, this.job, 1, -1, null, errorOnFailed);

            return pawnReserved && benchReserved;
        }


        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);

            // Go to the Virus Lab bench
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);

            // Perform the virus analysis task
            Toil analyzeToil = ToilMaker.MakeToil("MakeNewToils");

            

            analyzeToil.tickAction = () =>
            {
                Pawn actor = analyzeToil.actor;
                float num = actor.GetStatValue(StatDefOf.ResearchSpeed, true, -1);
                num *= this.TargetThingA.GetStatValue(StatDefOf.ResearchSpeedFactor, true, -1);

                // Increment the virus analysis progress
                virusAnalysisProgress += num;

                Log.Message("Progress = " + virusAnalysisProgress);
                Log.Message("Duration = " + AnalysisDuration);
                Log.Message("Percentage Done = " + virusAnalysisProgress / AnalysisDuration);

                PercentageDone = virusAnalysisProgress / AnalysisDuration;

                // Check if the analysis is complete
                if (virusAnalysisProgress >= AnalysisDuration)
                {
                    // Reset the progress for the next analysis
                    virusAnalysisProgress = 0f;

                    // Perform the finish actions
                    var pawnsWithUnknownVirus = GetPawnsWithUnknownVirus();
                    foreach (Pawn pawn in pawnsWithUnknownVirus)
                    {
                        var unknownVirusHediff = pawn.health.hediffSet.GetFirstHediffOfDef(PandemicsDefOf.Pandemics_Virus_UnknownVirus);
                        if (unknownVirusHediff != null)
                        {
                            pawn.health.RemoveHediff(unknownVirusHediff);
                            pawn.health.AddHediff(PandemicsDefOf.Pandemics_Virus_KnownVirus);
                        }
                    }

                    this.EndJobWith(JobCondition.Succeeded); // Finish the job
                }
            };

            analyzeToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            analyzeToil.WithEffect(EffecterDefOf.Research, TargetIndex.A, null);
            analyzeToil.WithProgressBar(TargetIndex.A, () => PercentageDone, false, -0.5f, true);
            analyzeToil.defaultCompleteMode = ToilCompleteMode.Delay;
            analyzeToil.defaultDuration = (int)(AnalysisDuration * GenTicks.TicksPerRealSecond / pawn.GetStatValue(StatDefOf.ResearchSpeed));

            yield return analyzeToil;
        }

        private List<Pawn> GetPawnsWithUnknownVirus()
        {
            List<Pawn> pawnsWithUnknownVirus = new List<Pawn>();
            foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners)
            {
                var unknownVirusHediff = pawn.health.hediffSet.GetFirstHediffOfDef(PandemicsDefOf.Pandemics_Virus_UnknownVirus);
                if (unknownVirusHediff != null)
                {
                    pawnsWithUnknownVirus.Add(pawn);
                }
            }
            return pawnsWithUnknownVirus;
        }
    }
}
