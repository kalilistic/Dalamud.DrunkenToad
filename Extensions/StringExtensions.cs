// ReSharper disable UseCollectionExpression
namespace Dalamud.DrunkenToad.Extensions;

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

/// <summary>
/// Provides extension methods for string manipulation and transformation.
/// </summary>
public static class StringExtensions
{
    private static readonly char[] TerminationChars = { '!', '.', '?' };

    /// <summary>
    /// Compresses a string using GZip and encodes it to Base64.
    /// </summary>
    /// <param name="value">string to compress.</param>
    /// <returns>compressed string.</returns>
    public static string Compress(this string value)
    {
        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(value));
        using var compressedMemoryStream = new MemoryStream();
        using (var gzipStream = new GZipStream(compressedMemoryStream, CompressionMode.Compress))
        {
            memoryStream.CopyTo(gzipStream);
        }

        return Convert.ToBase64String(compressedMemoryStream.ToArray());
    }

    /// <summary>
    /// Decompresses a Base64 encoded, GZip compressed string.
    /// </summary>
    /// <param name="value">string to decompress.</param>
    /// <returns>decompressed string.</returns>
    public static string Decompress(this string value)
    {
        using var memoryStream = new MemoryStream(Convert.FromBase64String(value));
        using var decompressedMemoryStream = new MemoryStream();
        using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
        {
            gzipStream.CopyTo(decompressedMemoryStream);
        }

        return Encoding.UTF8.GetString(decompressedMemoryStream.ToArray());
    }

    /// <summary>
    /// Ensures the string ends with a termination character.
    /// </summary>
    /// <param name="value">string to check.</param>
    /// <returns>string with termination character.</returns>
    public static string EnsureEndsWithDot(this string value) => TerminationChars.Contains(value[^1]) ? value : value + ".";

    /// <summary>
    /// Capitalizes the first letter of each sentence in the string.
    /// </summary>
    /// <param name="value">string to capitalize.</param>
    /// <returns>capitalized string.</returns>
    public static string CapitalizeFirst(this string value)
    {
        var isNewSentence = true;
        var result = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            result.Append(isNewSentence && char.IsLetter(c) ? char.ToUpper(c) : c);
            isNewSentence = TerminationChars.Contains(c);
        }

        return result.ToString();
    }

    /// <summary>
    /// Truncates the string to a specified length and appends an ellipsis if truncated.
    /// </summary>
    /// <param name="value">string to truncate.</param>
    /// <param name="lengthLimit">maximum length of the string.</param>
    /// <returns>truncated string.</returns>
    public static string TruncateWithEllipsis(this string value, int lengthLimit)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value.Length > lengthLimit ? new StringBuilder(value, 0, lengthLimit, lengthLimit + 3).Append("...").ToString() : value;
    }

    /// <summary>
    /// Converts the string to snake_case.
    /// </summary>
    /// <param name="value">string to convert.</param>
    /// <returns>snake_case string.</returns>
    public static string ToUnderscoreCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(value.Length * 2);
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (char.IsUpper(c))
            {
                if (i > 0 && value[i - 1] != '_' && !char.IsWhiteSpace(value[i - 1]))
                {
                    sb.Append('_');
                }

                sb.Append(char.ToLower(c));
            }
            else if (char.IsWhiteSpace(c))
            {
                if (i == 0 || !char.IsWhiteSpace(value[i - 1]))
                {
                    sb.Append('_');
                }
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts the first character of the string to lowercase.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The string with its first character converted to lowercase.</returns>
    public static string FirstCharToLower(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        return char.ToLowerInvariant(input[0]) + input.Substring(1);
    }
}
