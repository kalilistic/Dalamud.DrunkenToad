// ReSharper disable InconsistentNaming
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Dalamud.DrunkenToad.Core.Models;

/// <summary>
/// ToadUIColor model.
/// </summary>
public class ToadUIColor
{
    /// <summary>
    /// Gets or sets ui color id.
    /// </summary>
    public uint Id { get; set; }

    /// <summary>
    /// Gets or sets ui foreground.
    /// </summary>
    public uint Foreground { get; set; }

    /// <summary>
    /// Gets or sets ui glow.
    /// </summary>
    public uint Glow { get; set; }
}
