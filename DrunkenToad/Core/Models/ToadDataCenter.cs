// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Dalamud.DrunkenToad.Core.Models;

/// <summary>
/// DataCenter data.
/// </summary>
public class ToadDataCenter
{
    /// <summary>
    /// Gets or sets data center id.
    /// </summary>
    public uint Id { get; set; }

    /// <summary>
    /// Gets data center name.
    /// </summary>
    public string Name { get; init; } = string.Empty;
}
