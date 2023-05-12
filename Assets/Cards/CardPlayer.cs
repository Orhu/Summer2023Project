using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interface for any object that can play card actions.
/// </summary>
public interface ICardPlayer
{
    /// <summary>
    /// Get the position that the action should be played from.
    /// </summary>
    /// <returns> The position in world space. </returns>
    public abstract Vector3 getActionSourcePosition();
}
