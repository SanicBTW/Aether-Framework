using AetherFramework.Attributes;
using AetherFramework.Data;
using System.Diagnostics;
using System.Reflection;

namespace AetherFramework
{
    // https://github.com/kwfnf/kfunkin/blob/master/source/kfunkin/modding/ClassRegistry.hx
    // I spent like 5 hours on this alone because of generics and more bs

    /// <summary>
    /// Represents the registry of classes that may be overwritten by mods.
    /// </summary>
    public static class ClassRegistry
    {
        private static readonly Dictionary<Type, ClassRegistryItem> classMap = [];

        /// <summary>
        /// Registers all classes marked with <see cref="OverridableClassAttribute"/>.
        /// </summary>
        /// <param name="assembly">The assembly to scan for overridable classes.</param>
        public static void RegisterOverridableClasses(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                OverridableClassAttribute attrib = type.GetCustomAttribute<OverridableClassAttribute>()!;

                if (attrib != null)
                {
                    // If the type is already registered, continue to the next one
                    if (classMap.ContainsKey(type))
                        continue;

                    CreateRegistry(getInstanceFromAttrib(attrib, type));
                }
            }
        }

        /// <summary>
        /// Register a class to the registry.
        /// </summary>
        /// <typeparam name="T">The type of class to register.</typeparam>
        /// <param name="cls">The default class to register.</param>
        /// <returns>A ClassRegistryItem for the registered class.</returns>
        public static ClassRegistryItem CreateRegistry<T>(T cls)
        {
            ClassRegistryItem reg = new(cls!);
            classMap[cls!.GetType()] = reg;
            return reg;
        }

        /// <summary>
        /// Reset all the classes to their original state.
        /// </summary>
        public static void ResetClasses()
        {
            foreach (ClassRegistryItem reg in classMap.Values)
            {
                reg.ResetClass();
            }
        }

        /// <summary>
        /// Gets all the class registries.
        /// </summary>
        /// <returns>A collection of ClassRegistryItems.</returns>
        public static ClassRegistryItem[] GetRegistries()
        {
            List<ClassRegistryItem> ret = [];
            foreach (ClassRegistryItem? reg in classMap.Values)
            {
                ret.Add(reg);
            }

            return [.. ret];
        }

        /// <summary>
        /// Create instances of a class (using ClassRegistry, falls back to default if not found).
        /// </summary>
        /// <param name="args">The arguments to pass to the constructor.</param>
        /// <returns>An instance of the specified class.</returns>
        public static T CreateInstance<T>(params object?[]? args) where T : class
        {
            classMap.TryGetValue(typeof(T), out ClassRegistryItem? reg);
            return (T)(reg?.CreateInstance(args) ?? Activator.CreateInstance(typeof(T), args))!;
        }

        public static event Action<ClassRegistryItem> ClassOverwritten = null!;

        /// <summary>
        /// Overwrite a class with a new class.
        /// </summary>
        /// <param name="from">The class to overwrite.</param>
        /// <param name="to">The class to overwrite with.</param>
        public static void OverwriteClass<From, To>() where From : class where To : class
        {
            classMap.TryGetValue(typeof(From), out ClassRegistryItem? reg);
            reg?.SetClass(createInstanceSmart<To>(reg!));
            ClassOverwritten?.Invoke(reg!);
        }

        // its not really "smart" its only a hacky way to create instances when there are no arguments 
        // and the constructor requires them, i should totally remove the possibility to create classes
        // with arguments in the constructor BUT thats only if this is too performance killer
        private static object createInstanceSmart<T>(ClassRegistryItem from) where T : class
        {
            // get the default class type since thats what we are overriding and that base class has the attribute for the constructor arguments list
            Type def = from.GetDefaultClass().GetType();

            OverridableClassAttribute attrib = def.GetCustomAttribute<OverridableClassAttribute>()!;
            Debug.Assert(attrib != null, "Failed to get the base type \"OverridableClass\" attribute, maybe it isn't the base class?");

            return getInstanceFromAttrib<T>(attrib);
        }

        private static object getInstanceFromAttrib<T>(OverridableClassAttribute attrib, T? targetType = null!) where T : class
        {
            // just to make sure the user doesnt pass a null attribute
            Debug.Assert(attrib != null, "Passed a null \"OverridableClass\" attribute.");

            // TODO: Look into this casting and a better way to get the type with or without passing a variable to help the reflection
            Type type = (targetType as Type) ?? typeof(T);
            object instance;

            // If the Attribute provides the types the constructor requires, make some sketchy work here
            if (attrib.Arguments != null && attrib.Arguments.Length > 0)
            {
                // basically this is used to cast the arguments from the attribute (objects) to usable types for reflection
                Type[] types = new Type[attrib.Arguments.Length];
                // this is used to add nulls to match the length of the types so when calling invoke on the constructor it doesnt crash
                object[] args = new object[types.Length];

                for (int i = 0; i < attrib.Arguments.Length; i++)
                {
                    types[i] = (Type)attrib.Arguments[i]!;
                    args[i] = null!;
                }

                instance = type.GetConstructor(types)!.Invoke(args);
            }
            else // no arguments needed in constructor, all good
                instance = Activator.CreateInstance(type)!;

            return instance;
        }
    }
}
