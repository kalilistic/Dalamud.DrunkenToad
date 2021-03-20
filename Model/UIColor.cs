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
        
        public static Vector4 Common = new Vector4(0.4f, 0.4f, 0.4f, 1f);
        public static Vector4 Uncommon = new Vector4(0.117f, 1f, 0f, 1f);
        public static Vector4 Rare = new Vector4(0f, 0.439f, 1f, 1f);
        public static Vector4 Epic = new Vector4(0.639f,0.207f,0.933f, 1f);
        public static Vector4 Legendary = new Vector4(1f,0.501f,0f, 1f);
        public static Vector4 Astounding = new Vector4(0.886f,0.407f,0.658f, 1f);
        public static Vector4 Artifact = new Vector4(0.898f, 0.8f, 0.501f, 1f);

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