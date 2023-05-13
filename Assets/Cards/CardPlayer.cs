using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interface for any object that can play card actions.
/// </summary>
public interface ICardPlayer
{
    /// <summary>
    /// Get the transform that the action should be played from.
    /// </summary>
    /// <returns> The transform. </returns>
    public abstract Transform GetActionSourceTransform();

    /// <summary>
    /// Get the position that the action should be aimed at.
    /// </summary>
    /// <returns> The position in world space. </returns>
    public abstract Vector3 GetActionAimPosition();

    /// <summary>
    /// Gets the collider of this player.
    /// </summary>
    /// <returns> The collider. </returns>
    public abstract Collider2D GetCollider();
}
