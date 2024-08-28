namespace AetherFramework.Events
{
    /// <summary>
    /// Hot Reload Cache Clear Event
    /// </summary>
    public class HRCacheClearEvent : Event
    {
        /// <summary>
        /// An array of <see cref="Type"/>s that are passed by the <see cref="Backend.HotReloadHandler.OnCacheClear"/> event.
        /// </summary>
        public readonly Type[] ClearTypes;

        /// <summary>
        /// Creates a new global Hot Reload Cache Clear event.
        /// </summary>
        /// <param name="types">The types that are targeted to clear their cache.</param>
        public HRCacheClearEvent(Type[] types)
        {
            ClearTypes = types;
        }
    }
}
