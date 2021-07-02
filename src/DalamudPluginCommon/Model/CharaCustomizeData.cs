using System.Runtime.InteropServices;

using Dalamud.Game.ClientState.Actors;

namespace DalamudPluginCommon
{
    /// <summary>
    /// Character customization data.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct CharaCustomizeData
    {
        /// <summary>
        /// The bust size of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.BustSize)]
        public readonly byte BustSize;

        /// <summary>
        /// The eyebrows of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.Eyebrows)]
        public readonly byte Eyebrows;

        /// <summary>
        /// The eye color of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.EyeColor)]
        public readonly byte EyeColor;

        /// <summary>
        /// The 2nd eye color of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.EyeColor2)]
        public readonly byte EyeColor2;

        /// <summary>
        /// The eye shape of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.EyeShape)]
        public readonly byte EyeShape;

        /// <summary>
        /// The face features of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.FaceFeatures)]
        public readonly byte FaceFeatures;

        /// <summary>
        /// The color of the face features of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.FaceFeaturesColor)]
        public readonly byte FaceFeaturesColor;

        /// <summary>
        /// The face paint of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.Facepaint)]
        public readonly byte Facepaint;

        /// <summary>
        /// The face paint color of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.FacepaintColor)]
        public readonly byte FacepaintColor;

        /// <summary>
        /// The face type of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.FaceType)]
        public readonly byte FaceType;

        /// <summary>
        /// The gender of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.Gender)]
        public readonly byte Gender;

        /// <summary>
        /// The hair color of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.HairColor)]
        public readonly byte HairColor;

        /// <summary>
        /// The highlights hair color of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.HairColor2)]
        public readonly byte HairColor2;

        /// <summary>
        /// The hair of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.HairStyle)]
        public readonly byte HairStyle;

        /// <summary>
        /// Whether or not the character has hair highlights.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.HasHighlights)]
        public readonly byte HasHighlights;

        /// <summary>
        /// The height of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.Height)]
        public readonly byte Height;

        /// <summary>
        /// The jaw shape of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.JawShape)]
        public readonly byte JawShape;

        /// <summary>
        /// The lip color of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.LipColor)]
        public readonly byte LipColor;

        /// <summary>
        /// The lip style of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.LipStyle)]
        public readonly byte LipStyle;

        /// <summary>
        /// The model type of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.ModelType)]
        public readonly byte ModelType;

        /// <summary>
        /// The nose shape of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.NoseShape)]
        public readonly byte NoseShape;

        /// <summary>
        /// The race of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.Race)]
        public readonly byte Race;

        /// <summary>
        /// The race feature size of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.RaceFeatureSize)]
        public readonly byte RaceFeatureSize;

        /// <summary>
        /// The race feature type of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.RaceFeatureType)]
        public readonly byte RaceFeatureType;

        /// <summary>
        /// The skin color of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.SkinColor)]
        public readonly byte SkinColor;

        /// <summary>
        /// The tribe of the character.
        /// </summary>
        [FieldOffset((int)CustomizeIndex.Tribe)]
        public readonly byte Tribe;

        /// <summary>
        /// Create customize data from byte array.
        /// </summary>
        /// <param name="customizeIndex">customize index byte array.</param>
        /// <returns>customize data struct.</returns>
        public static CharaCustomizeData MapCustomizeData(byte[] customizeIndex)
        {
            var handle = GCHandle.Alloc(customizeIndex, GCHandleType.Pinned);
            CharaCustomizeData charaCustomizeData;
            try
            {
                charaCustomizeData = (CharaCustomizeData)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(CharaCustomizeData));
            }
            finally
            {
                handle.Free();
            }

            return charaCustomizeData;
        }
    }
}
