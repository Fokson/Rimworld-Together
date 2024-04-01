
namespace Rcon
{
    //Main class that is used to handle the connection with the server

    public static class Network
    {
        //IP and Port that the connection will be bound to
        public static string ip = "";
        public static string port = "";

        //TCP listener that will handle the connection with the server
        public static Listener listener;

        //Useful booleans to check connection status with the server
        public static bool isConnectedToServer;
        public static bool isTryingToConnect;

        //Entry point function of the network class
        public static void StartConnection()
        {
            while (true)
            {
                GetConnectionDetails();

                if (TryConnectToServer())
                {

                    Threader.GenerateServerThread(Threader.RconMode.Console);

                    Threader.GenerateClientThread(listener, Threader.ClientMode.Listener);
                    Threader.GenerateClientThread(listener, Threader.ClientMode.Sender);
                    Threader.GenerateClientThread(listener, Threader.ClientMode.Health);
                    Threader.GenerateClientThread(listener, Threader.ClientMode.KAFlag);

                    Logger.WriteToConsole($"[Rimworld Together Rcon] > Connected to server Console");
                    break;
                }
                else
                {
                    Logger.WriteToConsole($"[Rimworld Together Rcon] > Could not connect to the server");
                    CleanValues();
                }
            }
        }

        public static void GetConnectionDetails()
        {
            while (true) {
                //get connection details
                Logger.WriteToConsole("IP: ");
                ip = Console.ReadLine();
                Logger.WriteToConsole("Port: ");
                port = Console.ReadLine();


                //make sure the connection details are valid
                bool isValid = true;



                if (string.IsNullOrWhiteSpace(ip)) isValid = false;
                if (string.IsNullOrWhiteSpace(port)) isValid = false;
                if (port.Count() > 5) isValid = false;
                if (!port.All(Char.IsDigit)) isValid = false;

                if (isValid)
                {
                    Threader.GenerateServerThread(Threader.RconMode.Start);
                    break;
                }
                else
                {
                    Logger.WriteToConsole($"[Rimworld Together Rcon] > Connection details are not valid, please try again");
                }
            }
        }



        //Tries to connect into the specified server
        private static bool TryConnectToServer()
        {
            if (isTryingToConnect || isConnectedToServer) return false;
            else
            {
                try
                {
                    isTryingToConnect = true;

                    isConnectedToServer = true;

                    listener = new Listener(new(ip, int.Parse(port)));

                    return true;
                }
                catch { return false; }
            }
        }

        //Disconnects client from the server
        public static void DisconnectFromServer()
        {
            listener.DestroyConnection();

            Logger.WriteToConsole($"[Rimworld Together] > Connected to server Console");
             
        }

        //Clears all related values
        public static void CleanValues()
        {
            isTryingToConnect = false;
            isConnectedToServer = false;
        }
    }
}
