namespace Dalamud.DrunkenToad.Gui.Windows;

using ImGuiNET;
using Interface;
using Interface.Colors;
using Plugin;

/// <summary>
/// Error window to use if plugin fails to load.
/// </summary>
public class ErrorWindow : SimpleWindow
{
    private readonly string message;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorWindow" /> class.
    /// </summary>
    /// <param name="pluginInterface">dalamud plugin interface.</param>
    /// <param name="message">error message.</param>
    public ErrorWindow(DalamudPluginInterface pluginInterface, string message)
        : base(pluginInterface) => this.message = message;

    /// <inheritdoc />
    protected override void Draw()
    {
        if (!this.isOpen)
        {
            return;
        }

        ImGui.SetNextWindowSize(ImGuiHelpers.ScaledVector2(500f, 300f), ImGuiCond.Appearing);
        ImGui.Begin($"{this.pluginInterface.InternalName}##Error", ref this.isOpen);
        ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DPSRed);
        ImGui.TextWrapped(this.message);
        ImGui.PopStyleColor();
        ImGui.End();
    }
}
