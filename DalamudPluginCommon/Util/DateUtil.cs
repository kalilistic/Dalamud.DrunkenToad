using System;

namespace DalamudPluginCommon
{
    /// <summary>
    /// Date util.
    /// </summary>
    public static class DateUtil
    {
        /// <summary>
        /// Return current time in unix timestamp (milliseconds).
        /// </summary>
        /// <returns>unix timestamp (milliseconds).</returns>
        public static long CurrentTime()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }
    }
}
