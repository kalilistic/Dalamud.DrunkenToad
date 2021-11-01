using System;
using System.Numerics;
using System.Text;

using Dalamud.Interface.Colors;
using ImGuiNET;

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

        /// <summary>
        /// Begin tab item with label and flags.
        /// </summary>
        /// <param name="label">label.</param>
        /// <param name="flags">flags.</param>
        /// <returns>tab item.</returns>
        public static unsafe bool BeginTabItem(string label, ImGuiTabItemFlags flags)
        {
            var unterminatedLabelBytes = Encoding.UTF8.GetBytes(label);
            var labelBytes = stackalloc byte[unterminatedLabelBytes.Length + 1];
            fixed (byte* unterminatedPtr = unterminatedLabelBytes)
            {
                Buffer.MemoryCopy(unterminatedPtr, labelBytes, unterminatedLabelBytes.Length + 1, unterminatedLabelBytes.Length);
            }

            labelBytes[unterminatedLabelBytes.Length] = 0;

            var num2 = (int)ImGuiNative.igBeginTabItem(labelBytes, null, flags);
            return (uint)num2 > 0U;
        }
    }
}
