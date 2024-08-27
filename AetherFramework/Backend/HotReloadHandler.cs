using AetherFramework.Backend;

// https://www.meziantou.net/supporting-hot-reload-in-your-dotnet-application.htm

[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(HotReloadHandler))]

namespace AetherFramework.Backend
{
    /// <summary>
    /// A Hot Reload handler to get related events and use them across the Framework, while also dispatching them on the mods.
    /// </summary>
    internal static class HotReloadHandler
    {
        public static event Action<Type[]?> OnCacheClear = null!;
        public static event Action<Type[]?> OnHotReload = null!;

        private static void ClearCache(Type[]? types)
        {
            OnCacheClear?.Invoke(types);
        }

        private static void UpdateApplication(Type[]? types)
        {
            OnHotReload?.Invoke(types);
        }
    }
}
