using UnityEngine;
using UnityEngine.Events;

namespace Cardificer
{
    [AddComponentMenu("Projectile Visuals")]
    public class ChordVisuals : MonoBehaviour
    {
        // The projectile this is representing the visuals for.
        protected Projectile projectile { private set; get; }

        // The primary sprite render of the projectile.
        protected SpriteRenderer visualSprite { private set; get; }



        [Tooltip("Called when a projectile is first cast.")]
        public UnityEvent onCast;
        protected virtual void OnCast() { }

        [Tooltip("Called when a projectile hits anything including enemies and walls. Passes in the hit collider.")]
        public UnityEvent<Collider2D> onHitAny;
        protected virtual void OnHitAny(Collider2D hitCollider) { }

        [Tooltip("Called when a projectile hits an enemy or other damageable object it could pass through. Passes in the hit collider.")]
        public UnityEvent<Collider2D> onHitEnemy;
        protected virtual void OnHitEnemy(Collider2D hitCollider) { }

        [Tooltip("Called when a projectile hits an enemy or other damageable object it could pass through. Passes in the hit data.")]
        public UnityEvent<Collision2D> onHitWall;
        protected virtual void OnHitWall(Collision2D hit) { }

        [Tooltip("Called when a projectile has been destroyed.")]
        public UnityEvent onDestroyed;
        protected virtual void OnDestroyed() { }


        /// <summary>
        /// Binds events.
        /// </summary>
        private void Start()
        {
            projectile = GetComponentInParent<Projectile>();
            projectile ??= transform.parent.GetComponentInParent<Projectile>();
            visualSprite = projectile.visualObject.GetComponent<SpriteRenderer>();


            onCast?.Invoke();
            OnCast();

            projectile.onOverlap += OnHitAny;
            projectile.onOverlap += (Collider2D hitCollider) => onHitAny?.Invoke(hitCollider);
            projectile.onOverlap += OnHitEnemy;
            projectile.onOverlap += (Collider2D hitCollider) => onHitEnemy?.Invoke(hitCollider);

            projectile.onHit += (Collision2D hit) => OnHitAny(hit.collider);
            projectile.onHit += (Collision2D hit) => onHitAny?.Invoke(hit.collider);
            projectile.onHit += OnHitWall;
            projectile.onHit += (Collision2D hit) => onHitWall?.Invoke(hit);

            projectile.onDestroyed += OnDestroyed;
            projectile.onDestroyed += () => onDestroyed?.Invoke();
        }

    }
}