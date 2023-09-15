namespace Dalamud.DrunkenToad.Extensions;

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
    public static bool IsValidPlayerCharacter(this GameObject value) => value is PlayerCharacter character &&
                                                                        value.ObjectId > 0 &&
                                                                        character.HomeWorld.Id != ushort.MaxValue &&
                                                                        character.HomeWorld.Id != 0 &&
                                                                        character.CurrentWorld.Id != ushort.MaxValue &&
                                                                        character.ClassJob.Id != 0;
}
