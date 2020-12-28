// ReSharper disable ConvertToAutoPropertyWhenPossible

using ImGuiNET;

namespace DalamudPluginCommon
{
	public abstract class WindowBase
	{
		public CustomWidgets CustomWidgets = new CustomWidgets();
		public bool IsVisible = false;
		public float Scale => ImGui.GetIO().FontGlobalScale;
	}
}