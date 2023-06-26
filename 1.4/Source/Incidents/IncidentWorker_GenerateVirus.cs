using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Pandemics
{
    public class IncidentWorker_GenerateVirus : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;

            // Create unique virus def
            string virusName = PandemicUtils.GenerateVirusName();
            HediffDef templateDef = DefDatabase<HediffDef>.GetNamed("TemplateHediffDef");
            HediffDef virusDef = new HediffDef();  
            virusDef.defName = "Pandem_" + Find.UniqueIDsManager.GetNextHediffID().ToString();

            // randomly generate number of stages
            int numOfStages = Rand.RangeInclusive(3, 5);
            virusDef.stages = new List<HediffStage>(numOfStages);

            for (int i = 0; i < numOfStages; i++)
            {
                HediffStage stage = new HediffStage();
                stage.label = "Stage " + i;
                
                                switch (i)
                {
                    case 0:
                        stage.capMods = new List<PawnCapacityModifier>
                        {
                            new PawnCapacityModifier
                            {
                                capacity = PawnCapacityDefOf.Consciousness,
                                offset = -0.2f // Reduce consciousness capacity by 20% in the initial stage
                            }
                        };
                        stage.hediffGivers = new List<HediffGiver>();

                        stage.hediffGivers.Add(new HediffGiver());
                        break;

                    

                
            }
                virusDef.stages.Add(stage);
            }


            virusDef.description = $"An unknown virus that causes the following symptoms:\n\n{stage.label}\n{symptom.label}";

        }
    }
}
