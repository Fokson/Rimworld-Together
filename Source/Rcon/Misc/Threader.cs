namespace Rcon
{
    public static class Threader
    {
        public enum ServerMode { Start, Sites, Console }

        public static Task GenerateServerThread(ServerMode mode)
        {
            return mode switch
            {
                ServerMode.Start => Task.Run(Network.ReadyServer),
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