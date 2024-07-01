namespace Dalamud.DrunkenToad.Helpers;

using System;

/// <summary>
/// Provides utility methods for working with Unix timestamps.
/// </summary>
public static class UnixTimestampHelper
{
    /// <summary>
    /// Gets the current time as a Unix timestamp in milliseconds, based on Coordinated Universal Time (UTC).
    /// </summary>
    /// <returns>The current Unix timestamp in milliseconds.</returns>
    public static long CurrentTime() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    /// <summary>
    /// Converts a Unix timestamp in milliseconds to a formatted date/time string in the system's local time zone.
    /// </summary>
    /// <param name="unixTimeMilliseconds">The Unix timestamp in milliseconds.</param>
    /// <returns>A formatted date/time string representing the converted Unix timestamp in the system's local time zone.</returns>
    public static string FormatUnixTimestamp(long unixTimeMilliseconds)
    {
        var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds).DateTime;
        var systemTimeZone = TimeZoneInfo.Local;
        var convertedDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, systemTimeZone);
        var formattedDateTime = convertedDateTime.ToString("yyyy-MM-dd h:mm tt");
        return formattedDateTime;
    }
}