using HugsLib.Utils;
using Shared;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
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
            worldObjectsNode = GetChildNodeInNode(worldObjectsNode, "worldObjects");

            worldDetailsJSON.WorldObjects  = worldObjectsNode.InnerXml;
            return worldDetailsJSON;
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
            XmlNode playerWorldObjects = GetChildNodeInNode(worldNode, "worldObjects");
            XmlNode ServerWorldObjects = new XmlDocument();
            ServerWorldObjects.InnerXml = worldDetailsJSON.WorldObjects;
            //foreach server object
            foreach (XmlNode ServerNode in ServerWorldObjects.ChildNodes)
            {
                //find the player object with the same ID as the server Object
                foreach (XmlNode playerNode in playerWorldObjects.ChildNodes)
                {
                    Logger.WriteToConsole($"IDs are {GetChildNodeInNode(playerNode, "ID")} : {GetChildNodeInNode(ServerNode, "ID")}",LogMode.Message);
                    Logger.WriteToConsole($"Inner xml: {playerNode.InnerXml}", LogMode.Message);
                    if (GetChildNodeInNode(playerNode, "ID") == GetChildNodeInNode(ServerNode, "ID"))
                    {
                        Logger.WriteToConsole($"Ids match : {GetChildNodeInNode(playerNode, "ID").InnerText}",LogMode.Message);
                        playerNode.InnerXml = ServerNode.InnerXml;
                        break;
                    }
                }


            }
            ServerWorldObjects.InnerXml = worldDetailsJSON.WorldObjects;
            playerWorldObjects.InnerXml = worldDetailsJSON.WorldObjects;
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