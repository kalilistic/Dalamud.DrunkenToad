using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CheapLoc;

namespace DalamudPluginCommon
{
	public class Localization : ILocalization
	{
		private readonly IPluginBase _plugin;

		public Localization(IPluginBase plugin)
		{
			_plugin = plugin;
		}

		internal void SetLanguage(int pluginLanguageIndex)
		{
			var language = PluginLanguage.GetLanguageByIndex(pluginLanguageIndex);
			SetLanguage(language);
		}

		internal void SetLanguage(PluginLanguage language)
		{
			var languageCode = GetLanguageCode(language);
			_plugin.LogInfo("Attempting to load lang {0}", languageCode);
			if (languageCode != PluginLanguage.English.Code)
				try
				{
					string locData;
					var locPath = _plugin.PluginFolder() + $"\\loc\\{languageCode}.json";
					_plugin.LogInfo("Loc path set to {0}", locPath);
					if (File.Exists(locPath))
					{
						_plugin.LogInfo("Loading loc from local resource");
						locData = File.ReadAllText(locPath);
						if (string.IsNullOrEmpty(locData))
						{
							_plugin.LogError("Local loc data is corrupt so falling back to embedded");
							locData = LoadEmbeddedLocData(languageCode);
						}
					}
					else
					{
						_plugin.LogInfo("Loading loc from embedded resource", languageCode);
						locData = LoadEmbeddedLocData(languageCode);
					}

					Loc.Setup(locData);
				}
				catch (Exception ex)
				{
					_plugin.LogError(ex, "Failed to load loc data so using fallback");
					Loc.SetupWithFallbacks();
				}
			else
				Loc.SetupWithFallbacks();
		}

		internal string GetLanguageCode(PluginLanguage language)
		{
			var languageCode = language.Code.ToLower();
			if (languageCode == PluginLanguage.Default.Code)
			{
				languageCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
				_plugin.LogInfo("PluginLanguage is default so using UICulture {0}", languageCode);
			}

			if (PluginLanguage.Languages.Any(lang => languageCode == lang.Code)) return languageCode;
			_plugin.LogInfo("UICulture {0} is not supported so using fallback", languageCode);
			languageCode = PluginLanguage.English.Code;
			return languageCode;
		}

		internal string LoadEmbeddedLocData(string languageCode)
		{
			var resourceFile = $"{_plugin.PluginName}.Resource.loc.{languageCode}.json";
			_plugin.LogInfo("Loading lang resource file {0}", resourceFile);
			var assembly = GetAssembly();
			var resourceStream =
				assembly.GetManifestResourceStream(resourceFile);
			if (resourceStream == null) return null;
			using (var reader = new StreamReader(resourceStream))
			{
				return reader.ReadToEnd();
			}
		}

		internal void ExportLocalizable()
		{
			Loc.ExportLocalizableForAssembly(GetAssembly());
		}

		private static Assembly GetAssembly()
		{
			return Assembly.GetExecutingAssembly();
		}
	}
}