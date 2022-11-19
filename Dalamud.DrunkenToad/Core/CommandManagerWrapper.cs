using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dalamud.Game.Command;

namespace Dalamud.DrunkenToad.Core;

/// <summary>
/// Dalamud command manager wrapper to provide state.
/// https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Game/Command/CommandManager.cs.
/// </summary>
public class CommandManagerWrapper
{
    private readonly List<string> commandRegistry = new ();
    private readonly CommandManager commandManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandManagerWrapper"/> class.
    /// </summary>
    /// <param name="commandManager">dalamud command manager.</param>
    public CommandManagerWrapper(CommandManager commandManager)
    {
        this.commandManager = commandManager;
    }

    /// <summary>
    /// Add a command handler, which you can use to add your own custom commands to the in-game chat.
    /// </summary>
    /// <param name="command">The command to register.</param>
    /// <param name="handler">The action to invoke on command use.</param>
    /// <returns>If adding was successful.</returns>
    public bool RegisterCommand(string command, CommandInfo.HandlerDelegate handler)
    {
        var addedSuccessfully = this.commandManager.AddHandler(command, new CommandInfo(handler)
        {
            ShowInHelp = false,
        });
        if (addedSuccessfully) this.commandRegistry.Add(command);
        return addedSuccessfully;
    }

    /// <summary>
    /// Add a command handler, which you can use to add your own custom commands to the in-game chat.
    /// </summary>
    /// <param name="command">The command to register.</param>
    /// <param name="helpMessage">The help text to show in installer.</param>
    /// <param name="handler">The action to invoke on command use.</param>
    /// <returns>If adding was successful.</returns>
    public bool RegisterCommand(string command, string helpMessage, CommandInfo.HandlerDelegate handler)
    {
        var addedSuccessfully = this.commandManager.AddHandler(command, new CommandInfo(handler)
        {
            HelpMessage = helpMessage,
            ShowInHelp = true,
        });
        if (addedSuccessfully) this.commandRegistry.Add(command);
        return addedSuccessfully;
    }

    /// <summary>
    /// Gets a read-only list of all registered commands.
    /// </summary>
    /// <returns>list of registered commands.</returns>
    public ReadOnlyDictionary<string, CommandInfo> Commands()
    {
        return this.commandManager.Commands;
    }

    /// <summary>
    /// Dispose wrapper.
    /// </summary>
    public void Dispose()
    {
        foreach (var command in this.commandRegistry)
        {
            this.commandManager.RemoveHandler(command);
        }
    }
}
