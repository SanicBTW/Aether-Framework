using JetBrains.Annotations;

namespace AetherFramework.Events
{
    /// <summary>
    /// Hot Reload Event.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class HotReloadEvent : Event
    {
        /// <summary>
        /// An array of <see cref="Type"/>s that are passed by the <see cref="Backend.HotReloadHandler.OnCacheClear"/> event.
        /// </summary>
        public readonly Type[]? ClearTypes;

        /// <summary>
        /// An array of <see cref="Type"/>s that are passed by the <see cref="Backend.HotReloadHandler.OnHotReload"/> event.
        /// </summary>
        public readonly Type[]? NewTypes;

        /// <summary>
        /// Creates a new global Hot Reload event.
        /// </summary>
        /// <param name="clearTypes">The types that are targeted to clear their cache.</param>
        /// <param name="newTypes">The types that were updated.</param>
        public HotReloadEvent(Type[]? clearTypes = null, Type[]? newTypes = null)
        {
            ClearTypes = clearTypes;
            NewTypes = newTypes;
        }
    }
}
