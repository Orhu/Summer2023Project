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
        Projectile projectile;

        // Binds on overlap
        public override Projectile modifiedProjectile
        {
            set
            {
                projectile = value;
                projectile.onOverlap +=
                    (Collider2D collider) =>
                    {
                        value.StartCoroutine(SetBindings(collider));
                    };
                projectile.onOverlap += HitAgain;
            }
        }


        private IEnumerator SetBindings(Collider2D collider)
        {
            yield return null;
            projectile.onOverlap += HitAgain;
        }

        // Causes the exact effects of hitting the hit objects again.
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
