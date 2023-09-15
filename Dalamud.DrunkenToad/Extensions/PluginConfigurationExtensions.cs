namespace Dalamud.DrunkenToad.Extensions;

using Configuration;

/// <summary>
/// Provides extension methods for the IPluginConfiguration interface.
/// </summary>
public static class PluginConfigurationExtensions
{
    /// <summary>
    /// Gets the value of a property by its name.
    /// </summary>
    /// <param name="config">The IPluginConfiguration instance on which the extension method is called.</param>
    /// <param name="propertyName">The name of the property to get the value of.</param>
    /// <returns>The value of the specified property, or null if the property is not found.</returns>
    public static object? GetPropertyByName(this IPluginConfiguration? config, string propertyName)
    {
        if (config == null || string.IsNullOrEmpty(propertyName))
        {
            return null;
        }

        var type = config.GetType();
        var property = type.GetProperty(propertyName);

        if (property == null)
        {
            return null;
        }

        return property.GetValue(config);
    }
}
