// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ArrangeMissingParentheses
namespace Dalamud.DrunkenToad.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Enums;
using Extensions;
using Helpers;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using Models;

/// <summary>
/// Dalamud data manager wrapper to provide extra methods.
/// https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Data/DataManager.cs.
/// </summary>
public class DataManagerEx
{
    private readonly IDataManager dataManager;
    private readonly IDalamudPluginInterface pluginInterface;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataManagerEx" /> class.
    /// </summary>
    /// <param name="dataManager">dalamud command manager.</param>
    /// <param name="pluginInterface">dalamud plugin interface.</param>
    public DataManagerEx(IDataManager dataManager, IDalamudPluginInterface pluginInterface)
    {
        this.dataManager = dataManager;
        this.pluginInterface = pluginInterface;
        this.Excel = dataManager.Excel;
        this.Worlds = this.LoadWorlds();
        this.DataCenters = this.LoadDataCenters();
        this.Locations = this.LoadLocations();
        this.ClassJobs = this.LoadClassJobs();
        this.Races = this.LoadRaces();
        this.Tribes = this.LoadTribes();
        this.UIColors = this.LoadUIColors();
    }

    /// <summary>
    /// Gets the excel module.
    /// </summary>
    public ExcelModule Excel { get; private set; }

    /// <summary>
    /// Gets all valid worlds (dc != 0, name starts with uppercase).
    /// </summary>
    public Dictionary<uint, ToadWorld> Worlds { get; }

    /// <summary>
    /// Gets all data centers.
    /// </summary>
    public Dictionary<uint, ToadDataCenter> DataCenters { get; }

    /// <summary>
    /// Gets all locations (territory type and content data).
    /// </summary>
    public Dictionary<ushort, ToadLocation> Locations { get; }

    /// <summary>
    /// Gets all class job abbreviations.
    /// </summary>
    public Dictionary<uint, ToadClassJob> ClassJobs { get; }

    /// <summary>
    /// Gets race data.
    /// </summary>
    public Dictionary<uint, ToadRace> Races { get; }

    /// <summary>
    /// Gets tribe data.
    /// </summary>
    public Dictionary<uint, ToadTribe> Tribes { get; }

    /// <summary>
    /// Gets ui color data.
    /// </summary>
    public Dictionary<uint, ToadUIColor> UIColors { get; }

    /// <summary>
    /// Dispose wrapper.
    /// </summary>
    public static void Dispose()
    {
    }

    /// <summary>
    /// Finds the closest color from the UIColors to the input color by foreground.
    /// </summary>
    /// <param name="inputColor">The input color to find the closest match for.</param>
    /// <param name="usedColorIds">Used colors to exclude.</param>
    /// <returns>The closest matching color from the UIColor foregrounds.</returns>
    public ToadUIColor FindClosestUIColor(Vector4 inputColor, HashSet<uint>? usedColorIds = null)
    {
        ToadUIColor? closestColor = null;
        var minDistance = float.MaxValue;

        foreach (var colorKVP in this.UIColors)
        {
            if (usedColorIds != null && usedColorIds.Contains(colorKVP.Key))
            {
                continue;
            }

            var color = this.GetUIColorAsVector4(colorKVP.Key);
            var distance = ColorDistance(inputColor, color);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestColor = colorKVP.Value;
            }
        }

