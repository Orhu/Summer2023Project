using UnityEngine;

namespace CardSystem.Effects
{
    [CreateAssetMenu(fileName = "NewProjectileShape", menuName = "Cards/ProjectileShapes/BoxProjectileShape")]
    internal class BoxProjectileShape : ProjectileShape
    {
        [Tooltip("The side lengths of the collider.")]
        public Vector2 size = new Vector2(0.5f, 0.5f);

        public override Collider2D CreateCollider(GameObject gameObject)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = size;
            collider.isTrigger = true;
            return collider;
        }
    }
}