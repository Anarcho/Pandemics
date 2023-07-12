using Verse;
using System.Collections.Generic;

namespace Pandemics
{
    public class VirusNameDef : Def
    {
        public List<string> prefixes;
        public List<string> suffixes;

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            if (prefixes == null)
                prefixes = new List<string>();

            if (suffixes == null)
                suffixes = new List<string>();
        }
    }
}
