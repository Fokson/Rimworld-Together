using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace GameClient
{
    public class RT_GenStep_Things :GenStep
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


            //Set generation settings
            MapDetailsJSON mapDetailsJSON = MapGenerationData.mapDetailsJSON;
            bool containsItems = MapGenerationData.containsItems;
            bool lessLoot = MapGenerationData.lessLoot;


            List<Thing> thingsToGetInThisTile = new List<Thing>();

            foreach (ItemDetailsJSON item in mapDetailsJSON.nonFactionThings)
            {
                try
                {
                    Thing toGet = ThingScribeManager.StringToItem(item);
                    thingsToGetInThisTile.Add(toGet);
                }
                catch { }
            }

            if (containsItems)
            {
                Random rnd = new Random();

                foreach (ItemDetailsJSON item in mapDetailsJSON.factionThings)
                {
                    try
                    {
                        Thing toGet = ThingScribeManager.StringToItem(item);

                        if (lessLoot)
                        {
                            if (rnd.Next(1, 100) > 70) thingsToGetInThisTile.Add(toGet);
                            else continue;
                        }
                        else thingsToGetInThisTile.Add(toGet);
                    }
                    catch { }
                }
            }

            foreach (Thing thing in thingsToGetInThisTile)
            {
                try { GenPlace.TryPlaceThing(thing, thing.Position, map, ThingPlaceMode.Direct, rot: thing.Rotation); }
                catch { Log.Warning($"Failed to place thing {thing.def.defName} at {thing.Position}"); }
            }
        }

    }
}
