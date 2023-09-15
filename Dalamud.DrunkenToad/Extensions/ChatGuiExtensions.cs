namespace Dalamud.DrunkenToad.Extensions;

using System.Collections.Generic;
using System.Linq;
using Core;
using Game.Gui;
using Game.Text;
using Game.Text.SeStringHandling;
using Game.Text.SeStringHandling.Payloads;

/// <summary>
/// Dalamud ChatGUI extensions.
/// </summary>
public static class ChatGuiExtensions
{
    /// <summary>
    /// Print message with plugin name to dalamud default channel.
    /// </summary>
    /// <param name="value">chat gui service.</param>
    /// <param name="message">chat message.</param>
    public static void PluginPrint(this ChatGui value, string message) => value.Print(BuildSeString(DalamudContext.PluginInterface.InternalName, message));

    /// <summary>
    /// Print payload with plugin name to dalamud default channel.
    /// </summary>
    /// <param name="value">chat gui service.</param>
    /// <param name="payloads">list of payloads.</param>
    public static void PluginPrint(this ChatGui value, IEnumerable<Payload> payloads) =>
        value.Print(BuildSeString(DalamudContext.PluginInterface.InternalName, payloads));

    /// <summary>
    /// Print message with plugin name to specified channel.
    /// </summary>
    /// <param name="value">chat gui service.</param>
    /// <param name="message">chat message.</param>
    /// <param name="chatType">chat type to use.</param>
    public static void PluginPrint(this ChatGui value, string message, XivChatType chatType) => value.PrintChat(new XivChatEntry
    {
        Message = BuildSeString(DalamudContext.PluginInterface.InternalName, message), Type = chatType,
    });

    /// <summary>
    /// Print message with plugin name to specified channel.
    /// </summary>
    /// <param name="value">chat gui service.</param>
    /// <param name="payloads">list of payloads.</param>
    /// <param name="chatType">chat type to use.</param>
    public static void PluginPrint(this ChatGui value, IEnumerable<Payload> payloads, XivChatType chatType) => value.PrintChat(new XivChatEntry
    {
        Message = BuildSeString(DalamudContext.PluginInterface.InternalName, payloads), Type = chatType,
    });

    /// <summary>
    /// Print message with plugin name to notice channel.
    /// </summary>
    /// <param name="value">chat gui service.</param>
    /// <param name="payloads">list of payloads.</param>
    public static void PluginPrintNotice(this ChatGui value, IEnumerable<Payload> payloads) => value.PrintChat(new XivChatEntry
    {
        Message = BuildSeString(DalamudContext.PluginInterface.InternalName, payloads), Type = XivChatType.Notice,
    });

    /// <summary>
    /// Print message with plugin name to notice channel.
    /// </summary>
    /// <param name="value">chat gui service.</param>
    /// <param name="message">chat message.</param>
    public static void PluginPrintNotice(this ChatGui value, string message) => value.PrintChat(new XivChatEntry
    {
        Message = BuildSeString(DalamudContext.PluginInterface.InternalName, message), Type = XivChatType.Notice,
    });

    /// <summary>
    /// Print chat with player and message.
    /// </summary>
    /// <param name="value">chat gui service.</param>
    /// <param name="playerName">player name.</param>
    /// <param name="worldId">player home world id.</param>
    /// <param name="message">message.</param>
    /// <param name="chatType">target channel.</param>
    public static void PluginPrint(this ChatGui value, string playerName, uint worldId, string message, XivChatType chatType)
    {
        var basePayloads = BuildBasePayloads(DalamudContext.PluginInterface.InternalName);
        var customPayloads = new List<Payload> { new PlayerPayload(playerName, worldId), new TextPayload(" " + message) };

        var seString = new SeString(basePayloads.Concat(customPayloads).ToList());
        value.PrintChat(new XivChatEntry { Message = seString, Type = chatType });
    }

    private static SeString BuildSeString(string? pluginName, string message)
    {
        var basePayloads = BuildBasePayloads(pluginName);
        var customPayloads = new List<Payload> { new TextPayload(message) };

        return new SeString(basePayloads.Concat(customPayloads).ToList());
    }

    private static SeString BuildSeString(string? pluginName, IEnumerable<Payload> payloads)
    {
        var basePayloads = BuildBasePayloads(pluginName);
        return new SeString(basePayloads.Concat(payloads).ToList());
    }

    private static IEnumerable<Payload> BuildBasePayloads(string? pluginName) => new List<Payload>
    {
        new UIForegroundPayload(0), new TextPayload($"[{pluginName}] "), new UIForegroundPayload(548),
    };
}
