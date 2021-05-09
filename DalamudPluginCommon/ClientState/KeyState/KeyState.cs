using Dalamud.Plugin;

namespace DalamudPluginCommon
{
    /// <summary>
    /// Gets KeyState.
    /// </summary>
    public class KeyState
    {
        private readonly DalamudPluginInterface pluginInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyState"/> class.
        /// </summary>
        /// <param name="pluginInterface">dalamud plugin interface.</param>
        public KeyState(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        /// <summary>
        /// Check if key is pressed.
        /// </summary>
        /// <param name="key">Key to evaluate.</param>
        /// <returns>Indicator if key is pressed.</returns>
        public bool IsKeyPressed(PrimaryKey.Enum key)
        {
            return this.pluginInterface.ClientState.KeyState[(byte)key];
        }

        /// <summary>
        /// Check if key is pressed.
        /// </summary>
        /// <param name="key">Key to evaluate.</param>
        /// <returns>Indicator if key is pressed.</returns>
        public bool IsKeyPressed(ModifierKey.Enum key)
        {
            return this.pluginInterface.ClientState.KeyState[(byte)key];
        }
    }
}
