﻿using Shared;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    //Main class that is used to handle the connection with the clients

    public static class Network
    {
        //IP and Port that the connection will be bound to
        private static IPAddress localAddress = IPAddress.Parse(Master.serverConfig.IP);
        private static int port = int.Parse(Master.serverConfig.Port);
        private static int rconPort = int.Parse(Master.serverConfig.RconPort);
        private static bool AllowRconConnections = Master.serverConfig.allowRconConnections;

        //TCP listener that will handle the connection with the clients, and list of currently connected clients
        private static TcpListener connection;
        private static TcpListener rconConnection;
        public static List<ServerClient> connectedClients = new List<ServerClient>();
        public static List<RconClient> connectedRcons = new List<RconClient>();
        //Entry point function of the network class

        public static void ReadyServer()
        {
            connection = new TcpListener(localAddress, port);
            connection.Start();

            Threader.GenerateServerThread(Threader.ServerMode.Sites);

            Logger.WriteToConsole("Type 'help' to get a list of available commands", Logger.LogMode.Warning);
            Logger.WriteToConsole($"Listening for users at {localAddress}:{port}", Logger.LogMode.Warning);
            Logger.WriteToConsole("Server launched", Logger.LogMode.Warning);
            Master.ChangeTitle();


            while (true) ListenForIncomingUsers();
        }

        //Entry Point function for Rcon connections

        public static void ReadyRcon()
        {

            if (!AllowRconConnections) return;

            rconConnection = new TcpListener(localAddress, rconPort);
            rconConnection.Start();


            Logger.WriteToConsole("Ready for Remote Console Connections", Logger.LogMode.Warning);

            while (true) ListenForIncomingRcons();
        }

        //Listens for any user that might connect and executes all required tasks  with it

        private static void ListenForIncomingUsers()
        {
            //Wait for a user to try and connect
            TcpClient newTCP = connection.AcceptTcpClient();


            //Initialize everything needed when a user tries to connect
            ServerClient newServerClient = new ServerClient(newTCP);
            Listener newListener = new Listener(newServerClient, newTCP);
            newServerClient.listener = newListener;

            Threader.GenerateClientThread(newServerClient.listener, Threader.ClientMode.Listener);
            Threader.GenerateClientThread(newServerClient.listener, Threader.ClientMode.Sender);
            Threader.GenerateClientThread(newServerClient.listener, Threader.ClientMode.Health);
            Threader.GenerateClientThread(newServerClient.listener, Threader.ClientMode.KAFlag);

            if (Master.isClosing) newServerClient.listener.disconnectFlag = true;
            else if (Master.worldValues == null && connectedClients.Count() > 0) UserManager.SendLoginResponse(newServerClient, CommonEnumerators.LoginResponse.NoWorld);
            else
            {
                if (connectedClients.ToArray().Count() >= int.Parse(Master.serverConfig.MaxPlayers))
                {
                    UserManager.SendLoginResponse(newServerClient, CommonEnumerators.LoginResponse.ServerFull);
                    Logger.WriteToConsole($"[Warning] > Server Full", Logger.LogMode.Warning);
                }

                else
                {
                    connectedClients.Add(newServerClient);

                    Master.ChangeTitle();

                    Logger.WriteToConsole($"[Connect] > {newServerClient.username} | {newServerClient.SavedIP}");
                }
            }
        }

        //Listens for any Remote Consoles that might connect and executes all required tasks with it

        private static void ListenForIncomingRcons()
        {
            //Wait for a user to try and connect
            TcpClient newTCP = connection.AcceptTcpClient();


            //Initialize everything needed when a user tries to connect
            RconClient newRconClient = new RconClient(newTCP);
            RconListener newListener = new RconListener(newRconClient, newTCP);
            newRconClient.listener = newListener;

            Task.Run(newRconClient.listener.Listen);

            if (Master.isClosing) newRconClient.listener.disconnectFlag = true;
            else
            {
                if (connectedClients.ToArray().Count() >= int.Parse(Master.serverConfig.MaxRcons))
                {
                    Logger.WriteToConsole($"[Warning] > Server Full", Logger.LogMode.Warning);
                }

                else
                {
                    connectedRcons.Add(newRconClient);

                    Master.ChangeTitle();

                    Logger.WriteToConsole($"[Connect Rcon] > {newRconClient.username} | {newRconClient.SavedIP}");
                }
            }
        }


        //Kicks specified client from the server

        public static void KickClient(ServerClient client)
        {
            try
            {
                connectedClients.Remove(client);
                client.listener.DestroyConnection();

                UserManager.SendPlayerRecount();

                Master.ChangeTitle();

                Logger.WriteToConsole($"[Disconnect] > {client.username} | {client.SavedIP}");
            }

            catch
            {
                Logger.WriteToConsole($"Error disconnecting user {client.username}, this will cause memory overhead", Logger.LogMode.Warning);
            }
        }

        public static void KickRcon(RconClient rconClient)
        {
            try
            {
                connectedRcons.Remove(rconClient);
                rconClient.listener.DestroyConnection();
                
                Master.ChangeTitle();

                Logger.WriteToConsole($"[Disconnect Rcon] > {rconClient.username} | {rconClient.SavedIP}");
            }

            catch
            {
                Logger.WriteToConsole($"Error disconnecting user {rconClient.username}, this will cause memory overhead", Logger.LogMode.Warning);
            }


        }
    }
}