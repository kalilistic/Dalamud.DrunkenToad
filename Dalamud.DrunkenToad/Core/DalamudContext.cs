using System;
using Dalamud.Data;
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
using Dalamud.Game.Command;
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

namespace Dalamud.DrunkenToad.Core;

/// <summary>
/// Dalamud server wrapper.
/// </summary>
public class DalamudContext
{
    public static bool Initialize(DalamudPluginInterface pluginInterface, bool addExtra = false)
    {
        try
        {
            pluginInterface.Create<DalamudContext>();
            if (addExtra)
            {
                Configuration = new DalamudConfiguration();
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    [PluginService] public static DataManager GameData { get; private set; } = null!;
    [PluginService] public static AetheryteList Aetherytes { get; private set; } = null!;
    [PluginService] public static BuddyList Buddies { get; private set; } = null!;
    [PluginService] public static Condition Conditions { get; private set; } = null!;
    [PluginService] public static FateTable Fates { get; private set; } = null!;
    [PluginService] public static GamepadState GamepadState { get; private set; } = null!;
    [PluginService] public static JobGauges Gauges { get; private set; } = null!;
    [PluginService] public static KeyState KeyState { get; private set; } = null!;
    [PluginService] public static ObjectTable Objects { get; private set; } = null!;
    [PluginService] public static TargetManager Targets { get; private set; } = null!;
    [PluginService] public static PartyList Party { get; private set; } = null!;
    [PluginService] public static ClientState ClientState { get; private set; } = null!;
    [PluginService] public static CommandManager Commands { get; private set; } = null!;
    [PluginService] public static DtrBar DtrBar { get; private set; } = null!;
    [PluginService] public static FlyTextGui FlyTexts { get; private set; } = null!;
    [PluginService] public static PartyFinderGui PartyFinder { get; private set; } = null!;
    [PluginService] public static ToastGui Toasts { get; private set; } = null!;
    [PluginService] public static ChatGui ChatGui { get; private set; } = null!;
    [PluginService] public static GameGui GameGui { get; private set; } = null!;
    [PluginService] public static LibcFunction LibC { get; private set; } = null!;
    [PluginService] public static GameNetwork Network { get; private set; } = null!;
    [PluginService] public static ChatHandlers ChatHandlers { get; private set; } = null!;
    [PluginService] public static Framework Framework { get; private set; } = null!;
    [PluginService] public static SigScanner SigScanner { get; private set; } = null!;
    [PluginService] public static TitleScreenMenu TitleScreenMenu { get; private set; } = null!;
    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;

    public static DalamudConfiguration Configuration { get; private set; } = null!;
}
