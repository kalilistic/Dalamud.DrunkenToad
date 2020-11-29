using System;

namespace DalamudPluginCommon
{
	public static class Util
	{
		public static string ToTimeSpan(DateTime dateTime)
		{
			var timeSpan = DateTime.Now - dateTime;
			var days = timeSpan.Days;
			var hours = timeSpan.Hours;
			var minutes = timeSpan.Minutes;

			var result = string.Empty;
			if (days > 0)
				result += days + "d " + hours + "h " + minutes + "m ago";
			else if (hours > 0)
				result += hours + "h " + minutes + "m ago";
			else if (minutes > 0)
				result += minutes + "m ago";
			else
				result += "Now";

			return result;
		}
	}
}