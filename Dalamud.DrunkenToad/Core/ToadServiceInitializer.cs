namespace Dalamud.DrunkenToad.Core;

using Game;
using Game.ClientState.Objects;
using Gui;
using IoC;
using Microsoft.Extensions.DependencyInjection;
using Plugin;
using Plugin.Services;
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
    public IAetheryteList AetheryteCollection { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of buddies in your squadron or trust party.
    /// </summary>
    [PluginService]
    public IBuddyList BuddyCollection { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides access to player state conditions.
    /// </summary>
    [PluginService]
    public ICondition ConditionHandler { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of currently available Fate events.
    /// </summary>
    [PluginService]
    public IFateTable FateCollection { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides access to the gamepad state.
    /// </summary>
    [PluginService]
    public IGamepadState GamepadStateHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides access to Job gauge data.
    /// </summary>
    [PluginService]
    public IJobGauges JobGaugeHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides a wrapper around the game's key state buffer, containing the pressed state for all
    /// keyboard keys, indexed by virtual vkCode.
    /// </summary>
    [PluginService]
    public IKeyState KeyStateHandler { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of currently spawned FFXIV game objects.
    /// </summary>
    [PluginService]
    public IObjectTable ObjectCollection { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of player targets.
    /// </summary>
    [PluginService]
    public ITargetManager TargetManager { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of actors in your party or alliance.
    /// </summary>
    [PluginService]
    public IPartyList PartyCollection { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides access to the current state of the game client at the time of access.
    /// </summary>
    [PluginService]
    public IClientState ClientStateHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides an interface for interacting with the server info bar.
    /// </summary>
    [PluginService]
    public IDtrBar ServerInfoBarHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that facilitates interaction with and creation of native in-game "fly text" windows.
    /// </summary>
    [PluginService]
    public IFlyTextGui FlyTextGuiHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that handles interaction with the native PartyFinder window.
    /// </summary>
    [PluginService]
    public IPartyFinderGui PartyFinderGuiHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that facilitates interaction with and creation of native toast windows.
    /// </summary>
    [PluginService]
    public IToastGui ToastGuiHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that handles interaction with the native chat UI.
    /// </summary>
    [PluginService]
    public IChatGui ChatGuiHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that handles various aspects of the in-game UI.
    /// </summary>
    [PluginService]
    public IGameGui GameGuiHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that provides functionality for creating C strings using native game methods.
    /// </summary>
    [PluginService]
    public ILibcFunction LibcFunctionHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that handles interaction with game network events.
    /// </summary>
    [PluginService]
    public IGameNetwork GameNetworkHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that represents the game client's Framework, providing access to various subsystems.
    /// </summary>
    [PluginService]
    public IFramework GameFramework { get; private set; } = null!;

    /// <summary>
    /// Gets service that facilitates searching for memory signatures in a given ProcessModule.
    /// </summary>
    [PluginService]
    public ISigScanner SignatureScanner { get; private set; } = null!;

    /// <summary>
    /// Gets service that manages elements in the title screen menu.
    /// </summary>
    [PluginService]
    public ITitleScreenMenu TitleScreenMenuHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that represents the state of the currently occupied duty.
    /// </summary>
    [PluginService]
    public IDutyState DutyStateHandler { get; private set; } = null!;

    /// <summary>
    /// Gets service that acts as an interface to various objects required for interaction with Dalamud and the game.
    /// </summary>
    [PluginService]
    public DalamudPluginInterface PluginInterface { get; private set; } = null!;

    /// <summary>
    /// Gets or sets service that manages the creation and handling of hooks for function call interception and modification.
    /// </summary>
    [PluginService]
    public IGameInteropProvider HookManager { get; set; } = null!;

    /// <summary>
    /// Gets or sets service that provides access to registered in-game slash commands.
    /// </summary>
    [PluginService]
    private ICommandManager CommandManager { get; set; } = null!;

    /// <summary>
    /// Gets or sets service that provides access to game data for Dalamud and plugins.
    /// </summary>
    [PluginService]
    private IDataManager DalamudDataManager { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of player targets.
    /// </summary>
    [PluginService]
    private ITargetManager DalamudTargetManager { get; set; } = null!;

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
        services.AddSingleton(this.GameFramework);
        services.AddSingleton(this.SignatureScanner);
        services.AddSingleton(this.TitleScreenMenuHandler);
        services.AddSingleton(this.DutyStateHandler);
        services.AddSingleton(this.PluginInterface);
        services.AddSingleton(this.HookManager);
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
        services.AddSingleton<SocialListHandler>();
        services.AddSingleton<PluginLocalization>();
        services.AddSingleton<WindowManager>();
    }
}
