namespace AetherFramework.Data
{
    // https://github.com/kwfnf/kfunkin/blob/master/source/kfunkin/modding/ClassRegistryItem.hx
    // this class was meant to be generic like mki's haxe implementation but failed horribly due to casting on C# and losing the generic data on the way
    // of creating the registry item, so i just decided to mark the variables as objects since thats what everything in .net extends to (apparently)

    // 1.1.0 here, I figured out that I can save the class type without having to create an instance to save the default value of it, since
    // the registry is always gonna create a new one based on the "Current Class (an instance)" type, so saving an instance and getting the type from it
    // its kind of dumb, maybe with this I can reduce allocations!!

    /// <summary>
    /// Represents a class that can be overwritten.
    /// </summary>
    public class ClassRegistryItem
    {
        private Type defaultClass;
        private Type currentClass;

        /// <summary>
        /// Creates a new instance of ClassRegistryItem with a default class type.
        /// </summary>
        /// <param name="def">The default class type.</param>
        public ClassRegistryItem(Type def)
        {
            defaultClass = def;
            currentClass = def;
        }

        /// <summary>
        /// Gets the current class type of the item.
        /// </summary>
        /// <returns>The current class type of the item.</returns>
        public Type GetCurrentClass() => currentClass!;

        /// <summary>
        /// Gets the default class type of the item.
        /// </summary>
        /// <returns>The default class type of the item.</returns>
        public Type GetDefaultClass() => defaultClass!;

        /// <summary>
        /// Sets the class type of the item.
        /// </summary>
        /// <param name="cls">The class type of the item.</param>
        /// <returns>The new class type of the item.</returns>
        public Type SetClass(Type cls) => currentClass = cls;

        /// <summary>
        /// Resets the class type of the item to the default one.
        /// </summary>
        public void ResetClass() => currentClass = defaultClass;

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="args">The arguments to pass to the constructor.</param>
        /// <returns>The new instance of the class.</returns>
        public object CreateInstance(params object?[]? args) => Activator.CreateInstance(currentClass, args)!;
    }
}
