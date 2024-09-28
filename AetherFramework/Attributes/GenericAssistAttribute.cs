namespace AetherFramework.Attributes
{
    /// <summary>
    /// Attribute used to assist <see cref="ClassRegistry"/> on generic instances.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class GenericAssistAttribute : Attribute
    {
        /// <summary>
        /// The types used when this generic override is created.
        /// </summary>
        public Type[]? Types { get; }

        /// <summary>
        /// Add this <see cref="Attribute"/> to assist the <see cref="ClassRegistry"/> when creating a new generic instance.
        /// </summary>
        /// <param name="types">The types you will use when creating this generic override.</param>
        public GenericAssistAttribute(params Type[]? types)
        {
            Types = types;
        }
    }
}
