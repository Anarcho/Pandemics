using RimWorld;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace Pandemics
{
    public class Building_VirusResearchBench : Building
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (var option in base.GetFloatMenuOptions(selPawn))
            {
                yield return option;
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
        }

        public override void Tick()
        {
            base.Tick();

            // Check if the research project is finished
            if (Find.ResearchManager.currentProj?.defName == "Research_UnknownVirus" && Find.ResearchManager.currentProj.IsFinished)
            {
                // Replace the unknown virus with the known virus for affected pawns
                foreach (Pawn pawn in PawnsAffectedByUnknownVirus())
                {
                    ReplaceUnknownVirusHediff(pawn);
                }
            }
        }

        private IEnumerable<Pawn> PawnsAffectedByUnknownVirus()
        {
            return Find.CurrentMap.mapPawns.AllPawns.Where(pawn => pawn.health.hediffSet.HasHediff(DefDatabase<HediffDef>.AllDefs
                .FirstOrDefault(def => def.defName.StartsWith("Pandemic_Virus") &&
                pawn.health.hediffSet.HasHediff(def))));
        }

        private void ReplaceUnknownVirusHediff(Pawn pawn)
        {
            // Remove the unknown virus Hediff from the pawn's hediff set
            Hediff unknownVirusHediff = pawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.AllDefs.FirstOrDefault(def => def.defName.StartsWith("Pandemic_Virus") && pawn.health.hediffSet.HasHediff(def)));
            if (unknownVirusHediff != null)
            {
                pawn.health.RemoveHediff(unknownVirusHediff);
            }

            // Add the known virus Hediff to the pawn's hediff set
            Hediff knownVirusHediff = HediffMaker.MakeHediff(DefDatabase<HediffDef>.AllDefs.FirstOrDefault(def => def.defName.StartsWith("Pandemic_Virus") && pawn.health.hediffSet.HasHediff(def)), pawn);
            pawn.health.AddHediff(knownVirusHediff);
        }
    }
}
