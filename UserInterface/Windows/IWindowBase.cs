namespace DalamudPluginCommon
{
	public interface IWindowBase
	{
		float Scale { get; }
		bool IsVisible { get; }
		void DrawView();
		void ToggleView();
		void ShowView();
		void HideView();
	}
}