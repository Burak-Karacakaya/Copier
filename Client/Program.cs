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

        WatchFile(options);

    }

    private static void WatchFile(CommandOptions options)
    {
        var watcher = new FileSystemWatcher
        {
            Path = options.SourceDirectoryPath,
            NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.FileName,
            Filter = options.FileGlobPattern
        };

        watcher.Changed += (sender, args) =>
        {
            if (args.ChangeType != WatcherChangeTypes.Changed) return;
            if (options.Verbose)
                Console.WriteLine($"{args.Name} file has changed.{args.ChangeType}");
            CopyFile(options.SourceDirectoryPath, args.Name, options.DestinationDirectoryPath, options.OverwriteTargetFiles);

        };
        watcher.Renamed += (sender, args) =>
        {
            if(options.Verbose)
                Console.WriteLine("File has been renamed");
            CopyFile(options.SourceDirectoryPath, args.Name, options.DestinationDirectoryPath, options.OverwriteTargetFiles);
        };

        //Start watching the file.
        watcher.EnableRaisingEvents = true;
    }

    private static void CopyFile(string sourceDirectoryPath ,string fileName, string targetDirectoryPath, bool overWriteTargetFile)
    {
        var absoluteSourceFilePath = Path.Combine(sourceDirectoryPath, fileName);
        var absoluteTargetFilePath = Path.Combine(targetDirectoryPath, fileName);

        if (File.Exists(absoluteTargetFilePath) && !overWriteTargetFile) return;

        File.Copy(absoluteSourceFilePath, absoluteTargetFilePath, overWriteTargetFile);
    }
}

