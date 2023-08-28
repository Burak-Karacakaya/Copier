using Client;
using CommandLine;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

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

        var sourceDiroctoryPath = string.IsNullOrWhiteSpace(options.SourceDirectoryPath)
            ? Directory.GetCurrentDirectory()
            : options.SourceDirectoryPath;

        WatchFile(options.FileGlobalPattern,sourceDiroctoryPath);

    }

    private static void WatchFile(string filePattern, string sourceDirectoryPath)
    {
        var watcher = new FileSystemWatcher
        {
            Path = sourceDirectoryPath,
            NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.FileName,
            Filter = filePattern
        };

        watcher.Changed += (sender, args) =>
        {
            if(args.ChangeType == WatcherChangeTypes.Changed)
            Console.WriteLine($"{args.Name} file has changed.{args.ChangeType}");
        };
        watcher.Renamed += (sender, args) => Console.WriteLine("File has been renamed");

        //Start watching the file.
        watcher.EnableRaisingEvents = true;
    }
}

