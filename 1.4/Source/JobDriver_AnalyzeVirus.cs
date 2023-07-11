using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using System.Text.RegularExpressions;
using System.Linq;

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

                PercentageDone = virusAnalysisProgress / AnalysisDuration;

                // Check if the analysis is complete
                if (virusAnalysisProgress >= AnalysisDuration)
                {
                    // Reset the progress for the next analysis
                    virusAnalysisProgress = 0f;

                    // Get the next unknown virus in numerical order
                    string nextVirusName = VirusManager.GetNextVirusNumericalOrder();
                    string nextVirusLabel = VirusManager.GetVirusLabel(nextVirusName);

                    // Create the new HediffDef
                    List<Pawn> pawnsWithVirus = VirusManager.GetPawnsWithVirus(nextVirusName);
                    HediffDef newVirusHediff = VirusManager.GenerateUniqueVirusHediff(pawnsWithVirus.FirstOrDefault().health.hediffSet.GetFirstHediffOfDef(PandemicsDefOf.Pandemics_Virus_UnknownVirus));

                    if (!DefDatabase<HediffDef>.AllDefs.Any(def => def.defName == newVirusHediff.defName))
                    {
                        DefDatabase<HediffDef>.Add(newVirusHediff);
                    }

                    // Perform the finish actions only for the next unknown virus
                    if (pawnsWithVirus != null && pawnsWithVirus.Count > 0)
                    {
                        foreach (Pawn pawn in pawnsWithVirus)
                        {
                            var unknownVirusHediff = pawn.health.hediffSet.GetFirstHediffOfDef(PandemicsDefOf.Pandemics_Virus_UnknownVirus);
                            if (unknownVirusHediff != null)
                            {
                                // Add the new HediffDef to the DefDatabase
                                pawn.health.RemoveHediff(unknownVirusHediff);
                                VirusManager.ReplaceVirus(PandemicsDefOf.Pandemics_Virus_UnknownVirus.defName, newVirusHediff.defName);
                                pawn.health.AddHediff(newVirusHediff);
                            }
                        }
                    }

                    // Finish the job
                    this.EndJobWith(JobCondition.Succeeded);
                }
            };

            analyzeToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            analyzeToil.WithEffect(EffecterDefOf.Research, TargetIndex.A, null);
            analyzeToil.WithProgressBar(TargetIndex.A, () => PercentageDone, false, -0.5f, true);
            analyzeToil.defaultCompleteMode = ToilCompleteMode.Delay;
            analyzeToil.defaultDuration = (int)(AnalysisDuration * GenTicks.TicksPerRealSecond / pawn.GetStatValue(StatDefOf.ResearchSpeed));

            yield return analyzeToil;
        }

    }
}
