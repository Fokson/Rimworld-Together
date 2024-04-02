using HugsLib.Utils;
using Shared;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using Verse;
using static Shared.CommonEnumerators;

namespace GameClient
{
    public static class XmlParser
    {
        //  Gets each deflate from the player's world save
        //  This is typically used once during server creation
        public static WorldDetailsJSON GetWorldXmlData(WorldDetailsJSON worldDetailsJSON)
        {
            string filePath = Path.Combine(new string[] { Master.savesFolderPath, SaveManager.customSaveName + ".rws" });

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            //Navigate to the grid in the xml file
            XmlNode docNode = GetChildNodeInNode(doc, "savegame");
            XmlNode gameNode = GetChildNodeInNode(docNode, "game");
            XmlNode worldNode = GetChildNodeInNode(gameNode, "world");
            XmlNode gridNode = GetChildNodeInNode(worldNode, "grid");



            foreach (XmlNode deflateNode in gridNode.ChildNodes)
            {
                worldDetailsJSON.deflateDictionary.Add(deflateNode.Name,deflateNode.InnerText);
            }

            XmlNode worldObjectsNode = GetChildNodeInNode(worldNode, "worldObjects");

            worldDetailsJSON.WorldObjects  = worldObjectsNode.InnerXml;
            return worldDetailsJSON;
        }

        public static TileData parseGrid(WorldDetailsJSON worldDetailsJSON)
        {
            TileData tileData = new TileData();

            tileData.tileBiome = CompressUtility.Decompress(Convert.FromBase64String(DataExposeUtility.RemoveLineBreaks(worldDetailsJSON.deflateDictionary["tileBiomeDeflate"])));

            tileData.tileElevation = CompressUtility.Decompress(Convert.FromBase64String(DataExposeUtility.RemoveLineBreaks(worldDetailsJSON.deflateDictionary["tileElevationDeflate"])));

            tileData.tileHilliness = CompressUtility.Decompress(Convert.FromBase64String(DataExposeUtility.RemoveLineBreaks(worldDetailsJSON.deflateDictionary["tileHillinessDeflate"])));

            tileData.tileTemperature = CompressUtility.Decompress(Convert.FromBase64String(DataExposeUtility.RemoveLineBreaks(worldDetailsJSON.deflateDictionary["tileTemperatureDeflate"])));

            tileData.tileRainfall = CompressUtility.Decompress(Convert.FromBase64String(DataExposeUtility.RemoveLineBreaks(worldDetailsJSON.deflateDictionary["tileRainfallDeflate"])));

            tileData.tileSwampiness = CompressUtility.Decompress(Convert.FromBase64String(DataExposeUtility.RemoveLineBreaks(worldDetailsJSON.deflateDictionary["tileSwampinessDeflate"])));

            tileData.tileFeature = CompressUtility.Decompress(Convert.FromBase64String(DataExposeUtility.RemoveLineBreaks(worldDetailsJSON.deflateDictionary["tileFeatureDeflate"])));

            tileData.tileRoadOrigins = CompressUtility.Decompress(Convert.FromBase64String(DataExposeUtility.RemoveLineBreaks(worldDetailsJSON.deflateDictionary["tileRoadOriginsDeflate"])));

            tileData.tileRoadAdjacency = CompressUtility.Decompress(Convert.FromBase64String(DataExposeUtility.RemoveLineBreaks(worldDetailsJSON.deflateDictionary["tileRoadAdjacencyDeflate"])));

            tileData.tileRoadDef = CompressUtility.Decompress(Convert.FromBase64String(DataExposeUtility.RemoveLineBreaks(worldDetailsJSON.deflateDictionary["tileRoadDefDeflate"])));

            tileData.tileRiverOrigins = CompressUtility.Decompress(Convert.FromBase64String(DataExposeUtility.RemoveLineBreaks(worldDetailsJSON.deflateDictionary["tileRiverOriginsDeflate"])));

            tileData.tileRiverAdjacency = CompressUtility.Decompress(Convert.FromBase64String(DataExposeUtility.RemoveLineBreaks(worldDetailsJSON.deflateDictionary["tileRiverAdjacencyDeflate"])));

            tileData.tileRiverDef = CompressUtility.Decompress(Convert.FromBase64String(DataExposeUtility.RemoveLineBreaks(worldDetailsJSON.deflateDictionary["tileRiverDefDeflate"])));

            return tileData;
        }

        //Modifies the existing XML file with the required details from the server

        public static void ModifyWorldXml(WorldDetailsJSON worldDetailsJSON)
        {
            string filePath = Path.Combine(new string[] { Master.savesFolderPath, SaveManager.customSaveName + ".rws" });

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            //Navigate to grid in the xml file
            XmlNode docNode = GetChildNodeInNode(doc, "savegame");
            XmlNode gameNode = GetChildNodeInNode(docNode, "game");
            XmlNode worldNode = GetChildNodeInNode(gameNode, "world");
            XmlNode gridNode = GetChildNodeInNode(worldNode, "grid");

            //World Deflates are the layers of the world generation. Save them in a dictionary
            Dictionary<string, string> worldDeflates = worldDetailsJSON.deflateDictionary;

            //set each deflate in the player's save to the deflate from the server
            foreach (string deflateLabel in worldDeflates.Keys)
            {
                XmlNode deflateNode = gridNode[deflateLabel];
                Logger.WriteToConsole($"{((deflateNode != null) ? ($"deflate node {deflateLabel} exists"): ($"deflate node {deflateLabel} was not found"))})", LogMode.Message);

                //if the player does not have that deflate, don't attempt to add it (it means its from a mod they dont have)
                if (deflateNode != null)
                {
                    gridNode[deflateLabel].InnerText = worldDeflates[deflateLabel];
                }
            }

            
            // replace every world object with the server copy of the world object
            // this ensures the objects are in the correct location with the correct settings.
            // Objects that only exist on the player's world will not be changed

            //grab player objects
            XmlNode localWorldObjects = GetChildNodeInNode(worldNode, "worldObjects");
            localWorldObjects = GetChildNodeInNode(localWorldObjects, "worldObjects");

            //grab server o
            XmlNode ServerWorldObjectsDoc = new XmlDocument();
            ServerWorldObjectsDoc.InnerXml = worldDetailsJSON.WorldObjects;
            XmlNode ServerWorldObjects = GetChildNodeInNode(ServerWorldObjectsDoc, "worldObjects");


            //foreach server object
            foreach (XmlNode ServerNode in ServerWorldObjects.ChildNodes)
            {
                //find the player object with the same ID as the server Object
                foreach (XmlNode playerNode in localWorldObjects.ChildNodes)
                {
                    if (GetChildNodeInNode(playerNode, "ID").InnerText == GetChildNodeInNode(ServerNode, "ID").InnerText)
                    {
                        playerNode.InnerXml = ServerNode.InnerXml;
                        break;
                    }
                }


            }


            doc.Save(filePath);
        }


        //Gets a specific child inside of the specified node's children

        private static XmlNode GetChildNodeInNode(XmlNode node, string targetName)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == targetName) return child;
            }

            return null;
        }
    }
}