// ReSharper disable UseCollectionExpression
// ReSharper disable ConvertSwitchStatementToSwitchExpression
// ReSharper disable InvertIf
namespace Dalamud.DrunkenToad.Extensions;

using Core;
using Core.Models;
using Game.Gui.ContextMenu;
using Utility;

/// <summary>
/// Provides extension methods for <see cref="IMenuItemClickedArgs"/>.
/// </summary>
public static class MenuItemClickedArgsExtensions
{
    /// <summary>
    /// Gets the player from the menu item clicked arguments.
    /// </summary>
    /// <param name="menuOpenedArgs">The menu item clicked arguments.</param>
    /// <returns>The player or <see langword="null" /> if the player is invalid.</returns>
    public static ToadPlayer? GetPlayer(this IMenuItemClickedArgs menuOpenedArgs)
    {
        if (menuOpenedArgs.Target is not MenuTargetDefault menuTargetDefault)
        {
            DalamudContext.PluginLog.Warning("ContextMenu: Invalid target");
            return null;
        }

        var playerName = menuTargetDefault.TargetName;
        var worldId = menuTargetDefault.TargetHomeWorld.Id;
        var contentId = menuTargetDefault.TargetContentId;
        var objectId = menuTargetDefault.TargetObjectId;
        if (playerName.IsValidCharacterName() && DalamudContext.DataManager.IsValidWorld(worldId) && contentId != 0 &&
            objectId != 0)
        {
            return new ToadPlayer
            {
                Name = playerName,
                HomeWorld = worldId,
                ContentId = contentId,
                GameObjectId = objectId,
                CompanyTag = string.Empty,
            };
        }

        DalamudContext.PluginLog.Warning($"ContextMenu: Invalid player {playerName} {worldId} {contentId} {objectId}");
        return null;
    }
}
