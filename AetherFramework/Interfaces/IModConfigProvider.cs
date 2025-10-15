using AetherFramework.Configuration;

namespace AetherFramework.Interfaces
{
    /// <summary>
    /// Interface for the <see cref="ModRegistry"/> to be able to save configuration.
    /// </summary>
    public interface IModConfigProvider
    {
        /// <summary>
        /// The name of this <see cref="IModConfigProvider"/> implementation.
        /// </summary>
        string ProviderName { get; }

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
        ConfigFile Load();

        /// <summary>
        /// Sanitizes the given string to fit the <see cref="IModConfigProvider"/>s rules. 
        /// </summary>
        /// <param name="content">The string to sanitize.</param>
        /// <returns>A sanitized string used for configuration or display purposes.</returns>
        string Sanitize(string content);
    }
}
