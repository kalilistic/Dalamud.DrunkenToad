using System;
using Dalamud.Data;
using Dalamud.DrunkenToad.ImGui;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Game.ClientState.Buddy;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Game.ClientState.GamePad;
using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Gui.FlyText;
using Dalamud.Game.Gui.PartyFinder;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Libc;
using Dalamud.Game.Network;
using Dalamud.Interface;
using Dalamud.IoC;
using Dalamud.Plugin;
using DalamudCommandManager = Dalamud.Game.Command.CommandManager;
using PluginConfiguration = FlexConfig.Configuration;
using PluginLocalization = Dalamud.Loc.Localization;

namespace Dalamud.DrunkenToad.Core;

/// <summary>
/// Dalamud server wrapper.
/// </summary>
public class DalamudContext
{
    /// <summary>
    /// Initialize dalamud context with dalamud and custom services.
    /// </summary>
    /// <param name="pluginInterface">dalamud plugin interface.</param>
    /// <returns>indicator if initialized successfully.</returns>
    public static bool Initialize(DalamudPluginInterface pluginInterface)
    {
        try
        {
            pluginInterface.Create<DalamudContext>();
            Commands = new CommandManager(DalamudCommandManager);
            Localization = new Loc.Localization(pluginInterface);
            PluginConfiguration = new PluginConfiguration(PluginInterface.ConfigFile.FullName);
            PluginConfiguration.Load();
            WindowManager = new WindowManager();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Dispose custom services.
    /// </summary>
    public static void Dispose()
    {
        WindowManager.Dispose();
        PluginConfiguration.Save();
        Localization.Dispose();
        Commands.Dispose();
    }

    /// <summary>
    /// Gets class for using FlexConfig's Configuration.
    /// </summary>
    public static PluginConfiguration PluginConfiguration { get; private set; } = null!;

    /// <summary>
    /// Gets localization class.
    /// </summary>
    public static PluginLocalization Localization { get; private set; } = null!;

    /// <summary>
    /// Gets modified dalamud window system to use abstracted windows.
    /// </summary>
    public static WindowManager WindowManager { get; private set; } = null!;

    /// <summary>
    /// Gets dalamud command manager wrapper to provide state.
    /// </summary>
    public static CommandManager Commands { get; private set; } = null!;

    /// <summary>
    /// Gets class providing game data for dalamud and plugins.
    /// </summary>
    [PluginService] public static DataManager GameData { get; private set; } = null!;

    /// <summary>
    /// Gets this collection represents the list of available Aetherytes in the Teleport window.
    /// </summary>
    [PluginService] public static AetheryteList Aetherytes { get; private set; } = null!;

    /// <summary>
    /// Gets collection representing the buddies present in your squadron or trust party.
    /// </summary>
    [PluginService] public static BuddyList Buddies { get; private set; } = null!;

    /// <summary>
    /// Gets class providing access to conditions (generally player state).
    /// </summary>
    [PluginService] public static Condition Conditions { get; private set; } = null!;

    /// <summary>
    /// Gets collection representing the currently available Fate events.
    /// </summary>
    [PluginService] public static FateTable Fates { get; private set; } = null!;

    /// <summary>
    /// Gets class providing access to gamepad state.
    /// Will block game's gamepad input if <see cref="F:ImGuiNET.ImGuiConfigFlags.NavEnableGamepad" /> is set.
    /// </summary>
    [PluginService] public static GamepadState GamepadState { get; private set; } = null!;

    /// <summary>
    /// Gets class providing access to Job gauge data.
    /// </summary>
    [PluginService] public static JobGauges JobGauges { get; private set; } = null!;

    /// <summary>
    /// Gets wrapper around the game keystate buffer, which contains the pressed state for all keyboard keys, indexed by virtual vkCode.
    /// </summary>
    [PluginService] public static KeyState KeyState { get; private set; } = null!;

    /// <summary>
    /// Gets collection representing the currently spawned FFXIV game objects.
    /// </summary>
    [PluginService] public static ObjectTable Objects { get; private set; } = null!;

    /// <summary>
    /// Gets collection representing targets for the player.
    /// </summary>
    [PluginService] public static TargetManager Targets { get; private set; } = null!;

    /// <summary>
    /// Gets collection representing the actors present in your party or alliance.
    /// </summary>
    [PluginService] public static PartyList Party { get; private set; } = null!;

    /// <summary>
    /// Gets this class represents the state of the game client at the time of access.
    /// </summary>
    [PluginService] public static ClientState ClientState { get; private set; } = null!;

    /// <summary>
    /// Gets class used to interface with the server info bar.
    /// </summary>
    [PluginService] public static DtrBar DtrBar { get; private set; } = null!;

    /// <summary>
    /// Gets class facilitating interacting with and creating native in-game "fly text".
    /// </summary>
    [PluginService] public static FlyTextGui FlyTexts { get; private set; } = null!;

    /// <summary>
    /// Gets class handling interacting the native PartyFinder window.
    /// </summary>
    [PluginService] public static PartyFinderGui PartyFinder { get; private set; } = null!;

    /// <summary>
    /// Gets class facilitating interacting with and creating native toast windows.
    /// </summary>
    [PluginService] public static ToastGui ToastGui { get; private set; } = null!;

    /// <summary>
    /// Gets class handling interacting with the native chat UI.
    /// </summary>
    [PluginService] public static ChatGui ChatGui { get; private set; } = null!;

    /// <summary>Gets a class handling many aspects of the in-game UI.</summary>
    [PluginService] public static GameGui GameGui { get; private set; } = null!;

    /// <summary>
    /// Gets class handling creating c strings utilizing native game methods.
    /// </summary>
    [PluginService] public static LibcFunction LibC { get; private set; } = null!;

    /// <summary>
    /// Gets class handling interacting with game network events.
    /// </summary>
    [PluginService] public static GameNetwork Network { get; private set; } = null!;

    /// <summary>
    /// Gets chat events and public helper functions.
    /// </summary>
    [PluginService] public static ChatHandlers ChatHandlers { get; private set; } = null!;

    /// <summary>
    /// Gets class representing the Framework of the native game client and grants access to various subsystems.
    /// </summary>
    [PluginService] public static Framework Framework { get; private set; } = null!;

    /// <summary>
    /// Gets a SigScanner facilitates searching for memory signatures in a given ProcessModule.
    /// </summary>
    [PluginService] public static SigScanner SigScanner { get; private set; } = null!;

    /// <summary>
    /// Gets class responsible for managing elements in the title screen menu.
    /// </summary>
    [PluginService] public static TitleScreenMenu TitleScreenMenu { get; private set; } = null!;

    /// <summary>
    /// Gets class acting as an interface to various objects needed to interact with Dalamud and the game.
    /// </summary>
    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] private static DalamudCommandManager DalamudCommandManager { get; set; } = null!;
}