        return closestColor ?? this.UIColors.First().Value;
    }

    /// <summary>
    /// Gets the UI color as a vector4.
    /// </summary>
    /// <param name="colorId">color id.</param>
    /// <returns>color as vector4.</returns>
    public Vector4 GetUIColorAsVector4(uint colorId)
    {
        var uiColor = this.UIColors.TryGetValue(colorId, out var color) ? color : new ToadUIColor();
        return ImGuiUtil.UiColorToVector4(uiColor.Foreground);
    }

    /// <summary>
    /// Gets a random UI color.
    /// </summary>
    /// <returns>A ToadUIColor object representing a random UI color.</returns>
    public ToadUIColor GetRandomUIColor()
    {
        var randomIndex = new Random().Next(0, this.UIColors.Count);
        var randomColorId = this.UIColors.Keys.ElementAt(randomIndex);
        return this.UIColors[randomColorId];
    }

    /// <summary>
    /// Gets the world name by id.
    /// </summary>
    /// <param name="id">world id.</param>
    /// <returns>world name.</returns>
    public string GetWorldNameById(uint id) => this.Worlds.TryGetValue(id, out var world) ? world.Name : "Etheirys";

    /// <summary>
    /// Gets the world id by name.
    /// </summary>
    /// <param name="worldName">world name.</param>
    /// <returns>world id.</returns>
    public uint GetWorldIdByName(string worldName)
    {
        foreach (var world in this.Worlds)
        {
            if (world.Value.Name.Equals(worldName, StringComparison.OrdinalIgnoreCase))
            {
                return world.Key;
            }
        }

        return 0;
    }

    /// <summary>
    /// Validates if the world id is a valid world.
    /// </summary>
    /// <param name="worldId">world id.</param>
    /// <returns>indicator whether world is valid.</returns>
    public bool IsValidWorld(uint worldId) => this.Worlds.ContainsKey(worldId);

    /// <summary>
    /// Get indicator whether world is a test data center.
    /// </summary>
    /// <param name="worldId">world id.</param>
    /// <returns>indicator whether world is a test data center.</returns>
    public bool IsTestDC(uint worldId)
    {
        var worlds = this.dataManager.GetExcelSheet<World>();
        var dcs = this.dataManager.GetExcelSheet<WorldDCGroupType>();
        var world = worlds?.GetRow(worldId);
        if (world == null)
        {
            return false;
        }

        var region = dcs?.GetRow(world.DataCenter.Row)?.Region ?? 0;
        return region == 7;
    }

    private static float ColorDistance(Vector4 color1, Vector4 color2)
    {
        var rDiff = color1.X - color2.X;
        var gDiff = color1.Y - color2.Y;
        var bDiff = color1.Z - color2.Z;
        var aDiff = color1.W - color2.W;

        return (float)Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff + aDiff * aDiff);
    }

    private Dictionary<uint, ToadWorld> LoadWorlds()
    {
        var worldSheet = this.dataManager.GetExcelSheet<World>() !;
        var luminaWorlds = worldSheet.Where(
            world => !string.IsNullOrEmpty(world.Name) &&
                     world.DataCenter.Row != 0
                     && char.IsUpper((char)world.Name.RawData[0]) &&
                     !this.IsTestDC(world.RowId));

        return luminaWorlds.ToDictionary(
            luminaWorld => luminaWorld.RowId,
            luminaWorld => new ToadWorld { Id = luminaWorld.RowId, Name = this.pluginInterface.Sanitize(luminaWorld.Name), DataCenterId = luminaWorld.DataCenter.Row });
    }

    private Dictionary<uint, ToadDataCenter> LoadDataCenters()
    {
        var dataCenterSheet = this.dataManager.GetExcelSheet<WorldDCGroupType>() !;
        var luminaDataCenters = dataCenterSheet.Where(dataCenter => !string.IsNullOrEmpty(dataCenter.Name) && dataCenter.Region != 0 && dataCenter.Region != 7);

        return luminaDataCenters.ToDictionary(
            luminaDataCenter => luminaDataCenter.RowId,
            luminaDataCenter => new ToadDataCenter { Id = luminaDataCenter.RowId, Name = this.pluginInterface.Sanitize(luminaDataCenter.Name) });
    }

    private Dictionary<ushort, ToadLocation> LoadLocations()
    {
        var territoryTypeSheet = this.dataManager.GetExcelSheet<TerritoryType>() !;
        var cfcSheet = this.dataManager.GetExcelSheet<ContentFinderCondition>() !;

        return territoryTypeSheet.ToDictionary(
            territoryTypeSheetItem => (ushort)territoryTypeSheetItem.RowId,
            territoryTypeSheetItem =>
            {
                var location = new ToadLocation { TerritoryId = (ushort)territoryTypeSheetItem.RowId };
                location.TerritoryName = territoryTypeSheet.GetRow(location.TerritoryId)?.PlaceName.Value?.Name.ToString() ?? string.Empty;
                var cfc = cfcSheet.FirstOrDefault(condition => condition.TerritoryType.Row == location.TerritoryId);
                var cfcId = cfc?.RowId ?? 0;
                if (cfc != null && cfcId != 0)
                {
                    location.ContentId = cfcId;
                    location.ContentName = this.pluginInterface.Sanitize(cfc.Name);
                    location.LocationType = cfc.HighEndDuty ? ToadLocationType.HighEndContent : ToadLocationType.Content;
                }
                else
                {
                    location.LocationType = ToadLocationType.Overworld;
                }

                location.TerritoryName = this.pluginInterface.Sanitize(location.TerritoryName);
                return location;
            });
    }

    private Dictionary<uint, ToadClassJob> LoadClassJobs()
    {
        var classJobSheet = this.dataManager.GetExcelSheet<ClassJob>() !;

        return classJobSheet.ToDictionary(
            luminaClassJob => luminaClassJob.RowId,
            luminaClassJob => new ToadClassJob
            {
                Id = luminaClassJob.RowId,
                Name = this.pluginInterface.Sanitize(luminaClassJob.Name),
                Code = this.pluginInterface.Sanitize(luminaClassJob.Abbreviation),
            });
    }

    private Dictionary<uint, ToadRace> LoadRaces()
    {
        var raceSheet = this.dataManager.GetExcelSheet<Race>() !;

        return raceSheet.ToDictionary(
            luminaRace => luminaRace.RowId,
            luminaRace => new ToadRace
            {
                Id = luminaRace.RowId,
                MasculineName = this.pluginInterface.Sanitize(luminaRace.Masculine),
                FeminineName = this.pluginInterface.Sanitize(luminaRace.Feminine),
            });
    }

    private Dictionary<uint, ToadTribe> LoadTribes()
    {
        var tribeSheet = this.dataManager.GetExcelSheet<Tribe>() !;

        return tribeSheet.ToDictionary(
            luminaTribe => luminaTribe.RowId,
            luminaTribe => new ToadTribe
            {
                Id = luminaTribe.RowId,
                MasculineName = this.pluginInterface.Sanitize(luminaTribe.Masculine),
                FeminineName = this.pluginInterface.Sanitize(luminaTribe.Feminine),
            });
    }

    private Dictionary<uint, ToadUIColor> LoadUIColors()
    {
        var uiColorSheet = this.dataManager.GetExcelSheet<UIColor>() !;

        var uiColors = new Dictionary<uint, ToadUIColor>();

        foreach (var luminaUIColor in uiColorSheet)
        {
            var toadUIColor = new ToadUIColor { Id = luminaUIColor.RowId, Foreground = luminaUIColor.UIForeground, Glow = luminaUIColor.UIGlow };
            uiColors.Add(luminaUIColor.RowId, toadUIColor);
        }

        return uiColors;
    }
}
