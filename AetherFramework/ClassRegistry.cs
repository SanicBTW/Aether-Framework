using AetherFramework.Attributes;
using AetherFramework.Data;
using AetherFramework.Events;
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
                // Since we aren't using the attribute anymore, we don't need to assign the attribute to a variable 
                if (type.GetCustomAttribute<OverridableClassAttribute>() != null)
                {
                    // To avoid extra work in both places when creating a generic class, we use "GenericAssistAttribte" to get the possible types the developer will
                    // use when creating the generic class
                    if (type.IsGenericType)
                    {
                        GenericAssistAttribute genAssistant = type.GetCustomAttribute<GenericAssistAttribute>()!;
                        // If no attribute was found or if the attribute was found but no types were passed, use the old method of creating generic overrides
                        if (genAssistant == null || genAssistant.Types == null)
                        {
                            TryCreateGenericRegistry(type);
                            continue;
                        }

                        foreach (Type genType in genAssistant.Types)
                        {
                            Type newType = type.MakeGenericType(genType);

                            // Check if the constructed generic type is already registered
                            if (classMap.ContainsKey(newType))
                                continue;

                            CreateRegistry(newType);
                        }
                    }
                    else
                    {
                        // If the type is already registered, continue to the next one
                        if (classMap.ContainsKey(type))
                            continue;

                        CreateRegistry(type);
                    }
                }
            }
        }

        /// <summary>
        /// Register a class to the registry.
        /// </summary>
        /// <param name="classType">The default class type to register.</param>
        /// <returns>A ClassRegistryItem for the registered class type.</returns>
        public static ClassRegistryItem CreateRegistry(Type classType)
        {
            ClassRegistryItem reg = new(classType);
            classMap[classType] = reg;
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
            // No heavy generic work here since when passing the T it already passes the generic type ([Type1][Type2]) so we can use that directly
            Type target = typeof(T);
            classMap.TryGetValue(target, out ClassRegistryItem? reg);
            return (T)(reg?.CreateInstance(args) ?? Activator.CreateInstance(target, args))!;
        }

        /// <summary>
        /// Overwrite a class with a new class.
        /// </summary>
        /// <typeparam name="From">The class to overwrite.</typeparam>
        /// <typeparam name="To">The class to overwrite with.</typeparam>
        public static void OverwriteClass<From, To>() where From : class where To : class
        {
            classMap.TryGetValue(typeof(From), out ClassRegistryItem? reg);
            if (reg != null)
            {
                reg.SetClass(typeof(To));
                EventManager.TriggerGlobalEvent(new ClassOverwrittenEvent(reg));
            }
        }

        // OLD GENERIC CODE, SHOULD DEFINITELY AVOID THIS, PLEASE USE THE GENERIC ASSIST ATTRIBUTE
        // Since the generic classes CAN have multiple type parameters or something, we check for EACH generic argument in the type
        // I think this doesn't account for those type of classes where the generic type isn't assigned to a known type
        // Should totally look into it but for now it works on known types which is a big step taken
        // In future versions, a source generator should take care of this properly 
        private static void TryCreateGenericRegistry(Type type)
        {
            if (!type.ContainsGenericParameters)
                throw new InvalidOperationException("Cannot continue creating this generic class.");

            Type[] genericArgs = type.GetGenericArguments();
            foreach (Type genType in genericArgs)
            {
                if (genType.BaseType == null)
                    continue;

                Type newType = type.MakeGenericType(genType.BaseType);

                // Check if the constructed generic type is already registered
                if (classMap.ContainsKey(newType))
                    continue;

                CreateRegistry(newType);
            }
        }
    }
}
