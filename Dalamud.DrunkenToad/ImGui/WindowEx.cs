using System;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using PluginConfiguration = FlexConfig.Configuration;

namespace Dalamud.DrunkenToad.ImGui;

public abstract class WindowEx : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowEx"/> class.
    /// </summary>
    /// <param name="name">The name/ID of this window.
    /// If you have multiple windows with the same name, you will need to
    /// append an unique ID to it by specifying it after "###" behind the window title.
    /// </param>
    /// <param name="flags">The <see cref="T:ImGuiNET.ImGuiWindowFlags" /> of this window.</param>
    /// <param name="forceMainWindow">Whether or not this window should be limited to the main game window.</param>
    protected WindowEx(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false)
        : base(name, flags, forceMainWindow)
    {
    }

    /// <summary>
    /// Gets or sets plugin configuration.
    /// </summary>
    public PluginConfiguration Configuration { get; set; }

    /// <summary>
    /// Gets or sets callback to localize a string in the current language.
    /// </summary>
    public Func<string, string>? Localize { get; set; }
}
