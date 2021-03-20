// ReSharper disable InconsistentNaming
// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable MemberCanBePrivate.Global

using System.Numerics;

namespace DalamudPluginCommon
{
    public static class UIColor
    {
        public static Vector4 White = new Vector4(255, 255, 255, 1);
        public static Vector4 Green = new Vector4(0f, .8f, .133f, 1f);
        public static Vector4 Yellow = new Vector4(1f, 1f, .4f, 1f);
        public static Vector4 Red = new Vector4(.863f, 0f, 0f, 1f);
        public static Vector4 Violet = new Vector4(0.770f, 0.700f, 0.965f, 1.000f);
        
        public static Vector4 Common = new Vector4(102, 102, 102, 1);
        public static Vector4 Uncommon = new Vector4(30, 255, 0, 1);
        public static Vector4 Rare = new Vector4(0, 112, 255, 1);
        public static Vector4 Epic = new Vector4(163,53,238, 1);
        public static Vector4 Legendary = new Vector4(255,128,0, 1);
        public static Vector4 Astounding = new Vector4(226,104,168, 1);
        public static Vector4 Artifact = new Vector4(229, 204, 128, 1);

        public static Vector4 GetColorByNumber(uint num)
        {
            if (num < 25) return Common;
            if (num >= 25 && num <= 49) return Uncommon;
            if (num >= 50 && num <= 74) return Rare;
            if (num >= 75 && num <= 94) return Epic;
            if (num >= 95 && num <= 98) return Legendary;
            if (num == 99) return Astounding;
            return Artifact;
        }
    }
}