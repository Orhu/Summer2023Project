using UnityEngine;

namespace CardSystem.Effects
{
    internal abstract class ProjectileShape : ScriptableObject
    {
        public abstract Collider2D CreateCollider(GameObject gameObject);
    }

    internal class CircleProjectileShape : ProjectileShape
    {
        [Tooltip("The radius of the collider")]
        public float radius = 0.25f;

        public override Collider2D CreateCollider(GameObject gameObject)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = radius;
            return collider;
        }
    }

    internal class BoxProjectileShape : ProjectileShape
    {
        [Tooltip("The side lengths of the collider.")]
        public Vector2 size = new Vector2(0.5f, 0.5f);

        public override Collider2D CreateCollider(GameObject gameObject)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = size;
            return collider;
        }
    }
}