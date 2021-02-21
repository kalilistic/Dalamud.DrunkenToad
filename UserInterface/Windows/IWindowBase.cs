namespace DalamudPluginCommon
{
    public interface IWindowBase
    {
        float Scale { get; }
        bool IsVisible { get; set; }
        void DrawView();
        void ToggleView();
        void ShowView();
        void HideView();
    }
}