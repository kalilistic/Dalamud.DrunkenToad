namespace Dalamud.DrunkenToad.Extensions;

using System.Reflection;

/// <summary>
/// Assembly extensions.
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// Get assembly version as string.
    /// </summary>
    /// <param name="value">assembly.</param>
    /// <returns>assembly version as string.</returns>
    public static string Version(this Assembly value) =>
        value.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "1.0.0.0";

    /// <summary>
    /// Get assembly version as number.
    /// </summary>
    /// <param name="value">assembly.</param>
    /// <returns>assembly version as int.</returns>
    public static int VersionNumber(this Assembly value)
    {
        var pluginVersion = value.GetName().Version;
        var versionNumber = pluginVersion!.Major * 1000000 + pluginVersion.Minor * 10000 + pluginVersion.Build * 100 + pluginVersion.Revision;
        return versionNumber;
    }
}
