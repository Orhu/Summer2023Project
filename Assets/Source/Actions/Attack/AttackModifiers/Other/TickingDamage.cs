using System.Collections;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// An action modifier that changes the attack of an action modifier.
    /// </summary>
    [CreateAssetMenu(fileName = "NewTickingDamage", menuName = "Cards/AttackModifers/TickingDamage")]
    public class TickingDamage : AttackModifier
    {
        [Tooltip("The time in seconds to wait to apply damage.")]
        [Min(0f)]
        public float damageInterval = 0.5f;

        // The projectile to apply ticking damage under.
        private Rigidbody2D tickingDamageRigidbody;

        // The projectile to apply ticking damage under.
        private Projectile tickingDamageProjectile;

        // The projectile this modifies
        public override Projectile modifiedProjectile
        {
            set
            {
                value.onOverlap += StartTicking;
                tickingDamageProjectile = value;
                tickingDamageRigidbody = value.GetComponent<Rigidbody2D>();
            }
        }

        /// <summary>
        /// Starts dealing ticking damage to hit objects.
        /// </summary>
        /// <param name="collider"> The collider that was hit. </param>
        private void StartTicking(Collider2D collider)
        {
            Health health = collider.GetComponent<Health>();
            if (health != null)
            {
                tickingDamageProjectile.StartCoroutine(DealTickingDamage(health, collider));
            }
        }

        /// <summary>
        /// Deals damage on an interval.
        /// </summary>
        /// <param name="healthToDamage"> The health being damaged. </param>
        /// <param name="collider"> The collider of the health. </param>
        /// <returns> The time to wait until the next tick. </returns>
        private IEnumerator DealTickingDamage(Health healthToDamage, Collider2D collider)
        {
            yield return new WaitForSeconds(damageInterval);
            while (tickingDamageRigidbody != null && collider != null && tickingDamageRigidbody.IsTouching(collider))
            {
                healthToDamage.ReceiveAttack(tickingDamageProjectile.attackData);

                if (--tickingDamageProjectile.remainingHits <= 0)
                {
                    Destroy(tickingDamageProjectile.gameObject);
                }

                yield return new WaitForSeconds(damageInterval);
            }
        }
    }
}
