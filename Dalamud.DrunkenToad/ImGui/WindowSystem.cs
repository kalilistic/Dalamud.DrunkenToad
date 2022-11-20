using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.DrunkenToad.Core;
using Config = FlexConfig.Configuration;

namespace Dalamud.DrunkenToad.ImGui;

/// <summary>
/// Class running a WindowSystem using <see cref="Window"/> implementations to simplify ImGui windowing.
/// Copied from dalamud to decouple from dalamud services and other minor changes.
/// </summary>
public class WindowSystem
{
    private static DateTimeOffset lastAnyFocus;

    private bool isEnabled { get; set; }

    private readonly List<Window> windows = new ();

    private string lastFocusedWindowName = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowSystem"/> class.
    /// </summary>
    /// <param name="imNamespace">The name/ID-space of this <see cref="WindowSystem"/>.</param>
    /// <param name="config">FlexConfig instance.</param>
    public WindowSystem(string imNamespace, Config config)
    {
        this.Namespace = imNamespace;
        this.Configuration = config;
        FlexGui.Initialize(this.Configuration);
        DalamudContext.PluginInterface.UiBuilder.Draw += this.Draw;
    }

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
    /// Clean up windows and events.
    /// </summary>
    public void Dispose()
    {
        this.Disable();
        DalamudContext.PluginInterface.UiBuilder.Draw -= this.Draw;
        this.RemoveAllWindows();
    }

    /// <summary>
    /// Gets or sets callback to determine if escape key has been pressed.
    /// </summary>
    public static Func<bool>? IsEscapePressed { get; set; }

    /// <summary>
    /// Gets or sets callback to determine if focus management is enabled.
    /// </summary>
    public static Func<bool>? IsFocusManagementEnabled { get; set; }

    /// <summary>
    /// Gets or sets callback to localize a string in the current language.
    /// </summary>
    public static Func<string, string>? Localize { get; set; }

    /// <summary>
    /// Gets a value indicating whether any <see cref="WindowSystem"/> contains any <see cref="Window"/>
    /// that has focus and is not marked to be excluded from consideration.
    /// </summary>
    public static bool HasAnyWindowSystemFocus { get; internal set; }

    /// <summary>
    /// Gets the name of the currently focused window system that is redirecting normal escape functionality.
    /// </summary>
    public static string FocusedWindowSystemNamespace { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the timespan since the last time any window was focused.
    /// </summary>
    public static TimeSpan TimeSinceLastAnyFocus => DateTimeOffset.Now - lastAnyFocus;

    /// <summary>
    /// Gets or sets plugin configuration.
    /// </summary>
    public Config Configuration { get; set; }

    /// <summary>
    /// Gets a read-only list of all <see cref="Window"/>s in this <see cref="WindowSystem"/>.
    /// </summary>
    public IReadOnlyList<Window> Windows => this.windows;

    /// <summary>
    /// Gets a value indicating whether any window in this <see cref="WindowSystem"/> has focus and is
    /// not marked to be excluded from consideration.
    /// </summary>
    public bool HasAnyFocus { get; private set; }

    /// <summary>
    /// Gets or sets the name/ID-space of this <see cref="WindowSystem"/>.
    /// </summary>
    public string? Namespace { get; set; }

    /// <summary>
    /// Add a window to this <see cref="WindowSystem"/>.
    /// </summary>
    /// <param name="newWindows">The window(s) to add.</param>
    public void AddWindows(params Window[] newWindows)
    {
        foreach (var window in newWindows)
        {
            if (this.windows.Any(x => x.WindowName == window.WindowName))
                throw new ArgumentException("A window with this name/ID already exists.");

            window.IsFocusManagementEnabled = IsFocusManagementEnabled;
            window.IsEscapePressed = IsEscapePressed;
            window.Localize = Localize;
            window.Configuration = this.Configuration;

            this.windows.Add(window);
        }
    }

    /// <summary>
    /// Remove a window from this <see cref="WindowSystem"/>.
    /// </summary>
    /// <param name="window">The window to remove.</param>
    public void RemoveWindow(Window window)
    {
        if (!this.windows.Contains(window))
            throw new ArgumentException("This window is not registered on this WindowSystem.");

        this.windows.Remove(window);
    }

    /// <summary>
    /// Get a window by name.
    /// </summary>
    /// <param name="windowName">The name of the <see cref="Window"/>.</param>
    /// <returns>The <see cref="Window"/> object with matching name or null.</returns>
    public Window? GetWindow(string windowName) => this.windows.FirstOrDefault(w => w.WindowName == windowName);

    /// <summary>
    /// Draw all registered windows using ImGui.
    /// </summary>
    public void Draw()
    {
        if (!this.isEnabled) return;

        var hasNamespace = !string.IsNullOrEmpty(this.Namespace);

        if (hasNamespace)
            ImGuiNET.ImGui.PushID(this.Namespace);

        // Shallow clone the list of windows so that we can edit it without modifying it while the loop is iterating
        foreach (var window in this.windows.ToArray())
        {
            window.DrawInternal();
        }

        var focusedWindow = this.windows.FirstOrDefault(window => window.IsFocused && window.RespectCloseHotkey);
        this.HasAnyFocus = focusedWindow != default;

        if (this.HasAnyFocus)
        {
            if (this.lastFocusedWindowName != focusedWindow?.WindowName)
            {
                this.lastFocusedWindowName = focusedWindow?.WindowName ?? string.Empty;
            }

            HasAnyWindowSystemFocus = true;
            FocusedWindowSystemNamespace = this.Namespace ?? string.Empty;

            lastAnyFocus = DateTimeOffset.Now;
        }
        else
        {
            this.lastFocusedWindowName = string.Empty;
        }

        if (hasNamespace)
            ImGuiNET.ImGui.PopID();
    }

    /// <summary>
    /// Remove all windows from this <see cref="WindowSystem"/>.
    /// </summary>
    private void RemoveAllWindows() => this.windows.Clear();
}
