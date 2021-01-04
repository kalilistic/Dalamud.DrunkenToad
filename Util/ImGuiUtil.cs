using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace DalamudPluginCommon
{
	public static class ImGuiUtil
	{
		public static List<Vector4> CreatePalette()
		{
			var colorPalette = new List<Vector4>();
			for (var i = 0; i < 32; i++)
			{
				ImGui.ColorConvertHSVtoRGB(i / 31.0f, 0.7f, 0.8f, out var r, out var g, out var b);
				colorPalette.Add(new Vector4(r, g, b, 1.0f));
			}

			return colorPalette;
		}
	}
}