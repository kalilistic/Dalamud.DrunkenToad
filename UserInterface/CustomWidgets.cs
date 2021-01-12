using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;

namespace DalamudPluginCommon
{
	public class CustomWidgets
	{
		public void HelpMarker(string desc)
		{
			ImGui.SameLine();
			ImGui.PushFont(UiBuilder.IconFont);
			ImGui.TextDisabled(FontAwesomeIcon.InfoCircle.ToIconString());
			ImGui.PopFont();
			if (!ImGui.IsItemHovered()) return;
			ImGui.BeginTooltip();
			ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
			ImGui.TextUnformatted(desc);
			ImGui.PopTextWrapPos();
			ImGui.EndTooltip();
		}

		public void Text(string label, string value, string hint = "")
		{
			ImGui.Text(label + ": ");
			ImGui.SameLine();
			if (string.IsNullOrEmpty(hint))
			{
				ImGui.Text(value);
			}
			else
			{
				ImGui.Text(value + "*");
				if (ImGui.IsItemHovered()) ImGui.SetTooltip(hint);
			}
		}

		public static bool IconButton(FontAwesomeIcon icon, string id)
		{
			ImGui.PushStyleColor(ImGuiCol.Button, Vector4.Zero);
			ImGui.PushStyleColor(ImGuiCol.ButtonActive, Vector4.Zero);
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Vector4.Zero);
			ImGui.PushFont(UiBuilder.IconFont);
			var button = ImGui.Button($"{icon.ToIconString()}{id}");
			ImGui.PopFont();
			ImGui.PopStyleColor(3);
			return button;
		}
	}
}