using AetherFramework.Interfaces;

namespace AetherFramework.Engines
{
    // The base of a modding engine in order to avoid re-implementing existing code in other engines
    // Some methods might explicitly implement the interface fields but its made like that so other engines can override the base fields
    // TODO: Add the possibility to listen for directory changes

    /// <summary>
    /// Base Modding Engine, to make your own <see cref="IModEngine"/> to support a scripting language or extend an existing one.
    /// <remarks>
    /// See <see cref="AssemblyEngine"/> for the default modding engine used in the <see cref="ModLoader"/>.
    /// </remarks>
    /// </summary>
    public abstract class BaseModEngine : IModEngine
    {
        /// <summary>
        /// The <see cref="ModRegistry"/> created upon calling <see cref="BaseModEngine"/>, used to get the enabled/disabled mods from the configuration file safely.
        /// </summary>
        protected ModRegistry Registry;

        /// <inheritdoc />
        public IEnumerable<IMod> LoadedMods => [..EnabledMods, ..DisabledMods];

        /// <inheritdoc />
        public IEnumerable<IMod> EnabledMods => Registry.GetEnabledMods();

        /// <inheritdoc />
        public IEnumerable<IMod> DisabledMods => Registry.GetDisabledMods();

        string IModEngine.ConfigurationProvider => Registry.GetConfigProvider().ProviderName;

        /// <summary>
        /// Creates a new <see cref="BaseModEngine"/>.
        /// </summary>
        /// <param name="configFile">The configuration file passed to the <see cref="ModRegistry"/>.</param>
        /// <param name="config">A custom configuration provider for the <see cref="ModRegistry"/>.</param>
        protected BaseModEngine(string configFile = "aether_config.json", IModConfigProvider ?config = null)
        {
            Registry = new ModRegistry(configFile, config);
        }

        void IModEngine.LoadMods(string path, string filePrefix) => LoadMods(path, filePrefix);

        /// <inheritdoc cref="IModEngine.LoadMods"/>
        protected abstract void LoadMods(string path, string filePrefix);

        /// <inheritdoc />
        public IMod EnableMod(string modName) => Registry.EnableMod(modName);

        /// <inheritdoc />
        public IMod DisableMod(string modName) => Registry.DisableMod(modName);
    }
}
