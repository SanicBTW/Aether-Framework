namespace AetherFramework.Events
{
    /// <summary>
    /// Hot Reload Update Application Event
    /// </summary>
    public class HRUpdateApplicationEvent : Event
    {
        /// <summary>
        /// An array of <see cref="Type"/>s that are passed by the <see cref="Backend.HotReloadHandler.OnHotReload"/> event.
        /// </summary>
        public readonly Type[] NewTypes;

        /// <summary>
        /// Creates a new global Hot Reload Update Application event.
        /// </summary>
        /// <param name="types">The types that were updated.</param>
        public HRUpdateApplicationEvent(Type[] types)
        {
            NewTypes = types;
        }
    }
}
