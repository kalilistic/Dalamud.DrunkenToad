// ReSharper disable ConvertToAutoPropertyWhenPossible

using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace DalamudPluginCommon
{
	public abstract class WindowBase
	{
		public readonly List<Vector4> ColorPalette = new List<Vector4>();
		public CustomWidgets CustomWidgets = new CustomWidgets();
		public bool IsVisible = false;
		public float Scale => ImGui.GetIO().FontGlobalScale;
	}
}