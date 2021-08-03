using System.Linq;

using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;

namespace Dalamud.DrunkenToad
{
    /// <summary>
    /// Gets local player data.
    /// </summary>
    public class LocalPlayer
    {
        private readonly DalamudPluginInterface pluginInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalPlayer"/> class.
        /// </summary>
        /// <param name="pluginInterface">dalamud plugin interface.</param>
        public LocalPlayer(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        /// <summary>
        /// Get local player home world id.
        /// </summary>
        /// <returns>local player home world id.</returns>
        public uint HomeWorldId()
        {
            try
            {
                return this.pluginInterface.ClientState.LocalPlayer.HomeWorld.Id;
            }
            catch
            {
                Logger.LogDebug("LocalPlayer HomeWorldId is not available.");
                return 0;
            }
        }

        /// <summary>
        /// Gets data center id.
        /// </summary>
        /// <returns>data center id.</returns>
        public uint DataCenterId()
        {
            try
            {
                return this.pluginInterface.Data.GetExcelSheet<World>().First(world => world.RowId == this.pluginInterface.ClientState.LocalPlayer.HomeWorld.Id)
                           .DataCenter.Value.RowId;
            }
            catch
            {
                Logger.LogDebug("DataCenterId is not available.");
                return 0;
            }
        }

        /// <summary>
        /// Returns local player home world id.
        /// </summary>
        /// <returns>local player home world id.</returns>
        public string Name()
        {
            try
            {
                return this.pluginInterface.ClientState.LocalPlayer.Name;
            }
            catch
            {
                Logger.LogDebug("LocalPlayer Name is not available.");
                return string.Empty;
            }
        }
    }
}
