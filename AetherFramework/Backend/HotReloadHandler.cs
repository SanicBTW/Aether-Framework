using AetherFramework.Backend;
using AetherFramework.Events;

// https://www.meziantou.net/supporting-hot-reload-in-your-dotnet-application.htm

[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(HotReloadHandler))]

namespace AetherFramework.Backend
{
    /// <summary>
    /// A Hot Reload handler to get related events and use them across the Framework, while also dispatching them on the mods.
    /// </summary>
    internal static class HotReloadHandler
    {
        private static void clearCache(Type[]? types)
            => EventManager.TriggerGlobalEvent(new HotReloadEvent(types));

        private static void updateApplication(Type[]? types)
            => EventManager.TriggerGlobalEvent(new HotReloadEvent(newTypes: types));
    }
}
