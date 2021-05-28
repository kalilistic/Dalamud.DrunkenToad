using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace DalamudPluginCommon
{
    /// <summary>
    /// String extensions.
    /// </summary>
    public static class StringExtensions
    {
        private static readonly char[] TerminationChars = { '!', '.', '?' };

        /// <summary>
        /// Compress string to base64.
        /// </summary>
        /// <param name="value">uncompressed string.</param>
        /// <returns>compressed string.</returns>
        public static string Compress(this string value)
        {
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(value));
            using var compressedMemoryStream = new MemoryStream();
            var gzipStream = new GZipStream(compressedMemoryStream, CompressionMode.Compress);
            memoryStream.CopyTo(gzipStream);
            gzipStream.Dispose();
            string compressed = Convert.ToBase64String(compressedMemoryStream.ToArray());

            return compressed;
        }

        /// <summary>
        /// Decompress string from base64.
        /// </summary>
        /// <param name="value">compressed string.</param>
        /// <returns>decompressed string.</returns>
        public static string Decompress(this string value)
        {
            using var memoryStream = new MemoryStream(Convert.FromBase64String(value));
            using var decompressedMemoryStream = new MemoryStream();
            var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
            gzipStream.CopyTo(decompressedMemoryStream);
            gzipStream.Dispose();

            string decompressed = Encoding.UTF8.GetString(decompressedMemoryStream.ToArray());

            return decompressed;
        }

        /// <summary>
        /// Adds dot to end of string if doesn't exist.
        /// </summary>
        /// <param name="value">string to add dot to.</param>
        /// <returns>string with dot on end.</returns>
        public static string EnsureEndsWithDot(this string value)
        {
            if (!TerminationChars.Contains(value.Last()))
            {
                return value + ".";
            }

            return value;
        }

        /// <summary>
        /// Capitalize first word of sentence.
        /// </summary>
        /// <param name="value">sentence to capitalize.</param>
        /// <returns>sentence with proper capitalization.</returns>
        public static string CapitalizeFirst(this string value)
        {
            var isNewSentence = true;
            var result = new StringBuilder(value.Length);
            foreach (var t in value)
            {
                if (isNewSentence && char.IsLetter(t))
                {
                    result.Append(char.ToUpper(t));
                    isNewSentence = false;
                }
                else
                {
                    result.Append(t);
                }

                if (t is '!' or '?' || t == '.')
                {
                    isNewSentence = true;
                }
            }

            return result.ToString();
        }
    }
}
