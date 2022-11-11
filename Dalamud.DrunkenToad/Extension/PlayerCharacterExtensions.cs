using Dalamud.Game.ClientState.Objects.SubKinds;

namespace Dalamud.DrunkenToad;

/// <summary>
/// String extensions.
/// </summary>
public static class PlayerCharacterExtensions
{
    /// <summary>
    /// Get indicator if character is dead.
    /// </summary>
    /// <param name="value">player character.</param>
    /// <returns>indicator if character is dead or not.</returns>
    public static bool IsDead(this PlayerCharacter value)
    {
        return value.CurrentHp == 0;
    }
}
