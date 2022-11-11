using System;
using System.Reflection;

namespace Dalamud.DrunkenToad.Extension;

/// <summary>
/// Vector4 extensions.
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// Get assembly version as string.
    /// </summary>
    /// <param name="value">assembly.</param>
    /// <returns>assembly version as string.</returns>
    public static string Version(this Assembly value)
    {
        return value.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "1.0.0.0";
    }

    /// <summary>
    /// Get assembly version as number.
    /// </summary>
    /// <param name="value">assembly.</param>
    /// <returns>assembly version as int.</returns>
    public static int VersionNumber(this Assembly value)
    {
        var pluginVersion = value.Version();
        pluginVersion = pluginVersion.Replace(".", string.Empty);
        return Convert.ToInt32(pluginVersion);
    }
}
