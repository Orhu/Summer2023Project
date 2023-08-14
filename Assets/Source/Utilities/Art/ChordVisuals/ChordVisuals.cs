using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Cardificer
{
    [AddComponentMenu("Chord Visuals")]
    public class ChordVisuals : MonoBehaviour
    {

        [Tooltip("The priority that this visuals will be applied at. Lower priorities are applied first.")]
        [SerializeField] private int applicationPriority = 0;


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


        // Stores all of the visuals on a projectile sorted by priority for when they are bound.
        private static Dictionary<Projectile, List<ChordVisuals>> projectilesToOrderedVisuals = new Dictionary<Projectile, List<ChordVisuals>>();

        /// <summary>
        /// Binds events.
        /// </summary>
        private void Start()
        {
            projectile = GetComponentInParent<Projectile>();
            projectile ??= transform.parent.GetComponentInParent<Projectile>();
            visualSprite = projectile.visualObject.GetComponent<SpriteRenderer>();


            projectilesToOrderedVisuals.TryAdd(projectile, new List<ChordVisuals>());
            projectilesToOrderedVisuals[projectile].Add(this);
            projectilesToOrderedVisuals[projectile].Sort(
                (ChordVisuals visual1, ChordVisuals visual2) =>
                {
                    return visual2.applicationPriority - visual1.applicationPriority;
                });

            StartCoroutine(DelayedBindings());

            IEnumerator DelayedBindings()
            {
                yield return null;

                if (!projectilesToOrderedVisuals.TryGetValue(projectile, out List<ChordVisuals> visuals)) { yield break; }

                foreach (ChordVisuals visual in visuals)
                {
                    visual.BindEvents();
                }
                projectilesToOrderedVisuals.Remove(projectile);
            }
        }

        /// <summary>
        /// Binds all of this objects events.
        /// </summary>
        private void BindEvents()
        {
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