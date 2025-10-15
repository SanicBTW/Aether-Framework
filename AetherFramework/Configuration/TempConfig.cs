using AetherFramework.Interfaces;

namespace AetherFramework.Configuration
{
    /// <summary>
    /// A <see cref="IModConfigProvider"/> for saving configuration in memory without writing to disk.
    /// </summary>
    public class TempConfig : IModConfigProvider
    {
        private ModRegistry registry = null!;
        private readonly ConfigFile backerConfig = new();

        /// <inheritdoc cref="IModConfigProvider.ProviderName"/>
        public string ProviderName => "Temporary Configuration (Saved in memory)";

        /// <inheritdoc cref="IModConfigProvider.Setup(string, ModRegistry)"/>
        public void Setup(string configFile, ModRegistry registry)
        {
            this.registry = registry;

            Save();
            Load();
        }

        /// <inheritdoc cref="IModConfigProvider.Save"/>
        public void Save()
        {
            backerConfig.EnabledMods = [..registry.GetEnabledMods().Select((mod) => mod.Manifest.Name)];
            backerConfig.DisabledMods = [..registry.GetDisabledMods().Select((mod) => mod.Manifest.Name)];
        }

        /// <inheritdoc cref="IModConfigProvider.Load"/>
        public ConfigFile Load() => backerConfig;

        /// <inheritdoc cref="IModConfigProvider.Sanitize(string)"/>
        public string Sanitize(string content) => content;
    }
}
