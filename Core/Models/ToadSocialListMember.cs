﻿// ReSharper disable UnassignedField.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
namespace Dalamud.DrunkenToad.Core.Models;

using Utility;

public class ToadSocialListMember
{
    /// <summary>
    /// The content ID of the local character.
    /// </summary>
    public ulong ContentId;

    /// <summary>
    /// Player Name.
    /// </summary>
    public string Name = string.Empty;

    /// <summary>
    /// Player HomeWorld ID.
    /// </summary>
    public ushort HomeWorld;

    /// <summary>
    /// Player is unable to retrieve (no name).
    /// </summary>
    public bool IsUnableToRetrieve;

    /// <summary>
    /// Player is expected to have content id (set to false where not available).
    /// </summary>
    public bool ShouldHaveContentId = true;

    /// <summary>
    /// Validate if player is valid (expected to have content id).
    /// </summary>
    /// <returns>Indicator if player is valid.</returns>
    public bool IsValid()
    {
        if (!this.ShouldHaveContentId)
        {
            return DalamudContext.DataManager.IsValidWorld(this.HomeWorld) && this.Name.IsValidCharacterName();
        }

        if (this.ContentId == 0)
        {
            return false;
        }

        // If the player's name is null or empty, the player is valid only if they are marked as unable to retrieve.
        if (string.IsNullOrEmpty(this.Name))
        {
            return this.IsUnableToRetrieve;
        }

        return DalamudContext.DataManager.IsValidWorld(this.HomeWorld) && this.Name.IsValidCharacterName();
    }
}
