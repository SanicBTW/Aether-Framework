namespace AetherFramework.Events
{
    /// <summary>
    /// Hot Reload Cache Clear Event
    /// </summary>
    public class HRCacheClearEvent : Event
    {
        public readonly Type[] ClearTypes;

        public HRCacheClearEvent(Type[] types)
        {
            ClearTypes = types;
        }
    }
}
