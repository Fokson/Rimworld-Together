using System;
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
			FactionGenerator.GenerateFactionsIntoWorld(Current.CreatingWorld.info.factions);
		}

		public override void GenerateWithoutWorldData(string seed)
		{
		}
	}
}