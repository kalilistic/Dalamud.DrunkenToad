// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Dalamud.DrunkenToad.Core.Models;

/// <summary>
/// ClassJob data.
/// </summary>
public class ToadClassJob
{
    /// <summary>
    /// Gets or sets class job id.
    /// </summary>
    public uint Id { get; set; }

    /// <summary>
    /// Gets class job name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets class job abbreviation.
    /// </summary>
    public string Code { get; init; } = string.Empty;
}
