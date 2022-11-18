using Dalamud.Plugin;

namespace Dalamud.DrunkenToad.Core;

/// <summary>
/// Interface for using Dalamud DalamudConfiguration.
/// Ensure you use QueueSave if setting config fields to ensure they persist.
/// https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Configuration/Internal/DalamudConfiguration.cs.
/// </summary>
public class DalamudConfiguration : DalamudObject
{
    public DalamudConfiguration()
    {
        this.instance = typeof(IDalamudPlugin)
            .Assembly.GetType("Dalamud.Service`1") !
            .MakeGenericType(typeof(IDalamudPlugin).Assembly.GetType("Dalamud.Configuration.Internal.DalamudConfiguration") !)
            .GetMethod("Get", Flags) !
            .Invoke(null, null) !;
        this.instanceType = this.instance.GetType();
        verify(nameof(this.IsFocusManagementEnabled), this.IsFocusManagementEnabled);
        verify(nameof(this.IsMbCollect), this.IsMbCollect);
        verify(nameof(this.QueueSave), this.QueueSave);
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not focus management is enabled.
    /// </summary>
    public bool IsFocusManagementEnabled
    {
        get => (bool)this.instanceType.GetProperty(nameof(this.IsFocusManagementEnabled), Flags)?.GetValue(this.instance) !;
        set => this.instanceType.GetProperty(nameof(this.IsFocusManagementEnabled), Flags)?.SetValue(this.instance, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not market board data should be uploaded.
    /// </summary>
    public bool IsMbCollect
    {
        get => (bool)this.instanceType.GetProperty(nameof(this.IsMbCollect), Flags)?.GetValue(this.instance) !;
        set => this.instanceType.GetProperty(nameof(this.IsMbCollect), Flags)?.SetValue(this.instance, value);
    }

    /// <summary>
    /// Save the configuration at the path it was loaded from.
    /// </summary>
    public void QueueSave()
    {
        this.instanceType.GetMethod("QueueSave", Flags)?.Invoke(this.instance, null);
    }
}
