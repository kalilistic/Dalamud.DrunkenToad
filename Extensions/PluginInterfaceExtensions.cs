﻿// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MergeIntoPattern
namespace Dalamud.DrunkenToad.Extensions;

using System;
using System.IO;
using System.Linq;
using Game.Text.SeStringHandling;
using Plugin;
using Lumina.Text.ReadOnly;

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
    public static string Sanitize(this IDalamudPluginInterface value, string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return value.Sanitizer.Sanitize(str);
    }

    /// <summary>
    /// Sanitize dalamud se string to remove unprintable characters (shorthand method).
    /// </summary>
    /// <param name="value">dalamud plugin interface with sanitizer initialized.</param>
    /// <param name="str">string to sanitize.</param>
    /// <returns>Sanitized string.</returns>
    public static string Sanitize(this IDalamudPluginInterface value, SeString str) => Sanitize(value, str.ToString());

    /// <summary>
    /// Sanitize ReadOnlySeString to remove unprintable characters (shorthand method).
    /// </summary>
    /// <param name="value">dalamud plugin interface with sanitizer initialized.</param>
    /// <param name="str">string to sanitize.</param>
    /// <returns>Sanitized string.</returns>
    public static string Sanitize(this IDalamudPluginInterface value, ReadOnlySeString str) => Sanitize(value, str.ToString());

    /// <summary>
    /// Sanitize lumina se string to remove unprintable characters (shorthand method).
    /// </summary>
    /// <param name="value">dalamud plugin interface with sanitizer initialized.</param>
    /// <param name="str">string to sanitize.</param>
    /// <returns>Sanitized string.</returns>
    public static string Sanitize(this IDalamudPluginInterface value, Lumina.Text.SeString str) => Sanitize(value, str.ToString());

    /// <summary>
    /// Get the plugin backup directory for windows (don't use for Wine).
    /// </summary>
    /// <param name="value">dalamud plugin interface.</param>
    /// <returns>Plugin backup directory.</returns>
    public static string WindowsPluginBackupDirectory(this IDalamudPluginInterface value)
    {
        var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var backupsDir = Path.Combine(appDataDir, "XIVLauncher", $"{value.InternalName.FirstCharToLower()}Backups");
        Directory.CreateDirectory(backupsDir);
        return backupsDir;
    }

    /// <summary>
    /// Get the plugin backup directory.
    /// </summary>
    /// <param name="value">dalamud plugin interface.</param>
    /// <returns>Plugin backup directory.</returns>
    public static string PluginBackupDirectory(this IDalamudPluginInterface value)
    {
        var configDir = value.ConfigDirectory.Parent;
        var appDir = configDir?.Parent;
        if (appDir == null)
        {
            return WindowsPluginBackupDirectory(value); // use as a fallback
        }

        var backupsDir = Path.Combine(appDir.FullName, $"{value.InternalName.FirstCharToLower()}Backups");
        Directory.CreateDirectory(backupsDir);
        return backupsDir;
    }

    /// <summary>
    /// Check if different version of plugin is loaded.
    /// </summary>
    /// <param name="value">dalamud plugin interface.</param>
    /// <param name="version">version to check.</param>
    /// <returns>Indicator if another version of the plugin is loaded.</returns>
    public static bool IsDifferentVersionLoaded(this IDalamudPluginInterface value, string version = "Canary")
    {
        var internalName = value.InternalName;
        if (!internalName.EndsWith(version, StringComparison.CurrentCulture))
        {
            return IsPluginLoaded(value, $"{internalName}{version}");
        }

        var stableName = internalName.Replace(version, string.Empty);
        return IsPluginLoaded(value, stableName);
    }

    private static bool IsPluginLoaded(IDalamudPluginInterface pluginInterface, string pluginName)
    {
        var plugin = pluginInterface.InstalledPlugins.FirstOrDefault(p => p.Name == pluginName);
        return plugin != null && plugin.IsLoaded;
    }
}
