using AetherFramework.Interfaces;

namespace AetherFramework.Events
{
    /// <summary>
    /// Represents an event that is targeted at a specific mod.
    /// Inherits from the base Event class and adds a reference to the target mod.
    /// </summary>
    public class TargetedEvent : Event
    {
        /// <summary>
        /// Gets the mod that this event is targeted at.
        /// </summary>
        public IMod? TargetMod { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetedEvent"/> class.
        /// </summary>
        /// <param name="targetMod">The mod that this event is targeted at, if null, use <see cref="EventManager.TriggerEventByIntent(TargetedEvent, string)"/>.</param>
        public TargetedEvent(IMod? targetMod = null!)
        {
            // this had a null check but since we are using null for intent targeted events, there is no more null check!

            TargetMod = targetMod;
        }
    }
}
