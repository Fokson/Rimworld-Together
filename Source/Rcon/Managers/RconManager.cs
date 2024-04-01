using Shared;
using static Rcon.Logger;

namespace Rcon
{
    public static class RconManager
    {
        public static void RecieveConsoleMirror(Packet packet)
        {
            ConsoleMirrorDetails consoleMirrorDetails = (ConsoleMirrorDetails)Serializer.ConvertBytesToObject(packet.contents);
            Logger.WriteMirrorToConsole(consoleMirrorDetails.ConsoleText, (LogMode)consoleMirrorDetails.ConsoleColor);


        }


    }
}
