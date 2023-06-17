using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A circle projectile shape a serialized radius.
/// </summary>
[CreateAssetMenu(fileName = "NewProjectileShape", menuName = "Cards/ProjectileShapes/CircleProjectileShape")]
public class CircleProjectileShape : ProjectileShape
{
    [Tooltip("The radius of the collider")]
    [SerializeField] private float _radius = 0.25f;
    public float radius
    {
        set
        {
            _radius = value;
            foreach(CircleCollider2D collider in colliders)
            {
                collider.radius = value;
            }
        }
        get => _radius;
    }


    // All the colliders that have been created by this.
    private List<CircleCollider2D> colliders = new List<CircleCollider2D>();

    /// <summary>
    /// Gets the collider form of this shape.
    /// </summary>
    /// <param name="gameObject"> The game object to create the collider on. </param>
    /// <returns> The created collider. </returns>
    public override Collider2D CreateCollider(GameObject gameObject)
    {
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.radius = radius;
        colliders.Add(collider);
        return collider;
    }
}