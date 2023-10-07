namespace Dalamud.DrunkenToad.Gui.Windows;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ImGuiNET;
using Interface;
using Interface.Colors;
using Interface.Utility;
using Plugin;

/// <summary>
/// Window for displaying migration progress.
/// </summary>
public class MigrationWindow : SimpleWindow
{
    private readonly ConcurrentQueue<string> errorQueue = new ();
    private readonly ConcurrentQueue<Tuple<string, bool>> stepQueue = new ();
    private readonly List<string> errorMessages = new ();
    private readonly List<Tuple<string, bool>> steps = new ();
    private readonly Stopwatch stopwatch;
    private bool isMigrationFinished;
    private int previousStepCount;

    public MigrationWindow(DalamudPluginInterface pluginInterface)
        : base(pluginInterface)
    {
        this.stopwatch = new Stopwatch();
        this.stopwatch.Start();
    }

    public void LogWarning(string message) => this.stepQueue.Enqueue(new Tuple<string, bool>(message, false));

    public void LogInfo(string message) => this.stepQueue.Enqueue(new Tuple<string, bool>(message, true));

    public void LogError(string message) => this.errorQueue.Enqueue(message);

    public void StopMigration()
    {
        this.stopwatch.Stop();
        this.isMigrationFinished = true;
    }

    protected override void Draw()
    {
        if (!this.isOpen)
        {
            return;
        }

        ImGui.SetNextWindowSize(ImGuiHelpers.ScaledVector2(600f, 400f), ImGuiCond.Appearing);
        const ImGuiWindowFlags windowFlags = ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoResize;

        ImGui.Begin($"{this.pluginInterface.InternalName}##Migration", windowFlags);

        this.DrawHeaderSection();
        this.DrawStepsSection();
        this.DrawButtons();
        this.DrawMigrationTimer();

        ImGui.End();
    }

    private void DrawMigrationTimer()
    {
        ImGui.Text($"Elapsed Time: {this.stopwatch.Elapsed:mm\\:ss\\.fff}");
        ImGuiHelpers.ScaledDummy(3f);
    }

    private void DrawButtons()
    {
        ImGuiHelpers.ScaledDummy(4f);
        var isButtonDisabled = !this.isMigrationFinished;
        ImGui.BeginDisabled(isButtonDisabled);
        if (ImGui.Button("Close Window"))
        {
            this.isOpen = false;
        }

        ImGui.EndDisabled();

        if (this.errorMessages.Count > 0)
        {
            ImGui.SameLine();
            ImGui.BeginDisabled(isButtonDisabled);
            if (ImGui.Button("Copy Errors to Clipboard"))
            {
                ImGui.SetClipboardText(string.Join(
                    "\n",
                    new[] { $"{this.pluginInterface.InternalName} Migration Error Log" }.Concat(this.errorMessages)));
            }

            ImGui.EndDisabled();
        }

        ImGuiHelpers.ScaledDummy(1f);
    }

    private void DrawHeaderSection()
    {
        ImGui.TextColored(ImGuiColors.DalamudViolet, "Database Migration");
        ImGui.BulletText("Do not interrupt the upgrade.");
        ImGui.BulletText("Report any issues on Discord or GitHub.");
        ImGui.Spacing();
    }

    private void DrawStepsSection()
    {
        while (!this.stepQueue.IsEmpty)
        {
            if (this.stepQueue.TryDequeue(out var msg) && !string.IsNullOrEmpty(msg.Item1))
            {
                this.steps.Add(msg);
            }
        }

        ImGuiHelpers.ScaledDummy(1f);
        ImGui.TextColored(ImGuiColors.DalamudViolet, "Migration Progress");

        ImGui.BeginChild("Messages", ImGuiHelpers.ScaledVector2(0f, 100f), true);
        foreach (var step in this.steps)
        {
            ImGui.Indent(1f);
            ImGui.PushFont(UiBuilder.IconFont);
            if (step.Item2)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
                ImGui.Text(FontAwesomeIcon.Check.ToIconString());
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
                ImGui.Text(FontAwesomeIcon.ExclamationTriangle.ToIconString());
            }

            ImGui.PopStyleColor(1);
            ImGui.PopFont();
            ImGui.Unindent(1f);
            ImGui.SameLine();
            ImGui.Text(step.Item1);
        }

        while (!this.errorQueue.IsEmpty)
        {
            if (this.errorQueue.TryDequeue(out var msg) && !string.IsNullOrEmpty(msg))
            {
                this.errorMessages.Add(msg);
            }
        }

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
            ImGui.Text(" Migration failed! Report this on Discord or GitHub.");
            ImGui.SetScrollHereY(1.0f);
        }

        if (this.previousStepCount < this.steps.Count)
        {
            ImGui.SetScrollHereY(1.0f);
            this.previousStepCount = this.steps.Count;
        }

        ImGui.EndChild();
    }
}
