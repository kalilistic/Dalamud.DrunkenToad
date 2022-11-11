using Dalamud.Game.ClientState.Conditions;

namespace Dalamud.DrunkenToad.Extension;

/// <summary>
/// Dalamud condition extensions.
/// </summary>
public static class ConditionExtensions
{
    /// <summary>
    /// Indicator if player is in combat.
    /// </summary>
    /// <param name="value">condition.</param>
    /// <returns>Indicator whether player is in combat.</returns>
    public static bool InCombat(this Condition value)
    {
        return value[ConditionFlag.InCombat];
    }
}
