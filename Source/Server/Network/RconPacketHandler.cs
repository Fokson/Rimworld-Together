﻿using Shared;
using System.Reflection;

namespace GameServer
{
    public static class RconPacketHandler
    {


        //Function that opens handles the action that the packet should do, then sends it to the correct one below

        public static void HandlePacket(RconClient client, Packet packet)
        {
            if (client.password != Master.serverConfig.RconPassword) return;
            if (Master.serverConfig.verboseLogs) Logger.WriteToConsole($"[Header] > {packet.header}");

            Type toUse = typeof(RconPacketHandler);
            MethodInfo methodInfo = toUse.GetMethod(packet.header);
            methodInfo.Invoke(packet.header, new object[] { client, packet });
        }

        public static void RecieveRemoteCommandPacket(RconClient client, Packet packet)
        {
            RconManager.handleRconCommand(packet);
        }

        public static void RecieveConsoleMirrorPacket(RconClient client, Packet packet)
        {
            //Do nothing
        }

    }
}
