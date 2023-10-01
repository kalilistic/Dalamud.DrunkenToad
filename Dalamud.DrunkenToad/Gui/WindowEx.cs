namespace Dalamud.DrunkenToad.Gui;

using System.Numerics;
using Core;
using ImGuiNET;
using Interface.Windowing;
using Interfaces;

/// <inheritdoc />
public abstract class WindowEx : Window
{
    /// <summary>
    /// Plugin Configuration.
    /// </summary>
    protected readonly IPluginConfig config;

    /// <summary>
    /// Default window flags.
    /// </summary>
    protected readonly ImGuiWindowFlags defaultFlags;

    private const float IndentSpacing = 21f;
    private const float ChildBorderSize = 1;
    private readonly Vector2 cellPadding = new (4, 2);
    private readonly Vector2 framePadding = new (4, 3);
    private readonly Vector2 itemInnerSpacing = new (4, 4);
    private readonly Vector2 itemSpacing = new (8, 4);
    private readonly Vector2 windowPadding = new (8, 8);

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowEx" /> class.
    /// </summary>
    /// <param name="name">window name.</param>
    /// <param name="config">plugin configuration.</param>
    /// <param name="flags">window flags.</param>
    // ReSharper disable once UnusedParameter.Local
    protected WindowEx(string name, IPluginConfig config, ImGuiWindowFlags flags = ImGuiWindowFlags.None)
        : base(name, flags, true)
    {
        this.config = config;
        this.defaultFlags = flags;
        this.RespectCloseHotkey = false;
    }

    /// <summary>
    /// Function to invoke on pre-enable.
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Additional conditions for the window to be drawn, regardless of its open-state.
    /// Checks if logged in and logged in state toggle.
    /// </summary>
    /// <returns>
    /// True if the window should be drawn, false otherwise.
    /// </returns>
    public override bool DrawConditions()
    {
        if (DalamudContext.ClientStateHandler.IsLoggedIn)
        {
            return true;
        }

        if (!DalamudContext.ClientStateHandler.IsLoggedIn && !this.config.OnlyShowWindowWhenLoggedIn)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Code to be executed before conditionals are applied and the window is drawn.
    /// Enforces default padding and spacing styles.
    /// </summary>
    public override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, this.windowPadding);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, this.framePadding);
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, this.cellPadding);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, this.itemSpacing);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, this.itemInnerSpacing);
        ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, IndentSpacing);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, ChildBorderSize);
    }

    /// <summary>
    /// Code to be executed after the window is drawn.
    /// Enforces default padding and spacing styles.
    /// </summary>
    public override void PostDraw() => ImGui.PopStyleVar(7);

    /// <summary>
    /// Updates dynamic window flags.
    /// </summary>
    protected void SetWindowFlags()
    {
        var flags = this.defaultFlags;
        if (this.config.IsWindowSizeLocked)
        {
            flags |= ImGuiWindowFlags.NoResize;
        }

        if (this.config.IsWindowPositionLocked)
        {
            flags |= ImGuiWindowFlags.NoMove;
        }

        this.Flags = flags;
    }
}
