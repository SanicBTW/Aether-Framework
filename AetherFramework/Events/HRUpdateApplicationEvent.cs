namespace AetherFramework.Events
{
    /// <summary>
    /// Hot Reload Update Application Event
    /// </summary>
    public class HRUpdateApplicationEvent : Event
    {
        public readonly Type[] NewTypes;

        public HRUpdateApplicationEvent(Type[] types)
        {
            NewTypes = types;
        }
    }
}
