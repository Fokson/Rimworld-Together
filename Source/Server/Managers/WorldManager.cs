using Shared;
using static Shared.CommonEnumerators;

namespace GameServer
{
    public static class WorldManager
    {
        private static string worldFileName = "WorldValues.json";

        private static string worldFilePath = Path.Combine(Master.corePath, worldFileName);

        public static void ParseWorldPacket(ServerClient client, Packet packet)
        {
            WorldDetailsJSON worldDetailsJSON = (WorldDetailsJSON)Serializer.ConvertBytesToObject(packet.contents);

            switch (int.Parse(worldDetailsJSON.worldStepMode))
            {
                case (int)CommonEnumerators.WorldStepMode.Required:
                    SaveWorldPrefab(client, worldDetailsJSON);
                    break;

                case (int)CommonEnumerators.WorldStepMode.Existing:
                    //Do nothing
                    break;
            }
        }

        public static bool CheckIfWorldExists() { return File.Exists(worldFilePath); }

        public static void SaveWorldPrefab(ServerClient client, WorldDetailsJSON worldDetailsJSON)
        {
            Master.worldValues = worldDetailsJSON;
            Serializer.SerializeToFile(worldFilePath, Master.worldValues);
            Logger.WriteToConsole($"[Save world] > {client.username}", LogMode.Title);
        }

        public static void RequireWorldFile(ServerClient client)
        {
            WorldDetailsJSON worldDetailsJSON = new WorldDetailsJSON();
            worldDetailsJSON.worldStepMode = ((int)CommonEnumerators.WorldStepMode.Required).ToString();

            Packet packet = Packet.CreatePacketFromJSON(nameof(PacketHandler.WorldPacket), worldDetailsJSON);
            client.listener.EnqueuePacket(packet);
        }

        public static void SendWorldFile(ServerClient client)
        {

            WorldDetailsJSON worldDetailsJSON = Master.worldValues;
            worldDetailsJSON.worldStepMode = ((int)CommonEnumerators.WorldStepMode.Existing).ToString();

            Packet packet = Packet.CreatePacketFromJSON(nameof(PacketHandler.WorldPacket), worldDetailsJSON);
            client.listener.EnqueuePacket(packet);
        }

        public static void LoadWorldFile()
        {
            if (File.Exists(worldFilePath))
            {
                Master.worldValues = Serializer.SerializeFromFile<WorldDetailsJSON>(worldFilePath);

                Logger.WriteToConsole("Loaded world values", LogMode.Warning);
            }

            else Logger.WriteToConsole("[Warning] > World is missing. Join server to create it", LogMode.Warning);   
        }
    }
}
