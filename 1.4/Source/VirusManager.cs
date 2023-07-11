using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Verse;

namespace Pandemics
{
    public class VirusRecord
    {
        public string VirusDefName;
        public List<Pawn> InfectedPawns;
        public float ResearchPercentage;

        public VirusRecord(string defName)
        {
            VirusDefName = defName;
            InfectedPawns = new List<Pawn>();
            ResearchPercentage = 0f;
        }
    }

    public static class VirusManager
    {
        public static readonly Dictionary<string, VirusRecord> virusRecords = new Dictionary<string, VirusRecord>();

        public static void AddVirus(Pawn pawn, string virusName)
        {
            if (!virusRecords.ContainsKey(virusName))
            {
                Hediff_UnknownVirus hediff = HediffMaker.MakeHediff(PandemicsDefOf.Pandemics_Virus_UnknownVirus, pawn) as Hediff_UnknownVirus;
                hediff.virusName = virusName;
                pawn.health.AddHediff(hediff);

                VirusRecord virusRecord = new VirusRecord(hediff.def.defName);
                virusRecord.InfectedPawns.Add(pawn);

                virusRecords.Add(virusName, virusRecord);
            }
            else
            {
                VirusRecord virusRecord = virusRecords[virusName];
                if (!virusRecord.InfectedPawns.Contains(pawn))
                {
                    virusRecord.InfectedPawns.Add(pawn);
                }   
            }
        }

        public static bool HasVirus(Pawn pawn)
        {
            return virusRecords.Values.Any(record => record.InfectedPawns.Contains(pawn));
        }

        public static string GetVirusName(Pawn pawn)
        {
            var virusRecord = virusRecords.Values.FirstOrDefault(record => record.InfectedPawns.Contains(pawn));
            return virusRecord?.VirusDefName;
        }

        public static bool TryFindRandomMapPawn(out Pawn infectedPawn)
        {
            var allColonists = Find.AnyPlayerHomeMap.mapPawns.FreeColonistsSpawned.ToList();
            if (allColonists.Any())
            {
                infectedPawn = allColonists.RandomElement();
                return true;
            }

            infectedPawn = null;
            return false;
        }

        public static int GenerateUniqueVirusNumber()
        {
            int nextVirusNumber = Find.World.GetComponent<WorldComponent_UniqueVirusNumbers>().GetNextVirusNumber();
            return nextVirusNumber;
        }

        public static List<Pawn> GetPawnsWithVirus(string virusName)
        {
            if (virusRecords.TryGetValue(virusName, out var virusRecord))
            {
                return virusRecord.InfectedPawns;
            }
            return new List<Pawn>();
        }

        public static string GenerateUniqueVirusName()
        {
            string regexPattern = @"^Virus_(\d+)$";
            var existingNumbers = virusRecords.Keys
                .Select(name => int.TryParse(Regex.Match(name, regexPattern).Groups[1].Value, out var number) ? number : 0)
                .ToList();

            int nextNumber = 1;
            while (existingNumbers.Contains(nextNumber))
            {
                nextNumber++;
            }

            return "Virus_" + nextNumber.ToString();
        }

        public static string GetVirusLabel(string virusName)
        {
            string numberPart = Regex.Match(virusName, @"\d+").Value;
            return "Virus " + numberPart;
        }

        public static void ReplaceVirus(string virusName, string newVirusName)
        {
            if (virusRecords.TryGetValue(virusName, out var virusRecord))
            {
                virusRecord.VirusDefName = newVirusName;
                virusRecords.Remove(virusName);
                virusRecords.Add(newVirusName, virusRecord);
            }
        }

        public static void AddPawnToVirus(Pawn pawn, string virusName)
        {
            if (pawn != null && virusName != null)
            {
                if (virusRecords.TryGetValue(virusName, out VirusRecord virusRecord))
                {
                    if (!virusRecord.InfectedPawns.Contains(pawn))
                    {
                        virusRecord.InfectedPawns.Add(pawn);
                    }
                }
            }
        }

        public static string GetNextVirusNumericalOrder()
        {
            var nextVirus = virusRecords.OrderBy(pair => pair.Value).FirstOrDefault();
            return nextVirus.Value.VirusDefName;
        }

        public static bool VirusExists(string virusName)
        {
            if(virusRecords.ContainsKey(virusName))
                return true;
            return false;
        }

