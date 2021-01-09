// ReSharper disable UnusedMemberInSuper.Global

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Dalamud.Configuration;
using Dalamud.Game.Chat;
using Dalamud.Game.Chat.SeStringHandling;
using Dalamud.Game.Chat.SeStringHandling.Payloads;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Actors.Types;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;

namespace DalamudPluginCommon
{
	public abstract class PluginBase : IPluginBase
	{
		public readonly DalamudPluginInterface PluginInterface;
		private bool _isLoggedIn;
		private bool _showPlugin;

		protected PluginBase(string pluginName, DalamudPluginInterface pluginInterface)
		{
			PluginName = pluginName;
			PluginInterface = pluginInterface;
			ResourceManager = new ResourceManager(this);
			Localization = new Localization(this);
			SetupCommands();
			AddEventHandlers();
			UpdateLoggedInState();
		}

		public ResourceManager ResourceManager { get; }

		public Localization Localization { get; }
		public string PluginName { get; }

		public void PrintMessage(string message)
		{
			var payloadList = BuildMessagePayload();
			payloadList.Add(new UIForegroundPayload(PluginInterface.Data, ChatColor.Blue));
			payloadList.Add(new TextPayload(message));
			payloadList.Add(new UIForegroundPayload(PluginInterface.Data, ChatColor.White));
			SendMessagePayload(payloadList);
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
			RemoveEventHandlers();
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

		public string PluginVersion()
		{
			try
			{
				return Assembly.GetExecutingAssembly()
					.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
			}
			catch (Exception ex)
			{
				LogError(ex, "Failed to get plugin version so defaulting.");
				return "1.0.0.0";
			}
		}

		public bool IsLoggedIn()
		{
			return _isLoggedIn;
		}

		public string GetSeIcon(SeIconChar seIconChar)
		{
			return Convert.ToChar(seIconChar, CultureInfo.InvariantCulture)
				.ToString(CultureInfo.InvariantCulture);
		}

		public void UpdateLoggedInState()
		{
			if (PluginInterface.Data.IsDataReady && PluginInterface.ClientState.LocalPlayer != null) _isLoggedIn = true;
		}

		public void AddEventHandlers()
		{
			PluginInterface.ClientState.OnLogin += ClientStateOnOnLogin;
			PluginInterface.ClientState.OnLogout += ClientStateOnOnLogout;
		}

		private void ClientStateOnOnLogout(object sender, EventArgs e)
		{
			_isLoggedIn = false;
		}

		private void ClientStateOnOnLogin(object sender, EventArgs e)
		{
			_isLoggedIn = true;
		}

		public void RemoveEventHandlers()
		{
			PluginInterface.ClientState.OnLogin -= ClientStateOnOnLogin;
			PluginInterface.ClientState.OnLogout -= ClientStateOnOnLogout;
		}

		public void SetLanguage(PluginLanguage language)
		{
			Localization.SetLanguage(language);
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
			if (PluginInterface.ClientState.LocalPlayer?.ActorId == 0) return null;
			var actors = PluginInterface.ClientState.Actors.ToList();
			return actors.Where(actor =>
					actor is PlayerCharacter character &&
					actor.ActorId != PluginInterface.ClientState.LocalPlayer?.ActorId &&
					character.HomeWorld.Id != ushort.MaxValue &&
					character.CurrentWorld.Id != ushort.MaxValue)
				.Select(actor => actor as PlayerCharacter).ToList()
				.GroupBy(player => new {player.Name, player.HomeWorld.Id})
				.Select(player => player.First())
				.ToList();
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

		public string GetWorldName(uint worldId)
		{
			try
			{
				return PluginInterface.Data.GetExcelSheet<World>().GetRow(worldId).Name;
			}
			catch
			{
				LogInfo("WorldName is not available.");
				return null;
			}
		}

		public uint? GetWorldId(string worldName)
		{
			try
			{
				return PluginInterface.Data.GetExcelSheet<World>()
					.FirstOrDefault(world => world.Name.ToString().Equals(worldName))?.RowId;
			}
			catch
			{
				LogInfo("WorldId is not available.");
				return null;
			}
		}

		public string GetJobCode(uint classJobId)
		{
			try
			{
				return PluginInterface.Data.GetExcelSheet<ClassJob>().GetRow(classJobId).Abbreviation;
			}
			catch
			{
				LogInfo("JobCode is not available.");
				return null;
			}
		}

		public string GetPlaceName(uint territoryTypeId)
		{
			try
			{
				return PluginInterface.Data.GetExcelSheet<TerritoryType>().GetRow(territoryTypeId).PlaceName.Value.Name;
			}
			catch
			{
				LogInfo("PlaceName is not available.");
				return null;
			}
		}

		public string GetContentName(uint contentId)
		{
			try
			{
				return contentId == 0
					? string.Empty
					: PluginInterface.Data.GetExcelSheet<ContentFinderCondition>().GetRow(contentId).Name.ToString();
			}
			catch
			{
				LogInfo("ContentName is not available.");
				return string.Empty;
			}
		}

		public bool IsHighEndDuty(uint contentId)
		{
			try
			{
				return PluginInterface.Data.GetExcelSheet<ContentFinderCondition>().GetRow(contentId).HighEndDuty;
			}
			catch
			{
				LogInfo("Content HighEndDuty is not available.");
				return false;
			}
		}

		public uint GetContentId(uint territoryTypeId)
		{
			try
			{
				return PluginInterface.Data.GetExcelSheet<ContentFinderCondition>()
					.FirstOrDefault(condition => condition.TerritoryType.Row == territoryTypeId)?.RowId ?? 0;
			}
			catch
			{
				LogInfo("ContentName is not available.");
				return 0;
			}
		}

		public bool InCombat()
		{
			try
			{
				return PluginInterface.ClientState.Condition[ConditionFlag.InCombat];
			}
			catch
			{
				LogInfo("InCombat condition flag is not available.");
				return false;
			}
		}
	}
}