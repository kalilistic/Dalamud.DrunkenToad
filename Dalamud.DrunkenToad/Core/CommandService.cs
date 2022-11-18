using System.Collections.Generic;
using Dalamud.Game.Command;

namespace Dalamud.DrunkenToad.Core;

/// <summary>
/// Dalamud command manager.
/// </summary>
public static class CommandService
{
    private static readonly List<string> CommandRegistry = new ();

    public static void RegisterCommand(string command, CommandInfo.HandlerDelegate handler)
    {
        DalamudContext.Commands.AddHandler(command, new CommandInfo(handler)
        {
            ShowInHelp = false,
        });
        CommandRegistry.Add(command);
    }

    public static void RegisterCommand(string command, string helpMessage, CommandInfo.HandlerDelegate handler)
    {
        DalamudContext.Commands.AddHandler(command, new CommandInfo(handler)
        {
            HelpMessage = helpMessage,
            ShowInHelp = true,
        });
        CommandRegistry.Add(command);
    }

    public static void Dispose()
    {
        foreach (var command in CommandRegistry)
        {
            DalamudContext.Commands.RemoveHandler(command);
        }
    }
}
