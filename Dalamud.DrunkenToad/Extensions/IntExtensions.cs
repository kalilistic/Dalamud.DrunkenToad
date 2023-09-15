namespace Dalamud.DrunkenToad.Extensions;

/// <summary>
/// Int extensions.
/// </summary>
public static class IntExtensions
{
    /// <summary>
    /// Convert from milliseconds to seconds.
    /// </summary>
    /// <param name="value">milliseconds.</param>
    /// <returns>seconds.</returns>
    public static int FromMillisecondsToSeconds(this int value) => value / 1000;

    /// <summary>
    /// Convert from seconds to milliseconds.
    /// </summary>
    /// <param name="value">seconds.</param>
    /// <returns>milliseconds.</returns>
    public static int FromSecondsToMilliseconds(this int value) => value * 1000;

    /// <summary>
    /// Convert from milliseconds to minutes.
    /// </summary>
    /// <param name="value">milliseconds.</param>
    /// <returns>minutes.</returns>
    public static int FromMillisecondsToMinutes(this int value) => value / 60000;

    /// <summary>
    /// Convert from minutes to milliseconds.
    /// </summary>
    /// <param name="value">minutes.</param>
    /// <returns>milliseconds.</returns>
    public static int FromMinutesToMilliseconds(this int value) => value * 60000;

    /// <summary>
    /// Convert from milliseconds to hours.
    /// </summary>
    /// <param name="value">milliseconds.</param>
    /// <returns>hours.</returns>
    public static int FromMillisecondsToHours(this int value) => value / 3600000;

    /// <summary>
    /// Convert from hours to milliseconds.
    /// </summary>
    /// <param name="value">hours.</param>
    /// <returns>milliseconds.</returns>
    public static int FromHoursToMilliseconds(this int value) => value * 3600000;

    /// <summary>
    /// Convert from milliseconds to days.
    /// </summary>
    /// <param name="value">milliseconds.</param>
    /// <returns>days.</returns>
    public static int FromMillisecondsToDays(this int value) => value / 86400000;

    /// <summary>
    /// Convert from days to milliseconds.
    /// </summary>
    /// <param name="value">days.</param>
    /// <returns>milliseconds.</returns>
    public static int FromDaysToMilliseconds(this int value) => value * 86400000;
}
