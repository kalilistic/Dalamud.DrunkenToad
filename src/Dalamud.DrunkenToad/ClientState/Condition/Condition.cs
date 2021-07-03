using Dalamud.Game.ClientState;
using Dalamud.Plugin;

namespace Dalamud.DrunkenToad
{
    /// <summary>
    /// Gets condition.
    /// </summary>
    public class Condition
    {
        private readonly DalamudPluginInterface pluginInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="Condition"/> class.
        /// </summary>
        /// <param name="pluginInterface">dalamud plugin interface.</param>
        public Condition(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        /// <summary>
        /// Indicator if in combat.
        /// </summary>
        /// <returns>in combat state.</returns>
        public bool InCombat()
        {
            try
            {
                return this.pluginInterface.ClientState.Condition[ConditionFlag.InCombat];
            }
            catch
            {
                Logger.LogInfo("InCombat condition flag is not available.");
                return false;
            }
        }
    }
}
