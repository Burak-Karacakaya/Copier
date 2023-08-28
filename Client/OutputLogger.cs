namespace Client
{
    public class OutputLogger : IOutputLogger
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}

