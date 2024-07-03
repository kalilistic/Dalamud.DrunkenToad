namespace Dalamud.DrunkenToad.Core.Models;

/// <summary>
/// Subset of key properties from IPlayerCharacter for eventing.
/// </summary>
public class ToadPlayer
{
    /// <summary>
    /// Player Job ID.
    /// </summary>
    public uint ClassJob;

    /// <summary>
    /// Player Free Company.
    /// </summary>
    public string CompanyTag = null!;

    /// <summary>
    /// Player Customize Array.
    /// </summary>
    public byte[]? Customize;

    /// <summary>
    /// Player HomeWorld ID.
    /// </summary>
    public uint HomeWorld;

    /// <summary>
    /// GameObject ID.
    /// </summary>
    public uint Id;

    /// <summary>
    /// Is Player Dead.
    /// </summary>
    public bool IsDead;

    /// <summary>
    /// Is Local Player.
    /// </summary>
    public bool IsLocalPlayer;

    /// <summary>
    /// Player Job Level.
    /// </summary>
    public byte Level;

    /// <summary>
    /// Player Name.
    /// </summary>
    public string Name = null!;

    /// <summary>
    /// Player Content ID.
    /// </summary>
    public ulong ContentId;

    /// <summary>
    /// Is Player Valid.
    /// </summary>
    /// <remarks>
    /// Use Dalamud's IsValidCharacterName() for more robust checks.
    /// </remarks>
    /// <returns>Indicator if player is valid.</returns>
    public bool IsValid() => this.Id > 0 &&
                             this.ContentId > 0 &&
                             this.HomeWorld != ushort.MaxValue &&
                             this.HomeWorld != 0 &&
                             this.ClassJob != 0;
}
