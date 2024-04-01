using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    //Class object for the client connecting into the server. Contains all important data about it

    public class RconClient
    {
        //Reference to the listener instance of this client
        [NonSerialized] public RconListener listener;

        public string username = "Unknown";

        public string password;

        public bool isAdmin;

        public string SavedIP { get; set; }

        public RconClient(TcpClient tcp)
        {
            if (tcp == null) return;
            else SavedIP = ((IPEndPoint)tcp.Client.RemoteEndPoint).Address.ToString();
        }
    }
}
