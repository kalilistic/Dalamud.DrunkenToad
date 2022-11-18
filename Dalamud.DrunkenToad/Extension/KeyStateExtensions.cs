using Dalamud.Game.ClientState.Keys;

namespace Dalamud.DrunkenToad.Extension;

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
    public static bool IsEscapePressed(this KeyState value)
    {
        return value[VirtualKey.ESCAPE];
    }
}
