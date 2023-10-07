namespace Dalamud.DrunkenToad.Extensions;

using System;
using System.IO;
using Game.Text.SeStringHandling;
using Plugin;

/// <summary>
/// Dalamud PluginInterface extensions.
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
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return value.Sanitizer.Sanitize(str);
    }

    /// <summary>
    /// Sanitize dalamud se string to remove unprintable characters (short hand method).
    /// </summary>
    /// <param name="value">dalamud plugin interface with sanitizer initialized.</param>
    /// <param name="str">string to sanitize.</param>
    /// <returns>Indicator if player character is valid.</returns>
    public static string Sanitize(this DalamudPluginInterface value, SeString str) => Sanitize(value, str.ToString());

    /// <summary>
    /// Sanitize lumina se string to remove unprintable characters (short hand method).
    /// </summary>
    /// <param name="value">dalamud plugin interface with sanitizer initialized.</param>
    /// <param name="str">string to sanitize.</param>
    /// <returns>Indicator if player character is valid.</returns>
    public static string Sanitize(this DalamudPluginInterface value, Lumina.Text.SeString str) => Sanitize(value, str.ToString());

    /// <summary>
    /// Get the plugin backup directory.
    /// </summary>
    /// <param name="value">dalamud plugin interface.</param>
    /// <returns>Plugin backup directory.</returns>
    public static string PluginBackupDirectory(this DalamudPluginInterface value)
    {
        var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var backupsDir = Path.Combine(appDataDir, "XIVLauncher", "pluginBackups", value.InternalName);
        Directory.CreateDirectory(backupsDir);
        return backupsDir;
    }
}
