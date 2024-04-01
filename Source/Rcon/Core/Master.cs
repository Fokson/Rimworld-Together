namespace Rcon
{
    public static class Master
    {

        //Paths

        public static string mainPath;
        public static string corePath;
        public static string mapsPath;
        public static string logsPath;
        public static string usersPath;
        public static string savesPath;
        public static string sitesPath;
        public static string factionsPath;
        public static string settlementsPath;

        public static string modsPath;
        public static string requiredModsPath;
        public static string optionalModsPath;
        public static string forbiddenModsPath;

        //Lists

        public static List<string> loadedRequiredMods = new List<string>();
        public static List<string> loadedOptionalMods = new List<string>();
        public static List<string> loadedForbiddenMods = new List<string>();

        //Booleans

        public static bool isClosing;

        public static void Main()
        {




        }

    }
}