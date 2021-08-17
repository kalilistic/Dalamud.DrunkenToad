using Dalamud.Plugin;

namespace Dalamud.DrunkenToad
{
    /// <summary>
    /// Gets Targets.
    /// </summary>
    public class Targets
    {
        private readonly DalamudPluginInterface pluginInterface;
        private readonly Actors actors;
        private int previousFocusTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="Targets"/> class.
        /// </summary>
        /// <param name="pluginInterface">dalamud plugin interface.</param>
        /// <param name="actors">actors.</param>
        public Targets(DalamudPluginInterface pluginInterface, Actors actors)
        {
            this.pluginInterface = pluginInterface;
            this.actors = actors;
        }

            /// <summary>
        /// Set current target.
        /// </summary>
        /// <param name="actorId">actor id.</param>
        public void SetCurrentTarget(int actorId)
        {
            try
            {
                if (actorId == 0)
                {
                    return;
                }

                var actor = this.actors.Actor(actorId);
                if (actor == null)
                {
                    return;
                }

                this.pluginInterface.ClientState.Targets.SetCurrentTarget(actor);
            }
            catch
            {
                Logger.LogVerbose("Failed to target actor with id " + actorId);
            }
        }

        /// <summary>
        /// Revert focus target.
        /// </summary>
        public void RevertFocusTarget()
        {
            try
            {
                if (this.previousFocusTarget == 0)
                {
                    return;
                }

                var actor = this.actors.Actor(this.previousFocusTarget);
                if (actor == null)
                {
                    return;
                }

                this.pluginInterface.ClientState.Targets.SetFocusTarget(actor);
            }
            catch
            {
                Logger.LogVerbose("Failed to focus target actor with id " + this.previousFocusTarget);
            }
        }

        /// <summary>
        /// Set focus target.
        /// </summary>
        /// <param name="actorId">actor id.</param>
        public void SetFocusTarget(int actorId)
        {
            try
            {
                if (actorId == 0)
                {
                    return;
                }

                var actor = this.actors.Actor(actorId);
                if (actor == null)
                {
                    return;
                }

                if (this.pluginInterface.ClientState.Targets.FocusTarget != null)
                {
                    this.previousFocusTarget = this.pluginInterface.ClientState.Targets.FocusTarget.ActorId;
                }

                this.pluginInterface.ClientState.Targets.SetFocusTarget(actor);
            }
            catch
            {
                Logger.LogVerbose("Failed to target actor with id " + actorId);
            }
        }
    }
}
