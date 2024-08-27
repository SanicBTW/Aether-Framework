namespace AetherFramework.Interfaces
{
    /// <summary>
    /// Interface for the <see cref="ModRegistry"/> to be able to save configuration.
    /// </summary>
    public interface IModConfigProvider
    {
        /// <summary>
        /// Setups the configuration file in the disk.
        /// </summary>
        /// <param name="configFile">The config file name.</param>
        /// <param name="registry">The <see cref="ModRegistry"/> that will target this <see cref="IModConfigProvider"/>.</param>
        void Setup(string configFile, ModRegistry registry);

        /// <summary>
        /// Saves the configuration from the <see cref="ModRegistry"/>.
        /// </summary>
        void Save();

        /// <summary>
        /// Loads the configuration to <see cref="ModRegistry"/>.
        /// </summary>
        void Load();

        /// <summary>
        /// The configuration provider type.
        /// </summary>
        /// <returns>Returns this <see cref="IModConfigProvider"/> implementation name. (In a human readable way)</returns>
        string GetConfigType();
    }
}
