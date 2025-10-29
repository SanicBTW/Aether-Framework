using AetherFramework.Engines;
using AetherFramework.Interfaces;
using JetBrains.Annotations;

namespace AetherFramework
{
    /// <summary>
    /// The heart of Aether Framework.
    /// </summary>
    [UsedImplicitly(ImplicitUseKindFlags.Access | ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.Members)]
    public class ModLoader
    {
        // i believe IT is getting set before exiting the constructor, since the passed engine IS null by default, it will fallback into Assembly Engine, im gonna go lucid bruh
        // instead of adding pragma lets just mark the field to non null
        private readonly IModEngine engine = null!;

        /// <summary>
        /// List of the current loaded <see cref="IMod"/>s in the current <see cref="ModLoader"/>.
        /// </summary>
        public IEnumerable<IMod> LoadedMods => engine.LoadedMods;

        /// <summary>
        /// List of the enabled <see cref="IMod"/>s in the current <see cref="ModLoader"/>.
        /// </summary>
        public IEnumerable<IMod> EnabledMods => engine.EnabledMods;

        /// <summary>
        /// List of the disabled <see cref="IMod"/>s in the current <see cref="ModLoader"/>.
        /// </summary>
        public IEnumerable<IMod> DisabledMods => engine.DisabledMods;

        /// <summary>
        /// The configuration provider from this <see cref="ModLoader"/> usually coming from a <see cref="IModEngine"/>.
        /// </summary>
        public IModConfigProvider Configuration => engine.Configuration;

        /// <summary>
        /// Loads all the <see cref="IMod"/>s available using the provided arguments to be as modular as possible.
        /// </summary>
        /// <param name="folder">The folder to scan, if it exists it will scan for everything that extends <see cref="IMod"/>.
        /// <remarks>
        /// If null or empty it will be forced to the <c>mods</c> directory inside of the program directory.
        /// </remarks>
        /// </param>
        /// <param name="filePrefix">The file prefix to target, this is useful to reduce the files to load and check for an <see cref="IMod"/>.</param>
        /// <param name="engine">The Modding Engine to use in THIS Mod Loader, each engine will load their respective files.</param>
        /// <param name="config">The Modding Configuration Provider to use in the provided <paramref name="engine"/>.</param>
        public ModLoader(string folder = "", string filePrefix = "", IModEngine? engine = null, IModConfigProvider? config = null)
        {
            // force it in case of giving nothing
            string loadPath = (string.IsNullOrWhiteSpace(folder) ? Path.Join([AppDomain.CurrentDomain.BaseDirectory, "mods"]) : folder);

            // if the folder is empty or null is a risky thing to do depending on the modding engine
            // on the assembly engine its going to scan all the .dlls inside the output folder,
            // so maybe providing a file prefix could help reducing the assemblies to load to look for IMods

            // only check if the folder exists when the provided argument is not empty or null
            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(loadPath))
            {
                // No mods to load
                Directory.CreateDirectory(loadPath);
                return;
            }

            this.engine = engine ?? new AssemblyEngine(config);
            this.engine.LoadMods(loadPath, filePrefix);
        }

        /// <inheritdoc cref="IModEngine.EnableMod(string)"/>
        public IMod EnableMod(string modName) => engine.EnableMod(modName);

        /// <inheritdoc cref="IModEngine.DisableMod(string)"/>
        public IMod DisableMod(string modName) => engine.DisableMod(modName);
    }
}
