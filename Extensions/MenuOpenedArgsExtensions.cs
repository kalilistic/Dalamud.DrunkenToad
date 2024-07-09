// ReSharper disable UseCollectionExpression
// ReSharper disable ConvertSwitchStatementToSwitchExpression
// ReSharper disable InvertIf
namespace Dalamud.DrunkenToad.Extensions;

using Core;
using Game.Gui.ContextMenu;

/// <summary>
/// Provides extension methods for <see cref="IMenuOpenedArgs"/>.
/// </summary>
public static class MenuOpenedArgsExtensions
{
    /// <summary>
    /// Determines whether the context menu is a valid player menu.
    /// </summary>
    /// <param name="menuOpenedArgs">The menu opened arguments.</param>
    /// <param name="includeSelf">if set to <see langword="true" />, include the local player in the check.</param>
    /// <returns><see langword="true" /> if the context menu is a valid player menu; otherwise, <see langword="false" />.</returns>
    public static bool IsValidPlayerMenu(this IMenuOpenedArgs menuOpenedArgs, bool includeSelf = false)
    {
        if (!DalamudContext.PluginInterface.UiBuilder.ShouldModifyUi ||
            menuOpenedArgs.Target is not MenuTargetDefault menuTargetDefault)
        {
            return false;
        }

        switch (menuOpenedArgs.AddonName)
        {
            case null:
            case "LookingForGroup":
            case "PartyMemberList":
            case "FriendList":
            case "FreeCompany":
            case "SocialList":
            case "ContactList":
            case "ChatLog":
            case "_PartyList":
            case "LinkShell":
            case "CrossWorldLinkshell":
            case "ContentMemberList":
            case "BeginnerChatList":
                if (menuTargetDefault.TargetName != string.Empty && DalamudContext.DataManager.IsValidWorld(menuTargetDefault.TargetHomeWorld.Id))
                {
                    if (!includeSelf)
                    {
                        var name = DalamudContext.ClientStateHandler.LocalPlayer?.Name.TextValue;
                        var worldId = DalamudContext.ClientStateHandler.LocalPlayer?.HomeWorld.Id;
                        if (menuTargetDefault.TargetName == name && menuTargetDefault.TargetHomeWorld.Id == worldId)
                        {
                            DalamudContext.PluginLog.Verbose("ContextMenu: Self context menu.");
                            return false;
                        }
                    }

                    return true;
                }

                return false;
        }

        return false;
    }
}
