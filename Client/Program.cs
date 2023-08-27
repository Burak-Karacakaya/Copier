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
    }

    private static void StartWatching(CommandOptions options)
    {
        var files = GetMatchingFiles(options);
        var file = "";
        var watcher = new FileSystemWatcher
        {
            Path = options.SourceDirectoryPath,
            NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.FileName,
            Filter = file
        };

        watcher.Changed += (sender, args) => Console.WriteLine("File has changed");
        watcher.Renamed += (sender, args) => Console.WriteLine("File has been renamed");

        //Start watching the file.
        watcher.EnableRaisingEvents = true;


        Console.WriteLine(files.Select(a => a.Path).Aggregate((a, b) => a + ", " + b));
    }



    private static IEnumerable<FilePatternMatch> GetMatchingFiles(CommandOptions options)
    {
        var m = new Matcher();
        m.AddInclude(options.FileGlobalPattern);

        var directoryInfo = new DirectoryInfo(string.IsNullOrWhiteSpace(options.SourceDirectoryPath)
            ? Directory.GetCurrentDirectory() : options.SourceDirectoryPath);
        var dirInfo = new DirectoryInfoWrapper(directoryInfo);

        var files = m.Execute(dirInfo).Files;
        return files;
    }
}

