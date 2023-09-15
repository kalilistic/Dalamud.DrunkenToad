namespace Dalamud.DrunkenToad.Extensions;

using System;

/// <summary>
/// Long extensions.
/// </summary>
public static class LongExtensions
{
    /// <summary>
    /// Convert long to datetime.
    /// </summary>
    /// <param name="value">unix timestamp in milliseconds.</param>
    /// <returns>datetime.</returns>
    public static DateTime ToDT(this long value)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return dateTime.AddMilliseconds(value);
    }
}
