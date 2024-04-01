namespace GameServer
{
    [Serializable]
    public class ServerConfigFile
    {
        public string IP = "0.0.0.0";

        public string Port = "25555";

        public string RconPort = "25556";

        public string MaxPlayers = "100";

        public string MaxRcons = "10";

        public string MaxTimeoutInMS = "5000";

        public bool verboseLogs = false;
    }
}
