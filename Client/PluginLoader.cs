using System.Reflection;
using CopierPluginBase;

namespace Client
{
    public class PluginLoader : IPluginLoader
    {
        private readonly IOutputLogger _outputLogger;
        private readonly bool _showDebugMessage;
        private List<Type> _preCopyListeners = new();
        private List<Type> _postcopyListeners = new();

        

        public PluginLoader()
        {

            var pluginDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
            var assemblyFiles = Directory.GetFiles(pluginDirectory, "*.dll");

            foreach (var assemblyName in assemblyFiles)
            {
                var pluginAssembly = Assembly.LoadFile(assemblyName);

                if(_outputLogger != null && _showDebugMessage)
                {
                    _outputLogger.Write($"Loaded {assemblyName}");
                }

                //Post copy
                var preCopyListenersTypes = pluginAssembly.GetTypes().Where(x => x.IsClass
                    && x.IsSubclassOf(typeof(IPreCopyEventListener)));

                //Pre Copy
                var postCopyListenersTypes = pluginAssembly.GetTypes().Where(x => x.IsClass
                    && x.IsSubclassOf(typeof(IPostCopyEventListener)));

                _preCopyListeners.AddRange(preCopyListenersTypes);
            }

        }

        public PluginLoader(IOutputLogger outputLogger, bool showDebugMessage)
        {
            _outputLogger = outputLogger;
            _showDebugMessage = showDebugMessage;
        }

        public void Subscribe(IPreCopyEventBroadcaster pre, IPostCopyEventBroadcaster post)
        {
            _preCopyListeners.ForEach(a =>
            {
                var listenerObject = (IPreCopyEventListener)Activator.CreateInstance(a);
                pre.PreCopy += listenerObject.OnPreCopy;
            });

            _postcopyListeners.ForEach(a =>
            {
                var listenerObject = (IPostCopyEventListener)Activator.CreateInstance(a);
                post.PostCopy += listenerObject.OnPostCopy;
            });
        }

    }
}