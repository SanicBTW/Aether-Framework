using AetherFramework.Interfaces;
using System.Text.Json;

namespace AetherFramework.Configuration
{
    /// <summary>
    /// The default <see cref="IModConfigProvider"/> used when creating a new <see cref="ModRegistry"/>.
    /// </summary>
    public class BasicConfig : IModConfigProvider
    {
        private static JsonSerializerOptions jsonOptions => new() { WriteIndented = true };

        private ModRegistry registry = null!;
        private string configPath = "";

        // gets used on save calls and load calls to avoid creating new instances
        private PresetConfigFile backerConfig = new();

        /// <inheritdoc />
        public string ProviderName => "Basic Configuration (Default Provider)";

        /// <inheritdoc />
        public void Setup(string configFile, ModRegistry modRegistry)
        {
            registry = modRegistry;

            configPath = Path.Join([AppDomain.CurrentDomain.BaseDirectory, configFile]);
            if (!File.Exists(configPath))
                Save(); // we only call save during setup to format the file properly

            // since load is only called once, we call refresh to call load from here, avoiding having to use reflection
            modRegistry.Refresh();
        }

        /// <inheritdoc />
        public void Save()
        {
            if (configPath == null)
                throw new Exception("Configuration path was null, did you call \"Setup\"?");

            backerConfig.EnabledMods = [.. registry.GetEnabledMods().Select(mod => mod.Manifest.Name)];
            backerConfig.DisabledMods = [.. registry.GetDisabledMods().Select(mod => mod.Manifest.Name)];

            // yeah writes the whole file each time save gets called because this shi stinks lol
            string json = JsonSerializer.Serialize(backerConfig, jsonOptions);
            File.WriteAllText(configPath, json);
        }

        /// <inheritdoc />
        public ConfigFile Load()
        {
            string content = File.ReadAllText(configPath);
            backerConfig = JsonSerializer.Deserialize<PresetConfigFile>(content) ?? new PresetConfigFile();

            // TODO: Parse content more in-depth to avoid manipulating the file and missing the keys, tho it can be unnecessary
            // maybe the user wants to change something inside of it like refreshing the mod list or presets, yo thats a good idea im gonna implement it
            // 29/11/2024
            // 30/11/2024 - ok so im working on the idea now
            // 16/10/2025 - uhh took me a whole year to come back to im so sorry it took me so long to finish such a good idea bruh
            // 17/10/2025 - idk what i meant by that first comment but uhh idk yeah

            return backerConfig;
        }
    }
}
