using AetherFramework.Interfaces;
using System.Reflection;

namespace AetherFramework.Engines
{
    /// <summary>
    /// The default Modding Engine used in <see cref="ModLoader"/>, loads all the .dll files inside a designated folder and registering them.
    /// </summary>
    public class AssemblyEngine : BaseModEngine, IModEngine
    {
        /// <summary>
        /// Creates a new instance of an Assembly Engine.
        /// </summary>
        /// <param name="config">A custom <see cref="IModConfigProvider"/> to use in this Registry.</param>
        public AssemblyEngine(IModConfigProvider? config = null) : base(".assemblyengine", config) { }

        void IModEngine.LoadMods(string path, string filePrefix)
        {
            bool shouldCheckPrefix = !string.IsNullOrEmpty(filePrefix);

            // works as expected, maybe not depending on the prefixing but whatever lmao!!
            string[] mods = Directory.GetFiles(path, $"{filePrefix}*.dll");

            foreach (string modFile in mods)
            {
                if (shouldCheckPrefix && !modFile.Contains(filePrefix))
                    continue;

                Assembly modAssembly = Assembly.LoadFrom(modFile);
                ClassRegistry.RegisterOverridableClasses(modAssembly);
                IEnumerable<Type> modTypes = modAssembly.GetTypes().Where(t => typeof(IMod).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (Type type in modTypes)
                {
                    IMod mod = (IMod)Activator.CreateInstance(type)!;
                    Registry.RegisterMod(mod);
                }
            }
        }
    }
}
