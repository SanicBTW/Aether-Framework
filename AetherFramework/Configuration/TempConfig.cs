using AetherFramework.Interfaces;
using JetBrains.Annotations;

namespace AetherFramework.Configuration
{
    /// <summary>
    /// A <see cref="IModConfigProvider"/> for saving configuration in memory without writing to disk.
    /// </summary>
    [UsedImplicitly]
    public class TempConfig : IModConfigProvider
    {
        private ModRegistry registry = null!;
        private readonly ConfigFile backerConfig = new();

        /// <inheritdoc />
        public string ProviderName => "Temporary Configuration (Saved in memory)";

        /// <inheritdoc />
        public void Setup(string configFile, ModRegistry modRegistry)
        {
            registry = modRegistry;

            Save();
            Load();
        }

        /// <inheritdoc />
        public void Save()
        {
            backerConfig.EnabledMods = [..registry.GetEnabledMods().Select(mod => mod.Manifest.Name)];
            backerConfig.DisabledMods = [..registry.GetDisabledMods().Select(mod => mod.Manifest.Name)];
        }

        /// <inheritdoc />
        public ConfigFile Load() => backerConfig;
    }
}
