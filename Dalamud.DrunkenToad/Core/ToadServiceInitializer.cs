namespace Dalamud.DrunkenToad.Core;

using Data;
using Game;
using Game.ClientState;
using Game.ClientState.Aetherytes;
using Game.ClientState.Buddy;
using Game.ClientState.Conditions;
using Game.ClientState.Fates;
using Game.ClientState.GamePad;
using Game.ClientState.JobGauge;
using Game.ClientState.Keys;
using Game.ClientState.Objects;
using Game.ClientState.Party;
using Game.Command;
using Game.DutyState;
using Game.Gui;
using Game.Gui.Dtr;
using Game.Gui.FlyText;
using Game.Gui.PartyFinder;
using Game.Gui.Toast;
using Game.Libc;
using Game.Network;
using Gui;
using Interface;
using IoC;
using Microsoft.Extensions.DependencyInjection;
using Plugin;
using Services;
using PluginLocalization = Loc.Localization;

/// <summary>
/// Dalamud and custom service initializer.
/// </summary>
public class ToadServiceInitializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToadServiceInitializer" /> class.
    /// </summary>
    /// <param name="pluginInterface">plugin interface.</param>
    public ToadServiceInitializer(DalamudPluginInterface pluginInterface) => pluginInterface.Inject(this);

    /// <summary>
    /// Gets the collection of available Aetherytes in the Teleport window.
    /// </summary>
    [PluginService]
    public AetheryteList AetheryteCollection { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of buddies in your squadron or trust party.
    /// </summary>
    [PluginService]
    public BuddyList BuddyCollection { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides access to player state conditions.
    /// </summary>
    [PluginService]
    public Condition ConditionHandler { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of currently available Fate events.
    /// </summary>
    [PluginService]
    public FateTable FateCollection { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides access to the gamepad state.
    /// </summary>
    [PluginService]
    public GamepadState GamepadStateHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides access to Job gauge data.
    /// </summary>
    [PluginService]
    public JobGauges JobGaugeHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides a wrapper around the game's key state buffer, containing the pressed state for all
    /// keyboard keys, indexed by virtual vkCode.
    /// </summary>
    [PluginService]
    public KeyState KeyStateHandler { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of currently spawned FFXIV game objects.
    /// </summary>
    [PluginService]
    public ObjectTable ObjectCollection { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of player targets.
    /// </summary>
    [PluginService]
    public TargetManager TargetManager { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of actors in your party or alliance.
    /// </summary>
    [PluginService]
    public PartyList PartyCollection { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides access to the current state of the game client at the time of access.
    /// </summary>
    [PluginService]
    public ClientState ClientStateHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides an interface for interacting with the server info bar.
    /// </summary>
    [PluginService]
    public DtrBar ServerInfoBarHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that facilitates interaction with and creation of native in-game "fly text" windows.
    /// </summary>
    [PluginService]
    public FlyTextGui FlyTextGuiHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that handles interaction with the native PartyFinder window.
    /// </summary>
    [PluginService]
    public PartyFinderGui PartyFinderGuiHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that facilitates interaction with and creation of native toast windows.
    /// </summary>
    [PluginService]
    public ToastGui ToastGuiHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that handles interaction with the native chat UI.
    /// </summary>
    [PluginService]
    public ChatGui ChatGuiHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that handles various aspects of the in-game UI.
    /// </summary>
    [PluginService]
    public GameGui GameGuiHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides functionality for creating C strings using native game methods.
    /// </summary>
    [PluginService]
    public LibcFunction LibcFunctionHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that handles interaction with game network events.
    /// </summary>
    [PluginService]
    public GameNetwork GameNetworkHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides access to chat events and public helper functions.
    /// </summary>
    [PluginService]
    public ChatHandlers ChatHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that represents the game client's Framework, providing access to various subsystems.
    /// </summary>
    [PluginService]
    public Framework GameFramework { get; private set; } = null!;

    /// <summary>
    /// Gets service that facilitates searching for memory signatures in a given ProcessModule.
    /// </summary>
    [PluginService]
    public SigScanner SignatureScanner { get; private set; } = null!;

    /// <summary>
    /// Gets service that manages elements in the title screen menu.
    /// </summary>
    [PluginService]
    public TitleScreenMenu TitleScreenMenuHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that represents the state of the currently occupied duty.
    /// </summary>
    [PluginService]
    public DutyState DutyStateHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that acts as an interface to various objects required for interaction with Dalamud and the game.
    /// </summary>
    [PluginService]
    public DalamudPluginInterface PluginInterface { get; private set; } = null!;

    /// <summary>
    /// Gets or sets service that provides access to registered in-game slash commands.
    /// </summary>
    [PluginService]
    private CommandManager CommandManager { get; set; } = null!;

    /// <summary>
    /// Gets or sets service that provides access to game data for Dalamud and plugins.
    /// </summary>
    [PluginService]
    private DataManager DalamudDataManager { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of player targets.
    /// </summary>
    [PluginService]
    private TargetManager DalamudTargetManager { get; set; } = null!;

    /// <summary>
    /// Add dalamud services.
    /// </summary>
    /// <param name="services">service collection.</param>
    public void AddDalamudServices(IServiceCollection services)
    {
        services.AddSingleton(this.AetheryteCollection);
        services.AddSingleton(this.BuddyCollection);
        services.AddSingleton(this.ConditionHandler);
        services.AddSingleton(this.FateCollection);
        services.AddSingleton(this.GamepadStateHandler);
        services.AddSingleton(this.JobGaugeHandler);
        services.AddSingleton(this.KeyStateHandler);
        services.AddSingleton(this.ObjectCollection);
        services.AddSingleton(this.TargetManager);
        services.AddSingleton(this.PartyCollection);
        services.AddSingleton(this.ClientStateHandler);
        services.AddSingleton(this.ServerInfoBarHandler);
        services.AddSingleton(this.FlyTextGuiHandler);
        services.AddSingleton(this.PartyFinderGuiHandler);
        services.AddSingleton(this.ToastGuiHandler);
        services.AddSingleton(this.ChatGuiHandler);
        services.AddSingleton(this.GameGuiHandler);
        services.AddSingleton(this.LibcFunctionHandler);
        services.AddSingleton(this.GameNetworkHandler);
        services.AddSingleton(this.ChatHandler);
        services.AddSingleton(this.GameFramework);
        services.AddSingleton(this.SignatureScanner);
        services.AddSingleton(this.TitleScreenMenuHandler);
        services.AddSingleton(this.DutyStateHandler);
        services.AddSingleton(this.PluginInterface);
        services.AddSingleton(this.CommandManager);

        services.AddSingleton(this.DalamudDataManager);
        services.AddSingleton(this.DalamudTargetManager);
    }

    /// <summary>
    /// Add custom services.
    /// </summary>
    /// <param name="services">service collection.</param>
    public void AddToadServices(IServiceCollection services)
    {
        services.AddSingleton<DataManagerEx>();
        services.AddSingleton<TargetManagerEx>();
        services.AddSingleton<PlayerLocationManager>();
        services.AddSingleton<PlayerEventDispatcher>();
        services.AddSingleton<PluginLocalization>();
        services.AddSingleton<WindowManager>();
    }
}
