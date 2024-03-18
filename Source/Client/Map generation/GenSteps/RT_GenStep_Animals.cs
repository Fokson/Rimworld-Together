using Verse;
using Shared;

namespace GameClient
{

    internal class RT_GenStep_Animals : GenStep
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
            if (!MapGenerationData.containsAnimals) return;


            foreach (AnimalDetailsJSON pawn in mapDetailsJSON.nonFactionAnimals)
            {
                try
                {
                    Pawn animal = AnimalScribeManager.StringToAnimal(pawn);
                    GenSpawn.Spawn(animal, animal.Position, map, animal.Rotation);
                }
                catch { Log.Warning($"Failed to spawn animal {pawn.name}"); }
            }

            foreach (AnimalDetailsJSON pawn in mapDetailsJSON.factionAnimals)
            {
                try
                {
                    Pawn animal = AnimalScribeManager.StringToAnimal(pawn);
                    animal.SetFaction(FactionValues.neutralPlayer);

                    GenSpawn.Spawn(animal, animal.Position, map, animal.Rotation);
                }
                catch { Log.Warning($"Failed to spawn animal {pawn.name}"); }
            }
        }
    }
}
