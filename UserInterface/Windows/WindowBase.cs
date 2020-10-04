// ReSharper disable ConvertToAutoPropertyWhenPossible

namespace DalamudPluginCommon
{
	public abstract class WindowBase
	{
		public CustomWidgets CustomWidgets = new CustomWidgets();
		public bool IsVisible = false;
	}
}