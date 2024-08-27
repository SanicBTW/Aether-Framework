namespace AetherFramework.Data
{
    // https://github.com/kwfnf/kfunkin/blob/master/source/kfunkin/modding/ClassRegistryItem.hx
    // this class was meant to be generic like mki's haxe implementation but failed horribly due to casting on C# and losing the generic data on the way
    // of creating the registry item, so i just decided to mark the variables as objects since thats what everything in .net extends to (apparently)

    /// <summary>
    /// Represents a class that can be overwritten.
    /// </summary>
    public class ClassRegistryItem
    {
        private object defaultClass;
        private object currentClass;

        /// <summary>
        /// Creates a new instance of ClassRegistryItem with a default value.
        /// </summary>
        /// <param name="def">The default value of the class.</param>
        public ClassRegistryItem(object def)
        {
            defaultClass = def;
            currentClass = def;
        }

        /// <summary>
        /// Gets the current class of the item.
        /// </summary>
        /// <returns>The current class of the item.</returns>
        public object GetCurrentClass() => currentClass!;

        /// <summary>
        /// Gets the default class of the item.
        /// </summary>
        /// <returns>The default class of the item.</returns>
        public object GetDefaultClass() => defaultClass!;

        /// <summary>
        /// Sets the class of the item.
        /// </summary>
        /// <param name="cls">The class of the item.</param>
        /// <returns>The class of the item.</returns>
        public object SetClass(object cls) => currentClass = cls;

        /// <summary>
        /// Resets the class of the item to the default.
        /// </summary>
        public void ResetClass() => currentClass = defaultClass;

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="args">The arguments to pass to the constructor.</param>
        /// <returns>The new instance of the class.</returns>
        public object CreateInstance(params object?[]? args) => Activator.CreateInstance(currentClass.GetType(), args)!;
    }
}
