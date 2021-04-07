// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable ConvertIfStatementToReturnStatement// ReSharper disable UseNegatedPatternMatching

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Dalamud.Configuration;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Actors.Types;
using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;

namespace DalamudPluginCommon
{
    public abstract class PluginBase : IPluginBase
    {
        public readonly DalamudPluginInterface PluginInterface;
        private bool _isLoggedIn;
        private int _previousFocusTarget;
        private List<string> _worldNames;

        protected PluginBase(string pluginName, DalamudPluginInterface pluginInterface, string repoName = "")
        {
            PluginName = pluginName;
            PluginInterface = pluginInterface;
            ResourceManager = new ResourceManager(this, repoName);
            Localization = new Localization(this);
            SetupCommands();
            AddEventHandlers();
            UpdateLoggedInState();
        }

        public uint[] ContentIds { get; set; }
        public string[] ContentNames { get; set; }
        public uint[] ItemIds { get; set; }
        public string[] ItemNames { get; set; }
        public uint[] ItemCategoryIds { get; set; }
        public string[] ItemCategoryNames { get; set; }
        public List<KeyValuePair<uint, ItemList>> ItemLists { get; set; }

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

        public void LogVerbose(string messageTemplate)
        {
            PluginLog.LogVerbose(messageTemplate);
        }

