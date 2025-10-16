using AetherFramework.Interfaces;
using System.Text.Json;

namespace AetherFramework.Configuration
{
    /// <summary>
    /// The default <see cref="IModConfigProvider"/> used when creating a new <see cref="ModRegistry"/>.
    /// </summary>
    public class BasicConfig : IModConfigProvider
    {
        private static JsonSerializerOptions JsonOptions => new() { WriteIndented = true };

        private ModRegistry _registry = null!;
        private string _configPath = "";

        // gets used on save calls and load calls to avoid creating new instances
        private PresetConfigFile _backerConfig = new();

        public string ProviderName => "Basic Configuration (Default Provider)";

        public void Setup(string configFile, ModRegistry registry)
        {
            _registry = registry;

            _configPath = Path.Join([AppDomain.CurrentDomain.BaseDirectory, configFile]);
            if (!File.Exists(_configPath))
                Save(); // we only call save during setup to format the file properly

            // since load is only called once, we call refresh to call load from here, avoiding having to use reflection
            registry.Refresh();
        }

        public void Save()
        {
            if (_configPath == null)
                throw new Exception("Configuration path was null, did you call \"Setup\"?");

            _backerConfig.EnabledMods = [.. _registry.GetEnabledMods().Select(mod => mod.Manifest.Name)];
            _backerConfig.DisabledMods = [.. _registry.GetDisabledMods().Select(mod => mod.Manifest.Name)];

            // yeah writes the whole file each time save gets called because this shi stinks lol
            string json = JsonSerializer.Serialize(_backerConfig, JsonOptions);
            File.WriteAllText(_configPath, json);
        }

        public ConfigFile Load()
        {
            string content = File.ReadAllText(_configPath);
            _backerConfig = JsonSerializer.Deserialize<PresetConfigFile>(content) ?? new PresetConfigFile();

            // TODO: Parse content more in-depth to avoid manipulating the file and missing the keys, tho it can be unnecessary
            // maybe the user wants to change something inside of it like refreshing the mod list or presets, yo thats a good idea im gonna implement it
            // 29/11/2024
            // 30/11/2024 - ok so im working on the idea now
            // 16/10/2025 - uhh took me a whole year to come back to im so sorry it took me so long to finish such a good idea bruh

            return _backerConfig;
        }
    }
}
