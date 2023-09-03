using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client
{
    public class QueuedFileCopier : IFileCopier, IPreCopyEventBroadcaster, IPostCopyEventBroadcaster
    {
        public event Action<string> PreCopy = delegate {};
        public event Action<string> PostCopy = delegate {};

        private readonly CommandOptions _options;
        private readonly IFileCopier _fileCopier;
        private readonly HashSet<string> _fileNameQueue = new HashSet<string>();
        private Task _copyTask;
        private readonly IOutputLogger _logger;
        public QueuedFileCopier(IFileCopier fileCopier, IOutputLogger logger, CommandOptions options)
        {
            _logger = logger;
            _fileCopier = fileCopier;
            _options = options; 

            if(_options.Debug)
            {
                logger.LogInfo("Delay option has been specified. QueuedFileCopier is chosen as the copier strategy.");
            }
        }

        public void CopyFile(string fileName)
        {
            if (_copyTask == null)
            {
                _copyTask = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(_options.Delay));
                    if (_options.Verbose || _options.Debug)
                    {
                        _logger.LogInfo($"{_options.Delay} milliseconds have passed. The copy operation has started...");
                    }

                    PreCopy("");

                    foreach (var item in _fileNameQueue)
                    {
                        _fileCopier.CopyFile(item);
                    }

                    PostCopy("");

                    _copyTask = null;

                    if (_options.Verbose || _options.Debug)
                    {
                        _logger.LogInfo($"The copy operation has finished...");
                        _logger.LogInfo("The file queue has been emptied.");
                    }
                });
            }

            if (!_fileNameQueue.Contains(fileName))
            {
                if (_options.Verbose || _options.Debug)
                {
                    _logger.LogInfo(
                        $"{fileName} has been added to the file queue and will be copied over in {_options.Delay} milliseconds.");
                }

                _fileNameQueue.Add(fileName);
            }
            else if (_options.Debug)
            {
                _logger.LogInfo($"{fileName} exists in the file queue, thereby skipped.");
            }
        }
    }
}