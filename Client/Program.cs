using Client;
using CommandLine;

//Copier "specify location", copy to "target folder"

internal class Program
{
    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<CommandOptions>(args)
            .WithParsed(StartWatching)
            .WithNotParsed(a =>
            {
                Environment.Exit(1);
            });

        Console.WriteLine("Please press any key to exit.");
        Console.ReadLine();
    }

    private static void StartWatching(CommandOptions options)
    {
        Console.WriteLine("StartWatching has started...");

        options.SourceDirectoryPath = string.IsNullOrWhiteSpace(options.SourceDirectoryPath)
            ? Directory.GetCurrentDirectory()
            : options.SourceDirectoryPath;

        IFileCopier copier = new FileCopier();
        IOutputLogger outputLogger = new OutputLogger();
        IFileWatcher fileWatcher = new FileWatcher(copier, outputLogger);
        fileWatcher.Watch(options);

    }
}

