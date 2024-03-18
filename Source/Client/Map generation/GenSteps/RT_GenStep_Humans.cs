using Shared;
using Verse;

namespace GameClient
{
    public class RT_GenStep_Humans : GenStep
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

            foreach (HumanDetailsJSON pawn in mapDetailsJSON.nonFactionHumans)
            {
                try
                {
                    Pawn human = HumanScribeManager.StringToHuman(pawn);
                    GenSpawn.Spawn(human, human.Position, map, human.Rotation);
                }
                catch { Log.Warning($"Failed to spawn human {pawn.name}"); }
            }

            foreach (HumanDetailsJSON pawn in mapDetailsJSON.factionHumans)
            {
                try
                {
                    Pawn human = HumanScribeManager.StringToHuman(pawn);
                    human.SetFaction(FactionValues.neutralPlayer);

                    GenSpawn.Spawn(human, human.Position, map, human.Rotation);
                }
                catch { Log.Warning($"Failed to spawn human {pawn.name}"); }
            }

        }
    }
}
