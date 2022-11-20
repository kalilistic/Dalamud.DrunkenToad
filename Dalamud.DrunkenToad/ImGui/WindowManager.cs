using Dalamud.DrunkenToad.Core;
using Dalamud.DrunkenToad.Extension;
using Dalamud.Interface.Windowing;
using PluginConfiguration = FlexConfig.Configuration;

namespace Dalamud.DrunkenToad.ImGui;

/// <summary>
/// Wrapper for WindowSystem using implementations to simplify ImGui windowing.
/// </summary>
public class WindowManager
{
    private readonly WindowSystem windowSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowManager"/> class.
    /// </summary>
    public WindowManager()
    {
        this.windowSystem = new WindowSystem(DalamudContext.PluginInterface.PluginName());
        FlexGui.Initialize(DalamudContext.PluginConfiguration);
        DalamudContext.PluginInterface.UiBuilder.Draw += this.Draw;
    }

    private bool isEnabled { get; set; }

    /// <summary>
    /// Enable windows.
    /// </summary>
    public void Enable()
    {
        this.isEnabled = true;
    }

    /// <summary>
    /// Disable windows.
    /// </summary>
    public void Disable()
    {
        this.isEnabled = false;
    }

    /// <summary>
    /// Add a window to this <see cref="WindowSystem"/>.
    /// </summary>
    /// <param name="newWindows">The window(s) to add.</param>
    public void AddWindows(params WindowEx[] newWindows)
    {
        foreach (var window in newWindows)
        {
            window.Localize = key => DalamudContext.Localization.GetString(key);
            window.Configuration = DalamudContext.PluginConfiguration;
            this.windowSystem.AddWindow(window);
        }
    }

    /// <summary>
    /// Clean up windows and events.
    /// </summary>
    public void Dispose()
    {
        this.Disable();
        DalamudContext.PluginInterface.UiBuilder.Draw -= this.Draw;
        this.windowSystem.RemoveAllWindows();
    }

    private void Draw()
    {
        if (!this.isEnabled) return;
        this.windowSystem.Draw();
    }
}
