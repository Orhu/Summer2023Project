using System.Collections;
using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Causes an attack to hit enemies twice.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDoubleHit", menuName = "Cards/AttackModifers/DoubleHit")]
    public class DoubleHit : AttackModifier
    {
        // The projectile that will hit again.
        private Projectile projectile;

        // Binds on overlap
        public override Projectile modifiedProjectile
        {
            set
            {
                projectile = value;
                projectile.onOverlap +=
                    (Collider2D collider) =>
                    {
                        value.StartCoroutine(SetBindings());
                    };
                projectile.onOverlap += HitAgain;
            }
        }

        /// <summary>
        /// Rebinds hit again after a frame.
        /// </summary>
        /// <returns> Wait one frame. </returns>
        private IEnumerator SetBindings()
        {
            yield return null;
            projectile.onOverlap += HitAgain;
        }

        /// <summary>
        /// Causes the exact effects of hitting the hit objects again.
        /// </summary>
        /// <param name="collider"> The object that was hit. </param>
        private void HitAgain(Collider2D collider)
        {
            projectile.onOverlap -= HitAgain;
            projectile.onOverlap?.Invoke(collider);

            Health hitHealth = collider.gameObject.GetComponent<Health>();
            if (hitHealth != null && projectile.applyDamageOnHit)
            {
                hitHealth.ReceiveAttack(projectile.attackData);
            }
        }
    }
}
