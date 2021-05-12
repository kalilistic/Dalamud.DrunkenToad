using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace DalamudPluginCommon
{
    /// <summary>
    /// String extensions.
    /// </summary>
    public static class StringExtensions
    {
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
        /// <param name="str">string to evaluate.</param>
        /// <returns>string with dot on end.</returns>
        public static string EnsureEndsWithDot(this string str)
        {
            if (!str.EndsWith("."))
            {
                return str + ".";
            }

            return str;
        }
    }
}
