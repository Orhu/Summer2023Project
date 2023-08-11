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

        // The object that runs the double hit coroutines.
        private static MonoBehaviour _routineRunner;
        private static MonoBehaviour routineRunner
        {
            get
            {
                if (_routineRunner == null)
                {
                    _routineRunner = new GameObject("DoubleHitRoutineRunner").AddComponent<Empty>();
                }

                return _routineRunner;
            }
        }

        // Binds on overlap
        public override void Initialize(Projectile value)
        {
            projectile = value;

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
            foreach (System.Delegate method in projectile.onOverlap.GetInvocationList())
            {
                if (method.Target.GetType() == GetType())
                {
                    if (this == method.Target)
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

            Health hitHealth = collider.gameObject.GetComponent<Health>();
            bool applyDamage = hitHealth != null && projectile.applyDamageOnHit;

            DamageData damageData = new DamageData(projectile.attackData, projectile.attackData.causer);

            routineRunner.StartCoroutine(DelayedHit());
            IEnumerator DelayedHit()
            {
                yield return new WaitForSeconds(delay * index.Value);
                
                if (collider == null) { yield break; }
                overlapEffects?.Invoke(collider);

                if (applyDamage)
                {
                    hitHealth.ReceiveAttack(damageData);
                }

            }
        }

        private class Empty : MonoBehaviour { }
    }
}
