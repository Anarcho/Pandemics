using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;

namespace Pandemics
{
    public class WorkGiver_AnalyzeVirus : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(PandemicsDefOf.Pandemics_Building_VirusLab);

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_VirusLab virusLab = t as Building_VirusLab;
            if (virusLab == null || !virusLab.Spawned || virusLab.IsBurning())
                return null;

            // Check if the pawn has the unknown virus hediff
            if (!pawn.health.hediffSet.HasHediff(PandemicsDefOf.Pandemics_Virus_UnknownVirus))
                return null;

            // Create a new job to analyze the virus
            Job job = JobMaker.MakeJob(PandemicsDefOf.Pandemics_AnalyzeVirus, t);
            return job;
        }
    }
}
