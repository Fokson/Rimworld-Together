﻿using Shared;
using System;

namespace GameClient
{
    //Class that handles loging responses from the server

    public static class LoginManager
    {
        //Parses the received packet into an order

        public static void ReceiveLoginResponse(Packet packet)
        {
            Logs.Message("[Rimworld Together] > Recieved login Response");

            JoinDetailsJSON loginDetailsJSON = (JoinDetailsJSON)Serializer.ConvertBytesToObject(packet.contents);

            Action stopAndClear = delegate{
                Network.listener.disconnectFlag = true;
                DialogManager.clearStack();};

            switch(int.Parse(loginDetailsJSON.tryResponse))
            {
                case (int)CommonEnumerators.LoginResponse.InvalidLogin:
                    DialogManager.PushNewDialog(new RT_Dialog_Error("Login details are invalid! Please try again!", stopAndClear));
                    break;

                case (int)CommonEnumerators.LoginResponse.BannedLogin:
                    DialogManager.PushNewDialog(new RT_Dialog_Error("You are banned from this server!", stopAndClear));
                    break;

                case (int)CommonEnumerators.LoginResponse.RegisterSuccess:
                    DialogShortcuts.ShowRegisteredDialog();
                    break;

                case (int)CommonEnumerators.LoginResponse.RegisterInUse:
                    DialogManager.PushNewDialog(new RT_Dialog_Error("That username is already in use! Please try again!", stopAndClear));
                    break;

                case (int)CommonEnumerators.LoginResponse.RegisterError:
                    DialogManager.PushNewDialog(new RT_Dialog_Error("There was an error registering! Please try again!", stopAndClear));
                    break;

                case (int)CommonEnumerators.LoginResponse.ExtraLogin:
                    DialogManager.PushNewDialog(new RT_Dialog_Error("You connected from another place!", stopAndClear));
                    break;

                case (int)CommonEnumerators.LoginResponse.WrongMods:
                    ModManager.GetConflictingMods(packet);
                    break;

                case (int)CommonEnumerators.LoginResponse.ServerFull:
                    DialogManager.PopDialog();
                    DialogManager.PushNewDialog(new RT_Dialog_Error("Server is full!", stopAndClear));
                    break;

                case (int)CommonEnumerators.LoginResponse.Whitelist:
                    DialogManager.PushNewDialog(new RT_Dialog_Error("Server is whitelisted!", stopAndClear));
                    break;

                case (int)CommonEnumerators.LoginResponse.WrongVersion:
                    DialogManager.PushNewDialog(new RT_Dialog_Error($"Mod version mismatch! Expected version {loginDetailsJSON.extraDetails[0]}", stopAndClear));
                    break;

                case (int)CommonEnumerators.LoginResponse.NoWorld:
                    DialogManager.PushNewDialog(new RT_Dialog_Error($"Server is currently being set up! Join again later!"));
                    break;
            }
        }
    }
}
