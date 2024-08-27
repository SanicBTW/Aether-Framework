namespace AetherFramework.Events
{
    /// <summary>
    /// Represents the base class for all events in the Modding Framework.
    /// Provides support for event cancellation and adjustments.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Gets a value indicating whether the event has been cancelled.
        /// </summary>
        public bool IsCancelled { get; private set; } = false;

        /// <summary>
        /// Cancels the event, preventing further dispatching.
        /// </summary>
        public virtual void Cancel()
        {
            IsCancelled = true;
        }

        /// <summary>
        /// Adjusts the event. Can be overridden to provide custom adjustment logic.
        /// </summary>
        public virtual void Adjust() { }
    }
}
