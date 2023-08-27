﻿using Client;
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

        WatchFile(options.FileGlobalPattern, options.SourceDirectoryPath);
        
    }

    private static void WatchFile(string filePattern, string sourceDirectoryPath)
    {
        var watcher = new FileSystemWatcher
        {
            Path = sourceDirectoryPath,
            NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.FileName,
            Filter = filePattern
        };

        watcher.Changed += (sender, args) => Console.WriteLine("File has changed");
        watcher.Renamed += (sender, args) => Console.WriteLine("File has been renamed");

        //Start watching the file.
        watcher.EnableRaisingEvents = true;
    }

    
}

