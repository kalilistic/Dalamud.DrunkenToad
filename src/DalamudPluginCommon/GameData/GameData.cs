using System;
using System.Collections.Generic;
using System.Linq;

using Dalamud.Interface;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;

namespace DalamudPluginCommon
{
    /// <summary>
    /// Get plugin data.
    /// </summary>
    public class GameData
    {
        private readonly DalamudPluginInterface pluginInterface;
        private List<string>? worldNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameData"/> class.
        /// </summary>
        /// <param name="pluginInterface">dalamud plugin interface.</param>
        public GameData(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
            this.InitContent();
            this.InitItems();
        }

        /// <summary>
        /// Gets or sets returns contentIds.
        /// </summary>
        public uint[] ContentIds { get; set; } = null!;

        /// <summary>
        /// Gets or sets content names.
        /// </summary>
        public string[] ContentNames { get; set; } = null!;

        /// <summary>
        /// Gets or sets item ids.
        /// </summary>
        public uint[] ItemIds { get; set; } = null!;

        /// <summary>
        /// Gets or sets item names.
        /// </summary>
        public string[] ItemNames { get; set; } = null!;

        /// <summary>
        /// Gets or sets item category ids.
        /// </summary>
        public uint[] ItemCategoryIds { get; set; } = null!;

        /// <summary>
        /// Gets or sets item category names.
        /// </summary>
        public string[] ItemCategoryNames { get; set; } = null!;

        /// <summary>
        /// Gets or sets item lists.
        /// </summary>
        public List<KeyValuePair<uint, ItemList>>? ItemLists { get; set; }

        /// <summary>
        /// Get world names.
        /// </summary>
        /// <param name="dcId">data center id.</param>
        /// <returns>list of world names.</returns>
        public List<string> WorldNames(uint dcId)
        {
            try
            {
                if (this.worldNames != null)
                {
                    return this.worldNames;
                }

                this.worldNames = this.pluginInterface.Data.GetExcelSheet<World>()
                                      .Where(world => world.IsPublic && world.DataCenter.Value.RowId == dcId)
                                      .Select(world => world.Name.ToString()).OrderBy(worldName => worldName).ToList();
                return this.worldNames;
            }
            catch
            {
                Logger.LogInfo("WorldNames are not available.");
                return new List<string>();
            }
        }

        /// <summary>
        /// Get world name.
        /// </summary>
        /// <param name="worldId">world id.</param>
        /// <returns>world name.</returns>
        public string WorldName(uint worldId)
        {
            try
            {
                return this.pluginInterface.Data.GetExcelSheet<World>().GetRow(worldId).Name;
            }
            catch
            {
                Logger.LogInfo("WorldName is not available.");
                return string.Empty;
            }
        }

        /// <summary>
        /// Get world id.
        /// </summary>
        /// <param name="worldName">world name.</param>
        /// <returns>world id.</returns>
        public uint? WorldId(string worldName)
        {
            try
            {
                return this.pluginInterface.Data.GetExcelSheet<World>()
                           .FirstOrDefault(world => world.Name.ToString().Equals(worldName))?.RowId;
            }
            catch
            {
                Logger.LogInfo("WorldId is not available.");
                return null;
            }
        }

        /// <summary>
        /// Get job code.
        /// </summary>
        /// <param name="classJobId">job code id.</param>
        /// <returns>job code.</returns>
        public string ClassJobCode(uint classJobId)
        {
            try
            {
                return this.pluginInterface.Data.GetExcelSheet<ClassJob>().GetRow(classJobId).Abbreviation;
            }
            catch
            {
                Logger.LogInfo("JobCode is not available.");
                return string.Empty;
            }
        }

        /// <summary>
        /// Get place name.
        /// </summary>
        /// <param name="territoryTypeId">territory type id.</param>
        /// <returns>place name.</returns>
        public string PlaceName(uint territoryTypeId)
        {
            try
            {
                return this.pluginInterface.Sanitizer.Sanitize(this.pluginInterface.Data.GetExcelSheet<TerritoryType>().GetRow(territoryTypeId).PlaceName.Value.Name
                                                                   .ToString());
            }
            catch
            {
                Logger.LogInfo("PlaceName is not available.");
                return string.Empty;
            }
        }

        /// <summary>
        /// Get content name.
        /// </summary>
        /// <param name="contentId">content id.</param>
        /// <returns>content name.</returns>
        public string ContentName(uint contentId)
        {
            try
            {
                return contentId == 0
                           ? string.Empty
                           : this.pluginInterface.Sanitizer.Sanitize(
                               this.pluginInterface.Data.GetExcelSheet<ContentFinderCondition>().GetRow(contentId).Name
                                   .ToString());
            }
            catch
            {
                Logger.LogInfo("ContentName is not available.");
                return string.Empty;
            }
        }

        /// <summary>
        /// Get content id.
        /// </summary>
        /// <param name="territoryTypeId">territory type id.</param>
        /// <returns>content id.</returns>
        public uint ContentId(uint territoryTypeId)
        {
            try
            {
                return this.pluginInterface.Data.GetExcelSheet<ContentFinderCondition>()
                           .FirstOrDefault(condition => condition.TerritoryType.Row == territoryTypeId)?.RowId ?? 0;
            }
            catch
            {
                Logger.LogInfo("ContentName is not available.");
                return 0;
            }
        }

        /// <summary>
        /// Get item by item id.
        /// </summary>
        /// <param name="itemId">item id.</param>
        /// <returns>Item.</returns>
        public Item? Item(uint itemId)
        {
            try
            {
                return this.pluginInterface.Data.Excel.GetSheet<Item>().GetRow(itemId);
            }
            catch
            {
                Logger.LogInfo("Can't find item for item id.");
                return null;
            }
        }

