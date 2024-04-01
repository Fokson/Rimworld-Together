using Shared;
using System.Reflection;

namespace Rcon
{
    //Class that handles the management of all the received packets

    public static class PacketHandler
    {
        //Function that opens handles the action that the packet should do, then sends it to the correct one below

        public static void HandlePacket(RconClient client, Packet packet)
        {
            if (Master.serverConfig.verboseLogs) Logger.WriteToConsole($"[Header] > {packet.header}");

            Type toUse = typeof(PacketHandler);
            MethodInfo methodInfo = toUse.GetMethod(packet.header);
            methodInfo.Invoke(packet.header, new object[] { client, packet });
        }

        public static void KeepAlivePacket(RconClient client, Packet packet)
        {
            client.listener.KAFlag = true;
        }

        public static void LoginClientPacket(RconClient client, Packet packet)
        {
            UserLogin.TryLoginUser(client, packet);
        }

        public static void RegisterClientPacket(RconClient client, Packet packet)
        {
            UserRegister.TryRegisterUser(client, packet);
        }

        public static void RequestSavePartPacket(RconClient client, Packet packet)
        {
            SaveManager.SendSavePartToClient(client);
        }

        public static void ReceiveSavePartPacket(RconClient client, Packet packet)
        {
            SaveManager.ReceiveSavePartFromClient(client, packet);
        }

        public static void LikelihoodPacket(RconClient client, Packet packet)
        {
            LikelihoodManager.ChangeUserLikelihoods(client, packet);
        }

        public static void TransferPacket(RconClient client, Packet packet)
        {
            TransferManager.ParseTransferPacket(client, packet);
        }

        public static void SitePacket(RconClient client, Packet packet)
        {
            SiteManager.ParseSitePacket(client, packet);
        }

        public static void VisitPacket(RconClient client, Packet packet)
        {
            OnlineVisitManager.ParseVisitPacket(client, packet);
        }

        public static void OfflineVisitPacket(RconClient client, Packet packet)
        {
            OfflineVisitManager.ParseOfflineVisitPacket(client, packet);
        }

        public static void ChatPacket(RconClient client, Packet packet)
        {
            ChatManager.ParseClientMessages(client, packet);
        }

        public static void FactionPacket(RconClient client, Packet packet)
        {
            OnlineFactionManager.ParseFactionPacket(client, packet);
        }

        public static void MapPacket(RconClient client, Packet packet)
        {
            MapManager.SaveUserMap(client, packet);
        }

        public static void RaidPacket(RconClient client, Packet packet)
        {
            OfflineRaidManager.ParseRaidPacket(client, packet);
        }

        public static void SpyPacket(RconClient client, Packet packet)
        {
            OfflineSpyManager.ParseSpyPacket(client, packet);
        }

        public static void SettlementPacket(RconClient client, Packet packet)
        {
            SettlementManager.ParseSettlementPacket(client, packet);
        }

        public static void EventPacket(RconClient client, Packet packet)
        {
            EventManager.ParseEventPacket(client, packet);
        }

        public static void WorldPacket(RconClient client, Packet packet)
        {
            WorldManager.ParseWorldPacket(client, packet);
        }

        public static void CustomDifficultyPacket(RconClient client, Packet packet)
        {
            CustomDifficultyManager.ParseDifficultyPacket(client, packet);
        }

        public static void ResetSavePacket(RconClient client, Packet packet)
        {
            SaveManager.ResetClientSave(client);
        }

        //Empty functions

        public static void UserUnavailablePacket()
        {
            //Empty
        }

        public static void IllegalActionPacket()
        {
            //Empty
        }

        public static void BreakPacket()
        {
            //Empty
        }

        public static void PlayerRecountPacket()
        {
            //Empty
        }

        public static void ServerValuesPacket()
        {
            //Empty
        }

        public static void CommandPacket()
        {
            //Empty
        }

        public static void LoginResponsePacket()
        {
            //Empty
        }
    }
}
