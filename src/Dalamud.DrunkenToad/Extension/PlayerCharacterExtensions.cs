using Dalamud.Game.ClientState.Actors.Types;

namespace Dalamud.DrunkenToad
{
    /// <summary>
    /// String extensions.
    /// </summary>
    public static class PlayerCharacterExtensions
    {
        /// <summary>
        /// Get customization data for a player character.
        /// </summary>
        /// <param name="value">player character.</param>
        /// <returns>player character customization data.</returns>
        public static CharaCustomizeData CustomizeData(this PlayerCharacter value)
        {
            return CharaCustomizeData.MapCustomizeData(value.Customize);
        }

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
}
