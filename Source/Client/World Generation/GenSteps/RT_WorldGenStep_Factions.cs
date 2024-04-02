using System;
using System.Linq;
using Verse;

namespace RimWorld.Planet
{
	public class RT_WorldGenStep_Factions : WorldGenStep
	{
		public override int SeedPart
		{
			get
			{
				return 777998381;
			}
		}

		public override void GenerateFresh(string seed)
		{
            foreach (FactionDef item in DefDatabase<FactionDef>.AllDefs.OrderBy((FactionDef x) => x.hidden))
            {
				for (int i = 0; i < item.requiredCountAtGameStart; i++)
				{
					Current.CreatingWorld.info.factions.Add(item);
				}
            }
            FactionGenerator.GenerateFactionsIntoWorld(Current.CreatingWorld.info.factions);
		}

		public override void GenerateWithoutWorldData(string seed)
		{
		}
	}
}
