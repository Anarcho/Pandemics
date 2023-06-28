using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace Pandemics
{
    [HarmonyPatch(typeof(Hediff), nameof(Hediff.GetTooltip))]
    public class HediffDef_GetTooltip_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Hediff __instance, ref string __result)
        {
            HediffDef hediffDef = __instance.def;
            if (hediffDef.defName.Contains("Pandemic_"))
            {
                int stageIndex = __instance.CurStageIndex;
                List<HediffStage> stages = hediffDef.stages;

                if(stageIndex >= 0 && stageIndex < stages.Count)
                {
                    StringBuilder stringBuilder = new StringBuilder();

                    stringBuilder.AppendLine(__result);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("This is the result of an uknown virus.");
                    for(int i = 0; i <= stageIndex; i++)
                    {
                        string stageDescription = __instance.def.GetModExtension<ExentededHediffStages>().descriptions[i].Description;
                        stringBuilder.AppendLine(stageDescription);
                    }
                    __result = stringBuilder.ToString();

                }
                
            }
        }
    }
}
