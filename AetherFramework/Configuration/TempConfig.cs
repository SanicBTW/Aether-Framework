using AetherFramework.Interfaces;

namespace AetherFramework.Configuration
{
    /// <summary>
    /// A <see cref="IModConfigProvider"/> for saving configuration in memory without writing to disk.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class TempConfig : IModConfigProvider
    {
        private ModRegistry _registry = null!;
        private readonly ConfigFile _backerConfig = new();

        public string ProviderName => "Temporary Configuration (Saved in memory)";

        public void Setup(string configFile, ModRegistry registry)
        {
            _registry = registry;

            Save();
            Load();
        }

        public void Save()
        {
            _backerConfig.EnabledMods = [.._registry.GetEnabledMods().Select(mod => mod.Manifest.Name)];
            _backerConfig.DisabledMods = [.._registry.GetDisabledMods().Select(mod => mod.Manifest.Name)];
        }

        public ConfigFile Load() => _backerConfig;
    }
}
