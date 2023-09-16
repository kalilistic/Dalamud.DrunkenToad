namespace Dalamud.DrunkenToad.Core.Services;

using System.Collections.Generic;
using Extensions;
using Game;
using Game.ClientState.Objects;
using Game.ClientState.Objects.SubKinds;
using Models;

/// <summary>
/// Manages player character events, including addition, removal, and updates.
/// </summary>
public class PlayerEventDispatcher
{
    private readonly uint[] existingObjectIds = new uint[100];
    private readonly Framework gameFramework;
    private readonly ObjectTable objectCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerEventDispatcher" /> class.
    /// </summary>
    /// <param name="gameFramework">Dalamud Framework.</param>
    /// <param name="objectCollection">Dalamud ObjectTable.</param>
    public PlayerEventDispatcher(Framework gameFramework, ObjectTable objectCollection)
    {
        this.gameFramework = gameFramework;
        this.objectCollection = objectCollection;
    }

    /// <summary>
    /// Add Players Delegate.
    /// </summary>
    /// <param name="toadPlayers">Player objects with key properties from PlayerCharacter.</param>
    public delegate void DalamudAddPlayersDelegate(List<ToadPlayer> toadPlayers);

    /// <summary>
    /// Remove Players Delegate.
    /// </summary>
    /// <param name="playerIds">Player object ids.</param>
    public delegate void DalamudRemovePlayersDelegate(List<uint> playerIds);

    /// <summary>
    /// Update Players Delegate.
    /// </summary>
    /// <param name="toadPlayers">Player objects with key properties from PlayerCharacter.</param>
    public delegate void DalamudUpdatePlayersDelegate(List<ToadPlayer> toadPlayers);

    /// <summary>
    /// Add Player (fires when new player is added to object table).
    /// </summary>
    public event DalamudAddPlayersDelegate? AddPlayers;

    /// <summary>
    /// Remove Player (fires when player is removed from object table).
    /// </summary>
    public event DalamudRemovePlayersDelegate? RemovePlayers;

    /// <summary>
    /// Fires when a player character is updated.
    /// </summary>
    public event DalamudUpdatePlayersDelegate? UpdatePlayers;

    /// <summary>
    /// Starts the event dispatcher.
    /// </summary>
    public void Start() => this.gameFramework.Update += this.OnFrameworkUpdate;

    /// <summary>
    /// Retrieve player by object id.
    /// </summary>
    /// <param name="id">object (actor) id.</param>
    /// <returns>player if exists.</returns>
    public ToadPlayer? GetPlayerById(uint id)
    {
        for (var i = 0; i < this.existingObjectIds.Length; i++)
        {
            if (this.existingObjectIds[i] == id)
            {
                if (this.objectCollection[i * 2] is PlayerCharacter playerCharacter)
                {
                    return MapToadPlayer(playerCharacter);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Dispose manager.
    /// </summary>
    public void Dispose() => this.gameFramework.Update -= this.OnFrameworkUpdate;

    private static ToadPlayer MapToadPlayer(PlayerCharacter character) => new ()
    {
        Id = character.ObjectId,
        Name = character.Name.ToString(),
        HomeWorld = character.HomeWorld.Id,
        ClassJob = character.ClassJob.Id,
        Level = character.Level,
        Customize = character.Customize,
        CompanyTag = character.CompanyTag.ToString(),
        IsLocalPlayer = false,
        IsDead = character.IsDead,
    };

    private void CheckForUpdates(int index, ICollection<ToadPlayer> updatedPlayers)
    {
        if (this.objectCollection[index * 2] is PlayerCharacter character)
        {
            var toadPlayer = MapToadPlayer(character);
            if (toadPlayer.ClassJob != character.ClassJob.Id || toadPlayer.IsDead != character.IsDead)
            {
                updatedPlayers.Add(toadPlayer);
            }
        }
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        var addedPlayers = new List<ToadPlayer>();
        var removedPlayers = new List<uint>();
        var updatedPlayers = new List<ToadPlayer>();

        for (var i = 2; i < 200; i += 2)
        {
            var index = i / 2;
            var currentObjectId = this.objectCollection[i]?.ObjectId ?? 0;
            var existingId = this.existingObjectIds[index];

            // check if any update
            if (currentObjectId == existingId)
            {
                this.CheckForUpdates(index, updatedPlayers);
                continue;
            }

            // check if removed
            if (this.objectCollection[i] == null)
            {
                if (existingId != 0)
                {
                    removedPlayers.Add(existingId);
                    this.existingObjectIds[i / 2] = 0;
                }

                continue;
            }

            PlayerCharacter character;
            if (this.objectCollection[i].IsValidPlayerCharacter())
            {
                character = (this.objectCollection[i] as PlayerCharacter) !;
            }
            else
            {
                continue;
            }

            // check if new
            if (existingId == 0)
            {
                addedPlayers.Add(MapToadPlayer(character));
                this.existingObjectIds[i / 2] = currentObjectId;
                continue;
            }

            // otherwise replaced
            removedPlayers.Add(existingId);
            addedPlayers.Add(MapToadPlayer(character));
            this.existingObjectIds[i / 2] = currentObjectId;
        }

        if (removedPlayers.Count > 0)
        {
            this.RemovePlayers?.Invoke(removedPlayers);
        }

        if (addedPlayers.Count > 0)
        {
            this.AddPlayers?.Invoke(addedPlayers);
        }

        if (updatedPlayers.Count > 0)
        {
            this.UpdatePlayers?.Invoke(updatedPlayers);
        }
    }
}
