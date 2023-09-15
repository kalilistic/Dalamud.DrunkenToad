namespace Dalamud.DrunkenToad.Core.Models;

/// <summary>
/// World data.
/// </summary>
public class ToadWorld
{
    /// <summary>
    /// Gets or sets world id.
    /// </summary>
    public uint Id { get; set; }

    /// <summary>
    /// Gets world name.
    /// </summary>
    public string Name { get; init; } = string.Empty;
}
