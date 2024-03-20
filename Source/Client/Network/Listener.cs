﻿using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Verse;

namespace GameClient
{
    //Class that handles all incoming and outgoing packet instructions

    public class Listener
    {
        //TCP variables needed for the listener to comunicate
        public TcpClient connection;
        public NetworkStream networkStream;

        //Stream tools used to read and write the connection stream
        public StreamWriter streamWriter;
        public StreamReader streamReader;

        //Upload and download classes to send/receive files
        public UploadManager uploadManager;
        public DownloadManager downloadManager;

        //Data queue used to hold packets that are to be sent through the connection
        public Queue<Packet> dataQueue = new Queue<Packet>();

        //Useful variables to handle connection status
        public bool disconnectFlag;

        public Listener(TcpClient connection)
        {
            DialogShortcuts.ShowLoginOrRegisterDialogs();

            this.connection = connection;
            networkStream = connection.GetStream();
            streamWriter = new StreamWriter(networkStream);
            streamReader = new StreamReader(networkStream);
        }

        //Enqueues a new packet into the data queue if needed

        public void EnqueuePacket(Packet packet)
        {
            if (disconnectFlag) return;
            else dataQueue.Enqueue(packet);
        }

        //Runs in a separate thread and sends all queued packets through the connection

        public void SendData()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(1);

                    if (dataQueue.Count() > 0)
                    {
                        Packet packet = dataQueue.Dequeue();
                        if (packet == null) continue;

                        streamWriter.WriteLine(Serializer.SerializePacketToString(packet));
                        streamWriter.Flush();
                    }
                }
            }
            catch { disconnectFlag = true; }
        }

        //Runs in a separate thread and listens for any kind of information being sent through the connection

        public void Listen()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(1);

                    string data = streamReader.ReadLine();
                    if (string.IsNullOrEmpty(data)) continue;

                    Packet receivedPacket = Serializer.SerializeStringToPacket(data);
                    PacketHandler.HandlePacket(receivedPacket);
                }
            }

            catch (Exception e)
            {

                Logs.Message("[Rimworld Together] > closing connection due to exception ",true);
                Logs.Warning($"[Rimworld Together] > {e}",true);
                disconnectFlag = true;
            }
        }

        //Runs in a separate thread and checks if the connection should still be up

        public void CheckConnectionHealth()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(1);

                    if (disconnectFlag) break;
                }
            }
            catch { }
            //TODO - clean this up an write more informational logs reguarding how the server-client disconnect happened
            try
            {
                if (disconnectFlag) Logs.Message("[Rimworld Together] > disconnect flag true, closing connection");
                Thread.Sleep(1000);
                Master.threadDispatcher.Enqueue(Network.DisconnectFromServer);
            }
            catch(Exception e)
            {
                Logs.Error($"[Rimworld Together] > {e}");
            }
        }

        //Runs in a separate thread and sends alive pings towards the server

        public void SendKAFlag()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(1000);

                    KeepAliveJSON keepAliveJSON = new KeepAliveJSON();
                    Packet packet = Packet.CreatePacketFromJSON(nameof(PacketHandler.KeepAlivePacket), keepAliveJSON);
                    EnqueuePacket(packet);
                }
            }
            catch { }
        }

        //Forcefully ends the connection with the server and any important process associated with it

        public void DestroyConnection()
        {
            Logs.Message("[Rimworld Together] > Destroying connection");
            connection.Close();
            uploadManager?.fileStream.Close();
            downloadManager?.fileStream.Close();
        }
    }
}
