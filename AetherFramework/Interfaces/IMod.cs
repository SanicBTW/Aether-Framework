using AetherFramework.Data;
using AetherFramework.Engines;

namespace AetherFramework.Interfaces
{
    /// <summary>
    /// Interface that is required to mark a mod as loadable when loading the assemblies through an <see cref="AssemblyEngine"/>, 
    /// implementation may vary between engines.
    /// </summary>
    public interface IMod
    {
        /// <summary>
        /// The <see cref="ModManifest"/> the <see cref="ModRegistry"/> will use to identify the mod.
        /// </summary>
        ModManifest Manifest { get; }

        /// <inheritdoc cref="ModManifest.Intents"/>
        HashSet<string> Intents => Manifest.Intents;

        /// <summary>
        /// Called when the <see cref="IModEngine"/> enables this <see cref="IMod"/>.
        /// <remarks>
        /// It's recommended to register events or classes here.
        /// </remarks>
        /// </summary>
        void OnEnable();

        /// <summary>
        /// Called when the <see cref="IModEngine"/> disables this <see cref="IMod"/>.
        /// <remarks>
        /// It's recommended to clean up previously registered events or classes here.
        /// </remarks>
        /// </summary>
        void OnDisable();
    }
}
