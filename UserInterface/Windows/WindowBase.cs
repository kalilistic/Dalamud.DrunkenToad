// ReSharper disable ConvertToAutoPropertyWhenPossible

using System.Numerics;
using ImGuiNET;

namespace DalamudPluginCommon
{
	public abstract class WindowBase
	{
		public CustomWidgets CustomWidgets = new CustomWidgets();
		public bool IsVisible = false;
		public float Scale => ImGui.GetIO().FontGlobalScale;

		public Vector2 ReVector2(float x, float y)
		{
			return new Vector2(x * Scale, y * Scale);
		}


	}
}