using System.Reflection;
using System.Runtime.CompilerServices;
using CopierPluginBase;

namespace Client
{
    public class PluginLoader : IPluginLoader
    {
        private readonly IOutputLogger _debugLogger;
        private List<Type> _preCopyListeners = new();
        private List<Type> _postcopyListeners = new();

        private bool ShowDebugMessages { get; set; }

        public PluginLoader()
        {
            Initialize();
        }

        private void Initialize()
        {
            var pluginDirectory = string.Empty;

#if DEBUG
            pluginDirectory = Path.Combine(Directory.GetCurrentDirectory(), "bin/Debug/net7.0/", "plugins");
#else
                pluginDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
#endif

            var assemblyFiles = Directory.GetFiles(pluginDirectory, "*.dll");

            foreach (var assemblyName in assemblyFiles)
            {
                var pluginAssembly = Assembly.LoadFile(assemblyName);


                if (ShowDebugMessages)
                {
                    _debugLogger.Write($"Loaded {assemblyName}");
                }

                var existingType = pluginAssembly.GetTypes();


                // Filter where
                bool TypePredicate(Type child, Type parent) =>
                    child.IsPublic && !child.IsAbstract && child.IsClass && parent.IsAssignableFrom(child);

                // Pre Copy filter
                var preCopyListenersTypes = existingType
                    .Where(x => TypePredicate(x, typeof(IPreCopyEventListener)))
                    .ToList();

                // Post Copy filter
                var postCopyListenersTypes = existingType
                    .Where(x => TypePredicate(x, typeof(IPostCopyEventListener)))
                    .ToList();

                _preCopyListeners.AddRange(preCopyListenersTypes);

                //If enabled, logging debugmesssages for the found types in the iterated assembly.
                if (ShowDebugMessages)
                {
                    _debugLogger.Write($"Found the following PostCopy types from plugin {assemblyName} :");
                    _debugLogger.Write(string.Join("\n", postCopyListenersTypes.Select(x => x.Name).ToArray()));

                    _debugLogger.Write($"Found the following PreCopy types from plugin {assemblyName} :");
                    // used LINQ for func. basic example
                    var preCopyTypeNames = (from a in preCopyListenersTypes select a.Name).ToArray();
                    _debugLogger.Write(string.Join("\n", preCopyTypeNames));
                }

            }
        }

        public PluginLoader(IOutputLogger debugLogger, bool showDebugMessages = false)
        {
            _debugLogger = debugLogger;
            ShowDebugMessages = showDebugMessages && debugLogger != null;
            Initialize();
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