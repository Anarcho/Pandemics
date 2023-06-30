using Verse;
using HarmonyLib;

namespace Pandemics
{
    [StaticConstructorOnStartup]
    internal static class HarmonyInit
    {
        static HarmonyInit()
        {
            Harmony harmony = new Harmony("Anarcho.Pandemics");
            harmony.PatchAll();
        }
    }
}
