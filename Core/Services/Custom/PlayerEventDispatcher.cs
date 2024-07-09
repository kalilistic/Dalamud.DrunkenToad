namespace Dalamud.DrunkenToad.Core.Services;

using System;
using System.Collections.Generic;
using System.Threading;
using Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using Extensions;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Models;

/// <summary>
/// Manages player character events, including addition, removal, and updates.
/// </summary>
public class PlayerEventDispatcher : IDisposable
{
    private readonly ulong[] existingContentIds = new ulong[100];
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
    /// <param name="toadPlayers">Player objects with key properties from IPlayerCharacter.</param>
    public delegate void DalamudAddPlayersDelegate(List<ToadPlayer> toadPlayers);

    /// <summary>
    /// Remove Players Delegate.
    /// </summary>
    /// <param name="playerIds">Player object ids.</param>
    public delegate void DalamudRemovePlayersDelegate(List<ulong> playerIds);

    /// <summary>
    /// Update Players Delegate.
    /// </summary>
    /// <param name="toadPlayers">Player objects with key properties from IPlayerCharacter.</param>
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
    public ToadPlayer? GetPlayerByContentId(ulong id)
    {
        this.locker.EnterReadLock();
        try
        {
            for (var i = 0; i < this.existingContentIds.Length; i++)
            {
                // ReSharper disable once InvertIf
                if (this.existingContentIds[i] == id)
                {
                    if (this.objectCollection[i * 2] is IPlayerCharacter playerCharacter)
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
                if (gameObject is IPlayerCharacter playerCharacter)
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

    private static unsafe ToadPlayer MapToadPlayer(IPlayerCharacter character) => new ()
    {
        GameObjectId = character.GameObjectId,
        EntityId = character.EntityId,
        ContentId = ((Character*)character.Address)->ContentId,
        Name = character.Name.ToString(),
        HomeWorld = character.HomeWorld.Id,
        ClassJob = character.ClassJob.Id,
        Level = character.Level,
        Customize = character.Customize,
        CompanyTag = character.CompanyTag.ToString(),
        IsLocalPlayer = false,
        IsDead = character.IsDead,
    };

    private unsafe void OnFrameworkUpdate(IFramework framework)
    {
        this.locker.EnterWriteLock();
        try
        {
            var addedPlayers = new List<ToadPlayer>();
            var removedPlayers = new List<ulong>();

            for (var i = 2; i < 200; i += 2)
            {
                var index = i / 2;
                var gameObject = this.objectCollection[i];
                if (gameObject == null)
                {
                    continue;
                }

                var currentContentId = ((Character*)gameObject.Address)->ContentId;
                var existingId = this.existingContentIds[index];

                // check if same
                if (currentContentId == existingId)
                {
                    continue;
                }

                // check if removed
                if (this.objectCollection[i] == null)
                {
                    if (existingId != 0)
                    {
                        removedPlayers.Add(existingId);
                        this.existingContentIds[i / 2] = 0;
                    }

                    continue;
                }

                IPlayerCharacter character;
                if (this.objectCollection[i].IsValidIPlayerCharacter())
                {
                    character = (this.objectCollection[i] as IPlayerCharacter) !;
                }
                else
                {
                    continue;
                }

                // check if new
                if (existingId == 0)
                {
                    addedPlayers.Add(MapToadPlayer(character));
                    this.existingContentIds[i / 2] = currentContentId;
                    continue;
                }

                // otherwise replaced
                removedPlayers.Add(existingId);
                addedPlayers.Add(MapToadPlayer(character));
                this.existingContentIds[i / 2] = currentContentId;
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
