using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Pandemics
{
    public class Hediff_UnknownVirus : HediffWithComps
    {
        public string virusName;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref virusName, "virusName");
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            if (pawn != null && !string.IsNullOrEmpty(virusName) && !VirusManager.VirusExists(virusName))
            {
                VirusManager.AddVirus(pawn, virusName);
            }
        }
    }
}
