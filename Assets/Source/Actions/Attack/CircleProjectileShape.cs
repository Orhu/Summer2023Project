using UnityEngine;

namespace Attacks
{
    /// <summary>
    /// A circle projectile shape a serialized radius.
    /// </summary>
    [CreateAssetMenu(fileName = "NewProjectileShape", menuName = "Cards/ProjectileShapes/CircleProjectileShape")]
    internal class CircleProjectileShape : ProjectileShape
    {
        [Tooltip("The radius of the collider")]
        public float radius = 0.25f;

        /// <summary>
        /// Gets the collider form of this shape.
        /// </summary>
        /// <param name="gameObject"> The game object to create the collider on. </param>
        /// <returns> The created collider. </returns>
        public override Collider2D CreateCollider(GameObject gameObject)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = radius;
            collider.isTrigger = true;
            return collider;
        }
    }
}