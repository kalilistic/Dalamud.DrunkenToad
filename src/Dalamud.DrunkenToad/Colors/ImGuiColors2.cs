using System.Numerics;

namespace Dalamud.DrunkenToad
{
    /// <summary>
    /// Class containing frequently used colors for easier reference.
    /// </summary>
    public static class ImGuiColors2
    {
        /// <summary>
        /// Gets yellow color.
        /// </summary>
        public static Vector4 ToadYellow { get; } = new (1f, 1f, .4f, 1f);

        /// <summary>
        /// Gets violet color.
        /// </summary>
        public static Vector4 ToadViolet { get; } = new (0.770f, 0.700f, 0.965f, 1.000f);

        /// <summary>
        /// Gets FFLogs grey.
        /// </summary>
        public static Vector4 Common { get; } = new (0.4f, 0.4f, 0.4f, 1f);

        /// <summary>
        /// Gets FFLogs green.
        /// </summary>
        public static Vector4 Uncommon { get; } = new (0.117f, 1f, 0f, 1f);

        /// <summary>
        /// Gets FFLogs blue.
        /// </summary>
        public static Vector4 Rare { get; } = new (0f, 0.439f, 1f, 1f);

        /// <summary>
        /// Gets FFLogs purple.
        /// </summary>
        public static Vector4 Epic { get; } = new (0.639f, 0.207f, 0.933f, 1f);

        /// <summary>
        /// Gets FFLogs orange.
        /// </summary>
        public static Vector4 Legendary { get; } = new (1f, 0.501f, 0f, 1f);

        /// <summary>
        /// Gets FFLogs pink.
        /// </summary>
        public static Vector4 Astounding { get; } = new (0.886f, 0.407f, 0.658f, 1f);

        /// <summary>
        /// Gets FFLogs yellow.
        /// </summary>
        public static Vector4 Artifact { get; } = new (0.898f, 0.8f, 0.501f, 1f);
    }
}
