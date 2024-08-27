namespace AetherFramework.Interfaces
{
    // hear me out, wtf are these comments bruh
    /// <summary>
    /// Interface used to make Modding Engines on the framework and be able to use them without too much propietary code.
    /// </summary>
    public interface IModEngine
    {
        /// <summary>
        /// List of the current loaded <see cref="IMod"/>s in the current <see cref="IModEngine"/>.
        /// </summary>
        IEnumerable<IMod> LoadedMods { get; }

        /// <summary>
        /// List of the enabled <see cref="IMod"/>s in the current <see cref="IModEngine"/>.
        /// </summary>
        IEnumerable<IMod> EnabledMods { get; }

        /// <summary>
        /// List of the disabled <see cref="IMod"/>s in the current <see cref="IModEngine"/>.
        /// </summary>
        IEnumerable<IMod> DisabledMods { get; }

        /// <summary>
        /// The configuration provider type from this <see cref="IModEngine"/> usually coming from a <see cref="ModRegistry"/>.
        /// </summary>
        string ConfigurationProvider { get; }

        /// <summary>
        /// Enables a disabled mod.
        /// </summary>
        /// <param name="modName">The mod to enable.</param>
        /// <returns>The enabled <see cref="IMod"/></returns>
        IMod EnableMod(string modName);

        /// <summary>
        /// Disables an enabled mod.
        /// </summary>
        /// <param name="modName">The mod to disable.</param>
        /// <returns>The disabled <see cref="IMod"/></returns>
        IMod DisableMod(string modName);

        /// <summary>
        /// Loads all the mods based on the <see cref="IModEngine"/> implementation.
        /// </summary>
        /// <param name="path">The folder it will use to search for any mods.</param>
        /// <param name="filePrefix">The file prefix it will use to filter the files inside the provided <paramref name="path"/>.</param>
        void LoadMods(string path, string filePrefix);
    }
}
