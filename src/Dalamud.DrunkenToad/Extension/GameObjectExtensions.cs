using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;

namespace Dalamud.DrunkenToad
{
    /// <summary>
    /// Dalamud game object extensions.
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Validate if actor is valid player character.
        /// </summary>
        /// <param name="value">actor.</param>
        /// <returns>Indicator if player character is valid.</returns>
        public static bool IsValidPlayerCharacter(this GameObject value)
        {
            return value is PlayerCharacter character &&
                   value.ObjectId > 0 &&
                   character.HomeWorld.Id != ushort.MaxValue &&
                   character.HomeWorld.Id != 0 &&
                   character.CurrentWorld.Id != ushort.MaxValue &&
                   character.ClassJob.Id != 0 &&
                   !string.IsNullOrEmpty(character.Name.ToString());
        }
    }
}
