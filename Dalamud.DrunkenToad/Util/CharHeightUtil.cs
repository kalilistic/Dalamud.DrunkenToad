using System;
using System.Collections.Generic;
using System.Linq;

namespace Dalamud.DrunkenToad.Util;

/// <summary>
/// Calculate character heights.
/// </summary>
public class CharHeightUtil
{
    private static readonly List<CharHeightUtil> CharHeights = new ();
    private static readonly CharHeightUtil HyurMidlanderMale = new (1, 1, 0, 66.1, 0.055);
    private static readonly CharHeightUtil HyurMidlanderFemale = new (1, 1, 1, 62.0, 0.051);
    private static readonly CharHeightUtil HyurHighlanderMale = new (1, 2, 0, 72.7, 0.061);
    private static readonly CharHeightUtil HyurHighlanderFemale = new (1, 2, 1, 68.2, 0.056);
    private static readonly CharHeightUtil ElezenMale = new (2, 0, 76.4, 0.062);
    private static readonly CharHeightUtil ElezenFemale = new (2, 1, 72.2, 0.059);
    private static readonly CharHeightUtil MiqoteMale = new (4, 0, 62.7, 0.055);
    private static readonly CharHeightUtil MiqoteFemale = new (4, 1, 58.9, 0.049);
    private static readonly CharHeightUtil RoegadynMale = new (5, 0, 84.1, 0.066);
    private static readonly CharHeightUtil RoegadynFemale = new (5, 1, 75.6, 0.121);
    private static readonly CharHeightUtil AuRaMale = new (6, 0, 79.9, 0.055);
    private static readonly CharHeightUtil AuRaFemale = new (6, 1, 57.5, 0.049);
    private static readonly CharHeightUtil Lalafell = new (3, 34.2, 0.04);
    private static readonly CharHeightUtil Hrothgar = new (7, 77.2, 0.066);
    private static readonly CharHeightUtil Viera = new (8, 70.4, 0.049);

    private CharHeightUtil(byte raceId, byte tribeId, byte genderId, double minHeight, double ratio)
    {
        this.RaceId = raceId;
        this.TribeId = tribeId;
        this.GenderId = genderId;
        this.MinHeight = minHeight;
        this.Ratio = ratio;
        CharHeights.Add(this);
    }

    private CharHeightUtil(byte raceId, byte genderId, double minHeight, double ratio)
    {
        this.RaceId = raceId;
        this.GenderId = genderId;
        this.MinHeight = minHeight;
        this.Ratio = ratio;
        CharHeights.Add(this);
    }

    private CharHeightUtil(byte raceId, double minHeight, double ratio)
    {
        this.RaceId = raceId;
        this.MinHeight = minHeight;
        this.Ratio = ratio;
        CharHeights.Add(this);
    }

    private byte RaceId { get; set; }

    private byte TribeId { get; set; }

    private byte GenderId { get; set; }

    private double MinHeight { get; set; }

    private double Ratio { get; set; }

    /// <summary>
    /// Calc height in inches.
    /// </summary>
    /// <param name="height">scaler height.</param>
    /// <param name="raceId">race id.</param>
    /// <param name="tribeId">tribe id.</param>
    /// <param name="genderId">gender id.</param>
    /// <returns>height in inches.</returns>
    public static double CalcInches(byte height, byte raceId, byte tribeId, byte genderId)
    {
        try
        {
            var charHeight = GetCharHeight(raceId, tribeId, genderId);
            return charHeight.MinHeight + Math.Round(height * charHeight.Ratio, 1);
        }
        catch (Exception)
        {
            return 0;
        }
    }

    private static CharHeightUtil GetCharHeight(byte raceId, byte tribeId, byte genderId)
    {
        // check tribe variations for hyur
        if (raceId == 1)
        {
            return CharHeights.FirstOrDefault(height => height.RaceId == raceId && height.TribeId == tribeId && height.GenderId == genderId) !;
        }

        // check gender variations for elezen, miqo, roes, au ra
        if (raceId is 2 or 4 or 5 or 6)
        {
            return CharHeights.FirstOrDefault(height => height.RaceId == raceId && height.GenderId == genderId) !;
        }

        // return remaining based on raceId alone
        return CharHeights.FirstOrDefault(height => height.RaceId == raceId) !;
    }
}
