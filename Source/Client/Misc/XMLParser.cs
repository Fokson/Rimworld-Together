using Shared;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using Verse;

namespace GameClient
{
    public static class XmlParser
    {
        //Gets all the required details of the world from the save XML file
        //The data MUST BE ACCESSED IN PERFECT ORDER DUE TO XML LIMITATIONS

        public static WorldDetailsJSON GetWorldXmlData(WorldDetailsJSON worldDetailsJSON)
        {
            string filePath = Path.Combine(new string[] { Master.savesFolderPath, SaveManager.customSaveName + ".rws" });
            XmlReader reader = XmlReader.Create(filePath);
            XmlReader print = XmlReader.Create(filePath);
            print.Read();
            try { Log.Message(print.ReadContentAsString()); }
            catch (Exception e) { Log.Error(e.ToString()); }

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNode docNode = GetChildNodeInNode(doc, "savegame");
            XmlNode gameNode = GetChildNodeInNode(docNode, "game");
            XmlNode worldNode = GetChildNodeInNode(gameNode, "world");
            XmlNode gridNode = GetChildNodeInNode(worldNode, "grid");

            worldDetailsJSON.tileBiomeDeflate =             gridNode["tileBiomeDeflate"].InnerText; 
            worldDetailsJSON.tileElevationDeflate =         gridNode["tileElevationDeflate"].InnerText;
            worldDetailsJSON.tileHillinessDeflate =         gridNode["tileHillinessDeflate"].InnerText;
            worldDetailsJSON.tileTemperatureDeflate =       gridNode["tileTemperatureDeflate"].InnerText;
            worldDetailsJSON.tileRainfallDeflate =          gridNode["tileRainfallDeflate"].InnerText;
            worldDetailsJSON.tileSwampinessDeflate =        gridNode["tileSwampinessDeflate"].InnerText;
            worldDetailsJSON.tileFeatureDeflate =           gridNode["tileFeatureDeflate"].InnerText;
            worldDetailsJSON.tilePollutionDeflate =         gridNode["tilePollutionDeflate"].InnerText;
            worldDetailsJSON.tileRoadOriginsDeflate =       gridNode["tileRoadOriginsDeflate"].InnerText;
            worldDetailsJSON.tileRoadAdjacencyDeflate =     gridNode["tileRoadAdjacencyDeflate"].InnerText;
            worldDetailsJSON.tileRoadDefDeflate =           gridNode["tileRoadDefDeflate"].InnerText;
            worldDetailsJSON.tileRiverOriginsDeflate =      gridNode["tileRiverOriginsDeflate"].InnerText;
            worldDetailsJSON.tileRiverAdjacencyDeflate =    gridNode["tileRiverAdjacencyDeflate"].InnerText;
            worldDetailsJSON.tileRiverDefDeflate =          gridNode["tileRiverDefDeflate"].InnerText;

            reader.Close();

            return worldDetailsJSON;
        }

        //Gets the data from the specified XML element name

