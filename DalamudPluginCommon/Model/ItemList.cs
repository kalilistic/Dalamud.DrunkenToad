namespace DalamudPluginCommon
{
    /// <summary>
    /// Item list.
    /// </summary>
    public class ItemList
    {
        /// <summary>
        /// Gets or sets item ids.
        /// </summary>
        public uint[] ItemIds { get; set; } = null!;

        /// <summary>
        /// Gets or sets item names.
        /// </summary>
        public string[] ItemNames { get; set; } = null!;
    }
}