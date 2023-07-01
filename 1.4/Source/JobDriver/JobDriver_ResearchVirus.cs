using System.Collections.Generic;
using Verse;
using Verse.AI;
using System.Linq;

namespace Pandemics
{
    public class JobDriver_ResearchVirus : JobDriver
    {
        private const string VirusHediffDefName = "Pandemic_Virus";

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(500);

            // Generate a random name for the virus hediff
            string newVirusName = GenerateRandomName();

            // Replace the existing virus hediff on affected pawns with a modified copy
            yield return new Toil
            {
                initAction = () =>
                {
                    HediffDef virusHediffDef = DefDatabase<HediffDef>.GetNamed(VirusHediffDefName);

                    // Create a copy of the virus hediff definition
                    HediffDef newVirusHediffDef = virusHediffDef;

                    // Modify the defName and label of the new virus hediff definition
                    newVirusHediffDef.defName = VirusHediffDefName + "_" + newVirusName;
                    newVirusHediffDef.label = newVirusName;

                    // Replace the existing virus hediff with the modified copy on affected pawns
                    foreach (Pawn pawn in GetAffectedPawns())
                    {
                        Hediff virusHediff = pawn.health.hediffSet.GetFirstHediffOfDef(virusHediffDef);

                        if (virusHediff != null)
                        {
                            // Remove the current virus hediff
                            pawn.health.RemoveHediff(virusHediff);

                            // Add the new virus hediff
                            Hediff newVirusHediff = HediffMaker.MakeHediff(newVirusHediffDef, pawn);
                            pawn.health.AddHediff(newVirusHediff);
                        }
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };

            yield break;
        }

        private string GenerateRandomName()
        {
            // Generate a random alphanumeric name with a pattern similar to "Cov-19" or "ALX-45"
            string prefix = Rand.RangeInclusive(100, 999).ToString();
            string suffix = Rand.RangeInclusive(10, 99).ToString();
            return prefix + "-" + suffix;
        }

        private IEnumerable<Pawn> GetAffectedPawns()
        {
            // Get all pawns on the map that have the virus hediff
            return Find.CurrentMap.mapPawns.AllPawnsSpawned.Where(pawn => pawn.health.hediffSet.HasHediff(DefDatabase<HediffDef>.GetNamed(VirusHediffDefName)));
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            // Reserve the workbench
            return pawn.Reserve(TargetA, job, 1, -1, null, errorOnFailed);
        }
    }
}