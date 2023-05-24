using UnityEngine;

namespace CardSystem.Effects
{

    [CreateAssetMenu(fileName = "NewProjectileShape", menuName = "Cards/ProjectileShapes/CircleProjectileShape")]
    internal class CircleProjectileShape : ProjectileShape
    {
        [Tooltip("The radius of the collider")]
        public float radius = 0.25f;

        public override Collider2D CreateCollider(GameObject gameObject)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = radius;
            collider.isTrigger = true;
            return collider;
        }
    }
}