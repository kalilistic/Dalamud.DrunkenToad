using System;
using System.IO;

using CheapLoc;
using Dalamud.Game.Command;

namespace Dalamud.DrunkenToad
{
    /// <summary>
    /// Gets localization.
    /// </summary>
    public class Localization
    {
        private readonly IPluginBase plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="Localization"/> class.
        /// </summary>
        /// <param name="plugin">Current plugin base.</param>
        public Localization(IPluginBase plugin)
        {
            this.plugin = plugin;
            this.SetLanguage(this.plugin.PluginInterface.UiLanguage);
            this.plugin.PluginInterface.OnLanguageChanged += this.OnLanguageChanged;
            this.plugin.PluginInterface.CommandManager.AddHandler(
                "/" + this.plugin.PluginName.ToLower() + "exloc",
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
                    var resourceFile = $"{this.plugin.PluginName}.{this.plugin.PluginName}.Resource.translation.{languageCode}.json";
                    var resourceStream = this.plugin.Assembly.GetManifestResourceStream(resourceFile);
                    using (var reader = new StreamReader(resourceStream ?? throw new InvalidOperationException()))
                    {
                        locData = reader.ReadToEnd();
                    }

                    Loc.Setup(locData, this.plugin.Assembly);
                }
                catch (Exception)
                {
                    Loc.SetupWithFallbacks(this.plugin.Assembly);
                }
            }
            else
            {
                Loc.SetupWithFallbacks(this.plugin.Assembly);
            }
        }

        /// <summary>
        /// Dispose localization.
        /// </summary>
        public void Dispose()
        {
            this.plugin.PluginInterface.OnLanguageChanged -= this.OnLanguageChanged;
            this.plugin.PluginInterface.CommandManager.RemoveHandler("/" + this.plugin.PluginName.ToLower() + "exloc");
        }

        private void ExportLocalizable(string command, string args)
        {
            Loc.ExportLocalizableForAssembly(this.plugin.Assembly);
        }

        private void OnLanguageChanged(string langCode)
        {
            this.SetLanguage(langCode);
        }
    }
}