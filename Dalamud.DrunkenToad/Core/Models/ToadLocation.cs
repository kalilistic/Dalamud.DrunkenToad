// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Dalamud.DrunkenToad.Core.Models;

using Enums;

/// <summary>
/// Location (roll up of territory and content).
/// </summary>
public class ToadLocation
{
    /// <summary>
    /// Gets or sets territoryTypeId.
    /// </summary>
    public ushort TerritoryId { get; set; }

    /// <summary>
    /// Gets or sets territory type place name.
    /// </summary>
    public string TerritoryName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets CFC id.
    /// </summary>
    public uint ContentId { get; set; }

    /// <summary>
    /// Gets or sets CFC name.
    /// </summary>
    public string ContentName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Location type (e.g. overworld, duty, high-end duty).
    /// </summary>
    public ToadLocationType LocationType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the location is in content.
    /// </summary>
    /// <returns>indicator whether in content.</returns>
    public bool InContent() => this.ContentId != 0;

    /// <summary>
    /// Get content name if available otherwise place name.
    /// </summary>
    /// <returns>effective name.</returns>
    public string GetName()
    {
        if (!this.InContent())
        {
            return this.TerritoryName;
        }

        return this.ContentName;
    }
}
