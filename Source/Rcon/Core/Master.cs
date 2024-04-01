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
            SetCulture();
            LoadResources();
            ChangeTitle();
            Network.StartConnection();

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