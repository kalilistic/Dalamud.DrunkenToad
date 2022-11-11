using System.Linq;

using Dalamud.Data;
using Lumina.Excel.GeneratedSheets;

namespace Dalamud.DrunkenToad;

/// <summary>
/// Dalamud Data Manager extensions.
/// </summary>
public static class DataManagerExtensions
{
    /// <summary>
    /// Gets current content id.
    /// </summary>
    /// <param name="value">data manager.</param>
    /// <param name="territoryType">territory type id.</param>
    /// <returns>content id or zero if not in content.</returns>
    public static uint ContentId(this DataManager value, ushort territoryType)
    {
        uint contentId;
        try
        {
            contentId = GetContentId(value, territoryType);
        }
        catch
        {
            contentId = 0;
        }

        return contentId;
    }

    /// <summary>
    /// Get content name.
    /// </summary>
    /// <param name="value">data manager.</param>
    /// <param name="territoryType">territory type id.</param>
    /// <returns>content name.</returns>
    public static string ContentName(this DataManager value, ushort territoryType)
    {
        try
        {
            var contentId = GetContentId(value, territoryType);
            if (contentId == 0) return string.Empty;
            return value.GetExcelSheet<ContentFinderCondition>()?.GetRow(contentId)?.Name!;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Gets indicator whether current territory is content.
    /// </summary>
    /// <param name="value">data manager.</param>
    /// <param name="territoryType">territory type id.</param>
    /// <returns>indicator whether local player is in content.</returns>
    public static bool InContent(this DataManager value, ushort territoryType)
    {
        uint contentId;
        try
        {
            contentId = GetContentId(value, territoryType);
        }
        catch
        {
            contentId = 0;
        }

        return contentId != 0;
    }

    /// <summary>
    /// Gets indicator whether current territory is high-end duty content.
    /// </summary>
    /// <param name="value">data manager.</param>
    /// <param name="territoryType">territory type id.</param>
    /// <returns>indicator whether local player is in high-end duty content.</returns>
    public static bool InHighEndDuty(this DataManager value, ushort territoryType)
    {
        var isHighEndDuty = false;
        try
        {
            var contentId = GetContentId(value, territoryType);
            if (contentId == 0) return false;
            isHighEndDuty = value.GetExcelSheet<ContentFinderCondition>() !.GetRow(contentId) !.HighEndDuty;
        }
        catch
        {
            // ignored
        }

        return isHighEndDuty;
    }

    /// <summary>
    /// Get world names.
    /// </summary>
    /// <param name="value">data manager.</param>
    /// <returns>list of world names.</returns>
    public static string[] WorldNames(this DataManager value)
    {
        try
        {
            return value.GetExcelSheet<World>() !
                        .Where(world => world.IsPublic)
                        .Select(world => world.Name.ToString()).OrderBy(worldName => worldName).ToArray();
        }
        catch
        {
            return System.Array.Empty<string>();
        }
    }

    /// <summary>
    /// Get race name by race/gender id.
    /// </summary>
    /// <param name="value">data manager.</param>
    /// <param name="id">race id.</param>
    /// <param name="genderId">gender id.</param>
    /// <returns>race name.</returns>
    public static string Race(this DataManager value, int id, int genderId)
    {
        try
        {
            if (id == 0)
            {
                return string.Empty;
            }

            var race = value.GetExcelSheet<Race>() !
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
            return string.Empty;
        }
    }

    /// <summary>
    /// Get tribe name by race/gender id.
    /// </summary>
    /// <param name="value">data manager.</param>
    /// <param name="id">tribe id.</param>
    /// <param name="genderId">gender id.</param>
    /// <returns>race name.</returns>
    public static string Tribe(this DataManager value, int id, int genderId)
    {
        try
        {
            if (id == 0)
            {
                return string.Empty;
            }

            var tribe = value.GetExcelSheet<Tribe>() !
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
            return string.Empty;
        }
    }

    /// <summary>
    /// Get job code.
    /// </summary>
    /// <param name="value">data manager.</param>
    /// <param name="classJobId">job code id.</param>
    /// <returns>job code.</returns>
    public static string ClassJobCode(this DataManager value, uint classJobId)
    {
        try
        {
            return value.GetExcelSheet<ClassJob>()?.GetRow(classJobId)?.Abbreviation!;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Get place name.
    /// </summary>
    /// <param name="value">data manager.</param>
    /// <param name="territoryTypeId">territory type id.</param>
    /// <returns>place name.</returns>
    public static string PlaceName(this DataManager value, uint territoryTypeId)
    {
        try
        {
            return value.GetExcelSheet<TerritoryType>()?.GetRow(territoryTypeId)?.PlaceName.Value?.Name.ToString() !;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Get world id.
    /// </summary>
    /// <param name="value">data manager.</param>
    /// <param name="worldName">world name.</param>
    /// <returns>world id.</returns>
    public static uint WorldId(this DataManager value, string worldName)
    {
        try
        {
            var worldId = value.GetExcelSheet<World>() !
                               .FirstOrDefault(world => world.Name.ToString().Equals(worldName))?.RowId;
            return worldId ?? 0;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Get world id.
    /// </summary>
    /// <param name="value">data manager.</param>
    /// <param name="worldId">world id.</param>
    /// <returns>world name.</returns>
    public static string WorldName(this DataManager value, uint worldId)
    {
        try
        {
            return value.GetExcelSheet<World>()?.GetRow(worldId)?.Name.ToString() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static uint GetContentId(DataManager value, ushort territoryType)
    {
        return value.GetExcelSheet<ContentFinderCondition>() !
                    .FirstOrDefault(condition => condition.TerritoryType.Row == territoryType)?.RowId ?? 0;
    }
}
