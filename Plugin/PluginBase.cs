// ReSharper disable UnusedMemberInSuper.Global

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using Dalamud.Configuration;
using Dalamud.Game.Chat;
using Dalamud.Game.Chat.SeStringHandling;
using Dalamud.Game.Chat.SeStringHandling.Payloads;
using Dalamud.Game.ClientState.Actors.Types;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;

namespace DalamudPluginCommon
{
	public abstract class PluginBase : IPluginBase
	{
		public readonly DalamudPluginInterface PluginInterface;

		protected PluginBase(string pluginName, DalamudPluginInterface pluginInterface)
		{
			PluginName = pluginName;
			PluginInterface = pluginInterface;
			ResourceManager = new ResourceManager(this);
			Localization = new Localization(this);
			SetupCommands();
		}

		public ResourceManager ResourceManager { get; }

		public Localization Localization { get; }
		public string PluginName { get; }

		public void SetLanguage(PluginLanguage language)
		{
			Localization.SetLanguage(language);
		}

		public void PrintMessage(string message)
		{
			var payloadList = BuildMessagePayload();
			payloadList.Add(new UIForegroundPayload(PluginInterface.Data, 566));
			payloadList.Add(new TextPayload(message));
			payloadList.Add(new UIForegroundPayload(PluginInterface.Data, 0));
			SendMessagePayload(payloadList);
		}

		public string GetSeIcon(SeIconChar seIconChar)
		{
			return Convert.ToChar(seIconChar, CultureInfo.InvariantCulture)
				.ToString(CultureInfo.InvariantCulture);
		}

		public uint? GetLocalPlayerHomeWorld()
		{
			try
			{
				return PluginInterface.ClientState.LocalPlayer.HomeWorld.Id;
			}
			catch
			{
				LogInfo("LocalPlayer HomeWorldId is not available.");
				return null;
			}
		}

		public void LogInfo(string messageTemplate)
		{
			PluginLog.Log(messageTemplate);
		}

		public void LogInfo(string messageTemplate, params object[] values)
		{
			PluginLog.Log(messageTemplate, values);
		}

		public void LogError(string messageTemplate)
		{
			PluginLog.LogError(messageTemplate);
		}

		public void LogError(string messageTemplate, params object[] values)
		{
			PluginLog.LogError(messageTemplate, values);
		}

		public void LogError(Exception exception, string messageTemplate, params object[] values)
		{
			PluginLog.LogError(exception, messageTemplate, values);
		}

		public bool IsKeyPressed(ModifierKey.Enum key)
		{
			return PluginInterface.ClientState.KeyState[(byte) key];
		}

		public bool IsKeyPressed(PrimaryKey.Enum key)
		{
			return PluginInterface.ClientState.KeyState[(byte) key];
		}

		public void SaveConfig(dynamic config)
		{
			PluginInterface.SavePluginConfig((IPluginConfiguration) config);
		}

		public dynamic LoadConfig()
		{
			return PluginInterface.GetPluginConfig();
		}

		public void Dispose()
		{
			RemoveCommands();
		}

		public string PluginFolder()
		{
			return Path.Combine(new[]
			{
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"XIVLauncher",
				"pluginConfigs",
				PluginName
			});
		}

		public void UpdateResources()
		{
			ResourceManager.UpdateResources();
		}

		protected List<Payload> BuildMessagePayload()
		{
			return new List<Payload>
			{
				new UIForegroundPayload(PluginInterface.Data, 0),
				new TextPayload($"[{PluginName}] ")
			};
		}

		protected void SendMessagePayload(List<Payload> payloadList)
		{
			var payload = new SeString(payloadList);
			PluginInterface.Framework.Gui.Chat.PrintChat(new XivChatEntry
			{
				MessageBytes = payload.Encode()
			});
		}

		public void ExportLocalizable(string command, string args)
		{
			LogInfo("Running command {0} with args {1}", command, args);
			Localization.ExportLocalizable();
		}

		protected void SetupCommands()
		{
			PluginInterface.CommandManager.AddHandler("/" + PluginName.ToLower() + "exloc",
				new CommandInfo(ExportLocalizable)
				{
					ShowInHelp = false
				});
		}

		protected void RemoveCommands()
		{
			PluginInterface.CommandManager.RemoveHandler("/" + PluginName.ToLower() + "exloc");
		}

		public List<PlayerCharacter> GetPlayerCharacters()
		{
			return PluginInterface.ClientState.Actors.Where(actor =>
					actor is PlayerCharacter character &&
					actor.ActorId != PluginInterface.ClientState.LocalPlayer?.ActorId &&
					character.HomeWorld.Id != ushort.MaxValue && 
					character.CurrentWorld.Id != ushort.MaxValue)
				.Select(actor => actor as PlayerCharacter).ToList();
		}

		public uint GetTerritoryType()
		{
			try
			{
				return PluginInterface.ClientState.TerritoryType;
			}
			catch
			{
				LogInfo("TerritoryType is not available.");
				return 0;
			}
		}

		public string GetPlaceName(uint territoryType)
		{
			try
			{
				return PluginInterface.Data.GetExcelSheet<TerritoryType>().GetRow(territoryType).PlaceName.Value.Name;
			}
			catch
			{
				LogInfo("PlaceName is not available.");
				return null;
			}
		}

		public string PluginVersion()
		{
			return Assembly.GetExecutingAssembly()
				.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
		}

		public string CompressString(string str)
		{
			string compressed;
			using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(str)))
			{
				using (var compressedMemoryStream = new MemoryStream())
				{
					var gzipStream = new GZipStream(compressedMemoryStream, CompressionMode.Compress);
					memoryStream.CopyTo(gzipStream);
					gzipStream.Dispose();
					compressed = Convert.ToBase64String(compressedMemoryStream.ToArray());
				}
			}

			return compressed;
		}

		public string DecompressString(string str)
		{
			string decompressed;
			using (var memoryStream = new MemoryStream(Convert.FromBase64String(str)))
			{
				using (var decompressedMemoryStream = new MemoryStream())
				{
					var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
					gzipStream.CopyTo(decompressedMemoryStream);
					gzipStream.Dispose();

					decompressed = Encoding.UTF8.GetString(decompressedMemoryStream.ToArray());
				}
			}

			return decompressed;

		}

		public void CreateDataFolder()
		{
			try
			{
				Directory.CreateDirectory(PluginFolder() + "/data");
			}
			catch (Exception ex)
			{
				LogError(ex, "Failed to create data folder.");
			}
		}
	}
}