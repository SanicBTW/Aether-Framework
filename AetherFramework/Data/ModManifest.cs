namespace AetherFramework.Data
{
    // previously i had an API Version variable but since I dont know how to do versioning and sshit I trashed the idea, wouldve been cool tho
    // knowing this, the api shouldnt change that much to make all the mods work properly across versions of the application

    /// <summary>
    /// The Manifest Information the Mod will use for the <see cref="ModRegistry"/>.
    /// </summary>
    public record ModManifest
    {
        /// <summary>
        /// The name of the mod, no "," allowed.
        /// </summary>
        public string Name { get; set; } = "ModManifest";

        /// <summary>
        /// The description of the mod.
        /// </summary>
        public string Description { get; set; } = "The base of a Mod Manifest";

        /// <summary>
        /// The author of the mod.
        /// </summary>
        public string Author { get; set; } = "sanco";

        /// <summary>
        /// A list of intents to let the <see cref="EventManager"/> dispatch targeted mods more efficiently.
        /// <para>
        /// Use the intents offered by the application the Modding Framework was implemented into.
        /// </para>
        /// </summary>
        public HashSet<string> Intents { get; set; } = [];

        /// <summary>
        /// The version of the mod, if implemented it can be used to retrieve updates from some place.
        /// </summary>
        public Version Version { get; set; } = new(0, 0, 0, 0);
    }
}
