using System;
using System.IO;
using System.Reflection;

using CheapLoc;
using Dalamud.Game.Command;
using Dalamud.Plugin;

namespace Dalamud.DrunkenToad
{
    /// <summary>
    /// Gets localization.
    /// </summary>
    public class Localization
    {
        private readonly DalamudPluginInterface pluginInterface;
        private readonly CommandManager commandManager;
        private readonly string pluginName;
        private readonly Assembly assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="Localization"/> class.
        /// </summary>
        /// <param name="pluginInterface">plugin interface.</param>
        /// <param name="commandManager">command manager.</param>
        public Localization(DalamudPluginInterface pluginInterface, CommandManager commandManager)
        {
            this.pluginInterface = pluginInterface;
            this.commandManager = commandManager;
            this.assembly = Assembly.GetCallingAssembly();
            this.pluginName = this.assembly.GetName().Name?.ToLower() ?? string.Empty;
            this.SetLanguage(this.pluginInterface.UiLanguage);
            this.pluginInterface.LanguageChanged += this.LanguageChanged;
            this.commandManager.AddHandler(
                "/" + this.pluginName + "exloc",
                new CommandInfo(this.ExportLocalizable)
                {
                    ShowInHelp = false,
                });
        }

        /// <summary>
        /// Sets the plugin language.
        /// </summary>
        /// <param name="languageCode">The iso language code to use.</param>
        public void SetLanguage(string languageCode)
        {
            if (!string.IsNullOrEmpty(languageCode) && languageCode != "en")
            {
                try
                {
                    string locData;
                    var resourceFile = $"{this.pluginName}.{this.pluginName}.Resource.translation.{languageCode}.json";
                    var resourceStream = this.assembly.GetManifestResourceStream(resourceFile);
                    using (var reader = new StreamReader(resourceStream ?? throw new InvalidOperationException()))
                    {
                        locData = reader.ReadToEnd();
                    }

                    Loc.Setup(locData, this.assembly);
                }
                catch (Exception)
                {
                    Loc.SetupWithFallbacks(this.assembly);
                }
            }
            else
            {
                Loc.SetupWithFallbacks(this.assembly);
            }
        }

        /// <summary>
        /// Dispose localization.
        /// </summary>
        public void Dispose()
        {
            this.pluginInterface.LanguageChanged -= this.LanguageChanged;
            this.commandManager.RemoveHandler("/" + this.pluginName + "exloc");
        }

        private void ExportLocalizable(string command, string args)
        {
            Loc.ExportLocalizableForAssembly(this.assembly);
        }

        private void LanguageChanged(string langCode)
        {
            this.SetLanguage(langCode);
        }
    }
}
