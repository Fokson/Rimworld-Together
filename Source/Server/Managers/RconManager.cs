using Shared;
using static GameServer.Logger;

namespace GameServer
{
    public static class RconManager
    {


        public static void handleRconCommand(Packet packet)
        {
            ConsoleCommandDetails consoleCommandDetails = (ConsoleCommandDetails)Serializer.ConvertBytesToObject(packet.contents);
            ServerCommandManager.ParseServerCommands(consoleCommandDetails.UnparsedCommand);
        }

        public static void sendConsoleMirror(string text, LogMode color)
        {
            ConsoleMirrorDetails consoleMirrorDetails = new ConsoleMirrorDetails();
            consoleMirrorDetails.ConsoleColor = (int)color;
            consoleMirrorDetails.ConsoleText = text;

            byte[] contents = Serializer.ConvertObjectToBytes(consoleMirrorDetails);

            Packet packet = Packet.CreatePacketFromJSON(nameof(consoleMirrorDetails),contents);

            foreach (RconClient rconClient in Network.connectedRcons)
            {
                rconClient.listener.dataQueue.Enqueue(packet);
            }

        }

    }
}
