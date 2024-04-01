using Shared;
using System.Reflection;

namespace Rcon
{
    //Class that handles the management of all the received packets

    public static class RconPacketHandler
    {
        //Function that opens handles the action that the packet should do, then sends it to the correct one below

        public static void HandlePacket(RconClient client, Packet packet)
        {

            Type toUse = typeof(RconPacketHandler);
            MethodInfo methodInfo = toUse.GetMethod(packet.header);
            methodInfo.Invoke(packet.header, new object[] { client, packet });
        }

        public static void KeepAlivePacket(RconClient client, Packet packet)
        {
            client.listener.KAFlag = true;
        }

        public static void RecieveConsoleMirrorPacket(RconClient client, Packet packet)
        {
            RconManager.RecieveConsoleMirror(packet);
        }

    }
}
