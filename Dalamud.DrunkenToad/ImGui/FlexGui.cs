using System.Numerics;

namespace Dalamud.DrunkenToad.ImGui;

/// <summary>
/// FlexConfig backed ImGui components.
/// </summary>
public static class FlexGui
{
    private static FlexConfig.Configuration config;

    /// <summary>
    /// Initialize Flex ImGui.
    /// </summary>
    /// <param name="configuration">flex config instance.</param>
    public static void Initialize(FlexConfig.Configuration configuration) => config = configuration;

    /// <summary>
    /// Flex-backed checkbox.
    /// </summary>
    /// <param name="label">checkbox label.</param>
    /// <param name="key">flex config key.</param>
    public static void Checkbox(string label, string key)
    {
        var value = config.Get(key, default(bool));

        if (!ImGuiNET.ImGui.Checkbox(label, ref value.Reference))
        {
            return;
        }

        config.Set(key, value);
    }

    /// <summary>
    /// Flex-backed slider float.
    /// </summary>
    /// <param name="label">slider label.</param>
    /// <param name="key">flex config key.</param>
    /// <param name="min">slider min value.</param>
    /// <param name="max">slider max value.</param>
    /// <param name="format">string format (optional).</param>
    public static void SliderFloat(string label, string key, float min, float max, string? format)
    {
        var value = config.Get(key, 0.0f);

        if (!ImGuiNET.ImGui.SliderFloat(label, ref value.Reference, min, max, format ?? "%.3f"))
        {
            return;
        }

        config.Set(key, value);
    }

    /// <summary>
    /// Flex-backed vector4 color editor.
    /// </summary>
    /// <param name="label">editor label.</param>
    /// <param name="key">flex config key.</param>
    public static void ColorEdit4(string label, string key)
    {
        var value = config.Get(key, default(Vector4));

        if (!ImGuiNET.ImGui.ColorEdit4(label, ref value.Reference))
        {
            return;
        }

        config.Set(key, value);
    }

    /// <summary>
    /// Flex-backed text color.
    /// </summary>
    /// <param name="label">text label.</param>
    /// <param name="key">flex config key.</param>
    public static void TextColored(string label, string key)
    {
        var value = config.Get(key, default(Vector4));
        ImGuiNET.ImGui.TextColored(value.Reference, label);
    }

    /// <summary>
    /// Flex-backed combo.
    /// </summary>
    /// <param name="label">text label.</param>
    /// <param name="items">list of values.</param>
    /// <param name="key">flex config key.</param>
    public static void Combo(string label, string[] items, string key)
    {
        var value = config.Get(key, default(int));

        if (!ImGuiNET.ImGui.Combo(label, ref value.Reference, items, items.Length))
        {
            return;
        }

        config.Set(key, value);
    }
}
