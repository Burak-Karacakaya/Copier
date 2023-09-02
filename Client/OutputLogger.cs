namespace Client
{
    public class OutputLogger : IOutputLogger
    {
        public void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error : " + message);
            Console.ResetColor();
        }

        public void LogInfo(string message)
        {
            Console.WriteLine(message);
        }

        public void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Warning : " + message);
            Console.ResetColor();
        }

        public void LogDebug(string message)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Debug : " + message);
            Console.ResetColor();
        }
    }
}

