namespace Dalamud.DrunkenToad.Core.Services;

using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using Models;

/// <summary>
/// Manages player locations based on territory changes and game data.
/// Provides events for location start and end points.
/// </summary>
public class PlayerLocationManager(IClientState clientStateHandler, DataManagerEx dataManager)
{
    private readonly IClientState clientStateHandler = clientStateHandler ?? throw new ArgumentNullException(nameof(clientStateHandler));
    private readonly DataManagerEx dataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
    private ushort currentTerritoryType;

    public delegate void LocationDelegate(ToadLocation toadLocation);

    public event LocationDelegate? LocationStarted;

    public event LocationDelegate? LocationEnded;

    /// <summary>
    /// Starts the location manager and begins processing territory changes.
    /// </summary>
    public void Start()
    {
        this.clientStateHandler.TerritoryChanged += this.OnTerritoryChanged;
        this.clientStateHandler.Logout += this.OnLogout;
        if (this.clientStateHandler.IsLoggedIn)
        {
            this.ProcessTerritoryChange(this.clientStateHandler.TerritoryType);
        }
    }

    /// <summary>
    /// Retrieves the current location based on the territory type.
    /// </summary>
    /// <returns>The current location as a <see cref="ToadLocation" />.</returns>
    public ToadLocation? GetCurrentLocation() => this.dataManager.Locations.GetValueOrDefault(this.currentTerritoryType);

    /// <summary>
    /// Disposes the location manager and stops processing territory changes.
    /// </summary>
    public void Dispose()
    {
        this.clientStateHandler.TerritoryChanged -= this.OnTerritoryChanged;
        this.clientStateHandler.Logout -= this.OnLogout;
        this.LocationStarted = null;
        this.LocationEnded = null;
    }

    private static bool IsValidLocation(ushort territoryType) => territoryType != 0;

    private void ProcessTerritoryChange(ushort newTerritoryType)
    {
        if (IsValidLocation(this.currentTerritoryType))
        {
            this.LocationEnded?.Invoke(this.dataManager.Locations[this.currentTerritoryType]);
        }

        if (IsValidLocation(newTerritoryType))
        {
            this.LocationStarted?.Invoke(this.dataManager.Locations[newTerritoryType]);
        }

        this.currentTerritoryType = newTerritoryType;
    }

    private void OnLogout(int type, int code)
    {
        this.LocationEnded?.Invoke(this.dataManager.Locations[this.currentTerritoryType]);
        this.ProcessTerritoryChange(0);
    }

    private void OnTerritoryChanged(ushort territoryTypeId) => this.ProcessTerritoryChange(territoryTypeId);
}
