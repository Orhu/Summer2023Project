using UnityEngine;

namespace CardSystem.Effects
{
    public abstract class ProjectileShape : ScriptableObject
    {
        public abstract Collider2D CreateCollider(GameObject gameObject);
    }
}