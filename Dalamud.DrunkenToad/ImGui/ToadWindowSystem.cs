using System;
using System.Collections.Generic;
using System.Linq;

namespace Dalamud.DrunkenToad.ImGui;

/// <summary>
/// Class running a WindowSystem using <see cref="ToadWindow"/> implementations to simplify ImGui windowing.
/// Copied from dalamud to decouple from dalamud services and other minor changes.
/// </summary>
public class ToadWindowSystem
{
    private static DateTimeOffset lastAnyFocus;

    private readonly List<ToadWindow> windows = new ();

    private string lastFocusedWindowName = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToadWindowSystem"/> class.
    /// </summary>
    /// <param name="imNamespace">The name/ID-space of this <see cref="ToadWindowSystem"/>.</param>
    public ToadWindowSystem(string? imNamespace = null)
    {
        this.Namespace = imNamespace;
    }

    /// <summary>
    /// Gets a value indicating whether any <see cref="ToadWindowSystem"/> contains any <see cref="ToadWindow"/>
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
    /// Gets a read-only list of all <see cref="ToadWindow"/>s in this <see cref="ToadWindowSystem"/>.
    /// </summary>
    public IReadOnlyList<ToadWindow> Windows => this.windows;

    /// <summary>
    /// Gets a value indicating whether any window in this <see cref="ToadWindowSystem"/> has focus and is
    /// not marked to be excluded from consideration.
    /// </summary>
    public bool HasAnyFocus { get; private set; }

    /// <summary>
    /// Gets or sets the name/ID-space of this <see cref="ToadWindowSystem"/>.
    /// </summary>
    public string? Namespace { get; set; }

    /// <summary>
    /// Add a window to this <see cref="ToadWindowSystem"/>.
    /// </summary>
    /// <param name="toadWindow">The window to add.</param>
    public void AddWindow(ToadWindow toadWindow)
    {
        if (this.windows.Any(x => x.WindowName == toadWindow.WindowName))
            throw new ArgumentException("A window with this name/ID already exists.");

        this.windows.Add(toadWindow);
    }

    /// <summary>
    /// Remove a window from this <see cref="ToadWindowSystem"/>.
    /// </summary>
    /// <param name="toadWindow">The window to remove.</param>
    public void RemoveWindow(ToadWindow toadWindow)
    {
        if (!this.windows.Contains(toadWindow))
            throw new ArgumentException("This window is not registered on this WindowSystem.");

        this.windows.Remove(toadWindow);
    }

    /// <summary>
    /// Remove all windows from this <see cref="ToadWindowSystem"/>.
    /// </summary>
    public void RemoveAllWindows() => this.windows.Clear();

    /// <summary>
    /// Get a window by name.
    /// </summary>
    /// <param name="windowName">The name of the <see cref="ToadWindow"/>.</param>
    /// <returns>The <see cref="ToadWindow"/> object with matching name or null.</returns>
    public ToadWindow? GetWindow(string windowName) => this.windows.FirstOrDefault(w => w.WindowName == windowName);

    /// <summary>
    /// Draw all registered windows using ImGui.
    /// </summary>
    public void Draw()
    {
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
}
