using System.Numerics;

using Dalamud.Interface.Colors;

namespace Dalamud.DrunkenToad
{
    /// <summary>
    /// Utility functions for working with imgui colors.
    /// </summary>
    public static class ImGuiColorUtil
    {
        /// <summary>
        /// Get color by number (follows FFLogs ranks).
        /// </summary>
        /// <param name="num">number to use to determine color.</param>
        /// <returns>color.</returns>
        public static Vector4 GetColorByNumber(uint num)
        {
            return num switch
            {
                0 => ImGuiColors.DalamudWhite,
                < 25 => ImGuiColors2.Common,
                >= 25 and <= 49 => ImGuiColors2.Uncommon,
                >= 50 and <= 74 => ImGuiColors2.Rare,
                >= 75 and <= 94 => ImGuiColors2.Epic,
                >= 95 and <= 98 => ImGuiColors2.Legendary,
                99 => ImGuiColors2.Astounding,
                _ => ImGuiColors2.Artifact,
            };
        }
    }
}
