using RimWorld;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static Shared.CommonEnumerators;

namespace GameClient
{
    public class RT_GenStep_Terrain : GenStep
    {
        private struct GRLT_Entry
        {
            public float bestDistance;

            public IntVec3 bestNode;
        }

        public override int SeedPart
        {
            get
            {
                return 262606459;
            }
        }

        public override void Generate(Map map, GenStepParams parms)
        {
            MapDetailsJSON mapDetailsJSON = MapGenerationData.mapDetailsJSON;
            int index = 0;


            for (int z = 0; z < map.Size.z; ++z)
            {
                for (int x = 0; x < map.Size.x; ++x)
                {
                    IntVec3 vectorToCheck = new IntVec3(x, map.Size.y, z);

                    Building edifice = vectorToCheck.GetEdifice(map);
                    

                    try
                    {
                        TerrainDef terrainToUse = DefDatabase<TerrainDef>.AllDefs.ToList().Find(fetch => fetch.defName ==
                            mapDetailsJSON.tileDefNames[index]);


                        //  Sometimes SetTerrain needs to use the passability variable in a TerrainDef, but cant 
                        //  because no terrain exists at the index (a null reference gets thrown)
                        //  Here we are creating a TerrainDef and setting passability to avoid an exception
                        //  no idea why the terrain is null at all.
                        int ind = map.cellIndices.CellToIndex(vectorToCheck);
                        if (map.terrainGrid.TerrainAt(ind) == null)
                        {
                            map.terrainGrid.topGrid[ind] = new TerrainDef();
                            map.terrainGrid.topGrid[ind].passability = Traversability.Standable;
                        }
                        map.terrainGrid.SetTerrain(vectorToCheck, terrainToUse);

                    }
                    catch (Exception e) {

                        Logger.WriteToConsole($"Failed to set terrain at {vectorToCheck}",LogMode.Warning);
                        Logger.WriteToConsole(e.ToString(),LogMode.Error);
                    }

                    try
                    {
                        RoofDef roofToUse = DefDatabase<RoofDef>.AllDefs.ToList().Find(fetch => fetch.defName ==
                                    mapDetailsJSON.roofDefNames[index]);

                        map.roofGrid.SetRoof(vectorToCheck, roofToUse);
                    }
                    catch { Log.Warning($"Failed to set roof at {vectorToCheck}"); }

                    index++;
                }
            }
        }


    }
}
