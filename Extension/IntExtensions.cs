namespace DalamudPluginCommon
{
	public static class IntExtensions
	{
		public static int FromMillisecondsToSeconds(this int value)
		{
			return value / 1000;
		}

		public static int FromSecondsToMilliseconds(this int value)
		{
			return value * 1000;
		}

		public static int FromMillisecondsToMinutes(this int value)
		{
			return value / 60000;
		}

		public static int FromMinutesToMilliseconds(this int value)
		{
			return value * 60000;
		}

		public static int FromMillisecondsToHours(this int value)
		{
			return value / 3600000;
		}

		public static int FromHoursToMilliseconds(this int value)
		{
			return value * 3600000;
		}

		public static int FromMillisecondsToDays(this int value)
		{
			return value / 86400000;
		}

		public static int FromDaysToMilliseconds(this int value)
		{
			return value * 86400000;
		}
	}
}