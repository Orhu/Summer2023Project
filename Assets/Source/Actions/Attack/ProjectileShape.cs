using UnityEngine;

/// <summary>
/// Base class for the shape a projectile can take.
/// </summary>
public abstract class ProjectileShape : ScriptableObject
{
    /// <summary>
    /// Gets the collider form of this shape.
    /// </summary>
    /// <param name="gameObject"> The game object to create the collider on. </param>
    /// <returns> The created collider. </returns>
    public abstract Collider2D CreateCollider(GameObject gameObject);
}