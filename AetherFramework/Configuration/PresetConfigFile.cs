using AetherFramework.Interfaces;

namespace AetherFramework.Configuration
{
    /// <summary>
    /// Class that details a <see cref="ConfigFile"/> WITH presets, used in <see cref="IModConfigProvider"/> to give proper typing to some functions.
    /// <para>If you don't plan on using presets, use <see cref="ConfigFile"/>.</para>
    /// </summary>
    public class PresetConfigFile : ConfigFile
    {
        // the way this works is like: load up the selected preset, THEN remove it from the preset dictionary, to avoid having duplicates, and optimize it?
        // the thing is that enabled/disabled mods lists contain the CURRENT mods in use, presets are only made to save up references to get applied later

        /// <summary>
        /// The current preset KEY used inside the <see cref="Presets"/> dictionary.
        /// <para>Note that when changing presets this key will be used in order to save the current set in the dictionary on a future save call.</para>
        /// </summary>
        public string CurrentPreset { get; set; } = "default";

        /// <summary>
        /// The presets saved on the configuration file.
        /// </summary>
        public Dictionary<string, ConfigFile> Presets { get; set; } = [];
    }
}
