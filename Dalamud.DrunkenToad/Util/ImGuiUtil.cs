using System.Numerics;

using Dalamud.Interface.Colors;

namespace Dalamud.DrunkenToad
{
    /// <summary>
    /// Utility functions for working with imgui.
    /// </summary>
    public static class ImGuiUtil
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
                < 25 => ImGuiColors.ParsedGrey,
                >= 25 and <= 49 => ImGuiColors.ParsedGreen,
                >= 50 and <= 74 => ImGuiColors.ParsedBlue,
                >= 75 and <= 94 => ImGuiColors.ParsedPurple,
                >= 95 and <= 98 => ImGuiColors.ParsedOrange,
                99 => ImGuiColors.ParsedPink,
                _ => ImGuiColors.ParsedGold,
            };
        }
    }
}
