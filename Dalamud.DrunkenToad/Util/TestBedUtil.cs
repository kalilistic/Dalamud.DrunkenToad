using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using ImGuiNET;
using ImGuiScene;

namespace Dalamud.DrunkenToad.Util;

/// <summary>
/// Utility functions for working with imgui test bed.
/// </summary>
public class TestBedUtil
{
    public static unsafe void RunTestBedScene(SimpleImGuiScene.BuildUIDelegate draw)
    {
       var scene = new SimpleImGuiScene(
            RendererFactory.RendererBackend.DirectX11,
            new WindowCreateInfo
            {
                Title = "ImGui TestBed",
                Width = 900,
                Height = 500,
                XPos = 0x2FFF0000,
                YPos = 0x2FFF0000,
            });

       // set font
       ImFontConfigPtr fontConfig = ImGuiNative.ImFontConfig_ImFontConfig();
       fontConfig.MergeMode = true;
       fontConfig.PixelSnapH = true;
       var fontPathJp = Path.Combine("Resource", "NotoSansCJKjp-Medium.otf");
       ImGuiNET.ImGui.GetIO().Fonts.AddFontFromFileTTF(fontPathJp, 17.0f, null, ImGuiNET.ImGui.GetIO().Fonts.GetGlyphRangesJapanese());
       ImGuiNET.ImGui.GetIO().Fonts.Build();
       fontConfig.Destroy();

       // set style
       ImGuiNET.ImGui.GetStyle().Alpha = 1;
       ImGuiNET.ImGui.GetStyle().WindowPadding = new Vector2(8, 8);
       ImGuiNET.ImGui.GetStyle().WindowRounding = 4;
       ImGuiNET.ImGui.GetStyle().WindowBorderSize = 0;
       ImGuiNET.ImGui.GetStyle().WindowTitleAlign = new Vector2(0, 0.5f);
       ImGuiNET.ImGui.GetStyle().WindowMenuButtonPosition = ImGuiDir.Right;
       ImGuiNET.ImGui.GetStyle().ChildRounding = 0;
       ImGuiNET.ImGui.GetStyle().ChildBorderSize = 1;
       ImGuiNET.ImGui.GetStyle().PopupRounding = 0;
       ImGuiNET.ImGui.GetStyle().PopupBorderSize = 0;
       ImGuiNET.ImGui.GetStyle().FramePadding = new Vector2(4, 3);
       ImGuiNET.ImGui.GetStyle().FrameRounding = 4;
       ImGuiNET.ImGui.GetStyle().FrameBorderSize = 0;
       ImGuiNET.ImGui.GetStyle().ItemSpacing = new Vector2(8, 4);
       ImGuiNET.ImGui.GetStyle().ItemInnerSpacing = new Vector2(4, 4);
       ImGuiNET.ImGui.GetStyle().CellPadding = new Vector2(4, 2);
       ImGuiNET.ImGui.GetStyle().TouchExtraPadding = new Vector2(0, 0);
       ImGuiNET.ImGui.GetStyle().IndentSpacing = 21;
       ImGuiNET.ImGui.GetStyle().ScrollbarSize = 16;
       ImGuiNET.ImGui.GetStyle().ScrollbarRounding = 9;
       ImGuiNET.ImGui.GetStyle().GrabMinSize = 13;
       ImGuiNET.ImGui.GetStyle().GrabRounding = 3;
       ImGuiNET.ImGui.GetStyle().LogSliderDeadzone = 4;
       ImGuiNET.ImGui.GetStyle().TabRounding = 4;
       ImGuiNET.ImGui.GetStyle().TabBorderSize = 0;
       ImGuiNET.ImGui.GetStyle().ButtonTextAlign = new Vector2(0.5f, 0.5f);
       ImGuiNET.ImGui.GetStyle().SelectableTextAlign = new Vector2(0, 0);
       ImGuiNET.ImGui.GetStyle().DisplaySafeAreaPadding = new Vector2(3, 3);

       // build colors dict
       var colors = new Dictionary<string, Vector4>
        {
            { "Text", new Vector4(1, 1, 1, 1) },
            { "TextDisabled", new Vector4(0.5f, 0.5f, 0.5f, 1) },
            { "WindowBg", new Vector4(0.06f, 0.06f, 0.06f, 0.93f) },
            { "ChildBg", new Vector4(0, 0, 0, 0) },
            { "PopupBg", new Vector4(0.08f, 0.08f, 0.08f, 0.94f) },
            { "Border", new Vector4(0.43f, 0.43f, 0.5f, 0.5f) },
            { "BorderShadow", new Vector4(0, 0, 0, 0) },
            { "FrameBg", new Vector4(0.29f, 0.29f, 0.29f, 0.54f) },
            { "FrameBgHovered", new Vector4(0.54f, 0.54f, 0.54f, 0.4f) },
            { "FrameBgActive", new Vector4(0.64f, 0.64f, 0.64f, 0.67f) },
            { "TitleBg", new Vector4(0.022624433f, 0.022624206f, 0.022624206f, 0.85067874f) },
            { "TitleBgActive", new Vector4(0.38914025f, 0.10917056f, 0.10917056f, 0.8280543f) },
            { "TitleBgCollapsed", new Vector4(0, 0, 0, 0.51f) },
            { "MenuBarBg", new Vector4(0.14f, 0.14f, 0.14f, 1) },
            { "ScrollbarBg", new Vector4(0, 0, 0, 0) },
            { "ScrollbarGrab", new Vector4(0.31f, 0.31f, 0.31f, 1) },
            { "ScrollbarGrabHovered", new Vector4(0.41f, 0.41f, 0.41f, 1) },
            { "ScrollbarGrabActive", new Vector4(0.51f, 0.51f, 0.51f, 1) },
            { "CheckMark", new Vector4(0.86f, 0.86f, 0.86f, 1) },
            { "SliderGrab", new Vector4(0.54f, 0.54f, 0.54f, 1) },
            { "SliderGrabActive", new Vector4(0.67f, 0.67f, 0.67f, 1) },
            { "Button", new Vector4(0.71f, 0.71f, 0.71f, 0.4f) },
            { "ButtonHovered", new Vector4(0.3647059f, 0.078431375f, 0.078431375f, 0.94509804f) },
            { "ButtonActive", new Vector4(0.48416287f, 0.10077597f, 0.10077597f, 0.94509804f) },
            { "Header", new Vector4(0.59f, 0.59f, 0.59f, 0.31f) },
            { "HeaderHovered", new Vector4(0.5f, 0.5f, 0.5f, 0.8f) },
            { "HeaderActive", new Vector4(0.6f, 0.6f, 0.6f, 1) },
            { "Separator", new Vector4(0.43f, 0.43f, 0.5f, 0.5f) },
            { "SeparatorHovered", new Vector4(0.3647059f, 0.078431375f, 0.078431375f, 0.78280544f) },
            { "SeparatorActive", new Vector4(0.3647059f, 0.078431375f, 0.078431375f, 0.94509804f) },
            { "ResizeGrip", new Vector4(0.79f, 0.79f, 0.79f, 0.25f) },
            { "ResizeGripHovered", new Vector4(0.78f, 0.78f, 0.78f, 0.67f) },
            { "ResizeGripActive", new Vector4(0.3647059f, 0.078431375f, 0.078431375f, 0.94509804f) },
            { "Tab", new Vector4(0.23f, 0.23f, 0.23f, 0.86f) },
            { "TabHovered", new Vector4(0.58371043f, 0.30374074f, 0.30374074f, 0.7647059f) },
            { "TabActive", new Vector4(0.47963798f, 0.15843244f, 0.15843244f, 0.7647059f) },
            { "TabUnfocused", new Vector4(0.068f, 0.10199998f, 0.14800003f, 0.9724f) },
            { "TabUnfocusedActive", new Vector4(0.13599998f, 0.26199996f, 0.424f, 1) },
            { "DockingPreview", new Vector4(0.26f, 0.59f, 0.98f, 0.7f) },
            { "DockingEmptyBg", new Vector4(0.2f, 0.2f, 0.2f, 1) },
            { "PlotLines", new Vector4(0.61f, 0.61f, 0.61f, 1) },
            { "PlotLinesHovered", new Vector4(1, 0.43f, 0.35f, 1) },
            { "PlotHistogram", new Vector4(0.9f, 0.7f, 0, 1) },
            { "PlotHistogramHovered", new Vector4(1, 0.6f, 0, 1) },
            { "TableHeaderBg", new Vector4(0.19f, 0.19f, 0.2f, 1) },
            { "TableBorderStrong", new Vector4(0.31f, 0.31f, 0.35f, 1) },
            { "TableBorderLight", new Vector4(0.23f, 0.23f, 0.25f, 1) },
            { "TableRowBg", new Vector4(0, 0, 0, 0) },
            { "TableRowBgAlt", new Vector4(1, 1, 1, 0.06f) },
            { "TextSelectedBg", new Vector4(0.26f, 0.59f, 0.98f, 0.35f) },
            { "DragDropTarget", new Vector4(1, 1, 0, 0.9f) },
            { "NavHighlight", new Vector4(0.26f, 0.59f, 0.98f, 1) },
            { "NavWindowingHighlight", new Vector4(1, 1, 1, 0.7f) },
            { "NavWindowingDimBg", new Vector4(0.8f, 0.8f, 0.8f, 0.2f) },
            { "ModalWindowDimBg", new Vector4(0.8f, 0.8f, 0.8f, 0.35f) },
        };

       // set colors
       foreach (var imGuiCol in Enum.GetValues<ImGuiCol>())
       {
           if (imGuiCol == ImGuiCol.COUNT)
           {
               continue;
           }

           ImGuiNET.ImGui.GetStyle().Colors[(int)imGuiCol] = colors[imGuiCol.ToString()];
       }

       scene.OnBuildUI += draw;
       scene.Run();
    }
}
