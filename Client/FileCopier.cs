namespace Client
{
    public class FileCopier : IFileCopier, IPreCopyEventBroadcaster, IPostCopyEventBroadcaster
    {
        private readonly IOutputLogger _outputLogger;


        public event Action<string> PreCopy = delegate { };
        public event Action<string> PostCopy = delegate { };

        public FileCopier(IOutputLogger outputLogger)
        {
            _outputLogger = outputLogger;
        }


        public void CopyFile(CommandOptions options, string fileName)
        {
            var absoluteSourceFilePath = Path.Combine(options.SourceDirectoryPath, fileName);
            var absoluteTargetFilePath = Path.Combine(options.DestinationDirectoryPath, fileName);

            if (File.Exists(absoluteTargetFilePath) && !options.OverwriteTargetFiles)
            {
                _outputLogger.Write($"{fileName} exists. Skipped beacuse OverwriteTargetFile is set to false");
                return;
            };

            PreCopy(absoluteSourceFilePath);
            File.Copy(absoluteSourceFilePath, absoluteTargetFilePath, options.OverwriteTargetFiles);
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

