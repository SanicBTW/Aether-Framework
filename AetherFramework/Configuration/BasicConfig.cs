using System.Diagnostics;
using System.Reflection;
using AetherFramework.Interfaces;

namespace AetherFramework.Configuration
{
    // This code is some old code I did on the beginning of the modding framework, should totally rewrite it someday
    // not too much documented since its all dirty and stuff

    /// <summary>
    /// The default <see cref="IModConfigProvider"/> used when creating a new <see cref="ModRegistry"/>.
    /// </summary>
    public class BasicConfig : IModConfigProvider
    {
        private enum SAVE_TARGET
        {
            ENABLED_MODS_UPDATE,
            DISABLED_MODS_UPDATE,
            ALL
        }

        private const string FILE_HEADER = "// please do not remove this file as it serves as saving some engine configuration, like the enabled mods, disabled mods, etc...\n";
        private const char DELIMETER = '|';

        private ModRegistry registry = null!; // xd
        private string configPath = "";

        public void Setup(string configFile, ModRegistry registry)
        {
            this.registry = registry;

            configPath = Path.Join([AppDomain.CurrentDomain.BaseDirectory, configFile]);
            if (!File.Exists(configPath))
                File.WriteAllText(configPath, FILE_HEADER);

            save(SAVE_TARGET.ALL);
            Load();
        }

        public void Save()
        {
            if (configPath == null)
                throw new Exception("Configuration path was null, did you call \"Setup\"?");

            save(SAVE_TARGET.ENABLED_MODS_UPDATE);
            save(SAVE_TARGET.DISABLED_MODS_UPDATE);
        }

        private void save(SAVE_TARGET target)
        {
            string content = File.ReadAllText(configPath);
            if (target == SAVE_TARGET.ALL && (content.Length <= 0 || content == ""))
            {
                File.WriteAllText(configPath, FILE_HEADER);
                return;
            }

            IEnumerable<string> enabledMods = registry.GetEnabledMods().Select((mod) => mod.Manifest.Name);
            IEnumerable<string> disabledMods = registry.GetDisabledMods().Select((mod) => mod.Manifest.Name);

            string[] lines = content.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length <= 1)
            {
                content += $"enabled={string.Join(DELIMETER, enabledMods)}\n";
                content += $"disabled={string.Join(DELIMETER, disabledMods)}\n";
                lines = content.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            }

            foreach (string line in lines)
            {
                if (line.StartsWith("//"))
                    continue;

                string[] parts = line.Split("=");
                switch (target)
                {
                    case SAVE_TARGET.ENABLED_MODS_UPDATE:
                        if (parts[0] != "enabled")
                            continue;

                        string newLineEnabled = $"enabled={string.Join(DELIMETER, enabledMods)}\n";
                        content = content.Replace(line.Trim(), newLineEnabled.Trim());
                        break;

                    case SAVE_TARGET.DISABLED_MODS_UPDATE:
                        if (parts[0] != "disabled")
                            continue;

                        string newLineDisabled = $"disabled={string.Join(DELIMETER, disabledMods)}\n";
                        content = content.Replace(line.Trim(), newLineDisabled.Trim());
                        break;
                }
            }

            File.WriteAllText(configPath, content);
        }

        public void Load()
        {
            string content = File.ReadAllText(configPath);
            if (content.Length <= 0 || content == "")
                throw new Exception("Previously saved, content shoudn't be empty");

            string[] lines = content.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (line.StartsWith("//"))
                    continue;

                string[] parts = line.Split("=", StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length <= 0 || parts.Length <= 1)
                    continue;

                string key = parts[0];
                string value = parts[1];

                switch (key)
                {
                    case "enabled":
                        setOnRegistry("enabledMods", value.Split(DELIMETER, StringSplitOptions.RemoveEmptyEntries));
                        break;

                    case "disabled":
                        setOnRegistry("disabledMods", value.Split(DELIMETER, StringSplitOptions.RemoveEmptyEntries));
                        break;
                }
            }
        }

        public string GetConfigType() => "Basic Configuration (Default Provider)";

        private void setOnRegistry(string fieldName, IEnumerable<string> value)
        {
            Type type = typeof(ModRegistry);
            MethodInfo method = type.GetMethod("dynamicSetList", BindingFlags.Instance | BindingFlags.NonPublic, [typeof(string), typeof(IEnumerable<string>)])!;

            Debug.Assert(method != null, "Reflection failed.");

            method!.Invoke(registry, [fieldName, value]);
        }
    }
}
