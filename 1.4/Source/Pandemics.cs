using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Pandemics
{

    public class PandemicsSettings : ModSettings
    {
        public float AnalysisDuration = 4000f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref AnalysisDuration, "AnalysisDuration", 4000f);
        }
    }

    public class PandemicsMod : Mod
    {
        public static PandemicsSettings settings;


        public PandemicsMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<PandemicsSettings>();
        }

        public override string SettingsCategory() => "Pandemics";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.Label("Analysis Duration:");
            settings.AnalysisDuration = listing.Slider(settings.AnalysisDuration, 100f, 10000f);

            listing.End();
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            settings.Write();
        }

    }
}
