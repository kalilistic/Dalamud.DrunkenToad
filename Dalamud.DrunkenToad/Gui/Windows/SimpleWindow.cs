namespace Dalamud.DrunkenToad.Gui.Windows;

using Plugin;

/// <summary>
/// Simple window for basic display where the window system isn't needed.
/// </summary>
public abstract class SimpleWindow
{
    /// <summary>
    /// Gets the plugin interface.
    /// </summary>
    protected readonly DalamudPluginInterface pluginInterface;

    /// <summary>
    /// Gets a value indicating whether the window is open.
    /// </summary>
    protected bool isOpen = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleWindow" /> class.
    /// </summary>
    /// <param name="pluginInterface">dalamud plugin interface.</param>
    protected SimpleWindow(DalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
        this.pluginInterface.UiBuilder.Draw += this.Draw;
    }

    /// <summary>
    /// Dispose window.
    /// </summary>
    public void Dispose() => this.pluginInterface.UiBuilder.Draw -= this.Draw;

    /// <summary>
    /// Draw window.
    /// </summary>
    protected abstract void Draw();
}
