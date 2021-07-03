using Dalamud.Game.ClientState.Actors.Types;

namespace Dalamud.DrunkenToad
{
    /// <summary>
    /// String extensions.
    /// </summary>
    public static class ActorExtensions
    {
        /// <summary>
        /// Validate if actor is valid player character.
        /// </summary>
        /// <param name="value">actor.</param>
        /// <returns>Indicator if player character is valid.</returns>
        public static bool IsValidPlayerCharacter(this Actor value)
        {
            return value is PlayerCharacter character &&
                   value.ActorId != 0 &&
                   character.HomeWorld.Id != ushort.MaxValue &&
                   character.CurrentWorld.Id != ushort.MaxValue &&
                   character.ClassJob != null && character.ClassJob.Id != 0 &&
                   character.Name.IsValidCharacterName();
        }
    }
}
