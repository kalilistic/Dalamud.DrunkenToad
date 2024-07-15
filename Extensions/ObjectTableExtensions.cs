namespace Dalamud.DrunkenToad.Extensions;

using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Game.ClientState.Objects.Enums;
using Game.ClientState.Objects.SubKinds;
using Plugin.Services;

/// <summary>
/// ObjectTable extensions.
/// </summary>
public static class ObjectTableExtensions
{
    /// <summary>
    /// Retrieve all players.
    /// </summary>
    /// <param name="objectTable">Dalamud ObjectTable.</param>
    /// <returns>all players.</returns>
    public static IEnumerable<ToadPlayer> GetPlayers(this IObjectTable objectTable) =>
        objectTable.Skip(1)
            .Where(x => x.ObjectKind == ObjectKind.Player && x is IPlayerCharacter)
            .OfType<IPlayerCharacter>()
            .Select(pc => pc.ToToadPlayer())
            .ToList();

    /// <summary>
    /// Retrieve player by content id.
    /// </summary>
    /// <param name="objectTable">Dalamud ObjectTable.</param>
    /// <param name="contentId">content id.</param>
    /// <returns>player if exists.</returns>
    public static ToadPlayer? GetPlayerByContentId(this IObjectTable objectTable, ulong contentId) =>
        objectTable.OfType<IPlayerCharacter>()
            .FirstOrDefault(playerCharacter => playerCharacter.GetContentId() == contentId)?.ToToadPlayer();
}
