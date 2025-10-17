using AetherFramework.Interfaces;
using JetBrains.Annotations;

namespace AetherFramework.Data
{
    /// <summary>
    /// The Manifest Information the mod will use for the <see cref="ModRegistry"/>.
    /// </summary>
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.Members)]
    public record ModManifest
    {
        /// <summary>
        /// The name of the mod, depending on the <see cref="IModConfigProvider"/> used, it may not allow some characters.
        /// </summary>
        public string Name { get; set; } = "ModManifest";

        /// <summary>
        /// The description of the mod, only meant to be used for user interfaces.
        /// </summary>
        public string Description { get; set; } = "The base of a Mod Manifest";

        /// <summary>
        /// The author of the mod, only meant to be used for user interfaces.
        /// </summary>
        public string Author { get; set; } = "sanco";

        /// <summary>
        /// A list of intents to let the <see cref="EventManager"/> dispatch targeted mods more efficiently.
        /// <remarks>
        /// Use the intents offered by the application you're working with.
        /// </remarks>
        /// </summary>
        public HashSet<string> Intents { get; set; } = [];

        // dawg??

        /// <summary>
        /// The version of the mod, not really used in here but you can in your application.
        /// </summary>
        public Version Version { get; set; } = new(0, 0, 0, 0);
    }
}
