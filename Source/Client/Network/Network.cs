﻿using System;
using System.Net.Sockets;
using System.Threading;
using RimworldTogether.GameClient.Core;
using RimworldTogether.GameClient.Dialogs;
using RimworldTogether.GameClient.Managers;
using RimworldTogether.GameClient.Managers.Actions;
using RimworldTogether.GameClient.Misc;
using RimworldTogether.GameClient.Network.Listener;
using RimworldTogether.GameClient.Patches;
using RimworldTogether.GameClient.Values;
using RimworldTogether.Shared.JSON;
using RimworldTogether.Shared.Network;
using Verse;

namespace RimworldTogether.GameClient.Network
{
    public static class Network
    {
        public static ServerListener serverListener;
        public static string ip = "";
        public static string port = "";

        public static bool isConnectedToServer;
        public static bool isTryingToConnect;

        public static void StartConnection()
        {
            Action toDo;

            if (TryConnectToServer())
            {
                toDo = delegate
                {
                    DialogManager.PopWaitDialog();

                    ClientValues.ManageDevOptions();

                    SiteManager.SetSiteDefs();
                };
                Main.threadDispatcher.Enqueue(toDo);

                Threader.GenerateThread(Threader.Mode.Health);
                Threader.GenerateThread(Threader.Mode.KASender);

                serverListener.ListenToServer();
            }

            else
            {
                toDo = delegate
                {
                    DialogManager.PopWaitDialog();

                    RT_Dialog_Error d1 = new RT_Dialog_Error("The server did not respond in time");
                    DialogManager.PushNewDialog(d1);
                };
                Main.threadDispatcher.Enqueue(toDo);

                ClearAllValues();
            }
        }

        private static bool TryConnectToServer()
        {
            if (isTryingToConnect || isConnectedToServer) return false;
            else
            {
                isTryingToConnect = true;

                isConnectedToServer = true;

                serverListener = new ServerListener(new(ip, int.Parse(port)));

                return true;
            }
        }

        public static void DisconnectFromServer()
        {
            Action toDo = delegate
            {
                serverListener.connection.Dispose();

                Action r1 = delegate
                {
                    if (Current.ProgramState == ProgramState.Playing)
                    {
                        DisconnectionManager.DisconnectToMenu();
                    }
                };

                DialogManager.PushNewDialog(new RT_Dialog_Error_Loop(new string[]
                {
                        "Connection to the server has been lost!",
                        "Game will now quit to menu"
                }, r1));

                ClearAllValues();
            };

            Main.threadDispatcher.Enqueue(toDo);
        }

        public static void ClearAllValues()
        {
            isTryingToConnect = false;
            isConnectedToServer = false;

            ClientValues.CleanValues();
            ServerValues.CleanValues();
            ChatManager.ClearChat();
        }
    }
}
