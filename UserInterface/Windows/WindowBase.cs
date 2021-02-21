// ReSharper disable ConvertToAutoPropertyWhenPossible

using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace DalamudPluginCommon
{
    public abstract class WindowBase : IWindowBase
    {
        public readonly List<Vector4> ColorPalette = new List<Vector4>();
        public CustomWidgets CustomWidgets = new CustomWidgets();
        public bool IsVisible { get; set; }
        public float Scale => ImGui.GetIO().FontGlobalScale;

        public abstract void DrawView();

        public void ToggleView()
        {
            IsVisible = !IsVisible;
        }

        public void ShowView()
        {
            IsVisible = true;
        }

        public void HideView()
        {
            IsVisible = false;
        }
    }
}