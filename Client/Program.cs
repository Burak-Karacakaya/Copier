using Client;
using CommandLine;

//Copier "specify location", copy to "target folder"
class Program
{

    public static event Action ApplicationStarted = delegate { };

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
        IOutputLogger outputLogger = new OutputLogger();
        outputLogger.LogInfo("Please press any key to exit");

        options.SourceDirectoryPath = string.IsNullOrWhiteSpace(options.SourceDirectoryPath)
            ? Directory.GetCurrentDirectory()
            : options.SourceDirectoryPath;


        IPluginLoader loader = new PluginLoader(outputLogger, options.Debug);

        IFileCopier copier = new FileCopier(outputLogger);
        IFileWatcher fileWatcher = new FileWatcher(copier, outputLogger);

        loader.Subscribe((IPreCopyEventBroadcaster)copier, (IPostCopyEventBroadcaster)copier);
        fileWatcher.Watch(options);

    }
}

