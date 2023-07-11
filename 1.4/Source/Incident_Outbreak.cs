using RimWorld;
using System.Linq;
using Verse;

namespace Pandemics
{
    public class IncidentWorker_Outbreak : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && Find.AnyPlayerHomeMap.mapPawns.FreeColonistsSpawned.Any();
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!VirusManager.TryFindRandomMapPawn(out Pawn infectedPawn))
            {
                return false;
            }

            var hediffDefUnknownVirus = PandemicsDefOf.Pandemics_Virus_UnknownVirus;
            if (hediffDefUnknownVirus != null)
            {
                if (!VirusManager.HasVirus(infectedPawn))
                {
                    // Generate a unique virus name
                    string uniqueVirusName = "Unknown Virus " + VirusManager.GenerateUniqueVirusNumber().ToString();

                    // Create the Hediff_UnknownVirus instance and assign the unique virus name
                    Hediff_UnknownVirus hediff = HediffMaker.MakeHediff(hediffDefUnknownVirus, infectedPawn) as Hediff_UnknownVirus;
                    hediff.virusName = uniqueVirusName;
                    hediff.def.defName = uniqueVirusName;
                    hediff.def.label = "Unknown Virus";

                    // Add the hediff to the pawn's health
                    infectedPawn.health.AddHediff(hediff);

                    // Add the virus to the VirusManager
                    VirusManager.AddVirus(infectedPawn, uniqueVirusName);

                    // Show a notification or log the infection event
                    Find.LetterStack.ReceiveLetter("Outbreak", "An outbreak of an unknown virus has occurred. " + infectedPawn.LabelCap + " has been infected with " + hediff.def.label + ".", LetterDefOf.NegativeEvent, new TargetInfo(infectedPawn.Position, infectedPawn.Map));
                }
            }
            return true;
        }
    }
}
