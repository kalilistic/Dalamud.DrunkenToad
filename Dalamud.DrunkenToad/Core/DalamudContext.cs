namespace Dalamud.DrunkenToad.Core;

using System;
using Game;
using Game.ClientState.Objects;
using Gui;
using IoC;
using Plugin;
using Plugin.Services;
using Services;
using PluginLocalization = Loc.Localization;

/// <summary>
/// Dalamud server wrapper.
/// </summary>
public class DalamudContext
{
    /// <summary>
    /// Gets service that provides access to the localization class for handling localized text.
    /// </summary>
    public static PluginLocalization LocManager { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides a modified Dalamud WindowManager using abstracted window implementations for a
    /// simplified ImGui windowing experience.
    /// </summary>
    public static WindowManager WindowManager { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides a wrapper around the Dalamud DataManager for accessing pre-loaded game data.
    /// </summary>
    public static DataManagerEx DataManager { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of player targets with extension methods for interacting with them.
    /// </summary>
    public static TargetManagerEx TargetManager { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides access to the PlayerCharacterManager for interacting with the game object table.
    /// </summary>
    public static PlayerEventDispatcher PlayerEventDispatcher { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides an extended location data API derived from territory change events.
    /// </summary>
    public static PlayerLocationManager PlayerLocationManager { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of available Aetherytes in the Teleport window.
    /// </summary>
    [PluginService]
    public static IAetheryteList AetheryteCollection { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of buddies in your squadron or trust party.
    /// </summary>
    [PluginService]
    public static IBuddyList BuddyCollection { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides a wrapper around the Dalamud CommandManager for managing command states.
    /// </summary>
    [PluginService]
    public static ICommandManager CommandManager { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides access to player state conditions.
    /// </summary>
    [PluginService]
    public static ICondition ConditionHandler { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of currently available Fate events.
    /// </summary>
    [PluginService]
    public static IFateTable FateCollection { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides access to the gamepad state.
    /// </summary>
    [PluginService]
    public static IGamepadState GamepadStateHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides access to Job gauge data.
    /// </summary>
    [PluginService]
    public static IJobGauges JobGaugeHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides a wrapper around the game's key state buffer, containing the pressed state for all
    /// keyboard keys, indexed by virtual vkCode.
    /// </summary>
    [PluginService]
    public static IKeyState KeyStateHandler { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of currently spawned FFXIV game objects.
    /// </summary>
    [PluginService]
    public static IObjectTable ObjectCollection { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of actors in your party or alliance.
    /// </summary>
    [PluginService]
    public static IPartyList PartyCollection { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides access to the current state of the game client at the time of access.
    /// </summary>
    [PluginService]
    public static IClientState ClientStateHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides an interface for interacting with the server info bar.
    /// </summary>
    [PluginService]
    public static IDtrBar ServerInfoBarHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that facilitates interaction with and creation of native in-game "fly text" windows.
    /// </summary>
    [PluginService]
    public static IFlyTextGui FlyTextGuiHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that handles interaction with the native PartyFinder window.
    /// </summary>
    [PluginService]
    public static IPartyFinderGui PartyFinderGuiHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that facilitates interaction with and creation of native toast windows.
    /// </summary>
    [PluginService]
    public static IToastGui ToastGuiHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that handles interaction with the native chat UI.
    /// </summary>
    [PluginService]
    public static IChatGui ChatGuiHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that handles various aspects of the in-game UI.
    /// </summary>
    [PluginService]
    public static IGameGui GameGuiHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides functionality for creating C strings using native game methods.
    /// </summary>
    [PluginService]
    public static ILibcFunction LibcFunctionHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that handles interaction with game network events.
    /// </summary>
    [PluginService]
    public static IGameNetwork GameNetworkHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that represents the game client's Framework, providing access to various subsystems.
    /// </summary>
    [PluginService]
    public static IFramework GameFramework { get; private set; } = null!;

    /// <summary>
    /// Gets service that facilitates searching for memory signatures in a given ProcessModule.
    /// </summary>
    [PluginService]
    public static ISigScanner SignatureScanner { get; private set; } = null!;

    /// <summary>
    /// Gets service that manages elements in the title screen menu.
    /// </summary>
    [PluginService]
    public static ITitleScreenMenu TitleScreenMenuHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that represents the state of the currently occupied duty.
    /// </summary>
    [PluginService]
    public static IDutyState DutyStateHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that acts as an interface to various objects required for interaction with Dalamud and the game.
    /// </summary>
    [PluginService]
    public static DalamudPluginInterface PluginInterface { get; private set; } = null!;

    /// <summary>
    /// Gets or sets service that provides access to the Dalamud plugin logger.
    /// </summary>
    [PluginService]
    public static IPluginLog PluginLog { get; set; } = null!;

    /// <summary>
    /// Gets or sets service that manages the creation and handling of hooks for function call interception and modification.
    /// </summary>
    [PluginService]
    public static IGameInteropProvider HookManager { get; set; } = null!;

    /// <summary>
    /// Gets or sets service that provides access to game data for Dalamud and plugins.
    /// </summary>
    [PluginService]
    private static IDataManager DalamudDataManager { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of player targets.
    /// </summary>
    [PluginService]
    private static ITargetManager DalamudTargetManager { get; set; } = null!;

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
            DataManager = new DataManagerEx(DalamudDataManager, pluginInterface);
            TargetManager = new TargetManagerEx(DalamudTargetManager);
            PlayerLocationManager = new PlayerLocationManager(ClientStateHandler, DataManager);
            PlayerEventDispatcher = new PlayerEventDispatcher(GameFramework, ObjectCollection);
            LocManager = new PluginLocalization(pluginInterface);
            WindowManager = new WindowManager(pluginInterface);
            ToadGui.Initialize(LocManager, DataManager);
            return true;
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Failed to initialize dalamud context");
            return false;
        }
    }

    /// <summary>
    /// Dispose custom services.
    /// </summary>
    public static void Dispose()
    {
        try
        {
            WindowManager.Dispose();
            LocManager.Dispose();
            PlayerEventDispatcher.Dispose();
            PlayerLocationManager.Dispose();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Failed to dispose DalamudContext properly.");
        }
    }
}