        private static string GenerateUniqueNamedVirus()
        {
            // Get existing virus names
            List<string> existingNames = new List<string>();
            foreach (HediffDef hediffDef in DefDatabase<HediffDef>.AllDefsListForReading)
            {
                string defName = hediffDef.defName;
                if (defName.StartsWith("Virus_"))
                {
                    existingNames.Add(defName);
                }
            }

            // Get random virus prefix and suffix
            string randomPrefix = GetRandomVirusPrefix();
            string randomSuffix = GetRandomVirusSuffix();

            // Combine prefix and suffix to create a unique virus name
            string uniqueName = randomPrefix + " " + randomSuffix;

            // Check if the generated name already exists
            while (existingNames.Contains(uniqueName))
            {
                randomPrefix = GetRandomVirusPrefix();
                randomSuffix = GetRandomVirusSuffix();
                uniqueName = randomPrefix + " " + randomSuffix;
            }

            // Return the unique virus name
            return uniqueName;
        }

        private static string GetRandomVirusPrefix()
        {
            List<string> prefixes = DefDatabase<VirusNameDef>.GetNamedSilentFail("VirusNameLibrary").prefixes;
            return prefixes.RandomElementWithFallback() ?? "UndefinedPrefix";
        }

        private static string GetRandomVirusSuffix()
        {
            List<string> suffixes = DefDatabase<VirusNameDef>.GetNamedSilentFail("VirusNameLibrary").suffixes;
            return suffixes.RandomElementWithFallback() ?? "UndefinedSuffix";
        }



        // Get a random hediff definition
        private static HediffDef GetRandomHediffDef()
        {
            List<HediffDef> allHediffDefs = DefDatabase<HediffDef>.AllDefsListForReading;
            HediffDef randomHediffDef = allHediffDefs.RandomElement();

            return randomHediffDef;
        }

        // Get a random hediff giver
        private static HediffGiver GetRandomHediffGiver()
        {
            HediffDef randomHediffDef = GetRandomHediffDef();

            HediffGiver_Random_Symptom hediffGiver = new HediffGiver_Random_Symptom
            {
                hediff = randomHediffDef,
                mtbDays = Rand.Range(0.5f, 2f)
            };

            return hediffGiver;
        }

        // Get a random pawn capacity definition
        private static PawnCapacityDef GetRandomPawnCapacityDef()
        {
            List<PawnCapacityDef> allCapacityDefs = DefDatabase<PawnCapacityDef>.AllDefsListForReading;
            PawnCapacityDef randomCapacityDef = allCapacityDefs.RandomElement();

            return randomCapacityDef;
        }

        // Get a random pawn capacity modifier
        private static PawnCapacityModifier GetRandomPawnCapacityModifier()
        {
            PawnCapacityDef randomCapacityDef = GetRandomPawnCapacityDef();

            PawnCapacityModifier capacityModifier = new PawnCapacityModifier
            {
                capacity = randomCapacityDef,
                offset = -Rand.Range(0.1f, 0.5f) // Negative value
            };

            return capacityModifier;
        }

        public static HediffDef GenerateUniqueVirusHediff(Hediff currHediff)
        {
            string uniqueName = GenerateUniqueNamedVirus();
            string uniqueLabel = uniqueName;

            HediffDef newVirusHediff = new HediffDef
            {
                defName = uniqueName,
                label = uniqueLabel,
                initialSeverity = currHediff.Severity,
                labelNoun = "virus",
                description = "An unidentified pathogen that has not yet been studied or classified by medicalprofessionals. Its origins, mode of transmission, and potential long-term effects remain a mystery.",
                hediffClass = typeof(HediffWithComps),
                comps = new List<HediffCompProperties>
                {
                    new HediffCompProperties_Discoverable { sendLetterWhenDiscovered = true },
                    new HediffCompProperties_SocialTransmitter
                    {
                        transmitChance = 0.1f,
                        transmitSeverityFactor = 0.8f,
                        maxDistToPawnToReceiveTransmission = 1,
                        hashInterval = 100,
                        interactionCooldown = 10
                    }
                    // Add more components as needed
                },
                stages = new List<HediffStage>
                {
                    new HediffStage
                    {
                        label = "minor",
                        minSeverity = 0.1f,
                        hediffGivers = new List<HediffGiver>
                        {
                            GetRandomHediffGiver()
                        }
                    },
                    new HediffStage
                    {
                        label = "Moderate",
                        minSeverity = 0.4f,
                        painOffset = Rand.Range(0.05f, 0.15f),
                        capMods = new List<PawnCapacityModifier>
                        {
                            GetRandomPawnCapacityModifier()
                        }
                    },
                    new HediffStage
                    {
                        label = "Severe",
                        minSeverity = 0.7f,
                        painOffset = Rand.Range(0.1f, 0.2f),
                        lifeThreatening = true,
                        capMods = new List<PawnCapacityModifier>
                        {
                            GetRandomPawnCapacityModifier(),
                            GetRandomPawnCapacityModifier()
                        }
                    }
                }
            };

            return newVirusHediff;
        }
    }
}
