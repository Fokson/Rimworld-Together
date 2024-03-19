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
            WorldSyncSteps = new WorldGenStepDef[10];
            WorldSyncSteps[0] = new WorldGenStepDef();
            WorldSyncSteps[0].worldGenStep = new RT_WorldGenStep_Terrain();
            WorldSyncSteps[1] = new WorldGenStepDef();
            WorldSyncSteps[1].worldGenStep = new RT_WorldGenStep_Components();
            WorldSyncSteps[2] = new WorldGenStepDef();
            WorldSyncSteps[2].worldGenStep = new RT_WorldGenStep_Lakes();
            WorldSyncSteps[3] = new WorldGenStepDef();
            WorldSyncSteps[3].worldGenStep = new RT_WorldGenStep_Rivers();
            WorldSyncSteps[4] = new WorldGenStepDef();
            WorldSyncSteps[4].worldGenStep = new RT_WorldGenStep_AncientSites();
            WorldSyncSteps[5] = new WorldGenStepDef();
            WorldSyncSteps[5].worldGenStep = new RT_WorldGenStep_AncientRoads();
            WorldSyncSteps[6] = new WorldGenStepDef();
            WorldSyncSteps[6].worldGenStep = new WorldGenStep_Pollution();
            WorldSyncSteps[7] = new WorldGenStepDef();
            WorldSyncSteps[7].worldGenStep = new RT_WorldGenStep_Factions();
            WorldSyncSteps[8] = new WorldGenStepDef();
            WorldSyncSteps[8].worldGenStep = new WorldGenStep_Roads();
            WorldSyncSteps[9] = new WorldGenStepDef();
            WorldSyncSteps[9].worldGenStep = new WorldGenStep_Features();
        }
    }
}
