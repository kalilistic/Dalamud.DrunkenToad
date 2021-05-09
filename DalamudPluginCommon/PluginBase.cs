using System;
using System.Reflection;

using Dalamud.Configuration;
using Dalamud.Plugin;

namespace DalamudPluginCommon
{
    /// <summary>
    /// Base plugin.
    /// </summary>
    public abstract class PluginBase : IPluginBase
    {
        private readonly Localization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginBase"/> class.
        /// </summary>
        /// <param name="pluginName">plugin name.</param>
        /// <param name="pluginInterface">plugin interface.</param>
        /// <param name="assembly">plugin assembly.</param>
        protected PluginBase(string pluginName, DalamudPluginInterface pluginInterface, Assembly assembly)
        {
            this.PluginName = pluginName;
            this.PluginInterface = pluginInterface;
            this.Assembly = assembly;
            this.ClientState = new ClientState(pluginInterface);
            this.Chat = new Chat(pluginName, pluginInterface);
            this.localization = new Localization(this);
        }

        /// <summary>
        /// Gets plugin assembly.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Gets plugin interface.
        /// </summary>
        public DalamudPluginInterface PluginInterface { get; }

        /// <summary>
        /// Gets current client state.
        /// </summary>
        public ClientState ClientState { get; }

        /// <summary>
        /// Gets chat.
        /// </summary>
        public Chat Chat { get; }

        /// <summary>
        /// Gets plugin name.
        /// </summary>
        public string PluginName { get; }

        /// <summary>
        /// Save plugin configuration.
        /// </summary>
        /// <param name="config">configuration.</param>
        public void SaveConfig(dynamic config)
        {
            this.PluginInterface.SavePluginConfig((IPluginConfiguration)config);
        }

        /// <summary>
        /// Load plugin configuration.
        /// </summary>
        /// <returns>configuration.</returns>
        public dynamic LoadConfig()
        {
            return this.PluginInterface.GetPluginConfig();
        }

        /// <summary>
        /// Dispose base plugin.
        /// </summary>
        public void Dispose()
        {
            this.localization.Dispose();
            this.ClientState.Dispose();
        }

        /// <summary>
        /// Plugin config directory.
        /// </summary>
        /// <returns>Path to plugin config folder.</returns>
        public string PluginFolder()
        {
            return this.PluginInterface.GetPluginConfigDirectory();
        }

        /// <summary>
        /// Plugin version.
        /// </summary>
        /// <returns>plugin version.</returns>
        public string PluginVersion()
        {
            try
            {
                return Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to get plugin version so defaulting.");
                return "1.0.0.0";
            }
        }

        /// <summary>
        /// Gets plugin version number.
        /// </summary>
        /// <returns>plugin version number.</returns>
        public int PluginVersionNumber()
        {
            try
            {
                var pluginVersion = this.PluginVersion();
                pluginVersion = pluginVersion.Replace(".", string.Empty);
                return Convert.ToInt32(pluginVersion);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to parse plugin version.");
                return 0;
            }
        }
    }
}
