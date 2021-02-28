// ReSharper disable CommentTypo
// ReSharper disable ConvertIfStatementToSwitchStatement
// ReSharper disable IdentifierTypo
// ReSharper disable MemberCanBePrivate.Global

using System.Collections.Generic;
using System.Linq;

namespace DalamudPluginCommon
{
    public class CharHeight
    {
        public static readonly List<CharHeight> CharHeights = new List<CharHeight>();
        public static readonly CharHeight HyurMidlanderMale = new CharHeight(1, 1, 0, 66.1, 0.055);
        public static readonly CharHeight HyurMidlanderFemale = new CharHeight(1, 1, 1, 62.0, 0.051);
        public static readonly CharHeight HyurHighlanderMale = new CharHeight(1, 2, 0, 72.7, 0.061);
        public static readonly CharHeight HyurHighlanderFemale = new CharHeight(1, 2, 1, 68.2, 0.056);
        public static readonly CharHeight ElezenMale = new CharHeight(2, 0, 76.4, 0.062);
        public static readonly CharHeight ElezenFemale = new CharHeight(2, 1, 72.2, 0.059);
        public static readonly CharHeight MiqoteMale = new CharHeight(4, 0, 62.7, 0.055);
        public static readonly CharHeight MiqoteFemale = new CharHeight(4, 1, 58.9, 0.049);
        public static readonly CharHeight RoegadynMale = new CharHeight(5, 0, 84.1, 0.066);
        public static readonly CharHeight RoegadynFemale = new CharHeight(5, 1, 75.6, 0.121);
        public static readonly CharHeight AuRaMale = new CharHeight(6, 0, 79.9, 0.055);
        public static readonly CharHeight AuRaFemale = new CharHeight(6, 1, 57.5, 0.049);
        public static readonly CharHeight Lalafell = new CharHeight(3, 34.2, 0.04);
        public static readonly CharHeight Hrothgar = new CharHeight(7, 77.2, 0.066);
        public static readonly CharHeight Viera = new CharHeight(8, 70.4, 0.049);

        private CharHeight(int raceId, int tribeId, int genderId, double minHeight, double ratio)
        {
            RaceId = raceId;
            TribeId = tribeId;
            GenderId = genderId;
            MinHeight = minHeight;
            Ratio = ratio;
            CharHeights.Add(this);
        }

        private CharHeight(int raceId, int genderId, double minHeight, double ratio)
        {
            RaceId = raceId;
            GenderId = genderId;
            MinHeight = minHeight;
            Ratio = ratio;
            CharHeights.Add(this);
        }

        private CharHeight(int raceId, double minHeight, double ratio)
        {
            RaceId = raceId;
            MinHeight = minHeight;
            Ratio = ratio;
            CharHeights.Add(this);
        }

        public int RaceId { get; set; }
        public int TribeId { get; set; }
        public int GenderId { get; set; }
        public double MinHeight { get; set; }
        public double Ratio { get; set; }

        public static CharHeight GetCharHeight(int raceId, int tribeId, int genderId)
        {
            // check tribe variations for hyur
            if (raceId == 1)
                return CharHeights.FirstOrDefault(height =>
                    height.RaceId == raceId && height.TribeId == tribeId && height.GenderId == genderId);

            // check gender variations for elezen, miqo, roes, au ra
            if (raceId == 2 || raceId == 4 || raceId == 5 || raceId == 6)
                return CharHeights.FirstOrDefault(height =>
                    height.RaceId == raceId && height.GenderId == genderId);

            // return remaining based on raceId alone
            return CharHeights.FirstOrDefault(height =>
                height.RaceId == raceId);
        }
    }
}