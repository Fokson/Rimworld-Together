using HarmonyLib;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace GameClient.World_Generation
{
    public static class WorldGenerationData
    {
        public static WorldGenStepDef[] WorldSyncSteps;

        public static void initializeGenerationDefs()
        {
            List<WorldGenStepDef> WorldSynStepsList = new List<WorldGenStepDef>();
            WorldSynStepsList.Add(new WorldGenStepDef());
            WorldSynStepsList[WorldSynStepsList.Count - 1].worldGenStep = new RT_WorldGenStep_Terrain();
            WorldSynStepsList.Add(new WorldGenStepDef());
            WorldSynStepsList[WorldSynStepsList.Count-1].worldGenStep = new RT_WorldGenStep_Components();
            WorldSynStepsList.Add(new WorldGenStepDef());
            WorldSynStepsList[WorldSynStepsList.Count - 1].worldGenStep = new RT_WorldGenStep_Lakes();
            WorldSynStepsList.Add(new WorldGenStepDef());
            WorldSynStepsList[WorldSynStepsList.Count - 1].worldGenStep = new RT_WorldGenStep_Rivers();
            WorldSynStepsList.Add(new WorldGenStepDef());
            WorldSynStepsList[WorldSynStepsList.Count - 1].worldGenStep = new RT_WorldGenStep_AncientSites();
            WorldSynStepsList.Add(new WorldGenStepDef());
            WorldSynStepsList[WorldSynStepsList.Count - 1].worldGenStep = new RT_WorldGenStep_AncientRoads();

            if (ModLister.BiotechInstalled)
            {
                WorldSynStepsList.Add(new WorldGenStepDef());
                WorldSynStepsList[WorldSynStepsList.Count-1].worldGenStep = new WorldGenStep_Pollution();
            }

            WorldSynStepsList.Add(new WorldGenStepDef());
            WorldSynStepsList[WorldSynStepsList.Count-1].worldGenStep = new RT_WorldGenStep_Factions();
            WorldSynStepsList.Add(new WorldGenStepDef());
            WorldSynStepsList[WorldSynStepsList.Count-1].worldGenStep = new WorldGenStep_Roads();
            WorldSynStepsList.Add(new WorldGenStepDef());
            WorldSynStepsList[WorldSynStepsList.Count-1].worldGenStep = new WorldGenStep_Features();
            WorldSyncSteps = WorldSynStepsList.ToArray();
        }
    }
}
