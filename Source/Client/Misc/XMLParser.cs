using HugsLib.Utils;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine.ParticleSystemJobs;
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

            foreach (XmlNode deflateNode in gridNode.ChildNodes)
            {
                worldDetailsJSON.deflateDictionary.Add(deflateNode.Name,deflateNode.InnerText);
            }
            
            XmlNode worldObjectsNode = GetChildNodeInNode(worldNode, "worldObjects");
            worldDetailsJSON.WorldObjects  = worldObjectsNode.InnerXml;
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

            Logs.Message("Startin for");
            //set each deflate in the player's save to the deflate from the server
            foreach (string deflateLabel in worldDeflates.Keys)
            {
                Logs.Message($"Setting deflate for {deflateLabel}");
                XmlNode deflateNode = gridNode[deflateLabel];
                Logs.Message($"Resulting deflate is {((deflateNode == null) ? "null":"copied")}");
                //if the player does not have that label, don't attempt to add it
                if (deflateNode != null)
                {
                    gridNode[deflateLabel].InnerText = worldDeflates[deflateLabel];
                }
            }


            XmlNode worldObjectsNode = GetChildNodeInNode(worldNode, "worldObjects");
            worldObjectsNode.InnerXml = worldDetailsJSON.WorldObjects;
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