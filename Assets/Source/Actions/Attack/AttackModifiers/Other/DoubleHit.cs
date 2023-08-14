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
        [Tooltip("The time until the second hit is applied in seconds")] [Min(0)]
        [SerializeField] private float delay = 0.5f;
        
        // The projectile that will hit again.
        private Projectile projectile;

        // Whether or not this allows currently the destruction of the projectile this is attached to.
        public override bool allowDestruction { get => delayedHitCompleted; }

        // Whether or not the delayed hit has occurred.
        private bool delayedHitCompleted = false;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile attachedProjectile)
        {
            projectile = attachedProjectile;

            projectile.onOverlap += HitAgain;
        }

        /// <summary>
        /// Causes the exact effects of hitting the hit objects again.
        /// </summary>
        /// <param name="collider"> The object that was hit. </param>
        private void HitAgain(Collider2D collider)
        {
            System.Action<Collider2D> overlapEffects = null;
            int numOtherDoubleHits = 0;
            int? index = null;
            // Remove double hit bindings
            foreach (System.Delegate method in projectile.onOverlap.GetInvocationList())
            {
                if (method.Target.GetType() == GetType())
                {
#pragma warning disable CS0253 // Possible unintended reference comparison; right hand side needs cast
                    if (this == method.Target)
#pragma warning restore CS0253 // Possible unintended reference comparison; right hand side needs cast
                    {
                        index = numOtherDoubleHits + 1;
                    }
                    else
                    {
                        numOtherDoubleHits++;
                    }
                }
                else
                {
                    overlapEffects += method as System.Action<Collider2D>;
                }
            }


            projectile.StartCoroutine(DelayedHit());
            IEnumerator DelayedHit()
            {
                yield return new WaitForSeconds(delay * index.Value);
                
                if (collider == null) { yield break; }
                overlapEffects?.Invoke(collider);

                Health hitHealth = collider.gameObject.GetComponent<Health>();
                if (hitHealth != null && projectile.applyDamageOnHit)
                {
                    hitHealth.ReceiveAttack(projectile.attackData);
                }

                delayedHitCompleted = true;
            }
        }
    }
}
