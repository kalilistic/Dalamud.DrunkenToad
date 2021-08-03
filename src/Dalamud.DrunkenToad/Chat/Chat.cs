using System.Collections.Generic;

using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin;

namespace Dalamud.DrunkenToad
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
            var chatType = this.pluginInterface.GeneralChatType;
            this.Print(message, chatType);
        }

        /// <summary>
        /// Print multi line message with plugin name in default channel.
        /// </summary>
        /// <param name="messages">print notice chat message.</param>
        public void PrintNotice(IEnumerable<string> messages)
        {
            const XivChatType chatType = XivChatType.Notice;
            foreach (var message in messages)
            {
                this.Print(message, chatType);
            }
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

        /// <summary>
        /// Print chat with player and message.
        /// </summary>
        /// <param name="playerName">player name.</param>
        /// <param name="worldId">player home world id.</param>
        /// <param name="message">message.</param>
        /// <param name="chatType">target channel.</param>
        public void Print(string playerName, uint worldId, string message, XivChatType chatType)
        {
            var seString = new SeString(new List<Payload>
            {
                new UIForegroundPayload(this.pluginInterface.Data, 0),
                new TextPayload($"[{this.pluginName}] "),
                new UIForegroundPayload(this.pluginInterface.Data, 548),
                new PlayerPayload(this.pluginInterface.Data, playerName, worldId),
                new TextPayload(" " + message),
                new UIForegroundPayload(this.pluginInterface.Data, 0),
            });
            this.pluginInterface.Framework.Gui.Chat.PrintChat(new XivChatEntry
            {
                MessageBytes = seString.Encode(),
                Type = chatType,
            });
        }
    }
}