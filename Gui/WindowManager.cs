// ReSharper disable MemberCanBePrivate.Global
namespace Dalamud.DrunkenToad.Gui;

using Dalamud.Interface.Windowing;
using Dalamud.Plugin;

/// <summary>
/// Wrapper for WindowSystem using implementations to simplify ImGui windowing.
/// </summary>
public class WindowManager
{
    private readonly IDalamudPluginInterface pluginInterface;
    private readonly WindowSystem windowSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowManager" /> class.
    /// </summary>
    /// <param name="pluginInterface">The plugin interface.</param>
    public WindowManager(IDalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
        this.windowSystem = new WindowSystem(this.pluginInterface.InternalName);
        this.pluginInterface.UiBuilder.Draw += this.Draw;
    }

    private bool IsEnabled { get; set; }

    /// <summary>
    /// Enable windows.
    /// </summary>
    public void Enable()
    {
        foreach (var window in this.windowSystem.Windows)
        {
            var windowEx = (WindowEx)window;
            windowEx.Initialize();
        }

        this.IsEnabled = true;
    }

    /// <summary>
    /// Disable windows.
    /// </summary>
    public void Disable() => this.IsEnabled = false;

    /// <summary>
    /// Add a window to this <see cref="WindowSystem" />.
    /// </summary>
    /// <param name="newWindows">The window(s) to add.</param>
    public void AddWindows(params Window[] newWindows)
    {
        foreach (var window in newWindows)
        {
            this.windowSystem.AddWindow(window);
        }
    }

    /// <summary>
    /// Remove window to this <see cref="WindowSystem" />.
    /// </summary>
    /// <param name="windows">The window(s) to remove.</param>
    public void RemoveWindows(params Window[] windows)
    {
        foreach (var window in windows)
        {
            this.windowSystem.RemoveWindow(window);
        }
    }

    /// <summary>
    /// Remove all windows.
    /// </summary>
    public void RemoveWindows() => this.windowSystem.RemoveAllWindows();

    /// <summary>
    /// Clean up windows and events.
    /// </summary>
    public void Dispose()
    {
        this.Disable();
        this.pluginInterface.UiBuilder.Draw -= this.Draw;
        this.windowSystem.RemoveAllWindows();
    }

    private void Draw()
    {
        if (!this.IsEnabled)
        {
            return;
        }

        this.windowSystem.Draw();
    }
}
