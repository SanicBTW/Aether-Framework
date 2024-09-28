namespace AetherFramework.Attributes
{
    /// <summary>
    /// Attribute used to mark a class as "Overridable" in the <see cref="ClassRegistry"/> while being loaded through <see cref="ClassRegistry.RegisterOverridableClasses(System.Reflection.Assembly)"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OverridableClassAttribute : Attribute
    {
        /// <summary>
        /// Add this <see cref="Attribute"/> to a class to mark it as "Overridable" by the Modding Framework.
        /// </summary>
        public OverridableClassAttribute() { }
    }
}