        /// <summary>
        /// Check if contentId is high-end duty.
        /// </summary>
        /// <param name="contentId">content id.</param>
        /// <returns>indicator if high end duty.</returns>
        public bool IsHighEndDuty(uint contentId)
        {
            try
            {
                return this.pluginInterface.Data.GetExcelSheet<ContentFinderCondition>().GetRow(contentId).HighEndDuty;
            }
            catch
            {
                Logger.LogInfo("Content HighEndDuty is not available.");
                return false;
            }
        }

        /// <summary>
        /// Gender in font awesome icon format.
        /// </summary>
        /// <param name="gender">gender id.</param>
        /// <returns>gender icon.</returns>
        public string GenderIcon(byte gender)
        {
            return gender switch
            {
                0 => FontAwesomeIcon.Mars.ToIconString(),
                1 => FontAwesomeIcon.Venus.ToIconString(),
                _ => string.Empty,
            };
        }

        /// <summary>
        /// Get race name by race/gender id.
        /// </summary>
        /// <param name="id">race id.</param>
        /// <param name="genderId">gender id.</param>
        /// <returns>race name.</returns>
        public string RaceName(int id, int genderId)
        {
            try
            {
                if (id == 0)
                {
                    return string.Empty;
                }

                var race = this.pluginInterface.Data.GetExcelSheet<Race>()
                                          .FirstOrDefault(raceEntry => raceEntry.RowId == id);
                if (race == null)
                {
                    return string.Empty;
                }

                return genderId switch
                {
                    0 => race.Masculine,
                    1 => race.Feminine,
                    _ => string.Empty,
                };
            }
            catch
            {
                Logger.LogInfo("Race is not available.");
                return string.Empty;
            }
        }

        /// <summary>
        /// Get tribe name by race/gender id.
        /// </summary>
        /// <param name="id">tribe id.</param>
        /// <param name="genderId">gender id.</param>
        /// <returns>race name.</returns>
        public string TribeName(int id, int genderId)
        {
            try
            {
                if (id == 0)
                {
                    return string.Empty;
                }

                var tribe = this.pluginInterface.Data.GetExcelSheet<Tribe>()
                                           .FirstOrDefault(tribeEntry => tribeEntry.RowId == id);
                if (tribe == null)
                {
                    return string.Empty;
                }

                return genderId switch
                {
                    0 => tribe.Masculine,
                    1 => tribe.Feminine,
                    _ => string.Empty,
                };
            }
            catch
            {
                Logger.LogInfo("Tribe is not available.");
                return string.Empty;
            }
        }

        private void InitContent()
        {
            try
            {
                var excludedContent = new List<uint> { 69, 70, 71 };
                var contentTypes = new List<uint> { 2, 4, 5, 6, 26, 28, 29 };
                var contentList = this.pluginInterface.Data.GetExcelSheet<ContentFinderCondition>()
                    .Where(content =>
                        contentTypes.Contains(content.ContentType.Row) && !excludedContent.Contains(content.RowId))
                    .ToList();
                var contentNames = this.pluginInterface.Sanitizer.Sanitize(contentList.Select(content => content.Name.ToString())).ToArray();
                var contentIds = contentList.Select(content => content.RowId).ToArray();
                Array.Sort(contentNames, contentIds);
                this.ContentIds = contentIds;
                this.ContentNames = contentNames;
            }
            catch
            {
                Logger.LogInfo("Failed to initialize content list.");
            }
        }

        private void InitItems()
        {
            try
            {
                // create item list
                var itemDataList = this.pluginInterface.Data.GetExcelSheet<Item>().Where(item => item != null
                    && !string.IsNullOrEmpty(item.Name)).ToList();

                // add all items
                var itemIds = itemDataList.Select(item => item.RowId).ToArray();
                var itemNames = this.pluginInterface.Sanitizer.Sanitize(itemDataList.Select(item => item.Name.ToString())).ToArray();
                this.ItemIds = itemIds;
                this.ItemNames = itemNames;

                // item categories
                var categoryList = this.pluginInterface.Data.GetExcelSheet<ItemUICategory>()
                    .Where(category => category.RowId != 0).ToList();
                var categoryNames = this.pluginInterface.Sanitizer.Sanitize(categoryList.Select(category => category.Name.ToString())).ToArray();
                var categoryIds = categoryList.Select(category => category.RowId).ToArray();
                Array.Sort(categoryNames, categoryIds);
                this.ItemCategoryIds = categoryIds;
                this.ItemCategoryNames = categoryNames;

                // populate item lists by category
                var itemLists = new List<KeyValuePair<uint, ItemList>>();
                foreach (var categoryId in categoryIds)
                {
                    var itemCategoryDataList =
                        itemDataList.Where(item => item.ItemUICategory.Row == categoryId).ToList();
                    var itemCategoryIds = itemCategoryDataList.Select(item => item.RowId).ToArray();
                    var itemCategoryNames =
                        this.pluginInterface.Sanitizer.Sanitize(itemCategoryDataList.Select(item => item.Name.ToString())).ToArray();
                    Array.Sort(itemCategoryNames, itemCategoryIds);
                    var itemList = new ItemList
                    {
                        ItemIds = itemCategoryIds,
                        ItemNames = itemCategoryNames,
                    };
                    itemLists.Add(new KeyValuePair<uint, ItemList>(categoryId, itemList));
                }

                this.ItemLists = itemLists;
            }
            catch
            {
                Logger.LogInfo("Failed to initialize content list.");
            }
        }
    }
}
