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
    }
}
