namespace Dalamud.DrunkenToad.Extensions;

using Game.ClientState.Conditions;
using Plugin.Services;

/// <summary>
/// Dalamud Condition extensions.
/// </summary>
public static class ConditionExtensions
{
    /// <summary>
    /// Indicator if player is in combat.
    /// </summary>
    /// <param name="value">condition.</param>
    /// <returns>Indicator whether player is in combat.</returns>
    public static bool InCombat(this ICondition value) => value[ConditionFlag.InCombat];
}
