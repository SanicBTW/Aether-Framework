using AetherFramework.Events;
using AetherFramework.Interfaces;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace AetherFramework
{
    // chat gpt cooked all of the event code (with my assistance) since i already ran out of ideas at
    // the moment im working on the events system, also the comments are made by chat gpt lmao mb
    // (a day later) its actually a mix between prompts and my own cooked code,
    // apparently hash sets are faster sooo https://stackoverflow.com/a/10762995 we moving to them now (also keeping thread safety!!!)

    public enum EventRegistryType
    {
        GLOBAL,
        TARGETED
    }

    /// <summary>
    /// Manages the registration and triggering of events, including thread safety.
    /// Supports both global events and events targeted at specific mods.
    /// </summary>
    public static class EventManager
    {
        // Dictionary to store global event handlers by event type
        private static readonly ConcurrentDictionary<Type, HashSet<Delegate>> globalEvents = [];

        // Dictionary to store targeted event handlers by mod and event type
        // Maybe instead of saving mods, we save the intent of the mod?
        private static readonly ConcurrentDictionary<IMod, ConcurrentDictionary<Type, HashSet<Delegate>>> targetedEvents = [];

        // HashSet of all the mod registries created in the whole modding framework.
        private static readonly HashSet<ModRegistry> modRegistries = [];

        // For logging and monitoring
        private static Action<string> logger = msg => Debug.WriteLine(msg); // Default logger to the Debug Output

        /// <summary>
        /// Registers a new event handler in the Manager.
        /// </summary>
        /// <typeparam name="T">The type of event to handle.</typeparam>
        /// <param name="registry">Where to save the event handlers.</param>
        /// <param name="handler">The method that handles the event.</param>
        /// <param name="mod">
        ///     <para> The mod that the event is targeted at. </para>
        ///     Required argument when the <paramref name="registry"/> is <see cref="EventRegistryType.TARGETED"/>.
        /// </param>
        public static void Register<T>(EventRegistryType registry, Action<T> handler, IMod? mod = null!) where T : Event
        {
            ArgumentNullException.ThrowIfNull(handler);
            if (registry == EventRegistryType.TARGETED)
                ArgumentNullException.ThrowIfNull(mod);

            switch (registry)
            {
                case EventRegistryType.GLOBAL:
                    RegisterGlobalHandler(handler);
                    break;

                case EventRegistryType.TARGETED:
                    RegisterTargetedHandler(mod!, handler);
                    break;
            }
        }

        /// <summary>
        /// Unregisters a previously registered event in the Manager.
        /// </summary>
        /// <typeparam name="T">The type of event to unregister.</typeparam>
        /// <param name="registry">Place where the event should be unregistered.</param>
        /// <param name="handler">The method that handles the event.</param>
        /// <param name="mod">
        ///     <para> The mod that the event is targeted at. </para>
        ///     Required argument when the <paramref name="registry"/> is <see cref="EventRegistryType.TARGETED"/>.
        /// </param>
        public static void Unregister<T>(EventRegistryType registry, Action<T> handler, IMod? mod = null!) where T : Event
        {
            ArgumentNullException.ThrowIfNull(handler);
            if (registry == EventRegistryType.TARGETED)
                ArgumentNullException.ThrowIfNull(mod);

            switch (registry)
            {
                case EventRegistryType.GLOBAL:
                    UnregisterGlobalHandler(handler);
                    break;

                case EventRegistryType.TARGETED:
                    UnregisterTargetedHandler(mod!, handler);
                    break;
            }
        }

        /// <summary>
        /// Sets the logger action used for logging event-related activities.
        /// </summary>
        /// <param name="logger">The logger action.</param>
        public static void SetLogger(Action<string> logger) => logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Adds a new <see cref="ModRegistry"/> to the <see cref="EventManager"/> for <see cref="IMod"/> resolutions on event triggering.
        /// </summary>
        /// <param name="newRegistry">A new <see cref="ModRegistry"/> created from a <see cref="IModEngine"/>.</param>
        public static void AddModRegistry(ModRegistry newRegistry) => modRegistries.Add(newRegistry);

        /// <summary>
        /// Registers a handler for a global event of a specific type.
        /// </summary>
        /// <typeparam name="T">The type of event to handle.</typeparam>
        /// <param name="handler">The method that handles the event.</param>
        private static void RegisterGlobalHandler<T>(Action<T> handler) where T : Event
        {
            ArgumentNullException.ThrowIfNull(handler);

            Type eventType = typeof(T);
            // Add or update the global event handlers dictionary.
            globalEvents.AddOrUpdate(eventType,
                _ => [handler],
                (_, bag) =>
                {
                    lock (bag)
                    {
                        bag.Add(handler);
                    }
                    return bag;
                });

            logger?.Invoke($"Registered global event handler for {eventType.Name}.");
        }

        /// <summary>
        /// Unregisters a handler for a global event of a specific type.
        /// </summary>
        /// <typeparam name="T">The type of event to unregister.</typeparam>
        /// <param name="handler">The handler to remove.</param>
        private static void UnregisterGlobalHandler<T>(Action<T> handler) where T : Event
        {
            ArgumentNullException.ThrowIfNull(handler);

            Type eventType = typeof(T);

            if (!globalEvents.TryGetValue(eventType, out var handlers))
                return;

            lock (handlers)
            {
                handlers.Remove(handler);

                if (handlers.Count == 0)
                    globalEvents.TryRemove(eventType, out _);
            }

            logger?.Invoke($"Unregistered global event handler for {eventType.Name}.");
        }

        /// <summary>
        /// Triggers a global event, invoking all registered handlers.
        /// </summary>
        /// <param name="eventInstance">The event instance to trigger.</param>
        public static void TriggerGlobalEvent(Event eventInstance)
        {
            ArgumentNullException.ThrowIfNull(eventInstance);

            // Check if there are handlers registered for this event type
            if (!globalEvents.TryGetValue(eventInstance.GetType(), out var handlers))
                return;

            CallOnHandlers(handlers, eventInstance);
        }

        // these 2 methods should be like "where T : TargetedEvent" but when unifying it cries about it

        /// <summary>
        /// Registers a handler for an event targeted at a specific mod.
        /// </summary>
        /// <typeparam name="T">The type of event to handle.</typeparam>
        /// <param name="mod">The mod that the event is targeted at.</param>
        /// <param name="handler">The method that handles the event.</param>
        private static void RegisterTargetedHandler<T>(IMod mod, Action<T> handler) where T : Event
        {
            ArgumentNullException.ThrowIfNull(mod);
            ArgumentNullException.ThrowIfNull(handler);

            Type eventType = typeof(T);

            var modHandlers = targetedEvents.GetOrAdd(mod, _ => []);

            modHandlers.AddOrUpdate(eventType,
                _ => [handler],
                (_, bag) =>
                {
                    lock (bag)
                    {
                        bag.Add(handler);
                    }
                    return bag;
                });

            logger?.Invoke($"Registered targeted event handler for {eventType.Name} on mod {mod.Manifest.Name}.");
        }

        /// <summary>
        /// Unregisters a handler for an event targeted at a specific mod.
        /// </summary>
        /// <typeparam name="T">The type of event to unregister.</typeparam>
        /// <param name="mod">The mod that the event is targeted at.</param>
        /// <param name="handler">The handler to remove.</param>
        private static void UnregisterTargetedHandler<T>(IMod mod, Action<T> handler) where T : Event
        {
            ArgumentNullException.ThrowIfNull(mod);
            ArgumentNullException.ThrowIfNull(handler);

            if (!targetedEvents.TryGetValue(mod, out var modHandlers))
                return;

            Type eventType = typeof(T);

            if (!modHandlers.TryGetValue(eventType, out var handlers))
                return;

            lock (handlers)
            {
                handlers.Remove(handler);

                if (handlers.Count == 0)
                {
                    modHandlers.TryRemove(eventType, out _);

                    if (modHandlers.IsEmpty)
                        targetedEvents.TryRemove(mod, out _);
                }
            }

            logger?.Invoke($"Unregistered targeted event handler for {eventType.Name} on mod {mod.Manifest.Name}.");
        }

        /// <summary>
        /// Triggers a targeted event, invoking handlers for the specified mod.
        /// </summary>
        /// <param name="eventInstance">The targeted event instance to trigger.</param>
        public static void TriggerTargetedEvent(TargetedEvent eventInstance)
        {
            ArgumentNullException.ThrowIfNull(eventInstance);
            ArgumentNullException.ThrowIfNull(eventInstance.TargetMod);

            if (!targetedEvents.TryGetValue(eventInstance.TargetMod, out var modHandlers))
                return;

            if (!modHandlers.TryGetValue(eventInstance.GetType(), out var handlers))
                return;

            CallOnHandlers(handlers, eventInstance);
        }

        public static void TriggerEventByIntent(TargetedEvent eventInstance, string intent)
        {
            if (modRegistries.Count == 0)
            {
                logger?.Invoke($"There are no Mod Registries available");
                return;
            }

            ArgumentNullException.ThrowIfNull(eventInstance);
            ArgumentNullException.ThrowIfNull(intent);

            if (eventInstance.TargetMod != null)
            {
                logger?.Invoke($"Cannot dispatch a TargetedEvent by an Intent if the mod inside the TargetedEvent isn't null");
                return;
            }

            foreach (ModRegistry registry in modRegistries)
            {
                IEnumerable<IMod> targetMods = registry.GetModsByIntent(intent);
                foreach (IMod mod in targetMods)
                {
                    if (eventInstance is TargetedEvent targetedEvent)
                    {
                        targetedEvent.TargetMod = mod;
                        TriggerTargetedEvent(targetedEvent);
                    }
                    else
                    {
                        logger?.Invoke($"Event is not a TargetedEvent: {eventInstance.GetType()}");
                        break;
                    }

                    if (eventInstance.IsCancelled)
                        break;
                }
            }
        }

        // function made to avoid copy pasting
        private static void CallOnHandlers(HashSet<Delegate> handlers, Event eventInstance)
        {
            foreach (Delegate handler in handlers)
            {
                try
                {
                    handler.DynamicInvoke(eventInstance);

                    // If the event has been cancelled, stop further propagation
                    if (eventInstance.IsCancelled)
                        break;
                }
                catch (Exception ex)
                {
                    logger?.Invoke($"Exception while dispatching handlers for {eventInstance.GetType().Name}: {ex}");
                }
            }
        }
    }
}
