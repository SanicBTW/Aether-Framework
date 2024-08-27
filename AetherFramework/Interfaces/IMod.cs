using AetherFramework.Data;

namespace AetherFramework.Interfaces
{
    /// <summary>
    /// Interface that is required to mark a Mod as loadable when loading the Assemblies.
    /// </summary>
    public interface IMod
    {
        /// <summary>
        /// The <see cref="ModManifest"/> the Mod Registry will use to identify the mod.
        /// </summary>
        ModManifest Manifest { get; }

        /// <inheritdoc cref="ModManifest.Intents"/>
        HashSet<string> Intents => Manifest.Intents;

        /// <summary>
        /// Called when the <see cref="IModEngine"/> enables this <see cref="IMod"/>.
        /// It's recommended to register events or classes here.
        /// </summary>
        void OnEnable();

        /// <summary>
        /// Called when the <see cref="IModEngine"/> disables this <see cref="IMod"/>.
        /// It's recommended to clean up previously registered events or classes here.
        /// </summary>
        void OnDisable();
    }
}
