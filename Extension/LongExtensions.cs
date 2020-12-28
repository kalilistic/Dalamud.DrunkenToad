using System;
using System.Collections.Generic;

namespace DalamudPluginCommon
{
	public static class LongExtensions
	{
		public static DateTime ToDateTime(this long value)
		{
			var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return dateTime.AddMilliseconds(value);
		}

		public static string ToTimeSpan(this long value)
		{
			var timeSpan = ConvertToTimeSpan(DateUtil.CurrentTime() - value);
			return string.IsNullOrEmpty(timeSpan) ? "Now" : timeSpan + " ago";
		}

		public static string ToDuration(this long value)
		{
			var timeSpan = ConvertToTimeSpan(value);
			var result = string.Join(" ", timeSpan);
			return string.IsNullOrEmpty(timeSpan) ? "< 1m" : timeSpan;
		}

		private static string ConvertToTimeSpan(long value)
		{
			var parts = new List<string>();

			void Add(int val, string unit)
			{
				if (val > 0) parts.Add(val + unit);
			}

			var t = TimeSpan.FromMilliseconds(value);
			Add(t.Days, "d");
			Add(t.Hours, "h");
			Add(t.Minutes, "m");
			return string.Join(" ", parts);
		}
	}
}