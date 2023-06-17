using System.Collections.Generic;
using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// A box projectile shape a serialized size.
    /// </summary>
    [CreateAssetMenu(fileName = "NewProjectileShape", menuName = "Cards/ProjectileShapes/BoxProjectileShape")]
    public class BoxProjectileShape : ProjectileShape
    {
        [Tooltip("The side lengths of the collider.")]
        [SerializeField] private Vector2 _size = new Vector2(0.5f, 0.5f);
        public Vector2 size
        {
            set
            {
                _size = value;
                foreach (BoxCollider2D collider in colliders)
                {
                    collider.size = value;
                }
            }
            get => _size;
        }

        // All the colliders that have been created by this.
        private List<BoxCollider2D> colliders = new List<BoxCollider2D>();


        /// <summary>
        /// Gets the collider form of this shape.
        /// </summary>
        /// <param name="gameObject"> The game object to create the collider on. </param>
        /// <returns> The created collider. </returns>
        public override Collider2D CreateCollider(GameObject gameObject)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = size;
            return collider;
        }
    }
}