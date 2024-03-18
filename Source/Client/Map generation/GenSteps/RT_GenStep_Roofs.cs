using Shared;
using System.Linq;
using Verse;

namespace GameClient
{
    public class RT_GenStep_Roofs : GenStep
    {

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

            map.roofCollapseBuffer.Clear();
            map.roofGrid.Drawer.SetDirty();

        }
    }
}
