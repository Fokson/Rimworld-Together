using System.Collections.Generic;
using System.Linq;
using GameClient.World_Generation;
using RimWorld;
using RimWorld.Planet;
using Shared;
using Verse;
using Verse.Profile;
using static Shared.CommonEnumerators;

namespace GameClient
{
    public static class WorldGeneratorManager
    {
        public static string seedString;
        public static int persistentRandomValue;
        public static float planetCoverage;
        public static OverallRainfall rainfall;
        public static OverallTemperature temperature;
        public static OverallPopulation population;
        public static float pollution;
        public static List<FactionDef> factions = new List<FactionDef>();
        public static WorldDetailsJSON cachedWorldDetails;

        public static IEnumerable<WorldGenStepDef> GenStepsInOrder => from x in DefDatabase<WorldGenStepDef>.AllDefs
                                                                      orderby x.order, x.index
                                                                      select x;

        public static void SetValuesFromGame(string seedString, float planetCoverage, OverallRainfall rainfall, OverallTemperature temperature, OverallPopulation population, List<FactionDef> factions, float pollution)
        {
            WorldGeneratorManager.seedString = seedString;
            WorldGeneratorManager.persistentRandomValue = 0;
            WorldGeneratorManager.planetCoverage = planetCoverage;
            WorldGeneratorManager.rainfall = rainfall;
            WorldGeneratorManager.temperature = temperature;
            WorldGeneratorManager.population = population;
            WorldGeneratorManager.pollution = pollution;
            WorldGeneratorManager.factions = factions;

            WorldGeneratorManager.factions.Add(FactionValues.neutralPlayerDef);
            WorldGeneratorManager.factions.Add(FactionValues.allyPlayerDef);
            WorldGeneratorManager.factions.Add(FactionValues.enemyPlayerDef);
            WorldGeneratorManager.factions.Add(FactionValues.yourOnlineFactionDef);
        }

        public static void SetValuesFromServer(WorldDetailsJSON worldDetailsJSON)
        {
            seedString = worldDetailsJSON.seedString;
            persistentRandomValue = worldDetailsJSON.persistentRandomValue;
            planetCoverage = float.Parse(worldDetailsJSON.planetCoverage);
            rainfall = (OverallRainfall)int.Parse(worldDetailsJSON.rainfall);
            temperature = (OverallTemperature)int.Parse(worldDetailsJSON.temperature);
            population = (OverallPopulation)int.Parse(worldDetailsJSON.population);
            pollution = float.Parse(worldDetailsJSON.pollution);

            factions = new List<FactionDef>();
            FactionDef factionToAdd;
            Dictionary<string, FactionDetails> factionDictionary = new Dictionary<string, FactionDetails>();
            Dictionary<string, byte[]> cacheDetailsFactionDict = new Dictionary<string, byte[]>();
            //Convert the string-byte[] dictionary into a string-FactionDetails dictionary
            foreach (string str in worldDetailsJSON.factions.Keys)
            {
                factionDictionary[str] = (FactionDetails)Serializer.ConvertBytesToObject(worldDetailsJSON.factions[str]);
            }

            //for each faction in worldDetails, try to add it to the client's world

            FactionDetails factionDetails = new FactionDetails();
            foreach (string factionName in factionDictionary.Keys)
            {
                factionToAdd = DefDatabase<FactionDef>.AllDefs.FirstOrDefault(fetch => fetch.defName == factionName);

                //try to find a faction with similar details
                if (factionToAdd == null)
                {
                    factionToAdd = DefDatabase<FactionDef>.AllDefs.FirstOrDefault(
                        fetch => (fetch.permanentEnemy == factionDictionary[factionName].permanentEnemy) &&
                                ((byte)fetch.techLevel == factionDictionary[factionName].techLevel) &&
                                (fetch.hidden == factionDictionary[factionName].hidden));

                    //if a faction cannot be found with similar details, then make a new faction
                    if (factionToAdd == null)
                    {
                        factionToAdd = FactionScribeManager.factionDetailsToFaction(factionDictionary[factionName]);
                    }


                }
                factionToAdd.fixedName = factionDictionary[factionName].fixedName;
                factions.Add(factionToAdd);
                factionDetails = FactionScribeManager.factionToFactionDetails(factionToAdd);
                cacheDetailsFactionDict[factionName] = Serializer.ConvertObjectToBytes(factionDetails);
            }

           

            //Convert the string-string dictionary into a string-FactionDetails dictionary
            foreach (string str in worldDetailsJSON.factions.Keys)
            {
            }

            worldDetailsJSON.factions = cacheDetailsFactionDict;
            cachedWorldDetails = worldDetailsJSON;
        }

        public static void GeneratePatchedWorld(bool firstGeneration)
        {
            DialogManager.clearStack();
            LongEventHandler.QueueLongEvent(delegate
            {
                Find.GameInitData.ResetWorldRelatedMapInitData();
                Current.Game.World = GenerateWorld();
                LongEventHandler.ExecuteWhenFinished(delegate 
                {
                    if (!firstGeneration) ClientValues.ToggleRequireSaveManipulation(true);
                    Find.World.renderer.RegenerateAllLayersNow();
                    MemoryUtility.UnloadUnusedUnityAssets();
                    Current.CreatingWorld = null;
                    PostWorldGeneration();
                });
            }, "GeneratingWorld", doAsynchronously: true, null);
        }

