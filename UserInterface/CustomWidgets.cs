using ImGuiNET;

namespace DalamudPluginCommon
{
	public class CustomWidgets
	{
		public void HelpMarker(string desc)
		{
			ImGui.SameLine();
			ImGui.TextDisabled("(?)");
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
				if (ImGui.IsItemHovered())
				{
					ImGui.SetTooltip(hint);
				}
			}

		}

	}
}