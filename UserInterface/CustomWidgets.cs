using ImGuiNET;

namespace PriceCheck
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
	}
}