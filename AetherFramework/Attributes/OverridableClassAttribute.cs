namespace AetherFramework.Attributes
{
    /// <summary>
    /// Attribute used to mark a class as "Overridable" in the <see cref="ClassRegistry"/> while being loaded through <see cref="ClassRegistry.RegisterOverridableClasses(System.Reflection.Assembly)"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OverridableClassAttribute : Attribute
    {
        /// <summary>
        /// An array of <see cref="Type"/>s to help the <see cref="ClassRegistry"/> create classes with arguments.
        /// </summary>
        public object?[]? Arguments { get; }

        /// <summary>
        /// Add this <see cref="Attribute"/> to a class to mark it as "Overridable" by the Modding Framework.
        /// </summary>
        /// <param name="args">The arguments of the class that is gonna be overriden.</param>
        public OverridableClassAttribute(params object?[]? args)
        {
            Arguments = args;
        }
    }
}
