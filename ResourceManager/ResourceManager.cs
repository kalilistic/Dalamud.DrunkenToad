using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace DalamudPluginCommon
{
	public class ResourceManager
	{
		protected readonly IPluginBase Plugin;

		public ResourceManager(IPluginBase plugin)
		{
			Plugin = plugin;
			EmbeddedResourcePath = $"{Plugin.PluginName}.Resource";
			LocalResourcePath = Plugin.PluginFolder();
			RemoteResourcePath =
				$"https://raw.githubusercontent.com/kalilistic/{Plugin.PluginName}/master/src/{Plugin.PluginName}/Resource";
			AddLocResources();
		}

		public string EmbeddedResourcePath { get; set; }
		public string LocalResourcePath { get; set; }
		public string RemoteResourcePath { get; set; }
		public Dictionary<string, string> ResourceDictionary { get; set; } = new Dictionary<string, string>();

		public void AddLocResources()
		{
			foreach (var lang in PluginLanguage.Languages.Where(lang =>
				!lang.Code.Equals(PluginLanguage.Default.Code) && !lang.Code.Equals(PluginLanguage.English.Code)))
				ResourceDictionary.Add($"{RemoteResourcePath}/loc/{lang.Code}.json",
					$"{LocalResourcePath}/loc/{lang.Code}.json");
		}

		public string LoadEmbeddedResource(string resourceFile)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceStream =
				assembly.GetManifestResourceStream(resourceFile);
			if (resourceStream == null) return null;
			using (var reader = new StreamReader(resourceStream))
			{
				return reader.ReadToEnd();
			}
		}

		internal bool IsResourceDictionaryValid()
		{
			if (ResourceDictionary != null && ResourceDictionary.Count != 0) return true;
			Plugin.LogInfo("Resource dictionary is empty so nothing to download.");
			return false;
		}

		internal int GetEmbeddedResourceVersion()
		{
			try
			{
				return Convert.ToInt32(LoadEmbeddedResource($"{EmbeddedResourcePath}.version"));
			}
			catch (Exception ex)
			{
				Plugin.LogError(ex, "Failed to get embedded resource version so defaulting to 0.");
				return 0;
			}
		}

		internal int GetLocalResourceVersion()
		{
			try
			{
				return Convert.ToInt32(File.ReadAllText($"{LocalResourcePath}/version"));
			}
			catch (FileNotFoundException)
			{
				Plugin.LogInfo("No local resource version found so defaulting to 0.");
			}
			catch (Exception ex)
			{
				Plugin.LogError(ex, "Failed to get local resource version so defaulting to 0.");
			}

			return 0;
		}

		internal int GetRemoteResourceVersion()
		{
			try
			{
				int remoteVersion;
				using (var client = new WebClient())
				{
					remoteVersion =
						Convert.ToInt32(
							Encoding.Default.GetString(client.DownloadData($"{RemoteResourcePath}/version")));
				}

				return remoteVersion;
			}
			catch (Exception ex)
			{
				Plugin.LogError(ex, "Failed to get remote resource version so defaulting to 0.");
				return 0;
			}
		}

		internal bool CreateResourceDirectory()
		{
			try
			{
				Directory.CreateDirectory(LocalResourcePath);
				return true;
			}
			catch (Exception ex)
			{
				Plugin.LogError(ex, "Failed to create resource directory.");
				return false;
			}
		}

		internal bool IsValidFilePath(string filePath)
		{
			if (!string.IsNullOrEmpty(filePath)) return true;
			Plugin.LogError("Invalid resource file path.");
			return false;
		}

		internal bool CreateSubDirectory(string filePath)
		{
			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException());
				return true;
			}
			catch (Exception ex)
			{
				Plugin.LogError(ex, "Failed to create resource sub-directory.");
				return false;
			}
		}

		internal bool DownloadResource(string remoteLocalPath, string localFilePath)
		{
			try
			{
				using (var client = new WebClient())
				{
					var data = client.DownloadData(remoteLocalPath);
					File.WriteAllBytes(localFilePath, data);
					return true;
				}
			}
			catch (Exception ex)
			{
				Plugin.LogError(ex, "Failed to download resource.");
				return false;
			}
		}

		internal bool DownloadResources()
		{
			foreach (var resource in ResourceDictionary)
			{
				Plugin.LogInfo("Downloading {0} to {1}...", resource.Key, resource.Value);
				var localFilePath = Path.Combine(Plugin.PluginFolder(), resource.Value);
				if (IsValidFilePath(localFilePath) && CreateSubDirectory(localFilePath) &&
				    DownloadResource(resource.Key, localFilePath)) continue;
				Plugin.LogInfo("Failed to download resource {0}.", resource.Key);
				return false;
			}

			try
			{
				DownloadResource($"{RemoteResourcePath}/version", $"{LocalResourcePath}/version");
				Plugin.LogInfo("Successfully downloaded resources.");
				return true;
			}
			catch (Exception ex)
			{
				Plugin.LogError(ex, "Failed to update resource version.");
				return false;
			}
		}

		internal bool IsUpdateAvailable()
		{
			try
			{
				var embeddedVersion = GetEmbeddedResourceVersion();
				var localVersion = GetLocalResourceVersion();
				var remoteVersion = GetRemoteResourceVersion();

				var currentVersion = embeddedVersion >= localVersion ? embeddedVersion : localVersion;

				if (remoteVersion > currentVersion)
				{
					Plugin.LogInfo("Resource update is available (v{0}).", remoteVersion);
					return true;
				}

				Plugin.LogInfo("No resource update available.");
				return false;
			}
			catch (Exception ex)
			{
				Plugin.LogError(ex, "Failed to perform resource version check.");
				return false;
			}
		}

		public bool UpdateResources()
		{
			return IsResourceDictionaryValid() &&
			       CreateResourceDirectory() &&
			       IsUpdateAvailable() &&
			       DownloadResources();
		}
	}
}