        public static string GetDataFromXml(XmlNode node, string elementName)
        {
            string dataToReturn = "";

            try
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.Name == elementName)
                    {
                        Log.Message("In if");
                        dataToReturn = child.InnerText;
                        break;
                    }
                }
            }
            catch (Exception e)
            {

                Log.Message(e.ToString());
            }
            Log.Message($"Data : {dataToReturn}");
            return dataToReturn;
        }

        //Modifies the existing XML file with the required details from the server

        public static void ModifyWorldXml(WorldDetailsJSON worldDetailsJSON)
        {
            string path = Path.Combine(new string[] { Master.savesFolderPath, SaveManager.customSaveName + ".rws" });

            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNode docNode = GetChildNodeInNode(doc, "savegame");
            XmlNode gameNode = GetChildNodeInNode(docNode, "game");
            XmlNode worldNode = GetChildNodeInNode(gameNode, "world");
            XmlNode gridNode = GetChildNodeInNode(worldNode, "grid");
            Log.Message(doc.ToString());
            SetDataIntoXML(gridNode, "tileBiomeDeflate", worldDetailsJSON.tileBiomeDeflate);
            SetDataIntoXML(gridNode, "tileElevationDeflate", worldDetailsJSON.tileElevationDeflate);
            SetDataIntoXML(gridNode, "tileHillinessDeflate", worldDetailsJSON.tileHillinessDeflate);
            SetDataIntoXML(gridNode, "tileTemperatureDeflate", worldDetailsJSON.tileTemperatureDeflate);
            SetDataIntoXML(gridNode, "tileRainfallDeflate", worldDetailsJSON.tileRainfallDeflate);
            SetDataIntoXML(gridNode, "tileSwampinessDeflate", worldDetailsJSON.tileSwampinessDeflate);
            SetDataIntoXML(gridNode, "tileFeatureDeflate", worldDetailsJSON.tileFeatureDeflate);
            SetDataIntoXML(gridNode, "tilePollutionDeflate", worldDetailsJSON.tilePollutionDeflate);
            SetDataIntoXML(gridNode, "tileRoadOriginsDeflate", worldDetailsJSON.tileRoadOriginsDeflate);
            SetDataIntoXML(gridNode, "tileRoadAdjacencyDeflate", worldDetailsJSON.tileRoadAdjacencyDeflate);
            SetDataIntoXML(gridNode, "tileRoadDefDeflate", worldDetailsJSON.tileRoadDefDeflate);
            SetDataIntoXML(gridNode, "tileRiverOriginsDeflate", worldDetailsJSON.tileRiverOriginsDeflate);
            SetDataIntoXML(gridNode, "tileRiverAdjacencyDeflate", worldDetailsJSON.tileRiverAdjacencyDeflate);
            Log.Message(worldDetailsJSON.tileRiverDefDeflate);

            Log.Message($"Replaced : {worldDetailsJSON.tileRiverDefDeflate}");
            SetDataIntoXML(gridNode, "tileRiverDefDeflate", worldDetailsJSON.tileRiverDefDeflate);
            string deflate = "";
            foreach (XmlNode child in gridNode.ChildNodes)
            {
                Log.Message($"{child.Name.ToString()}");
                if (child.Name == "tileRiverDefDeflate")
                {
                    deflate = child.InnerText;
                    break;
                }
            }

            if (deflate.Count() > 120) { Log.Message($"tileRiverDefDeflate after setting:\n{deflate.Substring(0, 100)}"); }
            else Log.Message($"tileRiver after:\n{deflate}");

            doc.Save(path);

        }

        //Sets the data of the specified XML node

        public static void SetDataIntoXML(XmlNode gridNode, string elementName, string replacement)
        {
            try
            {
                if (replacement.Count() > 120) { Log.Message($"First 100 characters of deflate:\n{replacement.Substring(0, 100)}"); }
                else Log.Message($"First 100 characters of deflate:\n{replacement}");
            }
            catch (Exception e)
            {
                Log.Message(e.ToString());
            }

            int howMantReturns = ((replacement.Length - 1) / 100);
            Log.Message($"Needs {howMantReturns} Inserts");
            int index = 100;
            replacement = "\n" + replacement;
            for (int i = 0; i < howMantReturns; i++)
            {
                replacement = replacement.Substring(0, index) + "\n" + replacement.Substring(index);
                index += 101;
            }
            if (replacement.Length > 120)
                Log.Message($"At 100 : {replacement[100] == '\n'}");
            Log.Message($"Found {replacement.Contains("\n")}");


            try
            {
                foreach (XmlNode child in gridNode.ChildNodes)
                {
                    if (child.Name == elementName)
                    {
                        child.InnerText = replacement;
                        break;
                    }
                }
            }
            catch (Exception e)
            {

                Log.Message(e.ToString());
            }
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