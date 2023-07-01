using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Pandemics
{
    public class HediffGiver_Random_Symptom : HediffGiver
    {
        public float mtbDays;

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            float num = mtbDays;
            float num2 = ChanceFactor(pawn);

            int currStage = pawn.health.hediffSet.hediffs
                .Where(hediff => hediff.def.defName.StartsWith("Pandemic_Virus"))
                .Select(hediff => hediff.CurStageIndex)
                .FirstOrDefault();

            string severityLabel = "";


            switch (currStage)
            {
                case 0:
                    severityLabel = "Minor";
                    break;
                case 1:
                    severityLabel = "Moderate";
                    break;
                case 2:
                    severityLabel = "Severe";
                    break;
                default:
                    severityLabel = "Unknown";
                    break;
            }

            List<HediffDef> pandemicHediffDefs = DefDatabase<HediffDef>.AllDefsListForReading
                .Where(hediffDef => hediffDef.defName.StartsWith($"Pandemic_{severityLabel}"))
                .ToList();

            this.hediff = pandemicHediffDefs.RandomElement();

            if (num2 == 0f)
            {
                return;
            }

            if (Rand.MTBEventOccurs(num / num2, 60000f, 60f) && base.TryApply(pawn, null))
            {
                base.SendLetter(pawn, cause);
            }

        }
    }
}