        public void LogDebug(string messageTemplate)
        {
            PluginLog.LogDebug(messageTemplate);
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
            return PluginInterface.GetPluginConfigDirectory();
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

        public double ConvertHeightToInches(int raceId, int tribeId, int genderId, int sliderHeight)
        {
            try
            {
                var charHeight = CharHeight.GetCharHeight(raceId, tribeId, genderId);
                if (charHeight == null) return 0;
                return charHeight.MinHeight + Math.Round(sliderHeight * charHeight.Ratio, 1);
            }
            catch
            {
                LogInfo("Failed to calculate height.");
                return 0;
            }
        }

        public ushort ClientLanguage()
        {
            try
            {
                return (ushort) PluginInterface.ClientState.ClientLanguage;
            }
            catch
            {
                return (ushort) Dalamud.ClientLanguage.English;
            }
        }

        public string GetLocalPlayerName()
        {
            try
            {
                return PluginInterface.ClientState.LocalPlayer.Name;
            }
            catch
            {
                LogInfo("LocalPlayer Name is not available.");
                return null;
            }
        }

        public int PluginVersionNumber()
        {
            try
            {
                var pluginVersion = PluginVersion();
                pluginVersion = pluginVersion.Replace(".", "");
                return Convert.ToInt32(pluginVersion);
            }
            catch (Exception ex)
            {
                LogError(ex, "Failed to parse plugin version.");
                return 0;
            }
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
                    actor.ActorId != 0 &&
                    actor.ActorId != PluginInterface.ClientState.LocalPlayer?.ActorId &&
                    character.HomeWorld.Id != ushort.MaxValue &&
                    character.CurrentWorld.Id != ushort.MaxValue &&
                    character.ClassJob != null && character.ClassJob.Id != 0 &&
                    IsValidCharacterName(character.Name))
                .Select(actor => actor as PlayerCharacter).ToList()
                .GroupBy(player => new {player.Name, player.ActorId})
                .Select(player => player.First())
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

        public uint GetDataCenterId()
        {
            try
            {
                return PluginInterface.Data.GetExcelSheet<World>().First(world =>
                        world.RowId == PluginInterface.ClientState.LocalPlayer.HomeWorld.Id)
                    .DataCenter.Value.RowId;
            }
            catch
            {
                LogInfo("DataCenterId is not available.");
                return 0;
            }
        }

        public List<string> GetWorldNames(uint dcId)
        {
            try
            {
                if (_worldNames != null) return _worldNames;
                _worldNames = PluginInterface.Data.GetExcelSheet<World>()
                    .Where(world => world.IsPublic && world.DataCenter.Value.RowId == dcId)
                    .Select(world => world.Name.ToString()).OrderBy(worldName => worldName).ToList();
                return _worldNames;
            }
            catch
            {
                LogInfo("WorldNames are not available.");
                return null;
            }
        }

        public string GetGender(int? genderId)
        {
            try
            {
                switch (genderId)
                {
                    case 0:
                        return FontAwesomeIcon.Mars.ToIconString();
                    case 1:
                        return FontAwesomeIcon.Venus.ToIconString();
                    default:
                        return string.Empty;
                }
            }
            catch
            {
                LogInfo("Gender is not available.");
                return null;
            }
        }

        public string GetTribe(int id, int genderId)
        {
            try
            {
                if (id == 0) return string.Empty;
                var tribe = PluginInterface.Data.GetExcelSheet<Tribe>()
                    .FirstOrDefault(tribeEntry => tribeEntry.RowId == id);
                if (tribe == null) return string.Empty;
                switch (genderId)
                {
                    case 0:
                        return tribe.Masculine;
                    case 1:
                        return tribe.Feminine;
                    default:
                        return string.Empty;
                }
            }
            catch
            {
                LogInfo("Tribe is not available.");
                return string.Empty;
            }
        }

        public string GetRace(int id, int genderId)
        {
            try
            {
                if (id == 0) return string.Empty;
                var race = PluginInterface.Data.GetExcelSheet<Race>()
                    .FirstOrDefault(raceEntry => raceEntry.RowId == id);
                if (race == null) return string.Empty;
                switch (genderId)
                {
                    case 0:
                        return race.Masculine;
                    case 1:
                        return race.Feminine;
                    default:
                        return string.Empty;
                }
            }
            catch
            {
                LogInfo("Race is not available.");
                return string.Empty;
            }
        }

        protected void InitContent()
        {
            try
            {
                var excludedContent = new List<uint> {69, 70, 71};
                var contentTypes = new List<uint> {2, 4, 5, 6, 26, 28, 29};
                var contentList = PluginInterface.Data.GetExcelSheet<ContentFinderCondition>()
                    .Where(content =>
                        contentTypes.Contains(content.ContentType.Row) && !excludedContent.Contains(content.RowId))
                    .ToList();
                var contentNames = contentList.Select(content => content.Name.ToString().Sanitize()).ToArray();
                var contentIds = contentList.Select(content => content.RowId).ToArray();
                Array.Sort(contentNames, contentIds);
                ContentIds = contentIds;
                ContentNames = contentNames;
            }
            catch
            {
                LogInfo("Failed to initialize content list.");
            }
        }

        protected void InitItems()
        {
            try
            {
                // create item list
                var itemDataList = PluginInterface.Data.GetExcelSheet<Item>().Where(item => item != null
                    && !string.IsNullOrEmpty(item.Name)).ToList();

                // add all items
                var itemIds = itemDataList.Select(item => item.RowId).ToArray();
                var itemNames = itemDataList.Select(item => item.Name.ToString().Sanitize()).ToArray();
                ItemIds = itemIds;
                ItemNames = itemNames;

                // item categories
                var categoryList = PluginInterface.Data.GetExcelSheet<ItemUICategory>()
                    .Where(category => category.RowId != 0).ToList();
                var categoryNames = categoryList.Select(category => category.Name.ToString().Sanitize()).ToArray();
                var categoryIds = categoryList.Select(category => category.RowId).ToArray();
                Array.Sort(categoryNames, categoryIds);
                ItemCategoryIds = categoryIds;
                ItemCategoryNames = categoryNames;

                // populate item lists by category
                var itemLists = new List<KeyValuePair<uint, ItemList>>();
                foreach (var categoryId in categoryIds)
                {
                    var itemCategoryDataList =
                        itemDataList.Where(item => item.ItemUICategory.Row == categoryId).ToList();
                    var itemCategoryIds = itemCategoryDataList.Select(item => item.RowId).ToArray();
                    var itemCategoryNames = itemCategoryDataList.Select(item => item.Name.ToString().Sanitize()).ToArray();
                    Array.Sort(itemCategoryNames, itemCategoryIds);
                    var itemList = new ItemList
                    {
                        ItemIds = itemCategoryIds,
                        ItemNames = itemCategoryNames
                    };
                    itemLists.Add(new KeyValuePair<uint, ItemList>(categoryId, itemList));
                }

                ItemLists = itemLists;
            }
            catch
            {
                LogInfo("Failed to initialize content list.");
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
                return PluginInterface.Data.GetExcelSheet<TerritoryType>().GetRow(territoryTypeId).PlaceName.Value.Name
                    .ToString().Sanitize();
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
                    : PluginInterface.Data.GetExcelSheet<ContentFinderCondition>().GetRow(contentId).Name.ToString()
                        .Sanitize();
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

        public bool IsValidCharacterName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            var names = name.Split(' ');
            if (names.Length != 2) return false;
            if (names[0].Length < 2 || names[0].Length > 15) return false;
            if (names[1].Length < 2 || names[1].Length > 15) return false;
            if (names[0].Length + names[1].Length > 20) return false;
            if (!char.IsLetter(names[0][0])) return false;
            if (!char.IsLetter(names[1][0])) return false;
            if (!char.IsUpper(names[0][0])) return false;
            if (!char.IsUpper(names[1][0])) return false;
            if (name.Contains("  ")) return false;
            if (name.Contains("--")) return false;
            if (name.Contains("\'-")) return false;
            if (name.Contains("-\'")) return false;
            if (name.Any(c => !char.IsLetter(c) && !c.Equals('\'') && !c.Equals('-') && !c.Equals(' '))) return false;
            return true;
        }

        public Actor GetActorById(int actorId)
        {
            try
            {
                return PluginInterface.ClientState.Actors.ToList().FirstOrDefault(actor => actor.ActorId == actorId);
            }
            catch
            {
                LogInfo("Failed to find actor by id " + actorId);
                return null;
            }
        }

        public void SetCurrentTarget(int actorId)
        {
            try
            {
                if (actorId == 0) return;
                var actor = GetActorById(actorId);
                if (actor == null) return;
                PluginInterface.ClientState.Targets.SetCurrentTarget(actor);
            }
            catch
            {
                LogInfo("Failed to target actor with id " + actorId);
            }
        }

        public void RevertFocusTarget()
        {
            try
            {
                if (_previousFocusTarget == 0) return;
                var actor = GetActorById(_previousFocusTarget);
                if (actor == null) return;
                PluginInterface.ClientState.Targets.SetFocusTarget(actor);
            }
            catch
            {
                LogInfo("Failed to focus target actor with id " + _previousFocusTarget);
            }
        }

        public void SetFocusTarget(int actorId)
        {
            try
            {
                if (actorId == 0) return;
                var actor = GetActorById(actorId);
                if (actor == null) return;
                if (PluginInterface.ClientState.Targets.FocusTarget != null)
                    _previousFocusTarget = PluginInterface.ClientState.Targets.FocusTarget.ActorId;
                PluginInterface.ClientState.Targets.SetFocusTarget(actor);
            }
            catch
            {
                LogInfo("Failed to focus target actor with id " + actorId);
            }
        }
    }
}