using AetherFramework.Configuration;
using AetherFramework.Interfaces;
using System.Diagnostics;
using System.Reflection;

namespace AetherFramework
{
    /// <summary>
    /// A Registry that holds mods with a <see cref="IModConfigProvider"/> to save changes to the disk.
    /// </summary>
    public class ModRegistry
    {
        private readonly IModConfigProvider _config = null!;

        private readonly List<IMod> mods = [];
        private List<string> enabledMods = [];
        private List<string> disabledMods = [];

        /// <summary>
        /// Creates a Mod Registry to use in a <see cref="IModEngine"/>.
        /// </summary>
        /// <param name="configFile">The configuration filename the <paramref name="config"/> will use.</param>
        /// <param name="config">A custom <see cref="IModConfigProvider"/> to use in this Registry.</param>
        public ModRegistry(string configFile, IModConfigProvider ?config = null)
        {
            _config ??= config ?? new BasicConfig();
            _config.Setup(configFile, this);
            EventManager.AddModRegistry(this);
        }

        public void RegisterMod(IMod newMod)
        {
            if (mods.Contains(newMod))
                return;

            // Pipe is not allowed to be inside the mod's name because it is used for splitting and joining in the config file.
            if (newMod.Manifest.Name.Contains('|'))
                newMod.Manifest.Name = newMod.Manifest.Name.Replace('|', ' ');

            mods.Add(newMod);
            Debug.WriteLine($"Registered new mod! {newMod.Manifest.Name} by {newMod.Manifest.Author}, Version {newMod.Manifest.Version}.");

            // No configuration file found, no mods loaded, enable all of them / new mod not found in the config file
            if (!enabledMods.Contains(newMod.Manifest.Name) && !disabledMods.Contains(newMod.Manifest.Name))
            {
                // this already calls onEnable, so we return to avoid going to the next part of the code
                EnableMod(newMod.Manifest.Name);
                return;
            }

            // Since the lists get populated upon config loading, it makes sense to call the
            // necessary callbacks so we dont call "Enable/DisableMod" triggering stuff incorrectly
            if (disabledMods.Contains(newMod.Manifest.Name))
                newMod.OnDisable();
            else
                newMod.OnEnable();
        }

        public IEnumerable<IMod> GetEnabledMods() => mods.Where((mod) => enabledMods.Contains(mod.Manifest.Name));

        public IMod EnableMod(string modName)
        {
            IMod target = EnsureRegistry(modName);

            disabledMods.Remove(modName);

            if (!enabledMods.Contains(modName))
            {
                enabledMods.Add(modName);

                // Apparently I had an issue where this was outside of this if condition and it would run 3 times in a single call, a lil bit wild if you ask me
                _config.Save();
                target.OnEnable();
            }

            return target;
        }

        public IEnumerable<IMod> GetDisabledMods() => mods.Where((mod) => disabledMods.Contains(mod.Manifest.Name));

        public IMod DisableMod(string modName)
        {
            IMod target = EnsureRegistry(modName);

            enabledMods.Remove(modName);

            if (!disabledMods.Contains(modName))
            {
                disabledMods.Add(modName);
                _config.Save();
                target.OnDisable();
            }

            return target;
        }

        public IModConfigProvider GetConfigProvider() => _config;

        public IEnumerable<IMod> GetModsByIntent(string intent) => mods.Where(mod => mod.Intents.Contains(intent));

        private IMod EnsureRegistry(string modName)
        {
            IMod mod = mods.Where((mod) => mod.Manifest.Name == modName).First();
            return mod ?? throw new Exception($"Requested mod \"{modName}\" couldn't be found on the registry, maybe it didn't get registered?");
        }

        // Accessed through reflection, uses reflection inside, profit!!!
        protected void dynamicSetList(string fieldName, IEnumerable<string> value)
        {
            Type type = typeof(ModRegistry);
            FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!;

            Debug.Assert(field != null, "Reflection failed.");

            // "Cannot convert string[] to List<string>" when skipping this step
            List<string> conv = value.ToList();
            field.SetValue(this, conv);

            Debug.Assert(field.GetValue(this) == conv, "Reflection assign failed.");
        }
    }
}
