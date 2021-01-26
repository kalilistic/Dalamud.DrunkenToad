// ReSharper disable ConvertIfStatementToReturnStatement

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
			var timeSpan = ConvertToShortTimeSpan(DateUtil.CurrentTime() - value);
			return string.IsNullOrEmpty(timeSpan) ? "Now" : timeSpan + " ago";
		}

		public static string ToDuration(this long value)
		{
			var timeSpan = ConvertToTimeSpan(value);
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

		private static string ConvertToShortTimeSpan(long value)
		{
			var timeSpan = TimeSpan.FromMilliseconds(value);
			if (timeSpan.Days > 0) return timeSpan.Days + "d";
			if (timeSpan.Hours > 0) return timeSpan.Hours + "h";
			if (timeSpan.Minutes > 0) return timeSpan.Minutes + "m";
			return string.Empty;
		}
	}
}