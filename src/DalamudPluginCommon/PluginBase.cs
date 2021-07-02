using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Dalamud.Configuration;
using Dalamud.Interface;
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
            this.GameData = new GameData(pluginInterface);
            this.ClientState = new ClientState(pluginInterface);
            this.Chat = new Chat(pluginName, pluginInterface);
            this.localization = new Localization(this);
            this.InitializeFontAwesome();
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
        /// Gets game data.
        /// </summary>
        public GameData GameData { get; }

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
        /// Gets fontAwesomeIcon list.
        /// </summary>
        public FontAwesomeIcon[] Icons { get; private set; } = null!;

        /// <summary>
        /// Gets fontAwesomeIcon names.
        /// </summary>
        public string[] IconNames { get; private set; } = null!;

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

        /// <summary>
        /// Returns indicator if player is in content.
        /// </summary>
        /// <returns>indicator if in content.</returns>
        public bool InContent()
        {
            try
            {
                var territoryTypeId = this.ClientState.TerritoryType();
                if (territoryTypeId == 0)
                {
                    return false;
                }

                var contentId = this.GameData.ContentId(territoryTypeId);
                return contentId != 0;
            }
            catch
            {
                return false;
            }
        }

        private void InitializeFontAwesome()
        {
            // initial arrays from default list
            var iconNames = Enum.GetNames(typeof(FontAwesomeIcon)).ToList();
            var icons = Enum.GetValues(typeof(FontAwesomeIcon)).Cast<FontAwesomeIcon>().ToList();

            // get excluded icon
            var excludedIcons = new List<string>();
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DalamudPluginCommon.ExcludedIcons.txt");
            using var reader = new StreamReader(stream!);
            {
                while (!reader.EndOfStream)
                {
                    excludedIcons.Add(reader.ReadLine());
                }
            }

            // remove excluded icons
            for (var i = 0; i < iconNames.Count; i++)
            {
                if (excludedIcons.Contains(iconNames[i]))
                {
                    iconNames.RemoveAt(i);
                    icons.RemoveAt(i);
                    i--;
                }
            }

            // save as sorted arrays
            this.IconNames = iconNames.ToArray();
            this.Icons = icons.ToArray();
            Array.Sort(this.IconNames, this.Icons);
        }
    }
}
