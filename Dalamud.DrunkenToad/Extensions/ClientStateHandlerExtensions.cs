namespace Dalamud.DrunkenToad.Extensions;

using Core.Models;
using Plugin.Services;

/// <summary>
/// Dalamud ClientStateHandler extensions.
/// </summary>
public static class ClientStateHandlerExtensions
{
    /// <summary>
    /// Validate if actor is valid player character.
    /// </summary>
    /// <param name="value">actor.</param>
    /// <returns>Indicator if player character is valid.</returns>
    public static ToadLocalPlayer? GetLocalPlayer(this IClientState value)
    {
        if (value.LocalPlayer == null)
        {
            return null;
        }

        var localPlayer = new ToadLocalPlayer
        {
            Name = value.LocalPlayer.Name.ToString(),
            HomeWorld = value.LocalPlayer.HomeWorld.Id,
            ContentId = value.LocalContentId,
            Customize = value.LocalPlayer.Customize,
        };

        return localPlayer.IsValid() ? localPlayer : null;
    }
}
