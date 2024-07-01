// ReSharper disable MergeIntoPattern
// ReSharper disable MergeSequentialChecks
namespace Dalamud.DrunkenToad.Extensions;

using Core;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;

/// <summary>
/// Dalamud GameObject extensions.
/// </summary>
public static class GameObjectExtensions
{
    /// <summary>
    /// Validate if actor is valid player character.
    /// </summary>
    /// <param name="value">actor.</param>
    /// <returns>Indicator if player character is valid.</returns>
    public static bool IsValidIPlayerCharacter(this IGameObject? value) =>
        value != null &&
        value is IPlayerCharacter character &&
        value.EntityId > 0 &&
        DalamudContext.DataManager.Worlds.ContainsKey(character.HomeWorld.Id) &&
        DalamudContext.DataManager.Worlds.ContainsKey(character.CurrentWorld.Id) &&
        DalamudContext.DataManager.ClassJobs.ContainsKey(character.ClassJob.Id);
}
