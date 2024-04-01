using Shared;
using System.Globalization;

namespace Rcon
{
    public static class Master
    {

        //Booleans

        public static bool isClosing;

        public static void Main()
        {
            Console.ForegroundColor = ConsoleColor.White;

            TryDisablyQuickEdit();
            SetPaths();
            SetCulture();
            LoadResources();
            ChangeTitle();

            Threader.GenerateServerThread(Threader.ServerMode.Start);
            Threader.GenerateServerThread(Threader.ServerMode.Console);

            while (true) Thread.Sleep(1);



        }

        private static void TryDisablyQuickEdit()
        {
            try
            {
                QuickEdit quickEdit = new QuickEdit();
                quickEdit.DisableQuickEdit();
            }
            catch { };
        }

        private static void SetPaths()
        {
            mainPath = Directory.GetCurrentDirectory();
            corePath = Path.Combine(mainPath, "Core");
            mapsPath = Path.Combine(mainPath, "Maps");
            logsPath = Path.Combine(mainPath, "Logs");
            usersPath = Path.Combine(mainPath, "Users");
            savesPath = Path.Combine(mainPath, "Saves");
            sitesPath = Path.Combine(mainPath, "Sites");
            factionsPath = Path.Combine(mainPath, "Factions");
            settlementsPath = Path.Combine(mainPath, "Settlements");

            modsPath = Path.Combine(mainPath, "Mods");
            requiredModsPath = Path.Combine(modsPath, "Required");
            optionalModsPath = Path.Combine(modsPath, "Optional");
            forbiddenModsPath = Path.Combine(modsPath, "Forbidden");

            if (!Directory.Exists(corePath)) Directory.CreateDirectory(corePath);
            if (!Directory.Exists(usersPath)) Directory.CreateDirectory(usersPath);
            if (!Directory.Exists(savesPath)) Directory.CreateDirectory(savesPath);
            if (!Directory.Exists(mapsPath)) Directory.CreateDirectory(mapsPath);
            if (!Directory.Exists(logsPath)) Directory.CreateDirectory(logsPath);
            if (!Directory.Exists(sitesPath)) Directory.CreateDirectory(sitesPath);
            if (!Directory.Exists(factionsPath)) Directory.CreateDirectory(factionsPath);
            if (!Directory.Exists(settlementsPath)) Directory.CreateDirectory(settlementsPath);

            if (!Directory.Exists(modsPath)) Directory.CreateDirectory(modsPath);
            if (!Directory.Exists(requiredModsPath)) Directory.CreateDirectory(requiredModsPath);
            if (!Directory.Exists(optionalModsPath)) Directory.CreateDirectory(optionalModsPath);
            if (!Directory.Exists(forbiddenModsPath)) Directory.CreateDirectory(forbiddenModsPath);
        }

        private static void SetCulture()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            CultureInfo.CurrentUICulture = new CultureInfo("en-US", false);
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", false);
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US", false);

            Logger.WriteToConsole($"Loading server culture > [{CultureInfo.CurrentCulture}]", Logger.LogMode.Title);
        }

        public static void LoadResources()
        {
            Logger.WriteToConsole($"Loading version {CommonValues.executableVersion}", Logger.LogMode.Title);
            Logger.WriteToConsole($"----------------------------------------", Logger.LogMode.Title);

        }


        public static void ChangeTitle()
        {
            Console.Title = $"Rimworld Together {CommonValues.executableVersion}";
        }

    }
}