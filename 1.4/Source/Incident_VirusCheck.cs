using RimWorld;
using Verse;
using Verse.AI.Group;

namespace Pandemics
{
    public class IncidentWorker_VirusCheck : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            Faction empireFaction = Find.FactionManager.OfPlayerEmpire;

            // Spawn the Empire Faction on the map
            IntVec3 spawnCell = CellFinder.RandomEdgeCell(map);
            Faction faction = FactionGenerator.NewGeneratedFaction(empireFaction.def);
            faction.TrySetRelationKind(Faction.OfPlayer, FactionRelationKind.Hostile, true, "Virus check");
            faction.TryGenerateNewLeader();
            GenSpawn.Spawn(faction, spawnCell, map);

            bool infectedPawnFound = false;

            // Check each pawn on the map
            foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
            {
                // Check if the pawn has a virus hediff
                if (pawn.health.hediffSet.HasHediff(PandemicsDefOf.Pandemics_Virus_KnownVirus))
                {
                    infectedPawnFound = true;

                    // Move the Empire Faction to the pawn and walk off the map
                    LordJob_VisitColony lordJob = new LordJob_VisitColony(faction, map);
                    lordJob.setFactionCanEndHostility = false;
                    lordJob.rememberPreviousEnemy = false;
                    lordJob.canSteal = false;
                    lordJob.makePrisoners = false;
                    lordJob.ignorePlayerFaction = true;
                    lordJob.sendWounded = false;
                    lordJob.defendPoint = pawn.Position;
                    LordMaker.MakeNewLord(faction, lordJob, map, null);
                    pawn.Map.mapPawns.MakeLord(faction, lordJob, pawn);
                }
            }

            // If no infected pawns were found, remove the Empire Faction from the map
            if (!infectedPawnFound)
            {
                Find.FactionManager.Remove(faction);
                empireFaction.TrySetRelationKind(Faction.OfPlayer, FactionRelationKind.Neutral);
                map.GetComponent<MapComponent_Pandemics>().AddCooldown(PandemicsDefOf.Pandemics_Incident_VirusCheck, 30); // Add a cooldown to prevent immediate reoccurrence
            }

            return true;
        }
    }
}
