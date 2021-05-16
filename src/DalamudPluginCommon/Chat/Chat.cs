using System;
using System.Collections.Generic;
using System.Reflection;

using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin;

namespace DalamudPluginCommon
{
    /// <summary>
    /// Get plugin data.
    /// </summary>
    public class Chat
    {
        private readonly DalamudPluginInterface pluginInterface;
        private readonly string pluginName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Chat"/> class.
        /// </summary>
        /// <param name="pluginName">name of the plugin.</param>
        /// <param name="pluginInterface">dalamud plugin interface.</param>
        public Chat(string pluginName, DalamudPluginInterface pluginInterface)
        {
            this.pluginName = pluginName;
            this.pluginInterface = pluginInterface;
        }

        /// <summary>
        /// Print message with plugin name in default channel.
        /// </summary>
        /// <param name="message">print notice chat message.</param>
        public void Print(string message)
        {
            var chatType = this.DalamudChatType();
            this.Print(message, chatType);
        }

        /// <summary>
        /// Print message with plugin name in default channel.
        /// </summary>
        /// <param name="payloadList">list of chat payloads.</param>
        public void Print(List<Payload> payloadList)
        {
            var payload = new SeString(payloadList);
            this.pluginInterface.Framework.Gui.Chat.Print(payload);
        }

        /// <summary>
        /// Print message with plugin name in notice channel.
        /// </summary>
        /// <param name="message">print notice chat message.</param>
        public void PrintNotice(string message)
        {
            const XivChatType chatType = XivChatType.Notice;
            this.Print(message, chatType);
        }

        /// <summary>
        /// Print message with plugin name in target channel.
        /// </summary>
        /// <param name="message">print notice chat message.</param>
        /// <param name="chatType">chat type to use.</param>
        public void Print(string message, XivChatType chatType)
        {
            var seString = new SeString(new List<Payload>
            {
                new UIForegroundPayload(this.pluginInterface.Data, 0),
                new TextPayload($"[{this.pluginName}] "),
                new UIForegroundPayload(this.pluginInterface.Data, 548),
                new TextPayload(message),
                new UIForegroundPayload(this.pluginInterface.Data, 0),
            });
            this.pluginInterface.Framework.Gui.Chat.PrintChat(new XivChatEntry
            {
                MessageBytes = seString.Encode(),
                Type = chatType,
            });
        }

        private XivChatType DalamudChatType()
        {
            try
            {
                var dalamud = (Dalamud.Dalamud)this.pluginInterface.GetType().GetField("dalamud", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(this.pluginInterface) !;
                var config = dalamud.GetType().GetProperty("Configuration", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(dalamud);
                var chatType = (XivChatType)(config?.GetType().GetRuntimeProperty("GeneralChatType").GetValue(config) ?? throw new InvalidOperationException());
                return chatType;
            }
            catch (Exception)
            {
                return XivChatType.Debug;
            }
        }
    }
}