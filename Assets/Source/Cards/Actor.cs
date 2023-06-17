using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interface for any object that can play card actions.
/// </summary>
public interface IActor
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
    /// Gets the collider of this actor.
    /// </summary>
    /// <returns> The collider. </returns>
    public abstract Collider2D GetCollider();

    /// <summary>
    /// Gets the delegate that will fetch whether this actor can act.
    /// </summary>
    /// <returns> A delegate with a out parameter, that allows any subscribed objects to determine whether or not this actor can act. </returns>
    public ref CanActRequest GetOnRequestCanAct();
    delegate void CanActRequest(ref bool canAct);
    
    public abstract bool canAct { get; }
}
