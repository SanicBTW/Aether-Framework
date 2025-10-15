namespace AetherFramework.Configuration
{
    /// <summary>
    /// Class that details the structure of a configuration file.
    /// </summary>
    public class ConfigFile
    {
        /// <summary>
        /// The enabled mods of this configuration file.
        /// </summary>
        public List<string> EnabledMods { get; set; } = [];

        /// <summary>
        /// The disabled mods of this configuration file.
        /// </summary>
        public List<string> DisabledMods { get; set; } = [];
    }
}
