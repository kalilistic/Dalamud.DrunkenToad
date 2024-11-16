namespace Dalamud.DrunkenToad.Extensions;

using Core.Models;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Game.ClientState.Objects.SubKinds;

/// <summary>
/// PlayerCharacter extensions.
/// </summary>
public static class PlayerCharacterExtensions
{
    /// <summary>
    /// Convert IPlayerCharacter to ToadPlayer.
    /// </summary>
    /// <param name="character">IPlayerCharacter.</param>
    /// <returns>ToadPlayer.</returns>
    public static ToadPlayer ToToadPlayer(this IPlayerCharacter character) => new ()
    {
        GameObjectId = character.GameObjectId,
        EntityId = character.EntityId,
        ContentId = character.GetContentId(),
        Name = character.Name.ToString(),
        HomeWorld = character.HomeWorld.RowId,
        ClassJob = character.ClassJob.RowId,
        Level = character.Level,
        Customize = character.Customize,
        CompanyTag = character.CompanyTag.ToString(),
        IsLocalPlayer = false,
        IsDead = character.IsDead,
    };

    /// <summary>
    /// Get content id.
    /// </summary>
    /// <param name="character">IPlayerCharacter.</param>
    /// <returns>content id.</returns>
    public static unsafe ulong GetContentId(this IPlayerCharacter character) => ((Character*)character.Address)->ContentId;
}
