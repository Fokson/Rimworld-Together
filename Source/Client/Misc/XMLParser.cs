using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Verse;

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

            foreach (XmlNode deflateNode in docNode.ChildNodes)
            {
                worldDetailsJSON.deflateDictionary.Add(deflateNode.Name,deflateNode.InnerText);
            }

            return worldDetailsJSON;
        }

        //Modifies the existing XML file with the required details from the server

        public static void ModifyWorldXml(WorldDetailsJSON worldDetailsJSON)
        {
            string filePath = Path.Combine(new string[] { Master.savesFolderPath, SaveManager.customSaveName + ".rws" });

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            //Navigate to the grid in the xml file
            XmlNode docNode = GetChildNodeInNode(doc, "savegame");
            XmlNode gameNode = GetChildNodeInNode(docNode, "game");
            XmlNode worldNode = GetChildNodeInNode(gameNode, "world");
            XmlNode gridNode = GetChildNodeInNode(worldNode, "grid");

            Dictionary<string, string> worldDeflates = worldDetailsJSON.deflateDictionary;

            //set each deflate in the player's save to the deflate from the server
            foreach (string deflateLabel in worldDeflates.Keys)
            {
                XmlNode deflateNode = gridNode[deflateLabel];

                //if the player does not have that label, don't attempt to add it
                if (deflateNode != null)
                {
                    gridNode[deflateLabel].InnerText = worldDeflates[deflateLabel];
                }
            }
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