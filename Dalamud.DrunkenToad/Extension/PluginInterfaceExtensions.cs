using System;
using System.Reflection;
using Dalamud.Plugin;

namespace Dalamud.DrunkenToad.Extension;

/// <summary>
/// Dalamud game object extensions.
/// </summary>
public static class PluginInterfaceExtensions
{
    /// <summary>
    /// Sanitize string to remove unprintable characters (short hand method).
    /// </summary>
    /// <param name="value">dalamud plugin interface with sanitizer initialized.</param>
    /// <param name="str">string to sanitize.</param>
    /// <returns>Indicator if player character is valid.</returns>
    public static string Sanitize(this DalamudPluginInterface value, string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return value.Sanitizer.Sanitize(str);
    }

    /// <summary>
    /// Sanitize dalamud se string to remove unprintable characters (short hand method).
    /// </summary>
    /// <param name="value">dalamud plugin interface with sanitizer initialized.</param>
    /// <param name="str">string to sanitize.</param>
    /// <returns>Indicator if player character is valid.</returns>
    public static string Sanitize(this DalamudPluginInterface value, Game.Text.SeStringHandling.SeString str)
    {
        return Sanitize(value, str.ToString());
    }

    /// <summary>
    /// Sanitize lumina se string to remove unprintable characters (short hand method).
    /// </summary>
    /// <param name="value">dalamud plugin interface with sanitizer initialized.</param>
    /// <param name="str">string to sanitize.</param>
    /// <returns>Indicator if player character is valid.</returns>
    public static string Sanitize(this DalamudPluginInterface value, Lumina.Text.SeString str)
    {
        return Sanitize(value, str.ToString());
    }

    /// <summary>
    /// Get plugin name from interface (private field).
    /// </summary>
    /// <param name="value">dalamud plugin interface.</param>
    /// <returns>Plugin name.</returns>
    public static string PluginName(this DalamudPluginInterface value)
    {
        try
        {
            var fieldInfo = typeof(DalamudPluginInterface).GetField("pluginName", BindingFlags.NonPublic);
            return fieldInfo?.GetValue(value) as string ?? string.Empty;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}
