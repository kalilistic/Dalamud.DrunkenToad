namespace Dalamud.DrunkenToad.Core.Models;

/// <summary>
/// Subset of key properties from local player for eventing.
/// </summary>
public class ToadLocalPlayer
{
    /// <summary>
    /// The content ID of the local character.
    /// </summary>
    public ulong ContentId;

    /// <summary>
    /// Player Name.
    /// </summary>
    public string Name = string.Empty;

    /// <summary>
    /// Player HomeWorld ID.
    /// </summary>
    public uint HomeWorld;

    /// <summary>
    /// Player Customize Array.
    /// </summary>
    public byte[]? Customize;

    /// <summary>
    /// Player Company Tag.
    /// </summary>
    public string CompanyTag = string.Empty;

    /// <summary>
    /// Validate if local player is valid.
    /// </summary>
    /// <returns>Indicator if local player is valid.</returns>
    public bool IsValid() => this.ContentId != 0 && this.Name != string.Empty && this.HomeWorld != 0;
}
