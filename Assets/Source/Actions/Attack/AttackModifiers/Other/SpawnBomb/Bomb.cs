using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A script that causes a game object to damage nearby things.
    /// </summary>
    public class Bomb : MonoBehaviour
    {
        // The damage dealt by this bomb.
        public DamageData damageData;

        // The knockback caused by this.
        public KnockbackInfo knockback;

        // Whether or not this should destroy blockers
        public bool destroyBlockers;

        // The radius in tiles of the explosion caused by the bomb.
        public float explosionRadius = 2f;

        // The time in seconds after the bomb is spawned until it detonates.
        public float fuseTime = 2f;

        // The objects not to damage when exploding.
        public List<GameObject> ignoredObjects = new List<GameObject>();

        // Invoked when this explodes.
        public System.Action onExploded;

        /// <summary>
        /// Update fuse time.
        /// </summary>
        private void Update()
        {
            fuseTime -= Time.deltaTime;
            if (fuseTime <= 0)
            {
                Explode();
            }
        }

        /// <summary>
        /// Causes this to explode.
        /// </summary>
        private void Explode()
        {
            onExploded?.Invoke();
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

            foreach (Collider2D hitCollider in hitColliders)
            {
                if (ignoredObjects.Contains(hitCollider.gameObject)) { continue; }

                hitCollider.GetComponent<Health>()?.ReceiveAttack(damageData);
                hitCollider.GetComponent<Movement>()?.Knockback((hitCollider.transform.position - transform.position).normalized, knockback);
                if (destroyBlockers && hitCollider.gameObject.layer == LayerMask.NameToLayer("Blockers"))
                {
                    Destroy(hitCollider.gameObject);
                }
            }

            transform.GetChild(0).transform.parent = null;
            Destroy(gameObject);
        }
    }
}