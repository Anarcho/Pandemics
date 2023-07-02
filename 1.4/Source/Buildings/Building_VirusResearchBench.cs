using RimWorld;
using System.Collections.Generic;
using System.Linq;

using Verse;

namespace Pandemics
{
    public class Building_VirusResearchBench : ThingWithComps
    {
        public override string GetInspectString()
        {
            // Add custom inspect string for the virus research bench
            return base.GetInspectString() + "\n" + "Researching unknown viruses.";
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            // Add custom Gizmos for the virus research bench
            foreach (var gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (!Spawned)
            {
                return;
            }

            HediffDef virusHediffDef = DefDatabase<HediffDef>.GetNamed("Pandemic_Virus_UnknownVirus");

            // Check if any pawns are currently assigned to the virus research work type
            var assignedPawns = Find.CurrentMap.mapPawns.FreeColonistsSpawned.Where(p => p.workSettings?.GetPriority(WorkTypeDefOf.Research) > 0);
            foreach (var pawn in assignedPawns)
            {
                // Check if the pawn has the "Pandemic_Virus_UnknownVirus" hediff
                var unknownVirusHediff = pawn.health.hediffSet.GetFirstHediffOfDef(virusHediffDef);
                if (unknownVirusHediff != null)
                {
                    // Generate a new unique HediffDef for the virus
                    var uniqueSuffix = GenText.StableStringHash(pawn.GetUniqueLoadID());
                    var newVirusHediffDef = new HediffDef
                    {
                        defName = "Pandemic_Virus_" + uniqueSuffix,
                        label = "COV-19",
                        // Add any other properties you want to set for the new HediffDef
                    };

                    // Replace the existing hediff with the newly generated HediffDef
                   pawn.health.RemoveHediff(unknownVirusHediff);

                // Add the new hediff to the pawn
                var newVirusHediff = HediffMaker.MakeHediff(newVirusHediffDef, pawn);
                pawn.health.AddHediff(newVirusHediff);
                }
            }
        }
    }
}
