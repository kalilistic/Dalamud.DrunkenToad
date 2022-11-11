using System.Collections.Generic;
using System.Reflection;

using Dalamud.Game.Gui;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace Dalamud.DrunkenToad;

/// <summary>
/// Dalamud Chat GUI extensions.
/// </summary>
public static class ChatGuiExtensions
{
    /// <summary>
    /// Print message with plugin name to dalamud default channel.
    /// </summary>
    /// <param name="value">chat gui service.</param>
    /// <param name="message">chat message.</param>
    public static void PluginPrint(this ChatGui value, string message)
    {
        value.Print(BuildSeString(Assembly.GetCallingAssembly().GetName().Name, message));
    }

    /// <summary>
    /// Print payload with plugin name to dalamud default channel.
    /// </summary>
    /// <param name="value">chat gui service.</param>
    /// <param name="payloads">list of payloads.</param>
    public static void PluginPrint(this ChatGui value, IEnumerable<Payload> payloads)
    {
        value.Print(BuildSeString(Assembly.GetCallingAssembly().GetName().Name, payloads));
    }

    /// <summary>
    /// Print message with plugin name to specified channel.
    /// </summary>
    /// <param name="value">chat gui service.</param>
    /// <param name="message">chat message.</param>
    /// <param name="chatType">chat type to use.</param>
    public static void PluginPrint(this ChatGui value, string message, XivChatType chatType)
    {
        value.PrintChat(new XivChatEntry
        {
            Message = BuildSeString(Assembly.GetCallingAssembly().GetName().Name, message),
            Type = chatType,
        });
    }

    /// <summary>
    /// Print message with plugin name to specified channel.
    /// </summary>
    /// <param name="value">chat gui service.</param>
    /// <param name="payloads">list of payloads.</param>
    /// <param name="chatType">chat type to use.</param>
    public static void PluginPrint(this ChatGui value, IEnumerable<Payload> payloads, XivChatType chatType)
    {
        value.PrintChat(new XivChatEntry
        {
            Message = BuildSeString(Assembly.GetCallingAssembly().GetName().Name, payloads),
            Type = chatType,
        });
    }

    /// <summary>
    /// Print message with plugin name to notice channel.
    /// </summary>
    /// <param name="value">chat gui service.</param>
    /// <param name="message">chat message.</param>
    public static void PluginPrintNotice(this ChatGui value, string message)
    {
        value.PrintChat(new XivChatEntry
        {
            Message = BuildSeString(Assembly.GetCallingAssembly().GetName().Name, message),
            Type = XivChatType.Notice,
        });
    }

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
        var seString = new SeString(new List<Payload>
        {
            new UIForegroundPayload(0),
            new TextPayload($"[{Assembly.GetCallingAssembly().GetName().Name}] "),
            new UIForegroundPayload(548),
            new PlayerPayload(playerName, worldId),
            new TextPayload(" " + message),
            new UIForegroundPayload(0),
        });
        value.PrintChat(new XivChatEntry
        {
            Message = seString,
            Type = chatType,
        });
    }

    private static SeString BuildSeString(string? pluginName, string message)
    {
        return new (new List<Payload>
        {
            new UIForegroundPayload(0),
            new TextPayload($"[{pluginName}] "),
            new UIForegroundPayload(548),
            new TextPayload(message),
            new UIForegroundPayload(0),
        });
    }

    private static SeString BuildSeString(string? pluginName, IEnumerable<Payload> payloads)
    {
        var newPayloads = new List<Payload>
        {
            new UIForegroundPayload(0),
            new TextPayload($"[{pluginName}] "),
            new UIForegroundPayload(548),
        };
        newPayloads.AddRange(payloads);
        return new SeString(newPayloads);
    }
}
