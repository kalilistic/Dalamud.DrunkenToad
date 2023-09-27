namespace Dalamud.DrunkenToad.Consumers;

using System;
using System.Collections.Generic;
using Core;
using Plugin;
using Plugin.Ipc;

/// <summary>
/// IPC with Visibility Plugin.
/// </summary>
public class VisibilityConsumer
{
    private const string RequiredVisibilityVersion = "1";
    private readonly DalamudPluginInterface pluginInterface;
    private ICallGateSubscriber<string, uint, string, object> consumerAddToVoidList = null!;
    private ICallGateSubscriber<string, uint, string, object> consumerAddToWhiteList = null!;
    private ICallGateSubscriber<string> consumerApiVersion = null!;
    private ICallGateSubscriber<IEnumerable<string>> consumerGetVoidListEntries = null!;
    private ICallGateSubscriber<IEnumerable<string>> consumerGetWhiteListEntries = null!;
    private ICallGateSubscriber<string, uint, object> consumerRemoveFromVoidList = null!;
    private ICallGateSubscriber<string, uint, object> consumerRemoveFromWhiteList = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="VisibilityConsumer" /> class.
    /// </summary>
    /// <param name="pluginInterface">Dalamud Plugin Interface.</param>
    public VisibilityConsumer(DalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
        this.Subscribe();
    }

    /// <summary>
    /// Subscribe to visibility plugin methods.
    /// </summary>
    public void Subscribe()
    {
        try
        {
            this.consumerApiVersion =
                this.pluginInterface.GetIpcSubscriber<string>("Visibility.ApiVersion");

            this.consumerGetVoidListEntries =
                this.pluginInterface.GetIpcSubscriber<IEnumerable<string>>(
                    "Visibility.GetVoidListEntries");

            this.consumerAddToVoidList =
                this.pluginInterface.GetIpcSubscriber<string, uint, string, object>(
                    "Visibility.AddToVoidList");

            this.consumerRemoveFromVoidList =
                this.pluginInterface.GetIpcSubscriber<string, uint, object>(
                    "Visibility.RemoveFromVoidList");

            this.consumerGetWhiteListEntries =
                this.pluginInterface.GetIpcSubscriber<IEnumerable<string>>(
                    "Visibility.GetWhitelistEntries");

            this.consumerAddToWhiteList =
                this.pluginInterface.GetIpcSubscriber<string, uint, string, object>(
                    "Visibility.AddToWhitelist");

            this.consumerRemoveFromWhiteList =
                this.pluginInterface.GetIpcSubscriber<string, uint, object>(
                    "Visibility.RemoveFromWhitelist");
        }
        catch (Exception ex)
        {
            DalamudContext.PluginLog.Verbose($"Failed to subscribe to Visibility.:\n{ex}");
        }
    }

    /// <summary>
    /// Get void list entries.
    /// </summary>
    /// <returns>list of void entries.</returns>
    public IEnumerable<string> GetVoidListEntries() => this.consumerGetVoidListEntries.InvokeFunc();

    /// <summary>
    /// Adds entry to VoidList.
    /// </summary>
    /// <param name="name">Full player name.</param>
    /// <param name="worldId">World ID.</param>
    /// <param name="reason">Reason for adding.</param>
    public void AddToVoidList(string name, uint worldId, string reason)
    {
        DalamudContext.PluginLog.Verbose("Adding " + name);
        this.consumerAddToVoidList.InvokeAction(name, worldId, reason);
    }

    /// <summary>
    /// Removes entry from VoidList.
    /// </summary>
    /// <param name="name">Full player name.</param>
    /// <param name="worldId">World ID.</param>
    public void RemoveFromVoidList(string name, uint worldId)
    {
        DalamudContext.PluginLog.Verbose("Removing " + name);
        this.consumerRemoveFromVoidList.InvokeAction(name, worldId);
    }

    /// <summary>
    /// Fetch all entries from WhiteList.
    /// </summary>
    /// <returns>A collection of strings in the form of (name worldId reason).</returns>
    public IEnumerable<string> GetWhiteListEntries() => this.consumerGetWhiteListEntries.InvokeFunc();

    /// <summary>
    /// Adds entry to WhiteList.
    /// </summary>
    /// <param name="name">Full player name.</param>
    /// <param name="worldId">World ID.</param>
    /// <param name="reason">Reason for adding.</param>
    public void AddToWhiteList(string name, uint worldId, string reason)
    {
        DalamudContext.PluginLog.Verbose("Adding " + name);
        this.consumerAddToWhiteList.InvokeAction(name, worldId, reason);
    }

    /// <summary>
    /// Removes entry from WhiteList.
    /// </summary>
    /// <param name="name">Full player name.</param>
    /// <param name="worldId">World ID.</param>
    public void RemoveFromWhiteList(string name, uint worldId)
    {
        DalamudContext.PluginLog.Verbose("Removing " + name);
        this.consumerRemoveFromWhiteList.InvokeAction(name, worldId);
    }

    /// <summary>
    /// Check if visibility is available.
    /// </summary>
    /// <returns>Gets indicator whether visibility is available.</returns>
    public bool IsAvailable()
    {
        try
        {
            var version = this.consumerApiVersion.InvokeFunc();
            return version.Equals(RequiredVisibilityVersion, StringComparison.Ordinal);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
