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

        private string GetPluginsDirectory()
        {

#if DEBUG
            return Path.Combine(Directory.GetCurrentDirectory(), "bin/Debug/net7.0/", "plugins");
#else
            return Path.Combine(Directory.GetCurrentDirectory(), "plugins");
#endif
        }

        private void Initialize()
        {

            var pluginDirectory = GetPluginsDirectory();
            if (!Directory.Exists($"{pluginDirectory}"))
            {
                if (ShowDebugMessages)
                    _debugLogger.LogWarning("Cannot find plugins folder");
                return;
            }
            var assemblyFiles = Directory.GetFiles(pluginDirectory, "*.dll");

            foreach (var assemblyName in assemblyFiles)
            {
                var pluginAssembly = Assembly.LoadFile(assemblyName);


                if (ShowDebugMessages)
                {
                    _debugLogger.LogDebug($"Loaded {assemblyName}");
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
                    _debugLogger.LogDebug($"Found the following PostCopy types from plugin {assemblyName} :");
                    _debugLogger.LogDebug(string.Join("\n", postCopyListenersTypes.Select(x => x.Name).ToArray()));

                    _debugLogger.LogDebug($"Found the following PreCopy types from plugin {assemblyName} :");
                    // used LINQ for func. basic example
                    var preCopyTypeNames = (from a in preCopyListenersTypes select a.Name).ToArray();
                    _debugLogger.LogDebug(string.Join("\n", preCopyTypeNames));
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