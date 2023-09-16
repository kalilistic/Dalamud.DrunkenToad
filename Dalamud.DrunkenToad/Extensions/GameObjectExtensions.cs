namespace Dalamud.DrunkenToad.Extensions;

using Core;
using Game.ClientState.Objects.SubKinds;
using Game.ClientState.Objects.Types;

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
    public static bool IsValidPlayerCharacter(this GameObject? value) =>
        value != null &&
        value is PlayerCharacter character &&
        value.ObjectId > 0 &&
        DalamudContext.DataManager.Worlds.ContainsKey(character.HomeWorld.Id) &&
        DalamudContext.DataManager.Worlds.ContainsKey(character.CurrentWorld.Id) &&
        DalamudContext.DataManager.ClassJobs.ContainsKey(character.ClassJob.Id);
}
