using UnityEngine;

namespace CardSystem.Effects
{
    /// <summary>
    /// A box projectile shape a serialized size.
    /// </summary>
    [CreateAssetMenu(fileName = "NewProjectileShape", menuName = "Cards/ProjectileShapes/BoxProjectileShape")]
    internal class BoxProjectileShape : ProjectileShape
    {
        [Tooltip("The side lengths of the collider.")]
        public Vector2 size = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// Gets the collider form of this shape.
        /// </summary>
        /// <param name="gameObject"> The game object to create the collider on. </param>
        /// <returns> The created collider. </returns>
        public override Collider2D CreateCollider(GameObject gameObject)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = size;
            collider.isTrigger = true;
            return collider;
        }
    }
}