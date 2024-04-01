namespace Rcon
{
    public static class ServerCommandManager
    {
        public static string[] eventTypes = new string[]
        {
            "Raid",
            "Infestation",
            "MechCluster",
            "ToxicFallout",
            "Manhunter",
            "Wanderer",
            "FarmAnimals",
            "ShipChunks",
            "TraderCaravan"
        };

        public static string[] commandParameters;

        public static void ParseServerCommands(string parsedString)
        {
            string parsedPrefix = parsedString.Split(' ')[0].ToLower();
            int parsedParameters = parsedString.Split(' ').Count() - 1;
            commandParameters = parsedString.Replace(parsedPrefix + " ", "").Split(" ");

            try
            {
                ServerCommand commandToFetch = ServerCommandStorage.serverCommands.ToList().Find(x => x.prefix == parsedPrefix);
                if (commandToFetch == null) Logger.WriteToConsole($"[ERROR] > Command '{parsedPrefix}' was not found", Logger.LogMode.Warning);
                else
                {
                    if (commandToFetch.parameters != parsedParameters && commandToFetch.parameters != -1)
                    {
                        Logger.WriteToConsole($"[ERROR] > Command '{commandToFetch.prefix}' wanted [{commandToFetch.parameters}] parameters "
                            + $"but was passed [{parsedParameters}]", Logger.LogMode.Warning);
                    }

                    else
                    {
                        if (commandToFetch.commandAction != null) commandToFetch.commandAction.Invoke();

                        else Logger.WriteToConsole($"[ERROR] > Command '{commandToFetch.prefix}' didn't have any action built in", 
                            Logger.LogMode.Warning);
                    }
                }
            }
            catch (Exception e) { Logger.WriteToConsole($"[Error] > Couldn't parse command '{parsedPrefix}'. Reason: {e}", Logger.LogMode.Error); }
        }

        public static void ListenForServerCommands()
        {
            bool interactiveConsole;

            try
            {
                if (Console.In.Peek() != -1) interactiveConsole = true;
                else interactiveConsole = false;
            }

            catch
            {
                interactiveConsole = false;
                Logger.WriteToConsole($"[Warning] > Couldn't found interactive console, disabling commands", Logger.LogMode.Warning);
            }

            if (interactiveConsole)
            {
                while (true)
                {
                    ParseServerCommands(Console.ReadLine());
                }
            }
            else Logger.WriteToConsole($"[Warning] > Couldn't found interactive console, disabling commands", Logger.LogMode.Warning);
        }
    }

    public static class ServerCommandStorage
    {
        private static ServerCommand clearCommand = new ServerCommand("clear", 0,
            "Clears the console output",
            ClearCommandAction);


        public static ServerCommand[] serverCommands = new ServerCommand[]
        {

        };
        private static void ClearCommandAction()
        {
            Console.Clear();

            Logger.WriteToConsole("[Cleared console]", Logger.LogMode.Title);
        }
    }
}