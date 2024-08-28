using AetherFramework.Interfaces;
using System.Diagnostics;
using System.Reflection;

namespace AetherFramework.Configuration
{
    /// <summary>
    /// A <see cref="IModConfigProvider"/> for saving configuration in memory without writing to disk.
    /// </summary>
    public class TempConfig : IModConfigProvider
    {
        private ModRegistry registry = null!;
        private Dictionary<string, IEnumerable<string>> mods = [];

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
            mods["enabled"] = registry.GetEnabledMods().Select((mod) => mod.Manifest.Name);
            mods["disabled"] = registry.GetDisabledMods().Select((mod) => mod.Manifest.Name);
        }

        /// <inheritdoc cref="IModConfigProvider.Load"/>
        public void Load()
        {
            Type type = typeof(ModRegistry);
            MethodInfo method = type.GetMethod("dynamicSetList", BindingFlags.Instance | BindingFlags.NonPublic, [typeof(string), typeof(IEnumerable<string>)])!;

            Debug.Assert(method != null, "Reflection failed.");

            method!.Invoke(registry, ["enabledMods", mods["enabled"]]);
            method!.Invoke(registry, ["disabledMods", mods["disabled"]]);
        }

        /// <inheritdoc cref="IModConfigProvider.GetConfigType"/>
        public string GetConfigType() => "Temporary Configuration (Saved in memory)";
    }
}
