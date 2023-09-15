namespace Dalamud.DrunkenToad.Gui.Windows;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using Interface;
using Interface.Colors;
using Plugin;

/// <summary>
/// Window for displaying migration progress.
/// </summary>
public class MigrationWindow : SimpleWindow
{
    /// <summary>
    /// Gets a value for migration errors.
    /// </summary>
    public readonly ConcurrentQueue<string> ErrorQueue = new ();

    /// <summary>
    /// Gets a value for migration steps.
    /// </summary>
    public readonly ConcurrentQueue<string> StepQueue = new ();

    private readonly List<string> errorMessages = new ();
    private readonly List<string> steps = new ();
    private readonly Stopwatch stopwatch;
    private bool isMigrationFinished;

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationWindow" /> class.
    /// </summary>
    /// <param name="pluginInterface">dalamud plugin interface.</param>
    public MigrationWindow(DalamudPluginInterface pluginInterface)
        : base(pluginInterface)
    {
        this.stopwatch = new Stopwatch();
        this.stopwatch.Start();
    }

    /// <summary>
    /// End migration to show close button and stop timer.
    /// </summary>
    public void StopMigration()
    {
        this.stopwatch.Stop();
        this.isMigrationFinished = true;
    }

    /// <inheritdoc />
    protected override void Draw()
    {
        if (!this.isOpen)
        {
            return;
        }

        ImGui.SetNextWindowSize(ImGuiHelpers.ScaledVector2(700f, 500f), ImGuiCond.Appearing);
        ImGui.Begin($"{this.pluginInterface.InternalName}##Migration", ref this.isOpen);

        // Header Section
        ImGui.TextColored(ImGuiColors.DalamudViolet, $"{this.pluginInterface.InternalName} Migration");
        ImGui.BulletText($"We need to migrate your {this.pluginInterface.InternalName} data to a new database.");
        ImGui.BulletText("Do not close the window until the migration is complete.");
        ImGui.BulletText("Report any issues on discord or through the plugin installer.");
        ImGui.Spacing();

        // Steps Section
        while (!this.StepQueue.IsEmpty)
        {
            if (this.StepQueue.TryDequeue(out var msg) && !string.IsNullOrEmpty(msg))
            {
                this.steps.Add(msg);
            }
        }

        ImGuiHelpers.ScaledDummy(2f);
        ImGui.TextColored(ImGuiColors.DalamudViolet, "Migration Progress");

        var totalHeight = 0f;
        foreach (var step in this.steps)
        {
            var textSize = ImGui.CalcTextSize(step);
            totalHeight += textSize.Y + ImGui.GetTextLineHeightWithSpacing();
        }

        var childSize = new Vector2(-1, totalHeight - 50 * ImGuiHelpers.GlobalScale);
        ImGui.BeginChild("##stepsChildWindow", childSize, true);

        foreach (var step in this.steps)
        {
            ImGui.Indent(1f);
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
            ImGui.Text(FontAwesomeIcon.Check.ToIconString());
            ImGui.PopStyleColor(1);
            ImGui.PopFont();
            ImGui.Unindent(1f);
            ImGui.SameLine();
            ImGui.Text(step);
        }

        // Add Error Message
        if (this.errorMessages.Count > 0)
        {
            ImGui.Indent(3.5f);
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
            ImGui.Text(FontAwesomeIcon.Times.ToIconString());
            ImGui.PopStyleColor(1);
            ImGui.PopFont();
            ImGui.Unindent(3.5f);
            ImGui.SameLine();
            ImGui.Text(" Something went wrong... failed to migrate!");
        }

        ImGui.EndChild();

        while (!this.ErrorQueue.IsEmpty)
        {
            if (this.ErrorQueue.TryDequeue(out var msg) && !string.IsNullOrEmpty(msg))
            {
                this.errorMessages.Add(msg);
            }
        }

        // Footer Section
        ImGuiHelpers.ScaledDummy(10f);
        ImGui.Text($"Elapsed Time: {this.stopwatch.Elapsed:mm\\:ss\\.fff}");

        if (this.isMigrationFinished)
        {
            if (ImGui.Button("Close Window"))
            {
                this.isOpen = false;
            }

            if (this.errorMessages.Count > 0)
            {
                ImGui.SameLine();
                if (ImGui.Button("Copy Errors to Clipboard"))
                {
                    ImGui.SetClipboardText(string.Join(
                        "\n",
                        new[] { $"{this.pluginInterface.InternalName} Migration Error Log" }.Concat(this.errorMessages)));
                }
            }
        }

        ImGui.End();
    }
}
