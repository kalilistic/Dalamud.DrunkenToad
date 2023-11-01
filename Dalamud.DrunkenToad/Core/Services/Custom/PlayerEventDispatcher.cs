namespace Dalamud.DrunkenToad.Core.Services;

using System;
using System.Collections.Generic;
using System.Threading;
using Extensions;
using Game.ClientState.Objects.SubKinds;
using Models;
using Plugin.Services;

/// <summary>
/// Manages player character events, including addition, removal, and updates.
/// </summary>
public class PlayerEventDispatcher : IDisposable
{
    private readonly uint[] existingObjectIds = new uint[100];
    private readonly IFramework gameFramework;
    private readonly IObjectTable objectCollection;
    private readonly ReaderWriterLockSlim locker = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerEventDispatcher" /> class.
    /// </summary>
    /// <param name="gameFramework">Dalamud Framework.</param>
    /// <param name="objectCollection">Dalamud ObjectTable.</param>
    public PlayerEventDispatcher(IFramework gameFramework, IObjectTable objectCollection)
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
        this.locker.EnterReadLock();
        try
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
        finally
        {
            this.locker.ExitReadLock();
        }
    }

    /// <summary>
    /// Retrieve player by name and world id.
    /// </summary>
    /// <param name="name">player name.</param>
    /// <param name="worldId">player world id.</param>
    /// <returns>player if exists.</returns>
    public ToadPlayer? GetPlayerByNameAndWorldId(string name, uint worldId)
    {
        this.locker.EnterReadLock();
        try
        {
            foreach (var gameObject in this.objectCollection)
            {
                if (gameObject is PlayerCharacter playerCharacter)
                {
                    if (playerCharacter.Name.ToString().Equals(name, StringComparison.Ordinal) && playerCharacter.HomeWorld.Id == worldId)
                    {
                        return MapToadPlayer(playerCharacter);
                    }
                }
            }

            return null;
        }
        finally
        {
            this.locker.ExitReadLock();
        }
    }

    /// <summary>
    /// Dispose manager.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        this.gameFramework.Update -= this.OnFrameworkUpdate;
        this.locker.Dispose();
    }

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

    private void OnFrameworkUpdate(IFramework framework)
    {
        this.locker.EnterWriteLock();
        try
        {
            var addedPlayers = new List<ToadPlayer>();
            var removedPlayers = new List<uint>();

            for (var i = 2; i < 200; i += 2)
            {
                var index = i / 2;
                var currentObjectId = this.objectCollection[i]?.ObjectId ?? 0;
                var existingId = this.existingObjectIds[index];

                // check if same
                if (currentObjectId == existingId)
                {
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
        }
        finally
        {
            this.locker.ExitWriteLock();
        }
    }
}
