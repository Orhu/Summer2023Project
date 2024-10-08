using System;
using System.Collections;
using System.Linq;
using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// An action modifier that changes the attack of an action modifier.
    /// </summary>
    [CreateAssetMenu(fileName = "NewBouncy", menuName = "Cards/AttackModifers/Bouncy")]
    public class Bouncy : AttackModifier
    {
        //The cosine of the minimum angle between vectors to consider the same.
        private const float DOUBLE_BOUNCE_PROTECTION_FACTOR = 0.9f;

        // The rigid body to bounce.
        private Rigidbody2D bouncyRigidbody;

        // The projectile to bounce.
        private Projectile bouncyProjectile;

        // The normal of the last wall bounced off of this frame
        Vector2 lastBounceNormal = Vector2.zero;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            if (value.onHit == null || !value.onHit.GetInvocationList().Contains((Action<Collision2D>)Bounce))
            {
                value.onHit += Bounce;
                bouncyProjectile = value;
                bouncyRigidbody = value.GetComponent<Rigidbody2D>();
            }
        }

        /// <summary>
        /// Bounces the attacked projectile off of any hit walls.
        /// </summary>
        /// <param name="collision"> The collision data used to bounce. </param>
        private void Bounce(Collision2D collision)
        {
            Vector2 bounceNormal = collision.GetContact(0).normal;

            bouncyProjectile.CancelInvoke("DestroyOnWallHit");
            if (lastBounceNormal == Vector2.zero)
            {
                bouncyProjectile.StartCoroutine(ClearLastNormal());

                bouncyProjectile.transform.right = Vector2.Reflect(bouncyProjectile.transform.right, bounceNormal);
                bouncyProjectile.velocity = bouncyProjectile.speed * bouncyProjectile.transform.right;
                lastBounceNormal = bounceNormal;
            }
            else if (Vector2.Dot(bounceNormal, lastBounceNormal) < DOUBLE_BOUNCE_PROTECTION_FACTOR)
            {
                bouncyProjectile.transform.right = Vector2.Reflect(bouncyProjectile.transform.right, bounceNormal);
                bouncyProjectile.velocity = bouncyProjectile.speed * bouncyProjectile.transform.right;
                lastBounceNormal = bounceNormal;
            }
        }

        /// <summary>
        /// Clears the last bounce normal.
        /// </summary>
        /// <returns> The time to wait until clearing. </returns>
        private IEnumerator ClearLastNormal()
        {
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            lastBounceNormal = Vector2.zero;
        }
    }
}
