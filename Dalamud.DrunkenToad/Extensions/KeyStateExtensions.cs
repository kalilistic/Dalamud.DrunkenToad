namespace Dalamud.DrunkenToad.Extensions;

using Game.ClientState.Keys;
using Plugin.Services;

/// <summary>
/// Dalamud key state service extensions.
/// </summary>
public static class KeyStateExtensions
{
    /// <summary>
    /// Check if escape key is pressed.
    /// </summary>
    /// <param name="value">dalamud key state service.</param>
    /// <returns>Indicator if escape key is pressed.</returns>
    public static bool IsEscapePressed(this IKeyState value) => value[VirtualKey.ESCAPE];
}
