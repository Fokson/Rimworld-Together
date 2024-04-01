using Shared;
using System.Reflection;

namespace GameServer
{
    internal class RconPacketHandler
    {


        //Function that opens handles the action that the packet should do, then sends it to the correct one below

        public static void HandlePacket(ServerClient client, Packet packet)
        {
            if (Master.serverConfig.verboseLogs) Logger.WriteToConsole($"[Header] > {packet.header}");

            Type toUse = typeof(RconPacketHandler);
            MethodInfo methodInfo = toUse.GetMethod(packet.header);
            methodInfo.Invoke(packet.header, new object[] { client, packet });
        }


    }
}