        private static World GenerateWorld()
        {
            Rand.PushState(0);
            Current.CreatingWorld = new World();
            Logger.WriteToConsole($"Generating a world using the seed : {seedString}",LogMode.Message);
            Current.CreatingWorld.info.seedString = seedString;
            Current.CreatingWorld.info.persistentRandomValue = persistentRandomValue;
            Current.CreatingWorld.info.planetCoverage = planetCoverage;
            Current.CreatingWorld.info.overallRainfall = rainfall;
            Current.CreatingWorld.info.overallTemperature = temperature;
            Current.CreatingWorld.info.overallPopulation = population;
            Current.CreatingWorld.info.name = NameGenerator.GenerateName(RulePackDefOf.NamerWorld);
            Current.CreatingWorld.info.factions = factions;
            Current.CreatingWorld.info.pollution = pollution;

            //WorldGenStepDef[] worldGenSteps = GenStepsInOrder.ToArray();
            //worldGenSteps[0].worldGenStep = new RT_WorldGenStep_Terrain();

            WorldGenerationData.initializeGenerationDefs();
            WorldGenStepDef[] worldGenSteps = WorldGenerationData.WorldSyncSteps;
            Logger.WriteToConsole($"Steps count : {GenStepsInOrder.Count()}",LogMode.Message);

            foreach (WorldGenStepDef step in GenStepsInOrder){
                Logger.WriteToConsole($"step : {step.ToString()}",LogMode.Message);
            }
            for (int i = 0; i < worldGenSteps.Count(); i++)
            {
                worldGenSteps[i].worldGenStep.GenerateFresh(seedString);
            }

            Current.CreatingWorld.grid.StandardizeTileData();
            Current.CreatingWorld.FinalizeInit();
            Find.Scenario.PostWorldGenerate();

            if (!ModsConfig.IdeologyActive) Find.Scenario.PostIdeoChosen();
            return Current.CreatingWorld;
        }

        public static void SendWorldToServer()
        {
            WorldDetailsJSON worldDetailsJSON = new WorldDetailsJSON();
            worldDetailsJSON.worldStepMode = ((int)CommonEnumerators.WorldStepMode.Required).ToString();

            worldDetailsJSON.seedString = seedString;
            worldDetailsJSON.persistentRandomValue = persistentRandomValue;
            worldDetailsJSON.planetCoverage = planetCoverage.ToString();
            worldDetailsJSON.rainfall = ((int)rainfall).ToString();
            worldDetailsJSON.temperature = ((int)temperature).ToString();
            worldDetailsJSON.population = ((int)population).ToString();
            worldDetailsJSON.pollution = pollution.ToString();


            foreach (FactionDef factionDef in factions)
            {
                FactionDetails factionDetails = FactionScribeManager.factionToFactionDetails(factionDef);
                worldDetailsJSON.factions[factionDef.defName] = Serializer.ConvertObjectToBytes(factionDetails);
            }

            worldDetailsJSON = XmlParser.GetWorldXmlData(worldDetailsJSON);
            Logger.WriteToConsole(worldDetailsJSON.deflateDictionary[worldDetailsJSON.deflateDictionary.Keys.Last()],LogMode.Message);
            Packet packet = Packet.CreatePacketFromJSON(nameof(PacketHandler.WorldPacket), worldDetailsJSON);
            Network.listener.EnqueuePacket(packet);
        }

        public static void GetWorldFromServer()
        {
            XmlParser.ModifyWorldXml(cachedWorldDetails);

            Log.Message($"tile.count: {Find.WorldGrid.tiles.Count}\ntilecount: {Find.WorldGrid.TilesCount}");
            GameDataSaveLoader.LoadGame(SaveManager.customSaveName);
        }


        public static void PostWorldGeneration()
        {
            Page_SelectStartingSite newSelectStartingSite = new Page_SelectStartingSite();
            Page_ConfigureStartingPawns newConfigureStartingPawns = new Page_ConfigureStartingPawns();
            newConfigureStartingPawns.nextAct = PageUtility.InitGameStart;

            if (ModsConfig.IdeologyActive)
            {
                Page_ChooseIdeoPreset newChooseIdeoPreset = new Page_ChooseIdeoPreset();
                newChooseIdeoPreset.prev = newSelectStartingSite;
                newChooseIdeoPreset.next = newConfigureStartingPawns;

                newSelectStartingSite.next = newChooseIdeoPreset;
            }

            else
            {
                newSelectStartingSite.next = newConfigureStartingPawns;
                newConfigureStartingPawns.prev = newSelectStartingSite;
            }

            Find.WindowStack.Add(newSelectStartingSite);
            DialogShortcuts.ShowWorldGenerationDialogs();
        }
    }
}
