using System.Reflection;

using Dalamud.Plugin;

namespace DalamudPluginCommon
{
    /// <summary>
    /// Base plugin.
    /// </summary>
    public interface IPluginBase
    {
        /// <summary>
        /// Gets plugin interface.
        /// </summary>
        DalamudPluginInterface PluginInterface { get; }

        /// <summary>
        /// Gets current client state.
        /// </summary>
        ClientState ClientState { get; }

        /// <summary>
        /// Gets chat.
        /// </summary>
        Chat Chat { get; }

        /// <summary>
        /// Gets plugin assembly.
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// Gets plugin name.
        /// </summary>
        string PluginName { get; }

        /// <summary>
        /// Save plugin configuration.
        /// </summary>
        /// <param name="config">configuration.</param>
        void SaveConfig(dynamic config);

        /// <summary>
        /// Load plugin configuration.
        /// </summary>
        /// <returns>configuration.</returns>
        dynamic LoadConfig();

        /// <summary>
        /// Dispose base plugin.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Plugin config directory.
        /// </summary>
        /// <returns>Path to plugin config folder.</returns>
        string PluginFolder();

        /// <summary>
        /// Plugin version.
        /// </summary>
        /// <returns>plugin version.</returns>
        string PluginVersion();

        /// <summary>
        /// Gets plugin version number.
        /// </summary>
        /// <returns>plugin version number.</returns>
        int PluginVersionNumber();
    }
}
