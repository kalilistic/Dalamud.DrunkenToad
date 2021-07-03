using System;
using System.Linq;

using Dalamud.Game.ClientState.Actors.Types;
using Dalamud.Plugin;

namespace Dalamud.DrunkenToad
{
    /// <summary>
    /// Gets Actors.
    /// </summary>
    public class Actors
    {
        private readonly DalamudPluginInterface pluginInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="Actors"/> class.
        /// </summary>
        /// <param name="pluginInterface">dalamud plugin interface.</param>
        public Actors(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        /// <summary>
        /// Get actor by id.
        /// </summary>
        /// <param name="actorId">actor id.</param>
        /// <returns>actor.</returns>
        public Actor? Actor(int actorId)
        {
            try
            {
                return this.pluginInterface.ClientState.Actors.ToList().FirstOrDefault(actor => actor.ActorId == actorId) ?? throw new InvalidOperationException();
            }
            catch
            {
                Logger.LogInfo("Failed to find actor by id " + actorId);
                return null;
            }
        }
    }
}
