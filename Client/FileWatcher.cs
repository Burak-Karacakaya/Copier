namespace Client
{
    public class FileWatcher : IFileWatcher
    {
        readonly IFileCopier _fileCopier;
        readonly IOutputLogger _outputLogger;

        public FileWatcher(IFileCopier fileCopier, IOutputLogger outputChannel)
        {
            _fileCopier = fileCopier;
            _outputLogger = outputChannel;
        }

        public void Watch(CommandOptions options)
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
                    _outputLogger.Write($"{args.Name} file has changed.{args.ChangeType}");
                _fileCopier.CopyFile(options.SourceDirectoryPath, args.Name, options.DestinationDirectoryPath, options.OverwriteTargetFiles);

            };
            watcher.Renamed += (sender, args) =>
            {
                if (options.Verbose)
                    _outputLogger.Write($"{args.OldName} has been renamed to {args.Name}");
                _fileCopier.CopyFile(options.SourceDirectoryPath, args.Name, options.DestinationDirectoryPath, options.OverwriteTargetFiles);
            };

            //Start watching the file.
            watcher.EnableRaisingEvents = true;
        }
    }
}

