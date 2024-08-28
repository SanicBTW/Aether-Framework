using AetherFramework.Interfaces;

namespace AetherFramework.Engines
{
    // The base of a modding engine in order to avoid re-implementing existing code in other engines
    // Some methods might explicitly implement the interface fields but its made like that so other engines can override the base fields
    // TODO: Add the possibility to listen for directory changes

    /// <summary>
    /// Base Modding Engine, don't use this, only use it as a extendable class on your own Modding Engine if you don't want to copy all the boilerplate code for the interface.
    /// <para>
    /// See <see cref="AssemblyEngine"/> for the default Modding Engine used in <see cref="ModLoader"/>.
    /// </para>
    /// </summary>
    public class BaseModEngine : IModEngine
    {
        /// <summary>
        /// The <see cref="ModRegistry"/> created upon calling <see cref="BaseModEngine"/>, used to get the enabled/disabled mods from the configuration file safely.
        /// </summary>
        protected ModRegistry Registry;

        /// <inheritdoc cref="IModEngine.LoadedMods"/>
        public IEnumerable<IMod> LoadedMods => [..EnabledMods, ..DisabledMods];

        /// <inheritdoc cref="IModEngine.EnabledMods"/>
        public IEnumerable<IMod> EnabledMods => Registry.GetEnabledMods();

        /// <inheritdoc cref="IModEngine.DisabledMods"/>
        public IEnumerable<IMod> DisabledMods => Registry.GetDisabledMods();

        string IModEngine.ConfigurationProvider => Registry.GetConfigProvider().GetConfigType();

        /// <summary>
        /// Creates a new <see cref="BaseModEngine"/>.
        /// </summary>
        /// <param name="configFile">The configuration file passed to the <see cref="ModRegistry"/>.</param>
        /// <param name="config">A custom configuration provider for the <see cref="ModRegistry"/>.</param>
        public BaseModEngine(string configFile = ".baseengine", IModConfigProvider ?config = null)
        {
            Registry = new ModRegistry(configFile, config);
        }

        void IModEngine.LoadMods(string path, string filePrefix) => throw new NotImplementedException("This method should be overriden in engines extending this class.");

        /// <inheritdoc cref="IModEngine.EnableMod(string)"/>
        public IMod EnableMod(string modName) => Registry.EnableMod(modName);

        /// <inheritdoc cref="IModEngine.DisableMod(string)"/>
        public IMod DisableMod(string modName) => Registry.DisableMod(modName);
    }
}
