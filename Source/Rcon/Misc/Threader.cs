namespace Rcon
{
    public static class Threader
    {
        public enum RconMode { Start, Console }

        public static Task GenerateServerThread(RconMode mode)
        {
            return mode switch
            {
                RconMode.Start => Task.Run(Network.StartConnection),
                RconMode.Console => Task.Run(RconCommandManager.ListenForRconInput),
                _ => throw new NotImplementedException(),
            };
        }

        public enum ClientMode { Listener, Sender, Health, KAFlag }

        public static Task GenerateClientThread(Listener listener, ClientMode mode)
        {
            return mode switch
            {
                ClientMode.Listener => Task.Run(listener.Listen),
                ClientMode.Sender => Task.Run(listener.SendData),
                ClientMode.Health => Task.Run(listener.CheckConnectionHealth),
                ClientMode.KAFlag => Task.Run(listener.CheckKAFlag),
                _ => throw new NotImplementedException(),
            };
        }
    }
}