// ReSharper disable InconsistentNaming
namespace Dalamud.DrunkenToad.Gui;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Core;
using Core.Services;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Loc.ImGui;
using Enums;
using Helpers;
using ImGuiNET;
using PluginLocalization = Loc.Localization;

/// <summary>
/// Custom ImGui components.
/// </summary>
public static class ToadGui
{
    private static PluginLocalization Localization { get; set; } = null!;

    private static DataManagerEx DataManager { get; set; } = null!;

    /// <summary>
    /// Initialize localization.
    /// </summary>
    /// <param name="pluginLocalization">plugin localization.</param>
    /// <param name="dataManager">data manager.</param>
    public static void Initialize(PluginLocalization pluginLocalization, DataManagerEx dataManager)
    {
        Localization = pluginLocalization;
        DataManager = dataManager;
    }

    /// <summary>
    /// InfoBox for help text display.
    /// </summary>
    /// <param name="key">localization key.</param>
    /// <param name="size">help text box height.</param>
    public static void InfoBox(string key, InfoBoxSize size)
    {
        ImGui.BeginChild($"{key}_InfoBox", ImGuiHelpers.ScaledVector2(-1, (float)size), true);
        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudViolet);
        ImGui.Text(FontAwesomeIcon.InfoCircle.ToIconString());
        ImGui.PopStyleColor();
        ImGui.PopFont();
        ImGui.SameLine();
        LocGui.SafeTextWrapped(key);
        ImGui.EndChild();
        ImGuiHelpers.ScaledDummy(1f);
    }

    /// <summary>
    /// Section with header and spacing.
    /// </summary>
    /// <param name="key">localization key.</param>
    /// <param name="content">content to display.</param>
    public static void Section(string key, Action content)
    {
        LocGui.TextColored(key, ImGuiColors.DalamudViolet);
        content.Invoke();
        ImGuiHelpers.ScaledDummy(3f);
    }

    /// <summary>
    /// Checkbox with better label.
    /// </summary>
    /// <param name="key">localization key.</param>
    /// <param name="value">local value reference.</param>
    /// <param name="useLabel">use localized label.</param>
    /// <returns>Indicator if changed.</returns>
    public static bool Checkbox(string key, ref bool value, bool useLabel = true)
    {
        var result = ImGui.Checkbox($"###{key}_Checkbox", ref value);

        if (!useLabel)
        {
            return result;
        }

        ImGui.SameLine();
        LocGui.Text(key);

        return result;
    }

    /// <summary>
    /// Checkbox with better label and loopable key.
    /// </summary>
    /// <param name="key">localization key.</param>
    /// <param name="suffix">suffix for key.</param>
    /// <param name="value">local value reference.</param>
    /// <param name="useLabel">use localized label.</param>
    /// <returns>Indicator if changed.</returns>
    public static bool Checkbox(string key, string suffix, ref bool value, bool useLabel = true)
    {
        var result = ImGui.Checkbox($"###{key}_{suffix}_Checkbox", ref value);

        if (!useLabel)
        {
            return result;
        }

        ImGui.SameLine();
        LocGui.Text(key);

        return result;
    }

    /// <summary>
    /// Styled ComboBox with derived and localized options.
    /// </summary>
    /// <typeparam name="T">Enum for combobox list.</typeparam>
    /// <param name="key">primary key.</param>
    /// <param name="value">current selected index value.</param>
    /// <param name="comboWidth">width (default to fill).</param>
    /// <param name="padding">add scaled dummy to top for padding.</param>
    /// <param name="includeLabel">add label.</param>
    /// <returns>indicates if combo value is changed.</returns>
    public static bool Combo<T>(string key, ref T value, int comboWidth = 100, bool padding = true, bool includeLabel = true) where T : IConvertible
    {
        var isChanged = false;
        var options = Enum.GetNames(typeof(T));
        var localizedOptions = new List<string>();
        foreach (var option in options)
        {
            localizedOptions.Add(Localization.GetString(option));
        }

        if (padding)
        {
            ImGuiHelpers.ScaledDummy(1f);
        }

        var label = includeLabel ? Localization.GetString(key) : $"###{key}_Combo";
        ImGui.SetNextItemWidth(comboWidth == -1 ? comboWidth : ImGuiUtil.CalcScaledComboWidth(comboWidth));
        var val = Convert.ToInt32(value);
        if (ImGui.Combo(label, ref val, localizedOptions.ToArray(), localizedOptions.Count))
        {
            value = (T)(object)val;
            isChanged = true;
        }

        return isChanged;
    }

    /// <summary>
    /// Styled and localized ComboBox.
    /// </summary>
    /// <param name="key">primary key.</param>
    /// <param name="value">current selected index value.</param>
    /// <param name="options">keys for options.</param>
    /// <param name="comboWidth">width (default to fill).</param>
    /// <param name="padding">add scaled dummy to top for padding.</param>
    /// <param name="includeLabel">add label.</param>
    /// <returns>indicates if combo box value was changed.</returns>
    public static bool Combo(string key, ref int value, IEnumerable<string> options, int comboWidth = 100, bool padding = true, bool includeLabel = true)
    {
        var isChanged = false;
        var localizedOptions = new List<string>();
        foreach (var option in options)
        {
            localizedOptions.Add(Localization.GetString(option));
        }

        if (padding)
        {
            ImGuiHelpers.ScaledDummy(1f);
        }

        var label = includeLabel ? Localization.GetString(key) : $"###{key}";
        ImGui.SetNextItemWidth(comboWidth == -1 ? comboWidth : ImGuiUtil.CalcScaledComboWidth(comboWidth));
        if (ImGui.Combo(label, ref value, localizedOptions.ToArray(), localizedOptions.Count))
        {
            isChanged = true;
        }

        return isChanged;
    }

    /// <summary>
    /// Styled and localized ComboBox with loopable key.
    /// </summary>
    /// <param name="key">primary key.</param>
    /// <param name="suffix">suffix for key.</param>
    /// <param name="value">current selected index value.</param>
    /// <param name="options">keys for options.</param>
    /// <param name="comboWidth">width (default to fill).</param>
    /// <returns>indicates if combo box value was changed.</returns>
    public static bool Combo(string key, string suffix, ref int value, IEnumerable<string> options, int comboWidth = 100)
    {
        var isChanged = false;
        var localizedOptions = new List<string>();
        foreach (var option in options)
        {
            localizedOptions.Add(Localization.GetString(option));
        }

        ImGuiHelpers.ScaledDummy(1f);
        ImGui.SetNextItemWidth(ImGuiUtil.CalcScaledComboWidth(comboWidth));
        if (ImGui.Combo($"{Localization.GetString(key)}###{suffix}_Combo", ref value, localizedOptions.ToArray(), localizedOptions.Count))
        {
            isChanged = true;
        }

        return isChanged;
    }

    /// <summary>
    /// InputText with localization and label option.
    /// </summary>
    /// <param name="key">primary key.</param>
    /// <param name="input">text input.</param>
    /// <param name="length">max length.</param>
    /// <param name="includeLabel">indicator to display label.</param>
    /// <returns>indicator whether text was changed.</returns>
    public static bool InputText(string key, ref string input, uint length, bool includeLabel = true)
    {
        if (includeLabel)
        {
            return LocGui.InputText(key, ref input, length);
        }

        return ImGui.InputText($"###{key}", ref input, length);
    }

    /// <summary>
    /// Localized UIColorPicker with built-in swatch rows.
    /// </summary>
    /// <param name="key">primary key.</param>
    /// <param name="color">current color.</param>
    /// <param name="includeLabel">indicator to display label.</param>
    /// <returns>indicator whether color changed.</returns>
    public static bool UIColorPicker2(string key, ref Vector4 color, bool includeLabel = true)
    {
        var id = $"###{key}";
        var originalColor = color;
        var closestUIColor = DataManager.FindClosestUIColor(color);
        var originalColorId = closestUIColor.Id;

        if (ImGui.ColorButton($"{id}_UIColorButton", color))
        {
            ImGui.OpenPopup($"{id}_UIColorButton_Popup");
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip($"UIColor #{originalColorId}");
        }

        if (ImGui.BeginPopup($"{id}_UIColorButton_Popup"))
        {
            var colorChanged = ImGui.ColorPicker4($"{id}_UIColorButton_ColorPicker4", ref color);
            if (colorChanged)
            {
                var col = DataManager.FindClosestUIColor(color);
                color = DataManager.GetUIColorAsVector4(col.Id);
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip($"UIColor #{originalColorId}");
            }

            color = ImGuiUtil.UIColorPickerSwatchRow($"{id}_UIColorSwatchRow_1", color, 0, 8);
            color = ImGuiUtil.UIColorPickerSwatchRow($"{id}_UIColorSwatchRow_2", color, 8, 16);
            color = ImGuiUtil.UIColorPickerSwatchRow($"{id}_UIColorSwatchRow_3", color, 16, 24);
            color = ImGuiUtil.UIColorPickerSwatchRow($"{id}_UIColorSwatchRow_4", color, 24, 32);

            ImGui.EndPopup();
        }

        if (includeLabel)
        {
            ImGui.SameLine();
            LocGui.Text(key);
        }

        return color != originalColor;
    }

    /// <summary>
    /// Localized ColorPicker with built-in swatch rows.
    /// </summary>
    /// <param name="key">primary key.</param>
    /// <param name="color">current color.</param>
    /// <param name="includeLabel">indicator to display label.</param>
    /// <returns>indicator whether color changed.</returns>
    public static bool ColorPicker(string key, ref Vector4 color, bool includeLabel = true)
    {
        var id = $"###{key}";
        var originalColor = color;
        if (ImGui.ColorButton($"{id}_ColorButton", color))
        {
            ImGui.OpenPopup($"{id}_ColorButton_Popup");
        }

        if (ImGui.BeginPopup($"{id}_ColorButton_Popup"))
        {
            ImGui.ColorPicker4($"{id}_ColorButton_ColorPicker4", ref color);
            color = ImGuiUtil.ColorPickerSwatchRow($"{id}_ColorSwatchRow_1", color, 0, 8);
            color = ImGuiUtil.ColorPickerSwatchRow($"{id}_ColorSwatchRow_2", color, 8, 16);
            color = ImGuiUtil.ColorPickerSwatchRow($"{id}_ColorSwatchRow_3", color, 16, 24);
            color = ImGuiUtil.ColorPickerSwatchRow($"{id}_ColorSwatchRow_4", color, 24, 32);
            ImGui.EndPopup();
        }

        if (includeLabel)
        {
            ImGui.SameLine();
            LocGui.Text(key);
        }

        return color != originalColor;
    }

    /// <summary>
    /// Localized IconPicker.
    /// </summary>
    /// <param name="key">primary key.</param>
    /// <param name="icon">icon char code.</param>
    /// <param name="iconCodes">keys for options.</param>
    /// <param name="iconNames">add scaled dummy to top for padding.</param>
    /// <param name="includeLabel">indicator whether to include label.</param>
    /// <returns>indicator whether icon changed.</returns>
    /// <summary>
    /// Renders a combo picker for selecting icons.
    /// </summary>
    public static bool IconPicker(string key, ref char icon, char[] iconCodes, string[] iconNames, bool includeLabel = true)
    {
        var isChanged = false;
        var tempIcon = icon;
        var sortedIconNames = (string[])iconNames.Clone();
        var sortedIconCodes = (char[])iconCodes.Clone();

        Array.Sort(sortedIconNames, sortedIconCodes);

        var iconIndex = Array.FindIndex(sortedIconCodes, c => c == tempIcon);
        if (iconIndex == -1)
        {
            iconIndex = 0;
        }

        var currentIcon = ((FontAwesomeIcon)sortedIconCodes[iconIndex]).ToIconString();
        var currentIconName = sortedIconNames[iconIndex];

        if (LocGui.Button(key))
        {
            ImGui.OpenPopup($"##ComboPopup{key}");
        }

        if (ImGui.BeginPopup($"##ComboPopup{key}"))
        {
            if (sortedIconCodes.Length > 5)
            {
                var childHeight = 5 * 20.0f * ImGuiHelpers.GlobalScale;
                var childWidth = 200.0f * ImGuiHelpers.GlobalScale;
                ImGui.BeginChild("##IconChild", new Vector2(childWidth, childHeight), true);
            }

            for (var i = 0; i < sortedIconNames.Length; i++)
            {
                var isSelected = iconIndex == i;
                var spacing = 30.0f * ImGuiHelpers.GlobalScale;

                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.Text(((FontAwesomeIcon)sortedIconCodes[i]).ToIconString());
                ImGui.PopFont();

                ImGui.SameLine(ImGui.GetCursorPosX() + spacing);
                if (ImGui.Selectable($"{sortedIconNames[i]}##{i}", isSelected))
                {
                    iconIndex = i;
                    icon = sortedIconCodes[iconIndex];
                    isChanged = true;
                    ImGui.CloseCurrentPopup();
                }

                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }

            if (sortedIconCodes.Length > 5)
            {
                ImGui.EndChild();
            }

            ImGui.EndPopup();
        }

        ImGui.SameLine();
        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.Text(currentIcon);
        ImGui.PopFont();

        if (includeLabel)
        {
            ImGui.SameLine();
            LocGui.Text(currentIconName);
        }

        return isChanged;
    }

    /// <summary>
    /// Localized Action Prompt for Delete/Restore.
    /// </summary>
    /// <typeparam name="T">Item type for action to be performed upon (e.g. delete, restore).</typeparam>
    /// <param name="item">current item being evaluated.</param>
    /// <param name="icon">icon char code.</param>
    /// <param name="messageKey">confirmation message key to display.</param>
    /// <param name="request">tuple with action state and instance of item under review.</param>
    public static void Confirm<T>(T item, FontAwesomeIcon icon, string messageKey, ref Tuple<ActionRequest, T>? request)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.Text(icon.ToIconString());
        if (ImGui.IsItemClicked())
        {
            request = new Tuple<ActionRequest, T>(ActionRequest.Pending, item);
        }

        ImGui.PopFont();
        if (request != null)
        {
            dynamic newItem = item!;
            dynamic savedItem = request.Item2!;
            if (newItem.Id == savedItem.Id)
            {
                ImGui.SameLine();
                LocGui.TextColored(messageKey, ImGuiColors.DalamudYellow);
                ImGui.SameLine();
                if (LocGui.SmallButton("Cancel"))
                {
                    request = new Tuple<ActionRequest, T>(ActionRequest.None, request.Item2);
                }

                ImGui.SameLine();
                if (LocGui.SmallButton("OK"))
                {
                    request = new Tuple<ActionRequest, T>(ActionRequest.Confirmed, request.Item2);
                }
            }
        }
    }

    /// <summary>
    /// Localized Action Prompt for Delete/Restore.
    /// </summary>
    /// <typeparam name="T">Item type for action to be performed upon (e.g. delete, restore).</typeparam>
    /// <param name="item">current item being evaluated.</param>
    /// <param name="messageKey">confirmation message key to display.</param>
    /// <param name="request">tuple with action state and instance of item under review.</param>
    public static void Confirm<T>(T item, string messageKey, ref Tuple<ActionRequest, T>? request)
    {
        if (request == null)
        {
            return;
        }

        dynamic newItem = item!;
        dynamic savedItem = request.Item2!;
        if (newItem.Id != savedItem.Id)
        {
            return;
        }

        ImGui.SameLine();
        LocGui.TextColored(messageKey, ImGuiColors.DalamudYellow);
        ImGui.SameLine();
        if (LocGui.SmallButton("Cancel"))
        {
            request = new Tuple<ActionRequest, T>(ActionRequest.None, request.Item2);
        }

        ImGui.SameLine();
        if (LocGui.SmallButton("OK"))
        {
            request = new Tuple<ActionRequest, T>(ActionRequest.Confirmed, request.Item2);
        }

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() - (5.0f * ImGuiHelpers.GlobalScale));
    }

    /// <summary>
    /// Localized dummy tab to use while waiting for content to load.
    /// </summary>
    /// <param name="key">primary key.</param>
    public static void DummyTab(string key)
    {
        LocGui.BeginTabItem(key);
        ImGui.EndTabItem();
    }

    /// <summary>
    /// Scaled SameLine.
    /// </summary>
    /// <param name="offset">offset without scaling.</param>
    public static void SameLine(float offset) => ImGui.SameLine(offset * ImGuiHelpers.GlobalScale);

    /// <summary>
    /// Scaled SetNextItemWidth.
    /// </summary>
    /// <param name="width">offset without scaling.</param>
    public static void SetNextItemWidth(float width) => ImGui.SetNextItemWidth(width * ImGuiHelpers.GlobalScale);

    /// <summary>
    /// Scaled TableSetupColumn.
    /// </summary>
    /// <param name="label">column label.</param>
    /// <param name="flags">column flags.</param>
    /// <param name="initWidthOrWeight">column width.</param>
    public static void TableSetupColumn(string label, ImGuiTableColumnFlags flags, float initWidthOrWeight) =>
        ImGui.TableSetupColumn(label, flags, initWidthOrWeight * ImGuiHelpers.GlobalScale);

    /// <summary>
    /// Localized HelpMarker.
    /// </summary>
    /// <param name="key">primary key.</param>
    public static void HelpMarker(string key) => ImGuiComponents.HelpMarker(Localization.GetString(key));

    /// <summary>
    /// Localized Colored BeginTabItem.
    /// </summary>
    /// <param name="key">primary key.</param>
    /// <param name="tabColor">tab color.</param>
    /// <returns>indicator whether tab selected.</returns>
    public static bool BeginTabItemColored(string key, Vector4 tabColor)
    {
        var tabHoveredColor = new Vector4(Math.Min(1.0f, tabColor.X + 0.1f), Math.Min(1.0f, tabColor.Y + 0.1f), Math.Min(1.0f, tabColor.Z + 0.1f), 1.0f);
        var tabActiveColor = new Vector4(Math.Min(1.0f, tabColor.X + 0.2f), Math.Min(1.0f, tabColor.Y + 0.2f), Math.Min(1.0f, tabColor.Z + 0.2f), 1.0f);
        ImGui.PushStyleColor(ImGuiCol.Tab, tabColor);
        ImGui.PushStyleColor(ImGuiCol.TabHovered, tabHoveredColor);
        ImGui.PushStyleColor(ImGuiCol.TabActive, tabActiveColor);
        var result = LocGui.BeginTabItem(key);
        ImGui.PopStyleColor(3);
        return result;
    }

    /// <summary>
    /// Localized color picker with popup.
    /// </summary>
    /// <param name="key">primary key.</param>
    /// <param name="colorId">color id.</param>
    /// <param name="color">current color.</param>
    /// <param name="includeLabel">indicator whether to include label.</param>
    /// <returns>indicator whether color changed.</returns>
    public static bool SimpleUIColorPicker(string key, uint colorId, ref Vector4 color, bool includeLabel = true)
    {
        var id = $"###{key}";
        var originalColor = color;
        var itemsPerRow = (int)Math.Sqrt(DalamudContext.DataManager.UIColors.Count);
        var currentItemCount = 0;
        var addedColors = new HashSet<Vector4>();

        if (ImGui.ColorButton($"{id}_UIColorButton", color, ImGuiColorEditFlags.NoTooltip))
        {
            ImGui.OpenPopup($"{id}_UIColorButton_Popup");
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip($"UIColor #{colorId}");
        }

        if (ImGui.BeginPopup($"{id}_UIColorButton_Popup"))
        {
            var sortedColors = DalamudContext.DataManager.UIColors
                .OrderBy(pair => ColorToHue(DalamudContext.DataManager.GetUIColorAsVector4(pair.Value.Id)))
                .ToList();

            foreach (var uiColorPair in sortedColors)
            {
                var buttonColor = DalamudContext.DataManager.GetUIColorAsVector4(uiColorPair.Value.Id);

                if (addedColors.Any(existing => AreColorsSimilar(existing, buttonColor)))
                {
                    continue;
                }

                if (ImGui.ColorButton($"{id}_UIColorButton_{uiColorPair.Value.Id}", buttonColor))
                {
                    color = buttonColor;
                    ImGui.CloseCurrentPopup();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip($"UIColor #{uiColorPair.Value.Id}");
                }

                currentItemCount++;
                if (currentItemCount % itemsPerRow != 0)
                {
                    ImGui.SameLine();
                }

                addedColors.Add(buttonColor);
            }

            ImGui.EndPopup();
        }

        if (includeLabel)
        {
            ImGui.SameLine();
            LocGui.Text(key);
        }

        return color != originalColor;
    }

    private static bool AreColorsSimilar(Vector4 a, Vector4 b, float tolerance = 0.05f) => Math.Abs(a.X - b.X) <
        tolerance && Math.Abs(a.Y - b.Y) < tolerance && Math.Abs(a.Z - b.Z) < tolerance;

    private static float ColorToHue(Vector4 color)
    {
        var r = color.X;
        var g = color.Y;
        var b = color.Z;

        var min = Math.Min(r, Math.Min(g, b));
        var max = Math.Max(r, Math.Max(g, b));

        var delta = max - min;
        var hue = 0f;

        if (!(Math.Abs(delta) > float.Epsilon))
        {
            return hue;
        }

        if (Math.Abs(max - r) < float.Epsilon)
        {
            hue = (g - b) / delta;
        }
        else if (Math.Abs(max - g) < float.Epsilon)
        {
            hue = 2 + ((b - r) / delta);
        }
        else if (Math.Abs(max - b) < float.Epsilon)
        {
            hue = 4 + ((r - g) / delta);
        }

        hue = ((hue * 60) + 360) % 360;

        return hue;
    }
}
