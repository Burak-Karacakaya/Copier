namespace Client
{
    public class FileCopier : IFileCopier, IPreCopyEventBroadcaster, IPostCopyEventBroadcaster
    {
        private readonly IOutputLogger _outputLogger;
        private readonly CommandOptions _options;

        public event Action<string> PreCopy = delegate { };
        public event Action<string> PostCopy = delegate { };

        public FileCopier(IOutputLogger outputLogger, CommandOptions options)
        {
            _outputLogger = outputLogger;
            _options = options;
        }

        public void CopyFile(string fileName)
        {
            var absoluteSourceFilePath = Path.Combine(_options.SourceDirectoryPath, fileName);
            var absoluteTargetFilePath = Path.Combine(_options.DestinationDirectoryPath, fileName);

            if (File.Exists(absoluteTargetFilePath) && !_options.OverwriteTargetFiles)
            {
                _outputLogger.LogDebug($"{fileName} exists. Skipped the copy beacuse OverwriteTargetFile is set to false");
                return;
            }

            PreCopy(absoluteSourceFilePath);
            File.Copy(absoluteSourceFilePath, absoluteTargetFilePath, _options.OverwriteTargetFiles);
            PostCopy(absoluteSourceFilePath);
        }
    }

    public interface IPostCopyEventBroadcaster
    {
        event Action<string> PostCopy;
    }

    public interface IPreCopyEventBroadcaster
    {
        event Action<string> PreCopy;
    }
}

