// ReSharper disable ArrangeMissingParentheses
// ReSharper disable InconsistentNaming
namespace Dalamud.DrunkenToad.Helpers;

using System;
using System.Collections.Generic;
using System.Numerics;
using Core;
using Dalamud.Interface.Utility;
using ImGuiNET;

/// <summary>
/// ImGui utility class.
/// </summary>
public static class ImGuiUtil
{
    private static readonly List<Vector4> ColorPalette = ImGuiHelpers.DefaultColorPalette(36);

    /// <summary>
    /// Convert a UI color to a Vector4.
    /// </summary>
    /// <param name="col">color.</param>
    /// <returns>vector4.</returns>
    public static Vector4 UiColorToVector4(uint col)
    {
        var fa = (col & 255) / 255f;
        var fb = (col >> 8 & 255) / 255f;
        var fg = (col >> 16 & 255) / 255f;
        var fr = (col >> 24 & 255) / 255f;
        return new Vector4(fr, fg, fb, fa);
    }

    /// <summary>
    /// Returns a Vector4 representing a legible font color that contrasts with the given background color.
    /// </summary>
    /// <param name="backgroundColor">
    /// The background color as a Vector4 (in RGBA format) against which the font color needs to
    /// be legible.
    /// </param>
    /// <returns>
    /// A Vector4 representing a legible font color (either black or white) that contrasts with the provided
    /// background color.
    /// </returns>
    public static Vector4 GetLegibleFontColor(Vector4 backgroundColor)
    {
        var luminance = 0.299f * backgroundColor.X + 0.587f * backgroundColor.Y + 0.114f * backgroundColor.Z;

        if (luminance > 0.5f)
        {
            return new Vector4(0, 0, 0, 1);
        }

        return new Vector4(1, 1, 1, 1);
    }

    /// <summary>
    /// Return scaled width for combo boxes.
    /// </summary>
    /// <param name="max">max width.</param>
    /// <returns>width.</returns>
    public static float CalcScaledComboWidth(float max)
    {
        var maxWidth = max * ImGuiHelpers.GlobalScale;
        var relativeWidth = ImGui.GetWindowSize().X / 2;
        return relativeWidth < maxWidth ? relativeWidth : maxWidth;
    }

    /// <summary>
    /// Check if scroll event has occurred.
    /// </summary>
    /// <param name="lastScrollPosition">last scroll position.</param>
    /// <param name="epsilon">epsilon.</param>
    /// <returns>indicator if scroll has occurred.</returns>
    public static bool CheckScrollEvent(ref float lastScrollPosition, float epsilon = 0.001f)
    {
        var scrolled = false;
        var currentScrollPosition = ImGui.GetScrollY();

        if (ImGui.IsWindowFocused() && ImGui.IsWindowHovered())
        {
            if (Math.Abs(currentScrollPosition - lastScrollPosition) > epsilon)
            {
                scrolled = true;
            }
        }

        lastScrollPosition = currentScrollPosition;
        return scrolled;
    }

    /// <summary>
    /// Draw swatch row for color picker.
    /// </summary>
    /// <param name="id">component id.</param>
    /// <param name="color">current color.</param>
    /// <param name="min">min.</param>
    /// <param name="max">max.</param>
    /// <returns>selected color.</returns>
    public static Vector4 ColorPickerSwatchRow(string id, Vector4 color, int min, int max)
    {
        ImGui.Spacing();
        for (var i = min; i < max; i++)
        {
            if (ImGui.ColorButton($"{id}_ColorButton_{i}", ColorPalette[i]))
            {
                color = ColorPalette[i];
            }

            ImGui.SameLine();
        }

        return color;
    }

    /// <summary>
    /// Draw swatch row for color picker.
    /// </summary>
    /// <param name="id">component id.</param>
    /// <param name="color">current color.</param>
    /// <param name="min">min.</param>
    /// <param name="max">max.</param>
    /// <returns>selected color.</returns>
    public static Vector4 UIColorPickerSwatchRow(string id, Vector4 color, int min, int max)
    {
        ImGui.Spacing();
        var usedColorIds = new HashSet<uint>();

        for (var i = min; i < max; i++)
        {
            var originalColorId = DalamudContext.DataManager.FindClosestUIColor(ColorPalette[i], usedColorIds).Id;
            usedColorIds.Add(originalColorId);

            if (ImGui.ColorButton($"{id}_UIColorButton_{i}", ColorPalette[i]))
            {
                var col = DalamudContext.DataManager.FindClosestUIColor(ColorPalette[i], usedColorIds);
                color = DalamudContext.DataManager.GetUIColorAsVector4(col.Id);
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip($"UIColor #{originalColorId}");
            }

            ImGui.SameLine();
        }

        return color;
    }
}
