using System.Text;

namespace Rcon
{
    public static class Logger
    {
        public static Semaphore semaphore = new Semaphore(1, 1);

        public enum LogMode { Normal, Warning, Error, Title }

        public static Dictionary<LogMode, ConsoleColor> colorDictionary = new Dictionary<LogMode, ConsoleColor>
        {
            { LogMode.Normal, ConsoleColor.White },
            { LogMode.Warning, ConsoleColor.Yellow },
            { LogMode.Error, ConsoleColor.Red },
            { LogMode.Title, ConsoleColor.Green }
        };

        public static void WriteToConsole(string text, LogMode mode = LogMode.Normal)
        {
            semaphore.WaitOne();

            Console.ForegroundColor = colorDictionary[mode];
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] | " + text);
            Console.ForegroundColor = ConsoleColor.White;

            semaphore.Release();

        }
        public static void WriteMirrorToConsole(string text, LogMode mode = LogMode.Normal)
        {
            semaphore.WaitOne();

            Console.ForegroundColor = colorDictionary[mode];
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;

            semaphore.Release();

        }

    }
}
