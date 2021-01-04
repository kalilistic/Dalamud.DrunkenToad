namespace DalamudPluginCommon
{
	public static class FontAwesomeExtensions
	{
		public static char ToIconChar(this FontAwesomeIcon icon)
		{
			return (char) icon;
		}

		public static string ToIconString(this FontAwesomeIcon icon)
		{
			return "" + (char) icon;
		}
	}
}