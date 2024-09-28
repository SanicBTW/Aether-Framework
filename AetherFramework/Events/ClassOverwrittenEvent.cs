using AetherFramework.Data;

namespace AetherFramework.Events
{
    /// <summary>
    /// An event fired when <see cref="ClassRegistry.OverwriteClass{From, To}"/> is called and the overwrite is successful.
    /// </summary>
    public class ClassOverwrittenEvent : Event
    {
        /// <summary>
        /// The updated <see cref="ClassRegistryItem"/> from the <see cref="ClassRegistry"/>.
        /// </summary>
        public readonly ClassRegistryItem UpdatedItem;

        /// <summary>
        /// Creates a new global event for class overwrites.
        /// </summary>
        /// <param name="registryItem">The updated <see cref="ClassRegistryItem"/>.</param>
        public ClassOverwrittenEvent(ClassRegistryItem registryItem)
        {
            UpdatedItem = registryItem;
        }
    }
}
