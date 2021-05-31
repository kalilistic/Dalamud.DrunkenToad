using System;

using Dalamud.Plugin;

namespace DalamudPluginCommon
{
    /// <summary>
    /// Client state.
    /// </summary>
    public class ClientState
    {
        private readonly DalamudPluginInterface pluginInterface;
        private bool isLoggedIn;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientState"/> class.
        /// </summary>
        /// <param name="pluginInterface">Dalamud plugin interface.</param>
        public ClientState(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
            this.AddEventHandlers();
            this.UpdateLoggedInState();
            this.Condition = new Condition(pluginInterface);
            this.LocalPlayer = new LocalPlayer(pluginInterface);
            this.KeyState = new KeyState(pluginInterface);
            this.Actors = new Actors(pluginInterface);
            this.Targets = new Targets(pluginInterface, this.Actors);
        }

        /// <summary>
        /// TerritoryChanged delegate.
        /// </summary>
        /// <param name="territoryType">territory type.</param>
        public delegate void TerritoryChangedEventHandler(ushort territoryType);

        /// <summary>
        /// TerritoryChanged Event.
        /// </summary>
        public event TerritoryChangedEventHandler OnTerritoryChanged = null!;

        /// <summary>
        /// Gets condition.
        /// </summary>
        public Condition Condition { get; }

        /// <summary>
        /// Gets key state.
        /// </summary>
        public KeyState KeyState { get; }

        /// <summary>
        /// Gets actors.
        /// </summary>
        public Actors Actors { get; }

        /// <summary>
        /// Gets targets.
        /// </summary>
        public Targets Targets { get; }

        /// <summary>
        /// Gets local player.
        /// </summary>
        public LocalPlayer LocalPlayer { get; }

        /// <summary>
        /// Dispose client state class.
        /// </summary>
        public void Dispose()
        {
            this.RemoveEventHandlers();
        }

        /// <summary>
        /// Returns client language.
        /// </summary>
        /// <returns>client language.</returns>
        public ushort ClientLanguage()
        {
            try
            {
                return (ushort)this.pluginInterface.ClientState.ClientLanguage;
            }
            catch
            {
                return (ushort)Dalamud.ClientLanguage.English;
            }
        }

        /// <summary>
        /// Add event handlers.
        /// </summary>
        public void AddEventHandlers()
        {
            this.pluginInterface.ClientState.OnLogin += this.ClientStateOnLogin;
            this.pluginInterface.ClientState.OnLogout += this.ClientStateOnLogout;
            this.pluginInterface.ClientState.TerritoryChanged += this.TerritoryChanged;
        }

        /// <summary>
        /// Remove event handlers.
        /// </summary>
        public void RemoveEventHandlers()
        {
            this.pluginInterface.ClientState.OnLogin -= this.ClientStateOnLogin;
            this.pluginInterface.ClientState.OnLogout -= this.ClientStateOnLogout;
        }

        /// <summary>
        /// Indicator if character is logged in.
        /// </summary>
        /// <returns>logged in state.</returns>
        public bool IsLoggedIn()
        {
            return this.isLoggedIn;
        }

        /// <summary>
        /// Set logged in state.
        /// </summary>
        public void UpdateLoggedInState()
        {
            if (this.pluginInterface.Data.IsDataReady && this.pluginInterface.ClientState.LocalPlayer != null)
            {
                this.isLoggedIn = true;
            }
        }

        /// <summary>
        /// Gets territory type.
        /// </summary>
        /// <returns>territory type.</returns>
        public uint TerritoryType()
        {
            try
            {
                return this.pluginInterface.ClientState.TerritoryType;
            }
            catch
            {
                Logger.LogInfo("TerritoryType is not available.");
                return 0;
            }
        }

        private void ClientStateOnLogout(object sender, EventArgs e)
        {
            this.isLoggedIn = false;
        }

        private void ClientStateOnLogin(object sender, EventArgs e)
        {
            this.isLoggedIn = true;
        }

        private void TerritoryChanged(object sender, ushort e)
        {
            this.OnTerritoryChanged(e);
        }
    }
}
