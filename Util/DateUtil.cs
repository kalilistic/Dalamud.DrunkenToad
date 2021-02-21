using System;

namespace DalamudPluginCommon
{
    public static class DateUtil
    {
        public static long CurrentTime()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }
    }
}